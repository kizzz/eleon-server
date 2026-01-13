using Google.Api.Gax.ResourceNames;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Optimization.V1;
using Logging.Module;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.Google.Module.Configuration;

namespace VPortal.Google.Module.DomainServices
{

  public class GoogleFleetRoutingDomainService : DomainService, IScopedDependency
  {
    private readonly IConfiguration configuration;
    private readonly GoogleKeyProvider keyProvider;
    private readonly IVportalLogger<GoogleFleetRoutingDomainService> logger;
    private FleetRoutingClient cachedClient;

    public GoogleFleetRoutingDomainService(
        IConfiguration configuration,
        GoogleKeyProvider keyProvider,
        IVportalLogger<GoogleFleetRoutingDomainService> logger)
    {
      this.configuration = configuration;
      this.keyProvider = keyProvider;
      this.logger = logger;
    }

    public async Task<OptimizeToursResponse> OptimizeRoute(OptimizeToursRequest request)
    {
      OptimizeToursResponse response = null;
      try
      {
        request.Parent = new ProjectName(configuration["Google:Optimization:ProjectName"]).ToString();
        var client = await GetClient();
        response = await client?.OptimizeToursAsync(request);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }

      return response;
    }

    private async Task<FleetRoutingClient> GetClient()
        => cachedClient ??= await CreateClient();

    private async Task<FleetRoutingClient> CreateClient()
    {
      var key = await keyProvider.GetOptimizationKey();
      if (key.IsNullOrWhiteSpace())
      {
        return null;
      }

      var serviceAccountCredential = CredentialFactory.FromJson<ServiceAccountCredential>(key);
      var credential = GoogleCredential
          .FromServiceAccountCredential(serviceAccountCredential)
          .CreateScoped(FleetRoutingClient.DefaultScopes);
      var builder = new FleetRoutingClientBuilder
      {
        Credential = credential
      };
      return builder.Build();
    }
  }
}
