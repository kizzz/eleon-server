using Common.Module.Constants;
using Common.Module.ValueObjects;
using Logging.Module;
using Microsoft.Extensions.Logging;
using Migrations.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;

namespace VPortal.Infrastructure.Module.Domain.DomainServices
{
  public class OrganizationUnitPermissionDomainService : DomainService
  {
    private readonly IVportalLogger<OrganizationUnitPermissionDomainService> logger;
    private readonly IOrganizationUnitRepository organizationUnitRepository;
    private readonly IIdentityUserRepository identityUserRepository;
    private readonly OrganizationUnitManager organizationUnitManager;
    private readonly IPermissionGrantRepository permissionGrantRepository;
    private readonly ICurrentUser currentUser;


    public OrganizationUnitPermissionDomainService(
        IVportalLogger<OrganizationUnitPermissionDomainService> logger,
        IOrganizationUnitRepository organizationUnitRepository,
        IIdentityUserRepository identityUserRepository,
        OrganizationUnitManager organizationUnitManager,
        IPermissionGrantRepository permissionGrantRepository,
        ICurrentUser currentUser
        )
    {
      this.logger = logger;
      this.organizationUnitRepository = organizationUnitRepository;
      this.identityUserRepository = identityUserRepository;
      this.organizationUnitManager = organizationUnitManager;
      this.permissionGrantRepository = permissionGrantRepository;
      this.currentUser = currentUser;
      //this.companyRepository = companyRepository;
    }

    public async Task<bool> CheckOrgUnitPermission(Guid organizationUnitId, string permission)
    {

      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        var permissions = await permissionGrantRepository.GetListAsync("U", currentUser.Id.ToString());
        if (permissions.FirstOrDefault(p => p.Name == permission) != null)
        {
          return true;
        }

        var roles = await GetRoles(organizationUnitId);
        var userRoles = await identityUserRepository.GetRolesAsync(currentUser.Id.Value);

        if (userRoles.Any(r => r.Name == MigrationConsts.AdminRoleNameDefaultValue))
        {
          return true;
        }

        foreach (var role in userRoles.Where(x => roles.Select(y => y.Name).Contains(x.Name)))
        {
          List<PermissionGrant> rolePermissions = await permissionGrantRepository.GetListAsync("R", role.Name);
          var permissionGranted = rolePermissions.FirstOrDefault(p => p.Name == permission);
          if (permissionGranted != null)
          {
            return true;
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return false;
    }

    public async Task<bool> CheckOrgUnitParentsPermission(Guid organizationUnitId, string permission)
    {
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        var permissions = await permissionGrantRepository.GetListAsync("U", currentUser.Id.ToString());
        if (permissions.FirstOrDefault(p => p.Name == permission) != null)
        {
          return true;
        }

        var orgUnit = await organizationUnitRepository.GetAsync(organizationUnitId);
        var parents = await GetParentOrgUnits(orgUnit);
        parents.Add(orgUnit);

        foreach (var parent in parents)
        {
          var roles = await GetRoles(parent.Id);

          var userRoles = await identityUserRepository.GetRolesAsync(currentUser.Id.Value);

          if (userRoles.Any(r => r.Name == MigrationConsts.AdminRoleNameDefaultValue))
          {
            return true;
          }


          foreach (var role in userRoles.Where(x => roles.Select(y => y.Name).Contains(x.Name)))
          {
            List<PermissionGrant> rolePermissions = await permissionGrantRepository.GetListAsync("R", role.Name);
            var permissionGranted = rolePermissions.FirstOrDefault(p => p.Name == permission);
            if (permissionGranted != null)
            {
              return true;
            }
          }

        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return false;
    }

    public async Task<List<CommonRoleValueObject>> GetRoles(Guid orgUnitId)
    {

      List<CommonRoleValueObject> result = null;
      try
      {
        var orgUnit = await organizationUnitRepository.GetAsync(orgUnitId);
        result = await GetRoles(orgUnit);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }
    public async Task<List<CommonRoleValueObject>> GetRoles(OrganizationUnit organizationUnit, OrganizationUnit child = null)
    {

      List<CommonRoleValueObject> result = new List<CommonRoleValueObject>();
      try
      {
        if (organizationUnit == null)
        {
          return result;
        }

        var roles = await organizationUnitRepository.GetRolesAsync(organizationUnit);

        string inheritedFrom = null;
        bool isReadOnly = false;

        if (child != null)
        {
          inheritedFrom = organizationUnit.DisplayName;
          isReadOnly = true;
        }

        IEnumerable<CommonRoleValueObject> rolesToAdd = roles.Select(r => new CommonRoleValueObject()
        {
          Id = r.Id,
          Name = r.Name,
          InheritedFrom = inheritedFrom,
          IsReadOnly = isReadOnly,
        });

        result.AddRange(rolesToAdd);

        if (organizationUnit.ParentId.HasValue)
        {
          var parent = await organizationUnitRepository.FindAsync(organizationUnit.ParentId.Value);
          List<CommonRoleValueObject> topRoles = await GetRoles(parent, organizationUnit);
          result.AddRange(topRoles);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
      return result;

    }

    private async Task<List<OrganizationUnit>> GetParentOrgUnits(OrganizationUnit unit)
    {
      List<OrganizationUnit> list = null;
      try
      {
        if (!unit.ParentId.HasValue)
        {
          return new List<OrganizationUnit>();
        }

        var orgUnitParent = await organizationUnitRepository.GetAsync(unit.ParentId.Value);

        list = await GetParentOrgUnits(orgUnitParent);
        list.Add(orgUnitParent);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return list;
    }

    private async Task<OrganizationUnit> GetRootOrgUnit(OrganizationUnit unit, int depth = 0)
    {
      ArgumentNullException.ThrowIfNull(unit);
      depth++;
      if (depth > 1000)
      {
        logger.Log.LogCritical("GetRootOrgUnit recursion depth limit reached");
        throw new Exception("GetRootOrgUnit recursion depth limit reached");
      }

      OrganizationUnit result = null;
      try
      {
        if (!unit.ParentId.HasValue)
        {
          return unit;
        }

        var orgUnitParent = await organizationUnitRepository.GetAsync(unit.ParentId.Value);

        result = await GetRootOrgUnit(orgUnitParent);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<OrganizationUnit> GetOrgUnitById(Guid orgUnitId)
    {
      OrganizationUnit result = null;
      try
      {
        result = await organizationUnitRepository.GetAsync(orgUnitId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    //public async Task<List<string>> GetUserOrgUnitPermissionsAsync(Guid userId, Guid rootOrgUnitId)
    //{

    //    try
    //    {
    //        var unit = await GetOrgUnitById(rootOrgUnitId);
    //        var rootOrgUnit = await GetRootOrgUnit(unit);
    //        rootOrgUnit.Roles.

    //    }
    //    catch (Exception ex)
    //    {
    //        logger.Capture(ex);
    //    }
    //    finally
    //    {
    //    }
    //}
  }
}
