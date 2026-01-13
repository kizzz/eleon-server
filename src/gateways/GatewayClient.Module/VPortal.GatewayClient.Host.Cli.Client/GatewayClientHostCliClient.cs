using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Infrastructure.Module.Domain.CliBinding;
using VPortal.GatewayClient.Domain.Shared.Helpers;
using VPortal.GatewayClient.Domain.Shared.Status;

namespace VPortal.GatewayClient.Host.Cli.Client
{
    public class GatewayClientHostCliClient : ITransientDependency
    {
        private readonly ILogger<GatewayClientHostCliClient> logger;

        public GatewayClientHostCliClient(ILogger<GatewayClientHostCliClient> logger)
        {
            this.logger = logger;
        }

        public async IAsyncEnumerable<string> Register(string registrationKey)
        {
            logger.LogDebug("GatewayClientHostCliClient Register started");
            
            var cliBinding = CreateBinding($"-r {registrationKey}");
            Task read = cliBinding.RunToCompletion();
            await foreach (var stage in cliBinding.ReadLines())
            {
                if (ParseGatewayStatus(stage) == null)
                {
                    yield return stage.Trim();
                }
            }

            try
            {
                await read;
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient Register errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient Register errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }

            logger.LogDebug("GatewayClientHostCliClient Register finished");
        }

        public async Task<GatewayStatusInformation> GetGatewayStatus()
        {
            logger.LogDebug("GatewayClientHostCliClient GetGatewayStatus started");
            try
            {
                var cliBinding = CreateBinding(string.Empty);
                var status = await cliBinding.StardAndReadToEnd();
                return GatewayStatusInformation.Parse(status);
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient GetGatewayStatus errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient GetGatewayStatus errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("GatewayClientHostCliClient GetGatewayStatus finished");
            }
        }

        public async Task ResetRegistration()
        {
            logger.LogDebug("GatewayClientHostCliClient ResetRegistration started");
            try
            {
                var cliBinding = CreateBinding("-c");
                var status = await cliBinding.StardAndReadToEnd();
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient ResetRegistration errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient ResetRegistration errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("GatewayClientHostCliClient ResetRegistration finished");
            }
        }

        public async Task ChangePort(int newPort)
        {
            logger.LogDebug("GatewayClientHostCliClient ChangePort started");
            try
            {
                var cliBinding = CreateBinding($"-p {newPort}");
                _ = await cliBinding.StardAndReadToEnd();
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient ChangePort errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GatewayClientHostCliClient ChangePort errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("GatewayClientHostCliClient ChangePort finished");
            }
        }

        private GatewayStatusInformation? ParseGatewayStatus(string str)
        {
            try
            {
                return GatewayStatusInformation.Parse(str);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private CliBinding CreateBinding(string args)
        {
            string path = ExeHelper.GetHostExecutablePath()!;
            return new CliBinding(path, args);
        }
    }
}
