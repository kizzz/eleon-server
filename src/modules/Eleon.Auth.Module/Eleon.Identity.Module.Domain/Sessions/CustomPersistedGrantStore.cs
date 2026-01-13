using Logging.Module;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.Grants;
using Volo.Abp.ObjectMapping;

namespace VPortal.Identity.Module.Sessions;
public class CustomPersistedGrantStore : PersistedGrantStore, IScopedDependency
{
  private readonly IVportalLogger<CustomPersistedGrantStore> logger;
  private readonly IHttpContextAccessor httpContextAccessor;

  public CustomPersistedGrantStore(
      IVportalLogger<CustomPersistedGrantStore> logger,
      IPersistentGrantRepository persistentGrantRepository,
      IObjectMapper<AbpIdentityServerDomainModule> objectMapper,
      IGuidGenerator guidGenerator,
      IHttpContextAccessor httpContextAccessor
      ) : base(persistentGrantRepository, objectMapper, guidGenerator)
  {
    this.logger = logger;
    this.httpContextAccessor = httpContextAccessor;
  }

  public override async Task StoreAsync(IdentityServer4.Models.PersistedGrant grant)
  {
    var entity = await PersistentGrantRepository.FindByKeyAsync(grant.Key);
    if (entity == null)
    {
      entity = ObjectMapper.Map<IdentityServer4.Models.PersistedGrant, PersistedGrant>(grant);
      EntityHelper.TrySetId(entity, () => GuidGenerator.Create());
      SetExtraData(entity);
      await PersistentGrantRepository.InsertAsync(entity);
    }
    else
    {
      var sessionId = entity.SessionId;
      ObjectMapper.Map(grant, entity);
      entity.SessionId = sessionId;
      await PersistentGrantRepository.UpdateAsync(entity);
    }
  }

  private void SetExtraData(PersistedGrant entity)
  {
    var device = ParseSessionHelper.GetDeviceInfo(httpContextAccessor.HttpContext?.Request.Headers.UserAgent.FirstOrDefault());
    entity.SessionId = ParseSessionHelper.GenerateSessionId(httpContextAccessor.HttpContext);
    entity.SetProperty("Device", $"{device.DeviceName} {device.OsName} {device.OsVersion}");
    entity.SetProperty("DeviceInfo", device);
    entity.SetProperty("Browser", device.BrowserName);
    entity.SetProperty("Ip", ParseSessionHelper.GetIpAddress(httpContextAccessor.HttpContext));
  }
}

