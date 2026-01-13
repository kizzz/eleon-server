using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Networking;
using System.Windows.Input;
using Volo.Abp;
using VPortal.ProxyClient.Domain.Auth;
using VPortal.ProxyClient.Domain.Settings;
using VPortal.ProxyClient.Domain.Shared.Auth;
using VPortal.ProxyClient.Domain.Windows.DomainServices;
using VPortal.ProxyClient.UI.Windows.Models;
using VPortal.ProxyClient.ViewModels.Commands;

namespace VPortal.ProxyClient.ViewModels.ViewModels.Main
{
    public class HostViewModel : ViewModel
    {
        private readonly WindowsProxyClientHostProcessService hostProcessService;
        private readonly WindowsProxyClientHostServiceService hostServiceService;

        public HostViewModel(
            WindowsProxyClientHostProcessService hostProcessService,
            WindowsProxyClientHostServiceService serviceService,
            WindowsProxyClientDirectoryService directoryService)
            : base(directoryService)
        {
            this.hostProcessService = hostProcessService;
            this.hostServiceService = serviceService;
            SelectServiceRuntimeType();
        }

        private bool isService;
        public bool IsService
        {
            get => isService;
            set => NotifySet(ref isService, value);
        }

        private bool isProcess;
        public bool IsProcess
        {
            get => isProcess;
            set => NotifySet(ref isProcess, value);
        }

        private string processInfo;
        public string ProcessInfo
        {
            get => processInfo;
            set => NotifySet(ref processInfo, value);
        }

        private bool isProcessRunning;
        public bool IsProcessRunning
        {
            get => isProcessRunning;
            set => NotifySet(ref isProcessRunning, value);
        }

        private bool isProcessNotRunning;
        public bool IsProcessNotRunning
        {
            get => isProcessNotRunning;
            set => NotifySet(ref isProcessNotRunning, value);
        }

        private bool isServiceRunning;
        public bool IsServiceRunning
        {
            get => isServiceRunning;
            set => NotifySet(ref isServiceRunning, value);
        }

        private bool isServiceNotRunning;
        public bool IsServiceNotRunning
        {
            get => isServiceNotRunning;
            set => NotifySet(ref isServiceNotRunning, value);
        }

        private bool isServiceInstalled;
        public bool IsServiceInstalled
        {
            get => isServiceInstalled;
            set => NotifySet(ref isServiceInstalled, value);
        }

        private bool isServiceNotInstalled;
        public bool IsServiceNotInstalled
        {
            get => isServiceNotInstalled;
            set => NotifySet(ref isServiceNotInstalled, value);
        }

        private string serviceInfo;
        public string ServiceInfo
        {
            get => serviceInfo;
            set => NotifySet(ref serviceInfo, value);
        }

        private AsyncCommand init;
        public AsyncCommand Init => AsyncCmd(ref init, async () =>
        {
            await UpdateProcessInfo();
        });

        private AsyncCommand selectHostRuntimeType;
        public AsyncCommand SelectHostRuntimeType => AsyncCmd(ref selectHostRuntimeType, async (param) =>
        {
            switch (param)
            {
                case string str when str == "Service":
                    SelectServiceRuntimeType();
                    break;
                case string str when str == "Process":
                    SelectProcessRuntimeType();
                    break;
                default:
                    break;
            }
        });

        private AsyncCommand startProcess;
        public AsyncCommand StartProcess => AsyncCmd(ref startProcess, async (param) =>
        {
            hostProcessService.RunHostProcess();
            await Task.Delay(1000);
            await UpdateProcessInfo();
        });

        private AsyncCommand stopProcess;
        public AsyncCommand StopProcess => AsyncCmd(ref stopProcess, async (param) =>
        {
            hostProcessService.StopHostProcess();
            await Task.Delay(1000);
            await UpdateProcessInfo();
        });

        private AsyncCommand startService;
        public AsyncCommand StartService => AsyncCmd(ref startService, async (param) =>
        {
            hostServiceService.RunHostService();
            await Task.Delay(1000);
            UpdateServiceInfo();
        });

        private AsyncCommand stopService;
        public AsyncCommand StopService => AsyncCmd(ref stopService, async (param) =>
        {
            //hostService.StopHostService();
            await Task.Delay(1000);
            UpdateServiceInfo();
        });

        private AsyncCommand installService;
        public AsyncCommand InstallService => AsyncCmd(ref installService, async (param) =>
        {
            hostServiceService.InstallHostService();
            await Task.Delay(1000);
            UpdateServiceInfo();
        });

        private AsyncCommand uninstallService;
        public AsyncCommand UninstallService => AsyncCmd(ref uninstallService, async (param) =>
        {
            //hostServiceService.UninstallHostService();
            await Task.Delay(1000);
            UpdateServiceInfo();
        });

        private void SelectServiceRuntimeType()
        {
            IsService = true;
            IsProcess = false;
            
        }

        private void SelectProcessRuntimeType()
        {
            IsService = false;
            IsProcess = true;
            UpdateProcessInfo();
        }

        private async Task UpdateProcessInfo()
        {
            bool isProcessRunning = await hostProcessService.IsHostProcessRunning();
            if (isProcessRunning)
            {
                ProcessInfo = "Running";
            }
            else
            {
                ProcessInfo = "Not Running";
            }

            IsProcessRunning = isProcessRunning;
            IsProcessNotRunning = !isProcessRunning;
        }

        private void UpdateServiceInfo()
        {
            bool isServiceRunning = hostServiceService.IsHostServiceRunning();
            ServiceInfo = isServiceRunning ? "Running" : "Not Running";
            IsProcessRunning = isProcessRunning;
            IsProcessNotRunning = !isProcessRunning;

            bool isServiceInstalled = hostServiceService.IsHostServiceInstalled();
            ServiceInfo = isServiceRunning ? "Installed" : "Not Installed";
            IsServiceInstalled = isServiceInstalled;
            IsServiceNotInstalled = !isServiceInstalled;
        }
    }
}
