using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using Volo.Abp.MultiTenancy;

namespace ExternalLogin.Module
{
  public class OpenIdConnectOptionsProvider : IOptionsMonitor<OpenIdConnectOptions>
  {
    private readonly ConcurrentDictionary<(string name, string tenant), Lazy<OpenIdConnectOptions>> _cache = new();
    private readonly IOptionsFactory<OpenIdConnectOptions> _optionsFactory;
    private readonly ICurrentTenant currentTenant;

    public OpenIdConnectOptionsProvider(
        IOptionsFactory<OpenIdConnectOptions> optionsFactory,
        ICurrentTenant currentTenant)
    {
      _optionsFactory = optionsFactory;
      this.currentTenant = currentTenant;
    }

    public OpenIdConnectOptions CurrentValue => Get(Options.DefaultName);

    public OpenIdConnectOptions Get(string name)
    {
      Lazy<OpenIdConnectOptions> Create() => new Lazy<OpenIdConnectOptions>(() => _optionsFactory.Create(name));
      return _cache.GetOrAdd((name, currentTenant.Id?.ToString() ?? "host"), _ => Create()).Value;
    }

    public void ClearCache()
    {
      _cache.Clear();
    }

    public IDisposable OnChange(Action<OpenIdConnectOptions, string> listener) => null;
  }
}
