using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using EleonsoftProxy.Api;
using EleonsoftProxy.Model;
using EleoncoreProxy.Api;
using EleoncoreProxy.Model;

namespace Eleoncore.SDK.MicroserviceInitialization
{
  public class EleoncoreMicroserviceInitializer : ISingletonDependency
  {
    private const string ServiceIdKey = "ServiceId";
    private const string ServiceNameKey = "Name";
    private readonly ILogger<EleoncoreMicroserviceInitializer> logger;
    private readonly IConfiguration configuration;
    private readonly IMicroserviceApi microserviceApi;
    private MessagingMicroserviceInfoEto info = null;

    public EleoncoreMicroserviceInitializer(
        ILogger<EleoncoreMicroserviceInitializer> logger,
        IConfiguration configuration,
        IMicroserviceApi microserviceApi)
    {
      this.logger = logger;
      this.configuration = configuration;
      this.microserviceApi = microserviceApi;
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
        logger.LogError(ex, "GetOrCreateServiceId error");
        throw;
      }
      return result;
    }

    public async Task InitializeMicroservice(Guid? requestId, List<EleoncoreFeatureGroupDescription> eleoncoreFeatureGroupDescriptions)
    {
      logger.LogDebug("InitializeMicroservice started");
      try
      {
        if (info == null)
        {
          var id = await GetOrCreateServiceId();
          info = new MessagingMicroserviceInfoEto
          {
            ServiceId = id,
            DisplayName = await ManifestHelper.GetManifestSetting<string>(ServiceNameKey),
            Features = eleoncoreFeatureGroupDescriptions,
          };
        }

        var initMsg = new MessagingInitializeMicroserviceMsg();
        initMsg.Info = info;
        initMsg.RequestId = requestId;

        microserviceApi.UseApiAuth();

        var response = await microserviceApi.SitesManagementMicroserviceInitializeMicroserviceAsync(initMsg);
        if (response.TryOk(out var result))
        {
          logger.LogDebug("Microservice {0} is initialized", info.DisplayName);
        }

        microserviceApi.UseOAuthAuth();
      }
      catch (Exception ex)
      {
        logger.LogError(ex, "InitializeMicroservice error");
      }

      logger.LogDebug("InitializeMicroservice finished");
    }

    public async Task<Guid> GetOrCreateServiceId()
    {
      if (Guid.TryParse(await ManifestHelper.GetManifestSetting<string>(ServiceIdKey), out var id))
      {
        return id;
      }

      var newId = Guid.NewGuid();
      await ManifestHelper.AddOrUpdateManifestSetting(ServiceIdKey, newId);
      return newId;
    }
  }
}
