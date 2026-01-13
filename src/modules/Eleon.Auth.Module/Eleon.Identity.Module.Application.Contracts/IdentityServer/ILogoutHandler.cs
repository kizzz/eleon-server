
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Identity.Module.Application.Contracts.IdentityServerServices;
public interface ILogoutHandler : IScopedDependency
{
  public Task ExecuteAsync();
}
