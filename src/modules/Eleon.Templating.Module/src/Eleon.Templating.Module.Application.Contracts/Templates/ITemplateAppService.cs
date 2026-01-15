using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Eleon.Templating.Module.Templates;

public interface ITemplateAppService : IApplicationService
{
  Task<TemplateDto> GetAsync(Guid id, CancellationToken cancellationToken = default);
  Task<TemplateDto> GetByNameAndType(string name, TemplateType type, CancellationToken cancellationToken = default);
  Task<PagedResultDto<MinimalTemplateDto>> GetListAsync(GetTemplateListInput input, CancellationToken cancellationToken = default);
  Task<TemplateDto> CreateAsync(CreateUpdateTemplateDto input, CancellationToken cancellationToken = default);
  Task<TemplateDto> UpdateAsync(Guid id, CreateUpdateTemplateDto input, CancellationToken cancellationToken = default);
  Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
  Task<string> ApplyTemplateAsync(ApplyTemplateInput input, CancellationToken cancellationToken = default);
  Task<TemplateDto> ResetAsync(ResetTemplateInput input, CancellationToken cancellationToken = default);
}

