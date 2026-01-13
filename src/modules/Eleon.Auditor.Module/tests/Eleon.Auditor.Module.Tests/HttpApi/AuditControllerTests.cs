using System.Threading.Tasks;
using Eleon.TestsBase.Lib.TestHelpers;
using FluentAssertions;
using ModuleCollector.Auditor.Module.Auditor.Module.Application.Contracts.Audit;
using NSubstitute;
using VPortal.Auditor.Module.Application.Contracts.Audit;
using ModuleCollector.Auditor.Module.Auditor.Module.HttpApi.Controllers;
using VPortal.Infrastructure.Module.Entities;
using Xunit;

namespace Eleon.Auditor.Module.Tests.HttpApi;

public class AuditControllerTests
{
  [Fact]
  public async Task CreateAsync_Should_Forward_To_App_Service()
  {
    var auditAppService = Substitute.For<IAuditAppService>();
    var input = new CreateAuditDto
    {
      RefDocumentObjectType = "RefType",
      RefDocumentId = "ref-1",
      AuditedDocumentObjectType = "DocType",
      AuditedDocumentId = "doc-1",
      DocumentData = "payload"
    };

    auditAppService.CreateAsync(input).Returns(true);

    var controller = new AuditController(
        TestMockHelpers.CreateMockLogger<AuditController>(),
        auditAppService);

    var result = await controller.CreateAsync(input);

    result.Should().BeTrue();
    await auditAppService.Received(1).CreateAsync(input);
  }

  [Fact]
  public async Task GetAsync_Should_Forward_To_App_Service()
  {
    var auditAppService = Substitute.For<IAuditAppService>();
    var input = new GetAuditDto
    {
      AuditedDocumentObjectType = "DocType",
      AuditedDocumentId = "doc-1",
      Version = "v1"
    };

    auditAppService.GetAsync(input)
        .Returns(new AuditDto("payload", new DocumentVersionEntity { Version = "v1" }));

    var controller = new AuditController(
        TestMockHelpers.CreateMockLogger<AuditController>(),
        auditAppService);

    var result = await controller.GetAsync(input);

    result.Should().NotBeNull();
    result.Data.Should().Be("payload");
    result.DocumentVersion.Version.Should().Be("v1");
    await auditAppService.Received(1).GetAsync(input);
  }

  [Fact]
  public async Task GetCurrentVersionAsync_Should_Forward_To_App_Service()
  {
    var auditAppService = Substitute.For<IAuditAppService>();
    var input = new GetVersionDto
    {
      RefDocumentObjectType = "RefType",
      RefDocumentId = "ref-1"
    };

    auditAppService.GetCurrentVersionAsync(input)
        .Returns(new DocumentVersionEntity { Version = "v1" });

    var controller = new AuditController(
        TestMockHelpers.CreateMockLogger<AuditController>(),
        auditAppService);

    var result = await controller.GetCurrentVersionAsync(input);

    result.Should().NotBeNull();
    result.Version.Should().Be("v1");
    await auditAppService.Received(1).GetCurrentVersionAsync(input);
  }

  [Fact]
  public async Task IncrementAuditVersionAsync_Should_Forward_To_App_Service()
  {
    var auditAppService = Substitute.For<IAuditAppService>();
    var input = new IncrementVersionRequestDto
    {
      AuditedDocumentObjectType = "DocType",
      AuditedDocumentId = "doc-1",
      Version = new DocumentVersionEntity { Version = "v1" }
    };

    auditAppService.IncrementAuditVersionAsync(input)
        .Returns(new IncrementVersionResultDto
        {
          Success = true,
          NewDocumentVersion = new DocumentVersionEntity { Version = "v2" }
        });

    var controller = new AuditController(
        TestMockHelpers.CreateMockLogger<AuditController>(),
        auditAppService);

    var result = await controller.IncrementAuditVersionAsync(input);

    result.Should().NotBeNull();
    result.Success.Should().BeTrue();
    result.NewDocumentVersion.Version.Should().Be("v2");
    await auditAppService.Received(1).IncrementAuditVersionAsync(input);
  }
}
