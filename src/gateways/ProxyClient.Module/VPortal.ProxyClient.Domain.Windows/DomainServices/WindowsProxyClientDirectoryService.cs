using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using VPortal.ProxyClient.Config;
using VPortal.ProxyClient.Domain.Shared.Constants;
using VPortal.ProxyClient.Domain.Shared.Helpers;

namespace VPortal.ProxyClient.Domain.Windows.DomainServices
{
    public class WindowsProxyClientDirectoryService : ITransientDependency
    {
        private readonly ILogger<WindowsProxyClientDirectoryService> logger;

        public WindowsProxyClientDirectoryService(ILogger<WindowsProxyClientDirectoryService> logger)
        {
            this.logger = logger;
        }

        public string GetHostExecutablePath()
        {
            logger.LogDebug("WindowsSpecificConfigService GetHostExecutablePath started");
            string? result = null;
            try
            {
                result = ExeHelper.GetHostExecutablePath();
            }
            catch (Exception ex)
            {
                logger.LogError("WindowsSpecificConfigService GetHostExecutablePath errored");
                logger.LogDebug(ex, "WindowsSpecificConfigService GetHostExecutablePath errored");
            }

            if (result == null)
            {
                throw new UserFriendlyException("No host executable found. Make sure your installation is not corrupted.");
            }

            logger.LogDebug("WindowsSpecificConfigService GetHostExecutablePath finished");
            return result;
        }

        public void OpenConfigurationLogs()
        {
            logger.LogDebug("WindowsSpecificConfigService OpenConfigurationLogs started");
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", ProxyClientConfigConsts.UiLogsDirectoryPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService OpenConfigurationLogs errored");
            }

            logger.LogDebug("WindowsSpecificConfigService OpenConfigurationLogs finished");
        }

        public void OpenHostLogs()
        {
            logger.LogDebug("WindowsSpecificConfigService OpenHostLogs started");
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", ProxyClientConfigConsts.HostLogsPath);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService OpenHostLogs errored");
            }

            logger.LogDebug("WindowsSpecificConfigService OpenHostLogs finished");
        }
    }
}
