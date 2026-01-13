using Eleon.AbpSdk.Lib.modules.HostExtensions.Module.Auth;
using EleonsoftAbp.Auth;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Application.Contracts.CustomPermissions;
using ModuleCollector.SitesManagement.Module.SitesManagement.Module.Domain.DomainServices;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Domain.Messages;
using Volo.Abp.PermissionManagement;
using VPortal.SitesManagement.Module;

namespace VPortal.SitesManagement.Module.CustomPermissions
{
  [Authorize]
  public class CustomPermissionsAppService : SitesManagementAppService, ICustomPermissionsAppService
  {
    private readonly IVportalLogger<CustomPermissionsAppService> logger;
    private readonly CustomPermissionDomainService customPermissionDomainService;

    public CustomPermissionsAppService(
        IVportalLogger<CustomPermissionsAppService> logger,
        CustomPermissionDomainService customPermissionDomainService)
    {
      this.logger = logger;
      this.customPermissionDomainService = customPermissionDomainService;
    }

    public async Task<bool> CreateBulkForMicroserviceAsync(CustomPermissionsForMicroserviceDto customPermissionsForMicroserviceDto)
    {
      bool result = false;
      try
      {
        var apiKeyIdentifier = CurrentUser.GetApiKeyName();

        if (string.IsNullOrEmpty(apiKeyIdentifier))
        {
          throw new Exception("API Key identifier is required to create custom permissions for a microservice.");
        }

        var groups = ObjectMapper.Map<List<CustomPermissionGroupDto>, List<PermissionGroupDefinitionEto>>(customPermissionsForMicroserviceDto.Groups);
        var permissions = ObjectMapper.Map<List<CustomPermissionDto>, List<PermissionDefinitionEto>>(customPermissionsForMicroserviceDto.Permissions);

        result = await customPermissionDomainService.CreateBulkForMicroserviceAsync(apiKeyIdentifier, groups, permissions);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomPermissionGroupDto> CreateGroupAsync(CustomPermissionGroupDto createGroupDto)
    {
      CustomPermissionGroupDto result = null;
      try
      {
        createGroupDto.Id = Guid.NewGuid();
        var entity = ObjectMapper.Map<CustomPermissionGroupDto, PermissionGroupDefinitionEto>(createGroupDto);
        var createdEntity = await customPermissionDomainService.CreateGroupAsync(entity);
        result = ObjectMapper.Map<PermissionGroupDefinitionEto, CustomPermissionGroupDto>(createdEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomPermissionDto> CreatePermissionAsync(CustomPermissionDto createPermissionDto)
    {
      CustomPermissionDto result = null;
      try
      {
        createPermissionDto.Id = Guid.NewGuid();
        var entity = ObjectMapper.Map<CustomPermissionDto, PermissionDefinitionEto>(createPermissionDto);
        var createdEntity = await customPermissionDomainService.CreateAsync(entity);
        result = ObjectMapper.Map<PermissionDefinitionEto, CustomPermissionDto>(createdEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task DeleteGroupAsync(string name)
    {
      try
      {
        await customPermissionDomainService.DeleteGroupAsync(name);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task DeletePermissionAsync(string name)
    {
      try
      {
        await customPermissionDomainService.DeleteAsync(name);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<CustomPermissionGroupDto>> GetPermissionDynamicGroupCategoriesAsync()
    {
      List<CustomPermissionGroupDto> result = null;
      try
      {
        var entities = await customPermissionDomainService.GetPermissionGroupsAsync();
        result = ObjectMapper.Map<List<PermissionGroupDefinitionEto>, List<CustomPermissionGroupDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<List<CustomPermissionDto>> GetPermissionsDynamicAsync()
    {
      List<CustomPermissionDto> result = null;
      try
      {
        var entities = await customPermissionDomainService.GetPermissionsAsync();
        result = ObjectMapper.Map<List<PermissionDefinitionEto>, List<CustomPermissionDto>>(entities);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomPermissionGroupDto> UpdateGroupAsync(CustomPermissionGroupDto updateGroupDto)
    {
      CustomPermissionGroupDto result = null;
      try
      {
        var entity = ObjectMapper.Map<CustomPermissionGroupDto, PermissionGroupDefinitionEto>(updateGroupDto);
        var updatedEntity = await customPermissionDomainService.UpdateGroupAsync(entity);
        result = ObjectMapper.Map<PermissionGroupDefinitionEto, CustomPermissionGroupDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task<CustomPermissionDto> UpdatePermissionAsync(CustomPermissionDto updatePermissionDto)
    {
      CustomPermissionDto result = null;
      try
      {
        var entity = ObjectMapper.Map<CustomPermissionDto, PermissionDefinitionEto>(updatePermissionDto);
        var updatedEntity = await customPermissionDomainService.UpdateAsync(entity);
        result = ObjectMapper.Map<PermissionDefinitionEto, CustomPermissionDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }
  }
}


