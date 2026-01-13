using Common.Module.Extensions;
using Common.Module.ValueObjects;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.TenantManagement.Module.OrganizationUnits;

namespace VPortal.TenantManagement.Module.DomainServices
{

  public class OrganizationUnitDomainService : DomainService
  {
    private readonly IIdentityUserRepository userRepository;
    private readonly IVportalLogger<OrganizationUnitDomainService> logger;
    private readonly ICurrentUser currentUser;
    private readonly IOrganizationUnitRepository organizationUnitRepository;
    private readonly OrganizationUnitManager organizationUnitManager;
    private readonly OrganizationUnitPermissionDomainService organizationUnitPermissionDomainService;
    private readonly UnitOfWorkManager unitOfWorkManager;
    private readonly IdentityUserManager userManager;
    private readonly IdentityRoleManager roleManager;
    private readonly IDistributedEventBus requestClient;
    private readonly IDataFilter<ISoftDelete> softDeletedDataFilter;

    public OrganizationUnitDomainService(
        IIdentityUserRepository userRepository,

        IVportalLogger<OrganizationUnitDomainService> logger,
        ICurrentUser currentUser,
        IOrganizationUnitRepository organizationUnitRepository,
        OrganizationUnitManager organizationUnitManager,
        OrganizationUnitPermissionDomainService organizationUnitPermissionDomainService,
        UnitOfWorkManager unitOfWorkManager,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager,
        IDistributedEventBus requestClient,
        IDataFilter<ISoftDelete> softDeletedDataFilter)
    {
      this.userRepository = userRepository;
      this.logger = logger;
      this.currentUser = currentUser;
      this.organizationUnitRepository = organizationUnitRepository;
      this.organizationUnitManager = organizationUnitManager;
      this.organizationUnitPermissionDomainService = organizationUnitPermissionDomainService;
      this.unitOfWorkManager = unitOfWorkManager;
      this.userManager = userManager;
      this.roleManager = roleManager;
      this.requestClient = requestClient;
      this.softDeletedDataFilter = softDeletedDataFilter;
    }

