using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.PackageTemplates;

namespace VPortal.Accounting.Module.Controllers
{
  [Area(AccountingRemoteServiceConsts.ModuleName)]
  [RemoteService(Name = AccountingRemoteServiceConsts.RemoteServiceName)]
  [Route("api/account/packageTemplates")]
  public class PackageTemplateController : ModuleController, IPackageTemplateAppService
  {
    private readonly IPackageTemplateAppService appService;
    private readonly IVportalLogger<PackageTemplateController> logger;

    public PackageTemplateController(
        IVportalLogger<PackageTemplateController> logger,
        IPackageTemplateAppService appService)
    {
      this.logger = logger;
      this.appService = appService;
    }

    [HttpPost("CreatePackageTemplate")]
    public Task<PackageTemplateDto> CreatePackageTemplate(PackageTemplateDto updatedDto)
    {
      logger.LogStart();

      var response = appService.CreatePackageTemplate(updatedDto);

      logger.LogFinish();

      return response;
    }

    [HttpGet("GetPackageTemplateById")]
    public async Task<PackageTemplateDto> GetPackageTemplateById(Guid id)
    {

      var response = await appService.GetPackageTemplateById(id);


      return response;
    }

    [HttpPost("GetPackageTemplateList")]
    public async Task<PagedResultDto<PackageTemplateDto>> GetPackageTemplateList(PackageTemplateListRequestDto input)
    {

      var response = await appService.GetPackageTemplateList(input);


      return response;
    }

    [HttpPost("RemovePackageTemplate")]
    public async Task<string> RemovePackageTemplate(Guid id)
    {

      var response = await appService.RemovePackageTemplate(id);


      return response;
    }

    [HttpPost("UpdatePackageTemplate")]
    public async Task<PackageTemplateDto> UpdatePackageTemplate(PackageTemplateDto updatedDto)
    {

      var response = await appService.UpdatePackageTemplate(updatedDto);


      return response;
    }
  }
}
