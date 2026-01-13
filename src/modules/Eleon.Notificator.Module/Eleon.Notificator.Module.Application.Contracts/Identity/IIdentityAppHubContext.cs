using Volo.Abp.DependencyInjection;

namespace ModuleCollector.Notificator.Module.Notificator.Module.Application.Contracts.Identity;
public interface IIdentityAppHubContext : ITransientDependency
{
  Task CheckSessionAsync(Guid userId, object data);
}
