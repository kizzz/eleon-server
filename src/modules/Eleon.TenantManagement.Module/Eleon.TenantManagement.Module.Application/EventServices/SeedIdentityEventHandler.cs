using Common.EventBus.Module;
using Eleon.InternalCommons.Lib.Messages.Identity;
using Logging.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Uow;
using VPortal.Data;

namespace Eleon.TenantManagement.Module.Eleon.TenantManagement.Module.Application.EventServices;
public class SeedIdentityEventHandler : IDistributedEventHandler<SeedIdentityRequestMsg>, ITransientDependency
{
  private readonly IdentityDataSeeder _identityDataSeeder;
  private readonly IResponseContext _responseContext;
  private readonly IVportalLogger<SeedIdentityEventHandler> _logger;
  private readonly IUnitOfWorkManager _unitOfWorkManager;

  public SeedIdentityEventHandler(IdentityDataSeeder identityDataSeeder, IResponseContext responseContext, IVportalLogger<SeedIdentityEventHandler> logger, IUnitOfWorkManager unitOfWorkManager)
  {
    _identityDataSeeder = identityDataSeeder;
    _responseContext = responseContext;
    _logger = logger;
    _unitOfWorkManager = unitOfWorkManager;
  }

  public async Task HandleEventAsync(SeedIdentityRequestMsg eventData)
  {
    using (var uow = _unitOfWorkManager.Begin(true))
    {
      var response = new SeedIdentityResponseMsg { Success = false, Message = "Seeding failed." };
      try
      {
        var seedResult = await _identityDataSeeder.SeedAsync(eventData.AdminEmail, eventData.AdminPassword, eventData.TenantId, eventData.AdminUserName);

        response.Success = seedResult.CreatedAdminRole && seedResult.CreatedAdminUser;
        if (response.Success)
        {
          response.Message = "Seeding completed successfully.";
        }
        else if (!seedResult.CreatedAdminRole && !seedResult.CreatedAdminUser)
        {
          response.Message = "Seeding completed with warnings: Admin role and user were not created.";
        }
        else if (!seedResult.CreatedAdminRole)
        {
          response.Message = "Seeding completed with warnings: Admin role was not created.";
        }
        else if (!seedResult.CreatedAdminUser)
        {
          response.Message = "Seeding completed with warnings: Admin user was not created.";
        }
      }
      catch (Exception ex)
      {
        _logger.Capture(ex);
        response.Success = false;
        response.Message = $"Seeding exception: {ex.Message}";
      }
      finally
      {
        await _responseContext.RespondAsync(response);
      }
    }
  }
}
