using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp;
using VPortal.GatewayClient.Domain.Shared.Constants;

namespace VPortal.GatewayClient.Domain.Windows.DomainServices
{
    public class WindowsGatewayClientHostProcessService : ITransientDependency
    {
        private readonly ILogger<WindowsGatewayClientHostProcessService> logger;
        private readonly WindowsGatewayClientDirectoryService configService;

        public WindowsGatewayClientHostProcessService(
            ILogger<WindowsGatewayClientHostProcessService> logger,
            WindowsGatewayClientDirectoryService configService)
        {
            this.logger = logger;
            this.configService = configService;
        }

        public async Task<bool> IsHostProcessRunning()
        {
            logger.LogDebug("WindowsSpecificConfigService IsHostProcessRunning started");
            bool result = false;
            try
            {
                using (var appRunningMutex = new Mutex(false, GatewayClientHostConsts.HostMutexName))
                {
                    bool notRunning = appRunningMutex.WaitOne(0, false);
                    result = !notRunning;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService IsHostProcessRunning errored");
            }

            logger.LogDebug("WindowsSpecificConfigService IsHostProcessRunning finished");
            return result;
        }

        public void RunHostProcess()
        {
            logger.LogDebug("WindowsSpecificConfigService RunHostProcess started");
            try
            {
                string executablePath = configService.GetHostExecutablePath();
                Process.Start(new ProcessStartInfo(executablePath, "-s console")
                {
                    WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory
                });
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService RunHostProcess errored");
            }

            logger.LogDebug("WindowsSpecificConfigService RunHostProcess finished");
        }

        public void StopHostProcess()
        {
            logger.LogDebug("WindowsSpecificConfigService StopHostProcess started");
            try
            {
                var process = GetHostProcess();
                process.Kill();
            }
            catch (BusinessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "WindowsSpecificConfigService StopHostProcess errored");
            }

            logger.LogDebug("WindowsSpecificConfigService StopHostProcess finished");
        }

        private Process? GetHostProcess()
        {
            var processes = Process.GetProcesses();
            string processName = GatewayClientHostConsts.HostAssemblyName;
            string exeName = configService.GetHostExecutablePath();
            foreach (var process in processes)
            {
                string lastPart = Path.GetFileName(process.ProcessName);
                if (lastPart == exeName || lastPart == processName)
                {
                    return process;
                }
            }

            return null;
        }
    }
}
