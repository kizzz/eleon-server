using Microsoft.Extensions.Configuration;
using Microsoft.Maui.Networking;
using System.Windows.Input;
using Volo.Abp;
using VPortal.GatewayClient.Domain.Auth;
using VPortal.GatewayClient.Domain.Settings;
using VPortal.GatewayClient.Domain.Shared.Auth;
using VPortal.GatewayClient.Domain.Windows.DomainServices;
using VPortal.GatewayClient.Host.Cli.Client;
using VPortal.GatewayClient.ViewModels.Commands;

namespace VPortal.GatewayClient.ViewModels.ViewModels.Main
{
    public class MainViewModel : ViewModel
    {
        private readonly WindowsGatewayClientHostProcessService hostProcessService;
        private readonly GatewayClientHostCliClient hostCliClient;
        private readonly WindowsGatewayClientDirectoryService directoryService;

        public MainViewModel(
            WindowsGatewayClientHostProcessService hostProcessService,
            GatewayClientHostCliClient hostCliClient,
            WindowsGatewayClientDirectoryService directoryService)
            : base(directoryService)
        {
            this.hostProcessService = hostProcessService;
            this.hostCliClient = hostCliClient;
            this.directoryService = directoryService;
        }

        private string gatewayInfo;
        public string GatewayInfo
        {
            get => gatewayInfo;
            set => NotifySet(ref gatewayInfo, value);
        }

        private string internetConnectionInfo;
        public string InternetConnectionInfo
        {
            get => internetConnectionInfo;
            set => NotifySet(ref internetConnectionInfo, value);
        }

        private bool hasInternetConnection;
        public bool HasInternetConnection
        {
            get => hasInternetConnection;
            set => NotifySet(ref hasInternetConnection, value);
        }

        private string currentRegistrationStage;
        public string CurrentRegistrationStage
        {
            get => currentRegistrationStage;
            set => NotifySet(ref currentRegistrationStage, value);
        }

        private bool isRegistered;
        public bool IsRegistered
        {
            get => isRegistered;
            set => NotifySet(ref isRegistered, value);
        }

        private bool hostExecutablePresent;
        public bool HostExecutablePresent
        {
            get => hostExecutablePresent;
            set => NotifySet(ref hostExecutablePresent, value);
        }

        private bool hostExecutableAbsent;
        public bool HostExecutableAbsent
        {
            get => hostExecutableAbsent;
            set => NotifySet(ref hostExecutableAbsent, value);
        }

        private string portInfo;
        public string PortInfo
        {
            get => portInfo;
            set => NotifySet(ref portInfo, value);
        }

        private bool isStatusLoading;
        public bool IsStatusLoading
        {
            get => isStatusLoading;
            set => NotifySet(ref isStatusLoading, value);
        }

        private AsyncCommand init;
        public AsyncCommand Init => AsyncCmd(ref init, async () =>
        {
            UpdateHostExecutableInfo.Execute();
            UpdateInternetInfo.Execute();
        });

        private AsyncCommand updateHostExecutableInfo;
        public AsyncCommand UpdateHostExecutableInfo => AsyncCmd(ref updateHostExecutableInfo, async () =>
        {
            try
            {
                string _ = directoryService.GetHostExecutablePath();
                HostExecutableAbsent = false;
                HostExecutablePresent = true;
                UpdateStatus.Execute();
                await UpdateProcessInfo();
            }
            catch (Exception)
            {
                HostExecutablePresent = false;
                HostExecutableAbsent = true;
                throw;
            }
        });

        private AsyncCommand updateInternetInfo;
        public AsyncCommand UpdateInternetInfo => AsyncCmd(ref updateInternetInfo, async () =>
        {
            var currentNetworkAccess = Connectivity.Current.NetworkAccess;
            HasInternetConnection = currentNetworkAccess == NetworkAccess.Internet;
            switch (currentNetworkAccess)
            {
                case NetworkAccess.None:
                case NetworkAccess.Unknown:
                    InternetConnectionInfo = "Not Connected";
                    break;
                case NetworkAccess.Local:
                    InternetConnectionInfo = "Local Network Only";
                    break;
                case NetworkAccess.ConstrainedInternet:
                    InternetConnectionInfo = "Constrained";
                    break;
                case NetworkAccess.Internet:
                    InternetConnectionInfo = "Connected";
                    break;
                default:
                    break;
            }

            if (!HasInternetConnection)
            {
                throw new UserFriendlyException("Internet connection could not be established.");
            }
        });

        private AsyncCommand updateStatus;
        public AsyncCommand UpdateStatus => AsyncCmd(ref updateStatus, async () =>
        {
            IsStatusLoading = true;
            try
            {
                var status = await hostCliClient.GetGatewayStatus();

                IsRegistered = status.RegistrationStage == GatewayClientRegistrationStages.Completed;
                if (IsRegistered)
                {
                    CurrentRegistrationStage = "Registered";
                }
                else
                {
                    CurrentRegistrationStage = "Not Registered";
                }

                PortInfo = status.Port?.ToString() ?? "Not Configured";
            }
            finally
            {
                IsStatusLoading = false;
            }
        });

        private AsyncCommand register;
        public AsyncCommand Register => AsyncCmd(ref register, async () =>
        {
            await Shell.Current.GoToAsync("//registration");
        });

        private AsyncCommand configureHost;
        public AsyncCommand ConfigureHost => AsyncCmd(ref configureHost, async () =>
        {
            await Shell.Current.GoToAsync("//host");
        });

        private AsyncCommand resetRegistration;
        public AsyncCommand ResetRegistration => AsyncCmd(ref resetRegistration, async () =>
        {
            bool reset = await Application.Current.MainPage.DisplayAlert("Confirm reset", "Are you sure you want to reset registration?", "Yes", "No");
            if (reset)
            {
                IsStatusLoading = true;
                await hostCliClient.ResetRegistration();
                Reload();
            }
        });

        #region Process

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
        #endregion

        #region Port

        private AsyncCommand changePort;
        public AsyncCommand ChangePort => AsyncCmd(ref changePort, async () =>
        {
            var portAnswer = await Application.Current.MainPage.DisplayPromptAsync(
                "Enter new port value",
                "",
                placeholder: "Port value",
                initialValue: PortInfo ?? "44320",
                maxLength: 5);

            if (string.IsNullOrEmpty(portAnswer))
            {
                // 'Cancel' selected, ignore
            }
            else if (!int.TryParse(portAnswer, out int port) || !(port is >= 1 and <= 65535))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Invalid value",
                    "The value you entered is not a valid port. Please, enter an integer between 1 and 65535.",
                    "OK");
            }
            else
            {
                IsStatusLoading = true;
                await hostCliClient.ChangePort(port);
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Port is successfully changed.",
                    "OK");
                Reload();
            }
        });
        #endregion

        private void Reload()
        {
            Init.Execute(new object());
        }
    }
}
