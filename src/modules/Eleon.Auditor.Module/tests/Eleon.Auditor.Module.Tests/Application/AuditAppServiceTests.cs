using System;
using System.Threading;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.Auditor.Module.Tests.TestHelpers;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Messaging.Module.Messages;
using ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using VPortal.Auditor.Module.Application.Audit;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.Repositories;
using VPortal.Infrastructure.Module.Entities;
using Xunit;

namespace Eleon.Auditor.Module.Tests.Application;

public class AuditAppServiceTests
{
  private const string RefObjectType = "RefType";
  private const string RefId = "ref-1";
  private const string DocumentObjectType = "DocType";
  private const string DocumentId = "doc-1";

  [Fact]
  public async Task CreateAsync_Should_Return_Result_From_Domain_Service()
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

    var domainService = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository);

    var appService = new AuditAppService(
        TestMockHelpers.CreateMockLogger<AuditAppService>(),
        domainService);

    var result = await appService.CreateAsync(new CreateAuditDto
    {
      RefDocumentObjectType = RefObjectType,
      RefDocumentId = RefId,
      AuditedDocumentObjectType = DocumentObjectType,
      AuditedDocumentId = DocumentId,
      DocumentData = "payload",
      DocumentVersion = new DocumentVersionEntity { TransactionId = "tx-1" }
    });

    result.Should().BeTrue();
  }

  [Fact]
  public async Task GetAsync_Should_Return_AuditDto_From_Domain_Service()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();

    var record = new AuditHistoryRecordEntity(Guid.NewGuid())
    {
      AuditDataId = Guid.NewGuid(),
      Version = "v1",
      DocumentId = DocumentId,
      DocumentObjectType = DocumentObjectType
    };

    historyRepository.GetRecordByDocumentVersion(DocumentObjectType, DocumentId, "v1")
        .Returns(record);
    dataRepository.FindAsync(record.AuditDataId, Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(new AuditDataEntity(record.AuditDataId) { DocumentData = "payload" });

    var domainService = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository);

    var appService = new AuditAppService(
        TestMockHelpers.CreateMockLogger<AuditAppService>(),
        domainService);

    var result = await appService.GetAsync(new GetAuditDto
    {
      AuditedDocumentObjectType = DocumentObjectType,
      AuditedDocumentId = DocumentId,
      Version = "v1"
    });

    result.Should().NotBeNull();
    result.Data.Should().Be("payload");
    result.DocumentVersion.Should().NotBeNull();
    result.DocumentVersion.Version.Should().Be("v1");
  }

  [Fact]
  public async Task GetCurrentVersionAsync_Should_Return_Current_Version()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    var currentVersion = new AuditCurrentVersionEntity(Guid.NewGuid())
    {
      CurrentVersion = "v1",
      RefDocumentType = RefObjectType,
      RefDocId = RefId
    };
    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);

    var domainService = AuditTestHelpers.CreateDomainService(currentVersionRepository: currentVersionRepository);
    var appService = new AuditAppService(
        TestMockHelpers.CreateMockLogger<AuditAppService>(),
        domainService);

    var result = await appService.GetCurrentVersionAsync(new GetVersionDto
    {
      RefDocumentObjectType = RefObjectType,
      RefDocumentId = RefId
    });

    result.Should().NotBeNull();
    result.Version.Should().Be("v1");
  }

  [Fact]
  public async Task IncrementAuditVersionAsync_Should_Return_Result()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var dataRepository = Substitute.For<IAuditDataRepository>();
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
      RefDocId = RefId
    };

    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(currentVersion);
    currentVersionRepository.UpdateAsync(Arg.Any<AuditCurrentVersionEntity>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(callInfo => callInfo.Arg<AuditCurrentVersionEntity>());

    historyRepository.GetRecordsByVersion(currentVersion.Id, "v1")
        .Returns(new List<AuditHistoryRecordEntity>
        {
          new AuditHistoryRecordEntity(Guid.NewGuid())
          {
            AuditDataId = Guid.NewGuid(),
            Version = "v1",
            DocumentId = DocumentId,
            DocumentObjectType = DocumentObjectType,
            AuditVersionId = currentVersion.Id
          }
        });
    historyRepository.InsertManyAsync(Arg.Any<IEnumerable<AuditHistoryRecordEntity>>(), Arg.Any<bool>(), Arg.Any<CancellationToken>())
        .Returns(Task.CompletedTask);

    var domainService = AuditTestHelpers.CreateDomainService(
        historyRecordRepository: historyRepository,
        dataRepository: dataRepository,
        currentVersionRepository: currentVersionRepository,
        eventBus: eventBus);

    var appService = new AuditAppService(
        TestMockHelpers.CreateMockLogger<AuditAppService>(),
        domainService);

    var result = await appService.IncrementAuditVersionAsync(new IncrementVersionRequestDto
    {
      AuditedDocumentObjectType = RefObjectType,
      AuditedDocumentId = RefId,
      Version = new DocumentVersionEntity { Version = "v1" }
    });

    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.NewDocumentVersion.Should().NotBeNull();
    result.NewDocumentVersion.Version.Should().Be("v2");
  }
}
