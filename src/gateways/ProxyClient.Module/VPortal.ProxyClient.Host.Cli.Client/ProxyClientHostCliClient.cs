using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Infrastructure.Module.Domain.CliBinding;
using VPortal.ProxyClient.Domain.Shared.Helpers;
using VPortal.ProxyClient.Domain.Shared.Status;

namespace VPortal.ProxyClient.Host.Cli.Client
{
    public class ProxyClientHostCliClient : ITransientDependency
    {
        private readonly ILogger<ProxyClientHostCliClient> logger;

        public ProxyClientHostCliClient(ILogger<ProxyClientHostCliClient> logger)
        {
            this.logger = logger;
        }

        public async IAsyncEnumerable<string> Register(string registrationKey)
        {
            logger.LogDebug("ProxyClientHostCliClient Register started");
            
            var cliBinding = CreateBinding($"-r {registrationKey}");
            Task read = cliBinding.RunToCompletion();
            await foreach (var stage in cliBinding.ReadLines())
            {
                if (ParseProxyStatus(stage) == null)
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
                logger.LogError(ex, "ProxyClientHostCliClient Register errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient Register errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }

            logger.LogDebug("ProxyClientHostCliClient Register finished");
        }

        public async Task<ProxyStatusInformation> GetProxyStatus()
        {
            logger.LogDebug("ProxyClientHostCliClient GetProxyStatus started");
            try
            {
                var cliBinding = CreateBinding(string.Empty);
                var status = await cliBinding.StardAndReadToEnd();
                return ProxyStatusInformation.Parse(status);
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient GetProxyStatus errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient GetProxyStatus errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("ProxyClientHostCliClient GetProxyStatus finished");
            }
        }

        public async Task ResetRegistration()
        {
            logger.LogDebug("ProxyClientHostCliClient ResetRegistration started");
            try
            {
                var cliBinding = CreateBinding("-c");
                var status = await cliBinding.StardAndReadToEnd();
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient ResetRegistration errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient ResetRegistration errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("ProxyClientHostCliClient ResetRegistration finished");
            }
        }

        public async Task ChangePort(int newPort)
        {
            logger.LogDebug("ProxyClientHostCliClient ChangePort started");
            try
            {
                var cliBinding = CreateBinding($"-p {newPort}");
                _ = await cliBinding.StardAndReadToEnd();
            }
            catch (CliBindingProcessErrorException ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient ChangePort errored");
                throw new Volo.Abp.UserFriendlyException(ex.StdError);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ProxyClientHostCliClient ChangePort errored");
                throw new Volo.Abp.UserFriendlyException(ex.Message);
            }
            finally
            {
                logger.LogDebug("ProxyClientHostCliClient ChangePort finished");
            }
        }

        private ProxyStatusInformation? ParseProxyStatus(string str)
        {
            try
            {
                return ProxyStatusInformation.Parse(str);
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
