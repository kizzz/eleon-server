using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using VPortal.ProxyClient.Domain.Shared.Constants;
using System.ServiceProcess;
using System.Net.Http.Headers;
using VPortal.ProxyClient.Domain.Windows.ServiceHelpers;

namespace VPortal.ProxyClient.Domain.Windows.DomainServices
{
    public class WindowsProxyClientHostServiceService : ITransientDependency
    {
        private readonly ILogger<WindowsProxyClientHostServiceService> logger;
        private readonly WindowsProxyClientDirectoryService directoryService;

        public WindowsProxyClientHostServiceService(
            ILogger<WindowsProxyClientHostServiceService> logger,
            WindowsProxyClientDirectoryService configService)
        {
            this.logger = logger;
            this.directoryService = configService;
        }

        public bool IsHostServiceInstalled()
        {
            logger.LogDebug("WindowsSpecificConfigService IsHostServiceInstalled started");
            bool result = false;
            try
            {
                var service = GetHostServiceController();
                result = service != null;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService IsHostServiceInstalled errored");
            }

            logger.LogDebug("WindowsSpecificConfigService IsHostServiceInstalled finished");
            return result;
        }

        public bool IsHostServiceRunning()
        {
            logger.LogDebug("WindowsSpecificConfigService IsHostServiceRunning started");
            bool result = false;
            try
            {
                var service = GetHostServiceController();
                result = service?.Status == ServiceControllerStatus.Running;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService IsHostServiceRunning errored");
            }

            logger.LogDebug("WindowsSpecificConfigService IsHostServiceRunning finished");
            return result;
        }

        public void RunHostService()
        {
            logger.LogDebug("WindowsSpecificConfigService RunHostService started");
            try
            {
                var service = GetHostServiceController();
                if (service == null)
                {
                    throw new UserFriendlyException("VPortal Proxy Client host service is not installed.");
                }

                if (service.Status is not ServiceControllerStatus.StartPending and not ServiceControllerStatus.Running)
                {
                    service.Start();
                }
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService RunHostService errored");
            }

            logger.LogDebug("WindowsSpecificConfigService RunHostService finished");
        }

        public void StopHostService()
        {
            logger.LogDebug("WindowsSpecificConfigService StopHostService started");
            try
            {
                var service = GetHostServiceController();
                if (service == null)
                {
                    throw new UserFriendlyException("VPortal Proxy Client host service is not installed.");
                }

                if (service.Status is ServiceControllerStatus.StopPending and not ServiceControllerStatus.Stopped)
                {
                    service.Stop();
                }
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService StopHostService errored");
            }

            logger.LogDebug("WindowsSpecificConfigService StopHostService finished");
        }

        public void InstallHostService()
        {
            logger.LogDebug("WindowsSpecificConfigService InstallHostService started");
            try
            {
                var service = GetHostServiceController();
                if (service != null)
                {
                    throw new UserFriendlyException("VPortal Proxy Client host service is already installed.");
                }

                var exePath = directoryService.GetHostExecutablePath();

                ServiceInstaller.Install(ProxyClientHostConsts.HostServiceName, ProxyClientHostConsts.HostServiceName, exePath);
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService InstallHostService errored");
            }

            logger.LogDebug("WindowsSpecificConfigService InstallHostService finished");
        }

        private ServiceController GetHostServiceController()
        {
            var services = ServiceController.GetServices();
            var service = services.FirstOrDefault(x => x.ServiceName == ProxyClientHostConsts.HostServiceName);
            return service;
        }
    }
}
