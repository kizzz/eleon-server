using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Http.Modeling;
using Eleon.Host.Module.Eleoncore.Host.Module.HttpApi;

namespace VPortal.Abp;

[Area(HostRemoteServiceConsts.ModuleName)]
[RemoteService(Name = HostRemoteServiceConsts.RemoteServiceName)]
[Route("api/abp/api-definition")]
public class AbpApiDefinitionController : AbpController, IRemoteService
{
  protected readonly IApiDescriptionModelProvider ModelProvider;

  public AbpApiDefinitionController(IApiDescriptionModelProvider modelProvider)
  {
    ModelProvider = modelProvider;
  }

  [HttpGet]
  public virtual ApplicationApiDescriptionModel Get(ApplicationApiDescriptionModelRequestDto model)
  {
    var result = ModelProvider.CreateApiModel(model);
    //foreach (var param in result.Modules.Values.SelectMany(x => x.Controllers.Select(x => x.Value).SelectMany(x => x.Interfaces).SelectMany(x => x.Methods).SelectMany(x => x.ParametersOnMethod)))
    //{
    //    if (param.TypeSimple.Contains("enum"))
    //    {
    //        param.TypeSimple = param.Type;
    //        param.TypeSimple.Replace("System.Guid", "string");
    //        param.TypeSimple.Replace("System.String", "string");
    //    }
    //}

    return result;
  }
}
