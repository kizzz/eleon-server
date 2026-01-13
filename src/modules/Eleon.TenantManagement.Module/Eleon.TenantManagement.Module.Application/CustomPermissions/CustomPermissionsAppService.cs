using Logging.Module;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.PermissionGroups;
using Volo.Abp.Application.Services;
using Volo.Abp.PermissionManagement;
using Microsoft.AspNetCore.Authorization;
using Common.Module.Extensions;
using ModuleCollector.TenantManagement.Module.TenantManagement.Module.Application.Contracts.CustomPermissions;
using Common.EventBus.Module;
using Messaging.Module.Messages;

namespace VPortal.TenantManagement.Module.CustomPermissions
{
  [Authorize]
  public class CustomPermissionsAppService : TenantManagementAppService, ICustomPermissionsAppService
  {
    private readonly IVportalLogger<CustomPermissionsAppService> logger;
    private readonly CustomPermissionDomainService customPermissionDomainService;
    private readonly PermissionManager permissionManager;

    public CustomPermissionsAppService(
        IVportalLogger<CustomPermissionsAppService> logger,
        CustomPermissionDomainService customPermissionDomainService,
        PermissionManager permissionManager)
    {
      this.logger = logger;
      this.customPermissionDomainService = customPermissionDomainService;
      this.permissionManager = permissionManager;
    }

    public async Task<bool> CreateBulkForMicroserviceAsync(CustomPermissionsForMicroserviceDto customPermissionsForMicroserviceDto)
    {
      bool result = false;
      try
      {
        var groups = ObjectMapper.Map<List<CustomPermissionGroupDto>, List<PermissionGroupDefinitionRecord>>(customPermissionsForMicroserviceDto.Groups);
        foreach (var entity in groups.Select(e =>
        {
          var dto = customPermissionsForMicroserviceDto.Groups.First(t => t.Name == e.Name);
          return new
          {
            Record = e,
            Dto = dto,
          };
        }))
        {
          entity.Record.ExtraProperties.TryAdd("CategoryName", entity.Dto.CategoryName);
          entity.Record.ExtraProperties.TryAdd("Order", entity.Dto.Order);
        }
        var permissions = ObjectMapper.Map<List<CustomPermissionDto>, List<PermissionDefinitionRecord>>(customPermissionsForMicroserviceDto.Permissions);


        result = await customPermissionDomainService.CreateGroupsForMicroserviceAsync(customPermissionsForMicroserviceDto.SourceId, groups);
        result = await customPermissionDomainService.CreatePermissionsForMicroserviceAsync(customPermissionsForMicroserviceDto.SourceId, permissions);
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
        var entity = ObjectMapper.Map<CustomPermissionGroupDto, PermissionGroupDefinitionRecord>(createGroupDto);
        entity.ExtraProperties.TryAdd("CategoryName", createGroupDto.CategoryName);
        entity.ExtraProperties.TryAdd("Order", createGroupDto.Order);

        var createdEntity = await customPermissionDomainService.CreateGroupAsync(entity);
        result = ObjectMapper.Map<PermissionGroupDefinitionRecord, CustomPermissionGroupDto>(createdEntity);
        result.CategoryName = (string)createdEntity.ExtraProperties.GetValueOrDefault("CategoryName", string.Empty);
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
        var entity = ObjectMapper.Map<CustomPermissionDto, PermissionDefinitionRecord>(createPermissionDto);
        var createdEntity = await customPermissionDomainService.CreateAsync(entity, createPermissionDto.Order);
        result = ObjectMapper.Map<PermissionDefinitionRecord, CustomPermissionDto>(createdEntity);
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
        var entities = await customPermissionDomainService.GetPermissionDynamicGroupCategories();
        result = ObjectMapper.Map<List<PermissionGroupDefinitionRecord>, List<CustomPermissionGroupDto>>(entities);
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
        var entities = await customPermissionDomainService.GetPermissionsDynamic();
        result = ObjectMapper.Map<List<PermissionDefinitionRecord>, List<CustomPermissionDto>>(entities);
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
        var entity = ObjectMapper.Map<CustomPermissionGroupDto, PermissionGroupDefinitionRecord>(updateGroupDto);
        entity.ExtraProperties.TryAdd("CategoryName", updateGroupDto.CategoryName);
        var updatedEntity = await customPermissionDomainService.UpdateGroupAsync(entity);
        result = ObjectMapper.Map<PermissionGroupDefinitionRecord, CustomPermissionGroupDto>(updatedEntity);
        result.CategoryName = (string)updatedEntity.ExtraProperties.GetValueOrDefault("CategoryName", string.Empty);
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
        var entity = ObjectMapper.Map<CustomPermissionDto, PermissionDefinitionRecord>(updatePermissionDto);
        var updatedEntity = await customPermissionDomainService.UpdateAsync(entity);
        result = ObjectMapper.Map<PermissionDefinitionRecord, CustomPermissionDto>(updatedEntity);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task GrantUserPermissionAsync(Guid userId, string permission, bool isGranted)
    {

      try
      {
        await permissionManager.SetForUserAsync(userId, permission, isGranted);
      }
      catch (Exception ex)
      {
        logger.CaptureAndSuppress(ex);
      }
      finally
      {
      }
    }
  }
}
