using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.Auditor.Module.Tests.TestHelpers;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using NSubstitute;
using VPortal.Auditor.Module.AuditHistoryRecords;
using VPortal.Auditor.Module.Entities;
using VPortal.Auditor.Module.Repositories;
using Xunit;

namespace Eleon.Auditor.Module.Tests.Application;

public class AuditHistoryRecordAppServiceTests
{
  [Fact]
  public async Task GetDocumentHistory_Should_Return_Paged_Result()
  {
    var historyRepository = Substitute.For<IAuditHistoryRecordRepository>();
    var records = new List<AuditHistoryRecordEntity>
    {
      new AuditHistoryRecordEntity(Guid.NewGuid())
      {
        Version = "v1",
        DocumentId = "doc-1",
        DocumentObjectType = "DocType",
        CreatorId = Guid.NewGuid(),
        CreatorName = "Jane Doe",
        CreationTime = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Utc)
      }
    };

    historyRepository.GetRecordsByDocument(
        "DocType",
        "doc-1",
        Arg.Any<string?>(),
        Arg.Any<int>(),
        Arg.Any<int>(),
        Arg.Any<DateTime?>(),
        Arg.Any<DateTime?>())
      .Returns(new KeyValuePair<int, List<AuditHistoryRecordEntity>>(records.Count, records));

    var domainService = AuditTestHelpers.CreateDomainService(historyRecordRepository: historyRepository);
    var appService = new AuditHistoryRecordAppService(
        TestMockHelpers.CreateMockLogger<AuditHistoryRecordAppService>(),
        domainService);

    var result = await appService.GetDocumentHistory(new DocumentHistoryRequest
    {
      DocumentObjectType = "DocType",
      DocumentId = "doc-1"
    });

    result.Should().NotBeNull();
    result.TotalCount.Should().Be(1);
    result.Items.Should().ContainSingle();
    result.Items[0].Version.Should().Be("v1");
    result.Items[0].CreatedByUserName.Should().Be("Jane Doe");
  }
}
