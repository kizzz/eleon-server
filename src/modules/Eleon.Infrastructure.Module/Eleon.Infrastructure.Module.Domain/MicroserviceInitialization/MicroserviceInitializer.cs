using Common.Module.Helpers;
using Common.Module.Permissions;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Configuration;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Guids;

namespace Authorization.Module.MicroserviceInitialization
{
  public class MicroserviceInitializer : ISingletonDependency
  {
    private const string ServiceIdKey = "ServiceId";
    private const string ServiceNameKey = "Name";
    private readonly IVportalLogger<MicroserviceInitializer> logger;
    private readonly IGuidGenerator guidGenerator;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IConfiguration configuration;
    private readonly FeaturePermissionDefinitionProvider featureProvider;
    private MicroserviceInfoEto info = null;

    public MicroserviceInitializer(
        IVportalLogger<MicroserviceInitializer> logger,
        IGuidGenerator guidGenerator,
        IDistributedEventBus massTransitPublisher,
        IConfiguration configuration,
        FeaturePermissionDefinitionProvider featureProvider)
    {
      this.logger = logger;
      this.guidGenerator = guidGenerator;
      this.massTransitPublisher = massTransitPublisher;
      this.configuration = configuration;
      this.featureProvider = featureProvider;
    }

    public async Task<Guid> GetServiceId()
    {
      Guid result = default;
      try
      {
        result = await GetOrCreateServiceId();
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return result;
    }

    public async Task InitializeMicroservice(Guid? requestId)
    {
      try
      {
        if (info == null)
        {
          var id = await GetOrCreateServiceId();
          info = new MicroserviceInfoEto
          {
            ServiceId = id,
            DisplayName = await ManifestHelper.GetManifestSetting<string>(ServiceNameKey),
            Features = featureProvider.GetDefinitions(),
          };
        }

        var initMsg = new InitializeMicroserviceMsg();
        initMsg.Info = info;
        initMsg.RequestId = requestId;
        await massTransitPublisher.PublishAsync(initMsg);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

    }

    private async Task<Guid> GetOrCreateServiceId()
    {
      if (Guid.TryParse(await ManifestHelper.GetManifestSetting<string>(ServiceIdKey), out var id))
      {
        return id;
      }

      var newId = guidGenerator.Create();
      await ManifestHelper.AddOrUpdateManifestSetting(ServiceIdKey, newId);
      return newId;
    }
  }
}
