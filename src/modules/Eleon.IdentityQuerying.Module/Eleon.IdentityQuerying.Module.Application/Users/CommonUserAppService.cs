using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.OrganizationUnits;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Roles;
using Eleon.IdentityQuerying.Module.Full.Eleon.IdentityQuerying.Module.Application.Contracts.Users;
using Eleon.InternalCommons.Lib.Messages.Otp;
using Logging.Module;
using Messaging.Module.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Migrations.Module;
using System.Linq;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using VPortal.HealthCheckModule.Module;
using VPortal.TenantManagement.Module.DomainServices;
using VPortal.TenantManagement.Module.ValueObjects;

namespace VPortal.Core.Infrastructure.Module.Users
{
  [Authorize(IdentityPermissions.Users.Default)]
  public class CommonUserAppService : IdentityQueryingAppService, ICommonUserAppService
  {
    private readonly UserDomainService userDomainService;
    private readonly IVportalLogger<CommonUserAppService> logger;
    private readonly OrganizationUnitDomainService organizationUnitDomainService;
    private readonly IPermissionChecker permissionChecker;
    private readonly RoleDomainService roleDomainService;
    private readonly IIdentityUserAppService identityUserAppService;
    private readonly IDistributedEventBus _eventBus;

    public CommonUserAppService(
        UserDomainService userDomainService,
        IVportalLogger<CommonUserAppService> logger,
        OrganizationUnitDomainService organizationUnitDomainService,
        IPermissionChecker permissionChecker,
        RoleDomainService roleDomainService,
        IIdentityUserAppService identityUserAppService,
        IdentityUserManager userManager,
        IIdentityUserRepository identityUserRepository,
        IIdentityRoleRepository identityRoleRepository,
        IOptions<Microsoft.AspNetCore.Identity.IdentityOptions> identityOptions,
        IDistributedEventBus eventBus)
    {
      this.userDomainService = userDomainService;
      this.logger = logger;
      this.organizationUnitDomainService = organizationUnitDomainService;
      this.permissionChecker = permissionChecker;
      this.identityUserAppService = identityUserAppService;
      this.roleDomainService = roleDomainService;
      _eventBus = eventBus;
    }

    [Authorize(AuthenticationSchemes = "Bearer,ApiKey")]
    public async Task<PagedResultDto<CommonUserDto>> GetListAsync(GetCommonUsersInput input)
    {

      PagedResultDto<CommonUserDto> response = null;

      try
      {
        input.IgnoredUsers ??= new List<Guid>();
        if (input.IgnoreCurrentUser && CurrentUser.Id.HasValue && !input.IgnoredUsers.Contains(CurrentUser.Id.Value))
        {
          input.IgnoredUsers.Add(CurrentUser.Id.Value);
        }
        var source = await userDomainService.GetListAsync(input.Sorting,
                                                          input.MaxResultCount,
                                                          input.SkipCount,
                                                          input.Filter,
                                                          input.Permissions,
                                                          input.IgnoredUsers);

        var Items = ObjectMapper.Map<List<IdentityUser>, List<CommonUserDto>>(source.Value);
        response = new PagedResultDto<CommonUserDto>()
        {
          Items = Items,
          TotalCount = source.Key,
        };

        if (response != null && response.Items.Count > 0)
        {
          foreach (var user in response.Items)
          {
            try
            {
              user.Roles = await roleDomainService.GetRolesByUserId(user.Id);
              var orgUnits = await organizationUnitDomainService.GetUserOrganizationUnits(user.Id);

              if (orgUnits != null)
              {
                user.OrganizationUnits = ObjectMapper.Map<List<Volo.Abp.Identity.OrganizationUnit>, List<CommonOrganizationUnitDto>>(orgUnits);
              }
            }
            catch (Exception ex)
            {
              logger.CaptureAndSuppress(ex);
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

      return response;
    }

    public async Task<List<CommonUserDto>> GetAllUsersListAsync()
    {
      List<CommonUserDto> response = null;
      try
      {
        var users = await userDomainService.GetAllUsersListAsync();

        response = ObjectMapper.Map<List<IdentityUser>, List<CommonUserDto>>(users);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }

      return response;
    }

    public async Task<CommonUserDto> GetById(Guid id)
    {

      CommonUserDto result = null;
      try
      {
        IdentityUser entity = await userDomainService.GetById(id);

        result = ObjectMapper.Map<IdentityUser, CommonUserDto>(entity);
        if (result != null)
        {
          result.Roles = await roleDomainService.GetRolesByUserId(result.Id);
          result.LastLoginDate = await userDomainService.GetUserLastLoginDate(result.Id);
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

    public async Task<List<CommonRoleDto>> GetRoles(Guid id)
    {

      List<CommonRoleDto> result = null;
      try
      {
        var roles = await userDomainService.GetRoles(id);

        result = ObjectMapper.Map<List<IdentityRole>, List<CommonRoleDto>>(roles);
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

    [AllowAnonymous]
    public async Task<bool> CheckPermission(string permission)
    {
      bool result = false;
      try
      {
        result = await permissionChecker.IsGrantedAsync(permission);
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

    [AllowAnonymous]
    public async Task<IdentityUserDto> GetCurrentUser()
    {
      IdentityUserDto result = null;
      try
      {
        IdentityUser entity = await userDomainService.GetCurrentUser();

        result = ObjectMapper.Map<IdentityUser, IdentityUserDto>(entity);
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

    public async Task<ListResultDto<IdentityRoleDto>> GetIdentityRolesAsync(Guid id)
    {
      return await this.identityUserAppService.GetRolesAsync(id);
    }

    public async Task DeleteAsync(Guid id)
    {
      var user = await userDomainService.GetById(id);
      if (user.UserName == MigrationConsts.AdminUserNameDefaultValue)
      {
        throw new Exception("Cannot delete SA user.");

      }

      await this.identityUserAppService.DeleteAsync(id);

      await _eventBus.PublishAsync(new UserRemovedMsg
      {
        UserId = id
      });
    }

    public Task<ListResultDto<IdentityRoleDto>> GetAssignableRolesAsync()
    {
      return this.identityUserAppService.GetAssignableRolesAsync();
    }

    public Task<IdentityUserDto> FindByUsernameAsync(string userName)
    {
      return this.identityUserAppService.FindByUsernameAsync(userName);
    }

    public Task<IdentityUserDto> FindByEmailAsync(string email)
    {
      return this.identityUserAppService.FindByEmailAsync(email);
    }

    public Task<IdentityUserDto> GetAsync(Guid id)
    {
      return this.identityUserAppService.GetAsync(id);
    }
  }
}