    public async Task<List<OrganizationUnit>> GetListAsync()
    {
      List<OrganizationUnit> result = null;
      try
      {
        result = await organizationUnitRepository.GetListAsync();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<OrganizationUnit> CreateAsync(OrganizationUnit orgUnit)
    {
      OrganizationUnit result = null;

      try
      {
        orgUnit.ExtraProperties["IsEnabled"] = true;
        await organizationUnitManager.CreateAsync(orgUnit);
        result = orgUnit;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<OrganizationUnit> UpdateAsync(OrganizationUnit orgUnit)
    {
      OrganizationUnit result = null;

      try
      {
        var list = await GetListAsync();
        if (list != null && list.Count > 0 && list?.FirstOrDefault(x => x.DisplayName.ToLower() == orgUnit.DisplayName.ToLower() && x.Id != orgUnit.Id) != null)
        {
          throw new UserFriendlyException("Company with same name already exist");
        }

        var oldOrgUnit = await organizationUnitRepository.GetAsync(orgUnit.Id);
        oldOrgUnit.DisplayName = orgUnit.DisplayName;
        oldOrgUnit.ExtraProperties["IsEnabled"] = orgUnit.ExtraProperties["IsEnabled"];
        await organizationUnitManager.UpdateAsync(oldOrgUnit);
        result = oldOrgUnit;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> Move(Guid id, Guid newParentId)
    {
      bool result = false;

      try
      {
        if (newParentId == Guid.Empty)
        {
          await organizationUnitManager.MoveAsync(id, null);
        }
        else
        {
          await organizationUnitManager.MoveAsync(id, newParentId);
        }

        result = true;
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task DeleteAsync(Guid orgUnitId)
    {
      try
      {
        await organizationUnitManager.DeleteAsync(orgUnitId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task<List<UserOrganizationUnitLookup>> GetAvailableForCurrentUser()
    {
      List<UserOrganizationUnitLookup> result = null;

      try
      {
        result = await GetAvailableForUser(currentUser.Id.Value);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    // UserOrganizationUnitLookupDto
    public async Task<List<UserOrganizationUnitLookup>> GetAvailableForUser(Guid userId)
    {
      List<UserOrganizationUnitLookup> result = null;

      try
      {
        var allOrganizationUnits = new List<OrganizationUnit>();

        var currentUser = await userManager.GetByIdAsync(userId);
        allOrganizationUnits.AddRange(await userManager.GetOrganizationUnitsAsync(currentUser));

        foreach (var roleId in currentUser.Roles.Select(r => r.RoleId))
        {
          allOrganizationUnits.AddRange(await GetRoleOrganizationUnits(roleId));
        }

        allOrganizationUnits = allOrganizationUnits
            .DistinctBy(o => o.Id)
            .Where(a =>
            {
              if (a.ExtraProperties.TryGetValue("IsEnabled", out var isEnabled))
              {
                return Convert.ToBoolean(isEnabled);
              }

              return false;
            })
            .ToList();

        result = allOrganizationUnits
            .Select(x => new UserOrganizationUnitLookup { OrganizationUnit = x, IsMember = true })
            .ToList();

        var withParrents = (await GetParents(allOrganizationUnits))
            .Where(a =>
            {
              if (a.ExtraProperties.TryGetValue("IsEnabled", out var isEnabled))
              {
                return Convert.ToBoolean(isEnabled);
              }

              return false;
            });
        foreach (var unit in withParrents)
        {
          if (allOrganizationUnits.FirstOrDefault(x => x.Id == unit.Id) == null)
          {
            result.Add(new UserOrganizationUnitLookup
            {
              OrganizationUnit = unit,
              IsMember = false
            });
          }
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<OrganizationUnit>> GetUserOrganizationUnits(Guid userId)
    {
      List<OrganizationUnit> result = null;
      try
      {
        var user = await userManager.GetByIdAsync(userId);
        result = await userManager.GetOrganizationUnitsAsync(user);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task SetUserOrganizationUnits(Guid userId, List<Guid> organizationUnitIds)
    {
      try
      {
        var allOrgUnits = await GetListAsync();
        var newOrgUnits = allOrgUnits
            .Where(x => organizationUnitIds.Contains(x.Id))
            .ToList();

        var user = await userManager.GetByIdAsync(userId);
        var oldOrgUnits = await userManager.GetOrganizationUnitsAsync(user);

        var dif = oldOrgUnits.Difference(newOrgUnits, x => x.Id);
        foreach (var removed in dif.Removed)
        {
          await userManager.RemoveFromOrganizationUnitAsync(user, removed);
        }

        foreach (var added in dif.Added)
        {
          await userManager.AddToOrganizationUnitAsync(user, added);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<List<OrganizationUnit>> GetRoleOrganizationUnits(Guid roleId)
    {
      List<OrganizationUnit> result = null;
      try
      {
        var allOrgUnits = await organizationUnitRepository.GetListAsync(includeDetails: true);

        result = allOrgUnits
            .Where(x => x.IsInRole(roleId))
            .ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<OrganizationUnit>> GetRoleOrganizationUnits(string roleName)
    {
      List<OrganizationUnit> result = null;
      try
      {
        var allOrgUnits = await organizationUnitRepository.GetListAsync(includeDetails: true);
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
          throw new Exception("Invalid role name");
        }

        result = allOrgUnits
            .Where(x => x.IsInRole(role.Id))
            .ToList();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task SetRoleOrganizationUnits(string roleName, List<Guid> organizationUnitIds)
    {
      try
      {
        var allOrgUnits = await organizationUnitRepository.GetListAsync(includeDetails: true);
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
          throw new Exception("Invalid role name");
        }

        var newOrgUnits = allOrgUnits
            .Where(x => x.Id.IsIn(organizationUnitIds))
            .ToList();

        var oldOrgUnits = allOrgUnits
            .Where(x => x.IsInRole(role.Id))
            .ToList();

        var dif = oldOrgUnits.Difference(newOrgUnits, x => x.Id);
        foreach (var added in dif.Added)
        {
          await organizationUnitManager.AddRoleToOrganizationUnitAsync(role, added);
        }

        foreach (var removed in dif.Removed)
        {
          await organizationUnitManager.RemoveRoleFromOrganizationUnitAsync(role, removed);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    public async Task<OrganizationUnit> GetByIdAsync(Guid id, bool includeSoftDeleted = false)
    {
      OrganizationUnit result = null;
      try
      {
        if (includeSoftDeleted)
        {
          using (softDeletedDataFilter.Disable())
          {
            result = await organizationUnitRepository.FindAsync(id, true);
          }
        }
        else
        {
          result = await organizationUnitRepository.FindAsync(id, true);
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    private async Task<List<OrganizationUnit>> GetParents(List<OrganizationUnit> orgUnits)
    {
      var result = new Dictionary<Guid, OrganizationUnit>(orgUnits.Select(x => KeyValuePair.Create(x.Id, x)));

      var parentQueue = new Queue<Guid>(
          orgUnits
          .Where(x => x.ParentId != null)
          .Select(x => (Guid)x.ParentId));

      while (parentQueue.Any())
      {
        var idToLoad = parentQueue.Dequeue();

        if (result.ContainsKey(idToLoad))
        {
          continue;
        }

        var loaded = await organizationUnitRepository.FindAsync(idToLoad);
        if (loaded != null)
        {
          result[loaded.Id] = loaded;
          if (loaded.ParentId.HasValue)
          {
            parentQueue.Enqueue(loaded.ParentId.Value);
          }
        }
      }

      return result.Values.ToList();
    }

    private static readonly SemaphoreSlim AddMemberSemaphore = new SemaphoreSlim(1, 1);

    public async Task AddMemberAsync(Guid userId, Guid orgUnitId)
    {
      try
      {
        await AddMemberSemaphore.WaitAsync();
        try
        {
          await userManager.AddToOrganizationUnitAsync(userId, orgUnitId);
        }
        finally
        {
          AddMemberSemaphore.Release();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task RemoveMember(Guid userId, Guid orgUnitId)
    {
      try
      {
        await userManager.RemoveFromOrganizationUnitAsync(userId, orgUnitId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    private static readonly SemaphoreSlim AddRoleSemaphore = new SemaphoreSlim(1, 1);
    public async Task AddRoleAsync(Guid roleId, Guid orgUnitId)
    {
      try
      {
        await AddRoleSemaphore.WaitAsync();

        try
        {
          await organizationUnitManager.AddRoleToOrganizationUnitAsync(roleId, orgUnitId);
        }
        finally
        {
          AddRoleSemaphore.Release();
        }
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task RemoveRole(Guid roleId, Guid orgUnitId)
    {
      try
      {
        await organizationUnitManager.RemoveRoleFromOrganizationUnitAsync(roleId, orgUnitId);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task<List<IdentityUser>> GetMembers(OrganizationUnit orgUnit)
    {
      List<IdentityUser> result = null;
      try
      {
        result = await organizationUnitRepository.GetMembersAsync(orgUnit);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<List<CommonRoleValueObject>> GetRoles(OrganizationUnit orgUnit)
    {
      List<CommonRoleValueObject> result = new List<CommonRoleValueObject>();
      try
      {
        result = await organizationUnitPermissionDomainService.GetRoles(orgUnit);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }

      return result;
    }

    public async Task<TreeNodeValueObject<OrganizationUnit>> CloneAsync(
        Guid id,
        string newName,
        bool withRoles,
        bool withMembers,
        bool withChildren,
        Guid? parentId = null)
    {
      TreeNodeValueObject<OrganizationUnit> result = null;
      try
      {
        var oldOrgUnit = await GetByIdAsync(id);

        result = new TreeNodeValueObject<OrganizationUnit>()
        {
          Value = new OrganizationUnit(
                GuidGenerator.Create(),
                newName,
                parentId.HasValue ? parentId : oldOrgUnit.ParentId,
                oldOrgUnit.TenantId),
          Children = new(),
        };

        object isEnabled;
        if (oldOrgUnit.ExtraProperties.TryGetValue("IsEnabled", out isEnabled))
        {
          var value = Convert.ToBoolean(isEnabled);
          result.Value.ExtraProperties["IsEnabled"] = value;
        }

        using (var unitOfWork = unitOfWorkManager.Begin())
        {
          await organizationUnitManager.CreateAsync(result.Value);
          await unitOfWork.SaveChangesAsync();
        }

        if (withRoles)
        {
          foreach (var role in await GetRoles(oldOrgUnit))
          {
            if (!role.IsReadOnly)
            {
              await AddRoleAsync(role.Id, result.Value.Id);
            }
          }
        }

        if (withMembers)
        {
          foreach (var member in await GetMembers(oldOrgUnit))
          {
            await AddMemberAsync(member.Id, result.Value.Id);
          }
        }

        if (withChildren)
        {
          var oldChildren = await organizationUnitRepository.GetChildrenAsync(id, true);
          foreach (var oldChild in oldChildren)
          {
            TreeNodeValueObject<OrganizationUnit> newChild = await CloneAsync(
                oldChild.Id,
                oldChild.DisplayName,
                withRoles,
                withMembers,
                true,
                result.Value.Id);

            result.Children.Add(newChild);
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

      return result;
    }

    //public async Task CreateCompanyOrgUnit(OrganizationUnit organizationUnit, OrganizationUnit oldOrgUnit)
    //{
    //    try
    //    {
    //        var oldCompany = await companyRepository.GetCompanyByOrgUnit(oldOrgUnit.Id);
    //        var valueObject = new CompanyValueObject
    //        {
    //            OrganizationUnitName = organizationUnit.DisplayName,
    //            OrganizationUnitId = organizationUnit.Id,
    //            IsLocal = true,
    //            IsTransferred = false,
    //            IsEnabled = true,
    //            LocalCurrencyCode = oldCompany.LocalCurrencyCode,
    //            EntityName = organizationUnit.DisplayName,
    //            SystemCurrencyCode = oldCompany.SystemCurrencyCode,
    //            ParentUid = oldOrgUnit.ExtraProperties.ContainsKey("ParentUid")
    //           ? oldOrgUnit.ExtraProperties["ParentUid"].ToString()
    //           : null
    //        };

    //        var newId = GuidGenerator.Create();
    //        valueObject.EntityUid = UidStaticHelper.AppendUnitUid(
    //            valueObject.ParentUid,
    //            UidStaticHelper.CreateUnitUidUnit(BusinessObjectType.BuCompany, newId)
    //        );

    //        await companyRepository.UpdateAsync(valueObject, newId);

    //        var request = new SetDefaultSettingMsg
    //        {
    //            CompanyId = newId.ToString()
    //        };
    //        var response = await requestClient.RequestAsync<DefaultSettingGotMsg>(request);
    //        if (!string.IsNullOrEmpty(response.Response))
    //        {
    //            throw new UserFriendlyException(response.Response);
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        logger.Capture(e);
    //    }
    //    finally
    //    {
    //    }
    //}

    public async Task<bool> CheckIfMemberExist(List<IdentityUser> members)
    {
      bool result = false;
      try
      {
        if (!currentUser.Id.HasValue)
        {
          throw new AbpAuthorizationException();
        }

        if (members.FirstOrDefault(x => x.Id == currentUser.Id) != null)
        {
          return true;
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

    public async Task<bool> CheckIfOrgUnitIncludeUser(Guid orgUnitId, Guid? userId = null)
    {
      bool result = false;
      try
      {
        OrganizationUnit organizationUnit = await organizationUnitRepository.FindAsync(orgUnitId);
        if (organizationUnit != null)
        {
          var checkedUserId = userId ?? currentUser.Id.Value;
          var membersIds = await organizationUnitRepository.GetMemberIdsAsync(orgUnitId);
          result = membersIds.Contains(checkedUserId);
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

    public async Task<bool> CheckPermissionAsync(Guid orgUnitId, string permission)
    {

      try
      {
        return await organizationUnitPermissionDomainService.CheckOrgUnitPermission(orgUnitId, permission);
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

    public async Task<bool> CheckParentsPermissionAsync(Guid orgUnitId, string permission)
    {

      try
      {
        return await organizationUnitPermissionDomainService.CheckOrgUnitParentsPermission(orgUnitId, permission);
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

    public async Task<KeyValuePair<long, List<IdentityUser>>> GetAllUnitAndChildsMembersAsync(
        Guid unitId,
        string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        string searchQuery = null)
    {
      try
      {
        string pattern = searchQuery == null ? null : $"%{searchQuery}%";

        var currentUnit = await organizationUnitRepository.GetAsync(unitId);
        var orgUnits = await organizationUnitRepository.GetAllChildrenWithParentCodeAsync(currentUnit.Code, currentUnit.Id, true);
        orgUnits.Add(currentUnit);

        var orgUnitIds = orgUnits.Select(x => x.Id).Distinct().ToList();
        var roleIds = orgUnits.SelectMany(x => x.Roles).Select(x => x.RoleId).Distinct().ToList();

        var dbSet = await userRepository.GetDbSetAsync();
        var query = dbSet
            .IncludeDetails()
            .Where(
            x => x.Roles.Select(x => x.RoleId).Any(x => roleIds.Contains(x)) ||
            x.OrganizationUnits.Select(x => x.OrganizationUnitId).Any(x => orgUnitIds.Contains(x)));

        query = query.WhereIf(pattern != null, x => EF.Functions.Like(x.Name, pattern) || EF.Functions.Like(x.UserName, pattern) || EF.Functions.Like(x.Surname, pattern));

        var count = await query.CountAsync();

        var result = await query
            .OrderBy(sorting ?? "username asc")
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync();

        return new KeyValuePair<long, List<IdentityUser>>(count, result);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
        throw;
      }
      finally
      {
      }
    }
  }
}
