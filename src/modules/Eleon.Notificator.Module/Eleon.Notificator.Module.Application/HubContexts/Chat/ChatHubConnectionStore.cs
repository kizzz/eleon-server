using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.Uow;

namespace VPortal.Notificator.Module.Chat
{
  public class ChatHubConnectionStore : IChatHubConnectionStore, ISingletonDependency
  {
    private readonly IdentityUserManager userManager;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IVportalLogger<Guid> _logger;
    private List<Guid> userIds = new();
    private Dictionary<string, List<Guid>> usersByRole = new();
    private Dictionary<Guid, List<Guid>> usersByUnit = new();


    public ChatHubConnectionStore(
        IdentityUserManager userManager,
        IUnitOfWorkManager unitOfWorkManager,
        IVportalLogger<Guid> logger)
    {
      this.userManager = userManager;
      _unitOfWorkManager = unitOfWorkManager;
      _logger = logger;
    }

    public async Task AddConnectedUser(Guid userId)
    {
      try
      {
        var uow = _unitOfWorkManager.Begin(true);
        var user = await userManager.GetByIdAsync(userId);
        var roles = await userManager.GetRolesAsync(user);
        var units = await userManager.GetOrganizationUnitsAsync(user);

        foreach (var role in roles)
        {
          var usersInRole = usersByRole.GetOrAdd(role, () => new List<Guid>());
          usersInRole.Add(userId);
        }

        foreach (var unit in units)
        {
          var usersInUnit = usersByUnit.GetOrAdd(unit.Id, () => new List<Guid>());
          usersInUnit.Add(userId);
        }

        userIds.Add(userId);
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
      }
    }

    public void RemoveConnectedUser(Guid userId)
    {
      foreach (var users in usersByRole.Values)
      {
        if (users.Contains(userId))
        {
          users.Remove(userId);
        }
      }

      foreach (var users in usersByUnit.Values)
      {
        if (users.Contains(userId))
        {
          users.Remove(userId);
        }
      }

      userIds.Remove(userId);
    }

    public List<Guid> GetConnectedUsers(List<string> roles, List<Guid> exceptUsers, List<Guid> orgUnitIds, bool isPublic)
    {
      try
      {
        if (isPublic && roles.Count == 0 && orgUnitIds.Count == 0)
        {
          return userIds.Distinct().Except(exceptUsers).ToList();
        }

        return usersByUnit.Where(x => orgUnitIds.Contains(x.Key))
            .SelectMany(x => x.Value)
            .Concat(usersByRole
            .Where(x => roles.Contains(x.Key))
            .SelectMany(x => x.Value))
            .Distinct()
            .Except(exceptUsers)
            .ToList();
      }
      catch (Exception ex)
      {
        _logger.CaptureAndSuppress(ex);
        return new List<Guid>();
      }
    }
  }
}
