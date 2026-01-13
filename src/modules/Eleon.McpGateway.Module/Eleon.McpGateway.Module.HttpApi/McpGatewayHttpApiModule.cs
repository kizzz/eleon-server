using Eleon.McpGateway.Module.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Modularity;

namespace Eleon.McpGateway.Module.HttpApi;

[DependsOn(
    typeof(McpGatewayApplicationModule),
    typeof(AbpAspNetCoreMvcModule))]
public class McpGatewayHttpApiModule : AbpModule
{
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        // ApplicationBuilder may not be available in test contexts
        IApplicationBuilder? app = null;
        try
        {
            app = context.GetApplicationBuilder();
        }
        catch (ArgumentNullException)
        {
            // No application builder in test context.
            return;
        }
        
        // Add origin validation middleware before routing
        app.UseMiddleware<McpOriginValidationMiddleware>();
    }
}

