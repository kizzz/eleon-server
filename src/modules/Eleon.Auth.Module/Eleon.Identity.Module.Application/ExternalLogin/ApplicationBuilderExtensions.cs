using Microsoft.AspNetCore.Builder;
using System;

namespace ExternalLogin.Module
{
  public static class ApplicationBuilderExtensions
  {
    public static void UseMultiTenantAuthentication(this IApplicationBuilder app, bool enableMultitenancy, Action<IApplicationBuilder> configureAuthentication)
    {
      if (enableMultitenancy)
      {
        app.UseMiddleware<NonAuthorizedTenantResolveMiddleware>();
      }

      configureAuthentication?.Invoke(app);

      if (enableMultitenancy)
      {
        app.UseMultiTenancy();
      }
    }
  }
}
