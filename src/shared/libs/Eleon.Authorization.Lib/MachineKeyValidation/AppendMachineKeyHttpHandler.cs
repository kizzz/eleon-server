using Common.Module.Constants;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace VPortal.Identity.Module.MachineKeyValidation
{
  public class AppendMachineKeyHttpHandler : DelegatingHandler, ITransientDependency
  {
    private readonly MachineKeyHeaderGenerator machineKeyHeaderGenerator;

    public AppendMachineKeyHttpHandler(MachineKeyHeaderGenerator machineKeyHeaderGenerator)
    {
      this.machineKeyHeaderGenerator = machineKeyHeaderGenerator;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
      bool tokenPresent = request.Headers.TryGetValues("Authorization", out var tokens);
      if (tokenPresent && tokens.Any())
      {
        string token = tokens.First();
        string machineKeyHeader = await machineKeyHeaderGenerator.GenerateMachineKeyHeader(token);
        request.Headers.TryAddWithoutValidation(VPortalExtensionGrantsConsts.MachineKey.MachineKeyHeader, machineKeyHeader);
      }

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
