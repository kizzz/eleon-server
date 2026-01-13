using System.Collections.Generic;
using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using NSubstitute;
using Volo.Abp.Application.Dtos;
using VPortal.Auditor.Module.AuditHistoryRecords;
using VPortal.Auditor.Module.Controllers;
using VPortal.Infrastructure.Module.Entities;
using Xunit;

namespace Eleon.Auditor.Module.Tests.HttpApi;

public class AuditHistoryRecordControllerTests
{
  [Fact]
  public async Task GetDocumentHistory_Should_Forward_To_App_Service()
  {
    var appService = Substitute.For<IAuditHistoryRecordAppService>();
    var request = new DocumentHistoryRequest
    {
      DocumentObjectType = "DocType",
      DocumentId = "doc-1"
    };

    var expected = new PagedResultDto<DocumentVersionEntity>(
        1,
        new List<DocumentVersionEntity> { new() { Version = "v1" } });

    appService.GetDocumentHistory(request).Returns(expected);

    var controller = new AuditHistoryRecordController(
        appService,
        TestMockHelpers.CreateMockLogger<IAuditHistoryRecordAppService>());

    var result = await controller.GetDocumentHistory(request);

    result.Should().NotBeNull();
    result.TotalCount.Should().Be(1);
    result.Items.Should().ContainSingle();
    result.Items[0].Version.Should().Be("v1");
    await appService.Received(1).GetDocumentHistory(request);
  }
}
