using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace VPortal.Accounting.Module.PackageTemplates
{
  public interface IPackageTemplateAppService
  {
    Task<PagedResultDto<PackageTemplateDto>> GetPackageTemplateList(PackageTemplateListRequestDto input);
    Task<string> RemovePackageTemplate(Guid id);
    Task<PackageTemplateDto> GetPackageTemplateById(Guid id);
    Task<PackageTemplateDto> UpdatePackageTemplate(PackageTemplateDto updatedDto);
    Task<PackageTemplateDto> CreatePackageTemplate(PackageTemplateDto updatedDto);
  }
}
