using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Volo.Abp.Domain.Services;
using VPortal.SitesManagement.Module.Consts;
using VPortal.SitesManagement.Module.Entities;

namespace VPortal.SitesManagement.Module.Managers
{
  public class ModuleConfiguration
  {
    [JsonPropertyName("name")]
    public string Name { get; set; }
    [JsonPropertyName("submodules")]
    public Dictionary<string, string> Submodules { get; set; }
  }
  public class ClientAutodetectManager : DomainService
  {
    private readonly IVportalLogger<ClientAutodetectManager> logger;
    private readonly HttpClient httpClient;

    public ClientAutodetectManager(
        IVportalLogger<ClientAutodetectManager> logger,
        HttpClient httpClient)
    {
      this.logger = logger;
      this.httpClient = httpClient;
    }


    public async Task<List<ModuleEntity>> GetDetected(string url)
    {
      List<ModuleEntity> result = new List<ModuleEntity>();

      try
      {
        var response = await httpClient.GetAsync($"{url}/assets/module-configuration.manifest.json");
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var moduleConfig = JsonSerializer.Deserialize<ModuleConfiguration>(jsonString);

        foreach (var submodule in moduleConfig.Submodules)
        {
          var moduleEntity = new ModuleEntity(GuidGenerator.Create())
          {
            DisplayName = submodule.Key,
            Path = $"/client-modules/{submodule.Key}",
            IsEnabled = false,
            Type = ModuleType.Client,
            Source = submodule.Value
          };

          try
          {
            var submoduleResponse = await httpClient.GetAsync($"{submodule.Value}/assets/{submodule.Key}/module-configuration.manifest.json");
            submoduleResponse.EnsureSuccessStatusCode();

            var submoduleJson = await submoduleResponse.Content.ReadAsStringAsync();
          }
          catch (Exception submoduleException)
          {
            logger.CaptureAndSuppress(submoduleException);
            moduleEntity.HealthCheckStatusMessage = "Error with module: " + submoduleException.Message;
          }

          result.Add(moduleEntity);
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
  }
}


