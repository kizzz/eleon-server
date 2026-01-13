using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.Auditor.Module.Tests.TestHelpers;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;
using Volo.Abp.Users;
using VPortal.Auditor.Module.DomainServices;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.Repositories;
using VPortal.Infrastructure.Module.Entities;
using Xunit;

namespace Eleon.Auditor.Module.Tests.Domain;

public class AuditDomainServiceTests
{
  private const string RefObjectType = "RefType";
  private const string RefId = "ref-1";
  private const string DocumentObjectType = "DocType";
  private const string DocumentId = "doc-1";

  [Fact]
  public async Task GetCurrentVersion_Should_Map_Last_Modifier_Metadata()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    var currentVersion = new AuditCurrentVersionEntity(Guid.NewGuid())
    {
      CurrentVersion = "v1",
      RefDocumentType = RefObjectType,
      RefDocId = RefId,
      LastModifierName = "Jane Doe",
      CreationTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
      LastModificationTime = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Utc),
      CreatorId = Guid.NewGuid(),
      LastModifierId = Guid.NewGuid()
    };

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);

    var service = AuditTestHelpers.CreateDomainService(currentVersionRepository: currentVersionRepository);

    var result = await service.GetCurrentVersion(RefObjectType, RefId);

    result.Should().NotBeNull();
    result!.Version.Should().Be("v1");
    result.CreatedAt.Should().Be(currentVersion.LastModificationTime);
    result.CreatedByUserId.Should().Be(currentVersion.LastModifierId);
    result.CreatedByUserName.Should().Be("Jane Doe");
  }

  [Fact]
  public async Task GetCurrentVersion_Should_Return_Null_When_Not_Found()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns((AuditCurrentVersionEntity?)null);

    var service = AuditTestHelpers.CreateDomainService(currentVersionRepository: currentVersionRepository);

    var result = await service.GetCurrentVersion(RefObjectType, RefId);

    result.Should().BeNull();
  }

  [Fact]
  public async Task IncrementAuditVersion_Should_Insert_New_Current_Version()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    var eventBus = AuditTestHelpers.CreateResponseCapableEventBus();
    var responseBus = (IResponseCapableEventBus)eventBus;

    responseBus.RequestAsync<DocumentSeriaNumberGotMsg>(Arg.Any<object>(), Arg.Any<int>())
        .Returns(Task.FromResult(new DocumentSeriaNumberGotMsg { SeriaNumber = "v2" }));
    eventBus.PublishAsync(Arg.Any<AuditVersionChangeNotificationMsg>(), Arg.Any<bool>(), Arg.Any<bool>())
        .Returns(Task.CompletedTask);

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns((AuditCurrentVersionEntity?)null);
    currentVersionRepository.InsertAsync(Arg.Any<AuditCurrentVersionEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditCurrentVersionEntity>());

    historyRepository.InsertManyAsync(Arg.Any<IEnumerable<AuditHistoryRecordEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

    var currentUser = AuditTestHelpers.CreateCurrentUser(Guid.NewGuid(), "Jane", "Doe", "jdoe");
    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository,
        currentUser: currentUser,
        eventBus: eventBus);

    var (success, version) = await service.IncrementAuditVersion(RefObjectType, RefId, null);

    success.Should().BeTrue();
    version.Should().NotBeNull();
    version!.Version.Should().Be("v2");
    version.CreatedByUserName.Should().Be("Jane Doe");

    await currentVersionRepository.Received(1)
        .InsertAsync(Arg.Is<AuditCurrentVersionEntity>(entity =>
            entity.RefDocumentType == RefObjectType &&
            entity.RefDocId == RefId &&
            entity.CurrentVersion == "v2" &&
            entity.LastModifierName == "Jane Doe"),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

    await currentVersionRepository.DidNotReceive()
        .UpdateAsync(Arg.Any<AuditCurrentVersionEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

    await historyRepository.DidNotReceive()
        .InsertManyAsync(Arg.Any<IEnumerable<AuditHistoryRecordEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>());

    await eventBus.Received(1)
        .PublishAsync(Arg.Any<AuditVersionChangeNotificationMsg>(), Arg.Any<bool>(), Arg.Any<bool>());
  }

  [Fact]
  public async Task IncrementAuditVersion_Should_Copy_And_Update_When_Claim_Matches()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    var eventBus = AuditTestHelpers.CreateResponseCapableEventBus();
    var responseBus = (IResponseCapableEventBus)eventBus;

    responseBus.RequestAsync<DocumentSeriaNumberGotMsg>(Arg.Any<object>(), Arg.Any<int>())
        .Returns(Task.FromResult(new DocumentSeriaNumberGotMsg { SeriaNumber = "v2" }));
    eventBus.PublishAsync(Arg.Any<AuditVersionChangeNotificationMsg>(), Arg.Any<bool>(), Arg.Any<bool>())
        .Returns(Task.CompletedTask);

    var currentVersion = new AuditCurrentVersionEntity(Guid.NewGuid())
    {
      CurrentVersion = "v1",
      RefDocumentType = RefObjectType,
      RefDocId = RefId,
      ConcurrencyStamp = "tx-old"
    };

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);
    currentVersionRepository.UpdateAsync(Arg.Any<AuditCurrentVersionEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditCurrentVersionEntity>());

    var existingRecords = new List<AuditHistoryRecordEntity>
    {
      new AuditHistoryRecordEntity(Guid.NewGuid())
      {
        AuditDataId = Guid.NewGuid(),
        Version = "v1",
        DocumentId = DocumentId,
        DocumentObjectType = DocumentObjectType,
        AuditVersionId = currentVersion.Id
      }
    };
    historyRepository.GetRecordsByVersion(currentVersion.Id, "v1")
        .Returns(existingRecords);
    historyRepository.InsertManyAsync(Arg.Any<IEnumerable<AuditHistoryRecordEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

    var currentUser = AuditTestHelpers.CreateCurrentUser(Guid.NewGuid(), "Jane", "Doe", "jdoe");
    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository,
        currentUser: currentUser,
        eventBus: eventBus);

    var claimedVersion = new DocumentVersionEntity
    {
      Version = "v1",
      AppendToTransactionId = "append-123"
    };

    var (success, version) = await service.IncrementAuditVersion(RefObjectType, RefId, claimedVersion);

    success.Should().BeTrue();
    version.Should().NotBeNull();
    version!.Version.Should().Be("v2");
    version.TransactionId.Should().Be("append-123");

    await currentVersionRepository.Received(1)
        .UpdateAsync(Arg.Is<AuditCurrentVersionEntity>(entity =>
            entity.CurrentVersion == "v2" &&
            entity.LastModifierName == "Jane Doe"),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

    await historyRepository.Received(1)
        .InsertManyAsync(Arg.Is<IEnumerable<AuditHistoryRecordEntity>>(copies =>
            copies.All(copy =>
                copy.Version == "v2" &&
                copy.CreatorName == "Jane Doe" &&
                copy.TransactionId == "append-123" &&
                copy.DocumentId == DocumentId &&
                copy.DocumentObjectType == DocumentObjectType)),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());

    await eventBus.Received(1)
        .PublishAsync(Arg.Any<AuditVersionChangeNotificationMsg>(), Arg.Any<bool>(), Arg.Any<bool>());
  }

  [Fact]
  public async Task IncrementAuditVersion_Should_Return_False_When_Claim_Mismatch()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(new AuditCurrentVersionEntity(Guid.NewGuid()) { CurrentVersion = "v1" });

    var service = AuditTestHelpers.CreateDomainService(currentVersionRepository: currentVersionRepository);

    var claimedVersion = new DocumentVersionEntity { Version = "v0" };

    var (success, version) = await service.IncrementAuditVersion(RefObjectType, RefId, claimedVersion);

    success.Should().BeFalse();
    version.Should().BeNull();
  }

  [Fact]
  public async Task CreateAudit_Should_Insert_New_Record_When_Missing()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();

    var currentVersion = new AuditCurrentVersionEntity(Guid.NewGuid())
    {
      CurrentVersion = "v1",
      RefDocumentType = RefObjectType,
      RefDocId = RefId
    };

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);

    dataRepository.InsertAsync(Arg.Any<AuditDataEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditDataEntity>());

    historyRepository.GetRecordByDocumentVersion(DocumentObjectType, DocumentId, "v1")
        .Returns((AuditHistoryRecordEntity?)null);
    historyRepository.InsertAsync(Arg.Any<AuditHistoryRecordEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditHistoryRecordEntity>());

    var currentUser = AuditTestHelpers.CreateCurrentUser(Guid.NewGuid(), "Jane", "Doe", "jdoe");

    var fixedIds = Substitute.For<IGuidGenerator>();
    var dataId = Guid.NewGuid();
    var recordId = Guid.NewGuid();
    fixedIds.Create().Returns(dataId, recordId);

    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository,
        currentUser: currentUser,
        guidGenerator: fixedIds);

    var version = new DocumentVersionEntity { TransactionId = "tx-1" };

    var result = await service.CreateAudit(
        RefObjectType,
        RefId,
        DocumentObjectType,
        DocumentId,
        "payload",
        version);

    result.Should().BeTrue();

    await historyRepository.Received(1)
        .InsertAsync(Arg.Is<AuditHistoryRecordEntity>(entity =>
            entity.Version == "v1" &&
            entity.DocumentId == DocumentId &&
            entity.DocumentObjectType == DocumentObjectType &&
            entity.AuditVersionId == currentVersion.Id &&
            entity.TransactionId == "tx-1" &&
            entity.CreatorName == "Jane Doe"),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task CreateAudit_Should_Update_Record_When_Exists()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();

    var currentVersion = new AuditCurrentVersionEntity(Guid.NewGuid())
    {
      CurrentVersion = "v1",
      RefDocumentType = RefObjectType,
      RefDocId = RefId
    };

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);

    dataRepository.InsertAsync(Arg.Any<AuditDataEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditDataEntity>());

    var existingRecord = new AuditHistoryRecordEntity(Guid.NewGuid())
    {
      AuditDataId = Guid.NewGuid(),
      Version = "v1",
      DocumentId = DocumentId,
      DocumentObjectType = DocumentObjectType,
      AuditVersionId = currentVersion.Id
    };

    historyRepository.GetRecordByDocumentVersion(DocumentObjectType, DocumentId, "v1")
        .Returns(existingRecord);
    historyRepository.UpdateAsync(Arg.Any<AuditHistoryRecordEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditHistoryRecordEntity>());

    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository);

    var version = new DocumentVersionEntity { TransactionId = "tx-2" };

    var result = await service.CreateAudit(
        RefObjectType,
        RefId,
        DocumentObjectType,
        DocumentId,
        "payload",
        version);

    result.Should().BeTrue();

    await historyRepository.Received(1)
        .UpdateAsync(Arg.Is<AuditHistoryRecordEntity>(entity =>
            entity.TransactionId == "tx-2"),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task GetAuditDocument_Should_Return_Data_And_Metadata()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();

    var record = new AuditHistoryRecordEntity(Guid.NewGuid())
    {
      AuditDataId = Guid.NewGuid(),
      Version = "v1",
      DocumentId = DocumentId,
      DocumentObjectType = DocumentObjectType,
      CreatorId = Guid.NewGuid(),
      CreatorName = "Jane Doe",
      CreationTime = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc),
      TransactionId = "tx-3"
    };

    historyRepository.GetRecordByDocumentVersion(DocumentObjectType, DocumentId, "v1")
        .Returns(record);

    dataRepository.FindAsync(record.AuditDataId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(new AuditDataEntity(record.AuditDataId) { DocumentData = "payload" });

    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository);

    var (version, data) = await service.GetAuditDocument(DocumentObjectType, DocumentId, "v1");

    data.Should().Be("payload");
    version.Should().NotBeNull();
    version!.Version.Should().Be("v1");
    version.CreatedAt.Should().Be(record.CreationTime);
    version.CreatedByUserId.Should().Be(record.CreatorId);
    version.CreatedByUserName.Should().Be("Jane Doe");
    version.TransactionId.Should().Be("tx-3");
  }

  [Fact]
  public async Task GetAuditDocumentHistory_Should_Map_Result_List()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var records = new List<AuditHistoryRecordEntity>
    {
      new AuditHistoryRecordEntity(Guid.NewGuid())
      {
        Version = "v1",
        DocumentId = DocumentId,
        DocumentObjectType = DocumentObjectType,
        CreatorId = Guid.NewGuid(),
        CreatorName = "Jane Doe",
        CreationTime = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
      }
    };

    historyRepository.GetRecordsByDocument(
        DocumentObjectType,
        DocumentId,
        Arg.Any<string?>(),
        Arg.Any<int>(),
        Arg.Any<int>(),
        Arg.Any<DateTime?>(),
        Arg.Any<DateTime?>())
      .Returns(new KeyValuePair<int, List<AuditHistoryRecordEntity>>(records.Count, records));

    var service = AuditTestHelpers.CreateDomainService(historyRecordRepository: historyRepository);

    var result = await service.GetAuditDocumentHistory(DocumentObjectType, DocumentId);

    result.Key.Should().Be(1);
    result.Value.Should().NotBeNull();
    result.Value.Should().ContainSingle();
    result.Value[0].Version.Should().Be("v1");
    result.Value[0].CreatedByUserName.Should().Be("Jane Doe");
  }

  [Fact]
  public async Task CopyVersion_Should_Insert_Copies_With_New_Version()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var currentVersionId = Guid.NewGuid();
    var records = new List<AuditHistoryRecordEntity>
    {
      new AuditHistoryRecordEntity(Guid.NewGuid())
      {
        AuditDataId = Guid.NewGuid(),
        Version = "v1",
        DocumentId = DocumentId,
        DocumentObjectType = DocumentObjectType,
        AuditVersionId = currentVersionId
      }
    };

    historyRepository.GetRecordsByVersion(currentVersionId, "v1")
        .Returns(records);
    historyRepository.InsertManyAsync(Arg.Any<IEnumerable<AuditHistoryRecordEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

    var currentUser = AuditTestHelpers.CreateCurrentUser(Guid.NewGuid(), "Jane", "Doe", "jdoe");
    var service = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        currentUser: currentUser);

    await service.CopyVersion(currentVersionId, "v1", "v2", "tx-4");

    await historyRepository.Received(1)
        .InsertManyAsync(Arg.Is<IEnumerable<AuditHistoryRecordEntity>>(copies =>
            copies.All(copy =>
                copy.Version == "v2" &&
                copy.CreatorName == "Jane Doe" &&
                copy.TransactionId == "tx-4")),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
  }
}
