using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common.EventBus.Module;
using Eleon.Auditor.Module.Tests.TestHelpers;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using Messaging.Module.Messages;
using NSubstitute;
using Volo.Abp.EventBus.Distributed;
using VPortal.Auditor.Module.DomainServices;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.EventServices;
using VPortal.Auditor.Module.Repositories;
using VPortal.Infrastructure.Module.Entities;
using Xunit;

namespace Eleon.Auditor.Module.Tests.EventServices;

public class AuditEventServicesTests
{
  private const string RefObjectType = "RefType";
  private const string RefId = "ref-1";
  private const string DocumentObjectType = "DocType";
  private const string DocumentId = "doc-1";

  [Fact]
  public async Task CreateAuditEventService_Should_Respond_With_Result()
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

    var responseContext = Substitute.For<IResponseContext>();
    responseContext.RespondAsync(Arg.Any<object>()).Returns(Task.CompletedTask);

    var handler = new CreateAuditEventService(
        TestMockHelpers.CreateMockLogger<CreateAuditEventService>(),
        responseContext,
        domainService);

    await handler.HandleEventAsync(new CreateAuditMsg
    {
      RefDocumentObjectType = RefObjectType,
      RefDocumentId = RefId,
      AuditedDocumentObjectType = DocumentObjectType,
      AuditedDocumentId = DocumentId,
      DocumentData = "payload",
      DocumentVersion = new DocumentVersionEntity { TransactionId = "tx-1" }
    });

    await responseContext.Received(1)
        .RespondAsync(Arg.Is<AuditCreatedMsg>(msg => msg.CreatedSuccessfully));
  }

  [Fact]
  public async Task GetAuditDocumentEventService_Should_Respond_With_Document()
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

    var responseContext = Substitute.For<IResponseContext>();
    responseContext.RespondAsync(Arg.Any<object>()).Returns(Task.CompletedTask);

    var handler = new GetAuditDocumentEventService(
        TestMockHelpers.CreateMockLogger<GetAuditDocumentEventService>(),
        responseContext,
        domainService);

    await handler.HandleEventAsync(new GetAuditDocumentMsg
    {
      AuditedDocumentObjectType = DocumentObjectType,
      AuditedDocumentId = DocumentId,
      Version = "v1"
    });

    await responseContext.Received(1)
        .RespondAsync(Arg.Is<AuditDocumentGotMsg>(msg =>
            msg.AuditedDocument != null &&
            msg.AuditedDocument.Data == "payload" &&
            msg.AuditedDocument.Version.Version == "v1"));
  }

  [Fact]
  public async Task GetCurrentVersionEventService_Should_Respond_With_Current_Version()
  {
    var currentVersionRepository = Substitute.For<IAuditCurrentVersionRepository>();
    currentVersionRepository.GetCurrentVersion(RefObjectType, RefId)
        .Returns(new AuditCurrentVersionEntity(Guid.NewGuid())
        {
          CurrentVersion = "v1",
          RefDocumentType = RefObjectType,
          RefDocId = RefId
        });

    var domainService = AuditTestHelpers.CreateDomainService(currentVersionRepository: currentVersionRepository);

    var responseContext = Substitute.For<IResponseContext>();
    responseContext.RespondAsync(Arg.Any<object>()).Returns(Task.CompletedTask);

    var handler = new GetCurrentVersionEventService(
        TestMockHelpers.CreateMockLogger<GetCurrentVersionEventService>(),
        responseContext,
        domainService);

    await handler.HandleEventAsync(new GetAuditCurrentVersionMsg
    {
      RefDocumentObjectType = RefObjectType,
      RefDocumentId = RefId
    });

    await responseContext.Received(1)
        .RespondAsync(Arg.Is<AuditCurrentVersionGotMsg>(msg =>
            msg.CurrentVersion != null &&
            msg.CurrentVersion.Version == "v1"));
  }

  [Fact]
  public async Task IncrementAuditVersionEventService_Should_Respond_With_New_Version()
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

    var responseContext = Substitute.For<IResponseContext>();
    responseContext.RespondAsync(Arg.Any<object>()).Returns(Task.CompletedTask);

    var handler = new IncrementAuditVersionEventService(
        TestMockHelpers.CreateMockLogger<IncrementAuditVersionEventService>(),
        responseContext,
        domainService);

    await handler.HandleEventAsync(new IncrementAuditDocumentVersionMsg
    {
      AuditedDocumentObjectType = RefObjectType,
      AuditedDocumentId = RefId,
      Version = new DocumentVersionEntity { Version = "v1" }
    });

    await responseContext.Received(1)
        .RespondAsync(Arg.Is<AuditVersionIncrementedMsg>(msg =>
            msg.Success &&
            msg.NewVersion != null &&
            msg.NewVersion.Version == "v2"));
  }
}
