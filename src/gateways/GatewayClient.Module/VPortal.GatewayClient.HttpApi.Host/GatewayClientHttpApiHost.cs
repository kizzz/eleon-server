using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using VPortal.GatewayClient.Domain.Auth;
using Common.Module.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Certificate;
using System.Security.Cryptography.X509Certificates;

namespace VPortal.GatewayClient.HttpApi.Host
{
    public class GatewayClientHttpApiHost
    {
        public static async Task<IHost> InitializeHost(string[] args, Action<IHostBuilder>? postConfigureHost)
        {
            Log.Information("Starting VPortal.HttpApi.Host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host
                .ConfigureAppConfiguration(cfg =>
                {
                    var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var builder = new ConfigurationBuilder();
                    cfg.SetBasePath(AppContext.BaseDirectory);
                    cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                    cfg.AddJsonFile($"appsettings.{environmentName}.json", true, true);
                    cfg.AddJsonFile("appsettings.secrets.json", optional: false, reloadOnChange: true);
                })
                .AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();

            builder.WebHost.ConfigureKestrel(server =>
            {
                server.ConfigureHttpsDefaults(https =>
                {
                    https.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
                    https.AllowAnyClientCertificate();
                    https.ClientCertificateValidation = (cert, chain, errs) =>
                    {
                        var etalonCert = LicenseHelper.GetCertificate();
                        var etalonHash = CertificateHelper.GetCertificateHash(etalonCert);
                        var certHash = CertificateHelper.GetCertificateHash(cert);
                        return string.Equals(etalonHash, certHash, StringComparison.InvariantCultureIgnoreCase);
                    };
                });
            });

            postConfigureHost?.Invoke(builder.Host);

            await builder.AddApplicationAsync<GatewayClientHttpApiHostModule>();
            var app = builder.Build();


            app.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Hello World!");
            });

            await app.InitializeApplicationAsync();
            return app;
        }
    }
}
