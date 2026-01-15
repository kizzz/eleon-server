using System.Linq;
using Eleon.Templating.Module.Permissions;
using Eleon.Templating.Module.Templates;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace Eleon.Templating.Module.Templates;

[Authorize(ModulePermissions.Templates)]
public class TemplateAppService : ModuleAppService, ITemplateAppService
{
  private readonly TemplateManager _templateManager;
  private readonly IRepository<Template, Guid> _templateRepository;
  private readonly IVportalLogger<TemplateAppService> _logger;

  public TemplateAppService(
      TemplateManager templateManager,
      IRepository<Template, Guid> templateRepository,
      IVportalLogger<TemplateAppService> logger)
  {
    _templateManager = templateManager;
    _templateRepository = templateRepository;
    _logger = logger;
  }

  [Authorize(ModulePermissions.TemplatesGet)]
  public async Task<TemplateDto> GetAsync(Guid id, CancellationToken cancellationToken = default)
  {
    var template = await _templateRepository.GetAsync(id, cancellationToken: cancellationToken);
    return ObjectMapper.Map<Template, TemplateDto>(template);
  }

  [Authorize(ModulePermissions.TemplatesGetList)]
  public async Task<PagedResultDto<MinimalTemplateDto>> GetListAsync(GetTemplateListInput input, CancellationToken cancellationToken = default)
  {
    PagedResultDto<MinimalTemplateDto> result = null;
    try
    {
      var pair = await _templateManager.GetListAsync(
          input.Sorting,
          input.MaxResultCount,
          input.SkipCount,
          input.SearchQuery,
          input.Type,
          input.Format);

      var dtos = ObjectMapper.Map<List<Template>, List<MinimalTemplateDto>>(pair.Value);
      result = new PagedResultDto<MinimalTemplateDto>(pair.Key, dtos);
    }
    catch (Exception ex)
    {
      _logger.Capture(ex);
    }

    return result;
  }


  [Authorize(ModulePermissions.TemplatesCreate)]
  public async Task<TemplateDto> CreateAsync(CreateUpdateTemplateDto input, CancellationToken cancellationToken = default)
  {
    var template = await _templateManager.CreateAsync(
        input.Name,
        input.Type,
        input.Format,
        input.TemplateContent,
        input.RequiredPlaceholders,
        false,
        input.TemplateId,
        cancellationToken);

    return ObjectMapper.Map<Template, TemplateDto>(template);
  }

  [Authorize(ModulePermissions.TemplatesUpdate)]
  public async Task<TemplateDto> UpdateAsync(Guid id, CreateUpdateTemplateDto input, CancellationToken cancellationToken = default)
  {
    var template = await _templateManager.UpdateAsync(
        id,
        input.Name,
        input.Type,
        input.Format,
        input.TemplateContent,
        input.RequiredPlaceholders,
        input.TemplateId,
        cancellationToken);

    return ObjectMapper.Map<Template, TemplateDto>(template);
  }

  [Authorize(ModulePermissions.TemplatesDelete)]
  public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
  {
    await _templateManager.DeleteAsync(id, cancellationToken);
  }

  [Authorize(ModulePermissions.TemplatesApply)]
  public async Task<string> ApplyTemplateAsync(ApplyTemplateInput input, CancellationToken cancellationToken = default)
  {
    return await _templateManager.ApplyTemplateAsync(input.TemplateName, input.TemplateType, input.Placeholders, cancellationToken);
  }

  [Authorize(ModulePermissions.TemplatesGet)]
  public async Task<TemplateDto> GetByNameAndType(string name, TemplateType type, CancellationToken cancellationToken = default)
  {
    var entity = await _templateManager.GetByNameAndTypeAsync(name, type, cancellationToken);
    return ObjectMapper.Map<Template, TemplateDto>(entity);
  }

  [Authorize(ModulePermissions.TemplatesGet)]
  public async Task<TemplateDto> ResetAsync(ResetTemplateInput input, CancellationToken cancellationToken = default)
  {
    var template = await _templateManager.ResetAsync(input.Name, input.Type, cancellationToken: cancellationToken);
    return ObjectMapper.Map<Template, TemplateDto>(template);
  }
}

