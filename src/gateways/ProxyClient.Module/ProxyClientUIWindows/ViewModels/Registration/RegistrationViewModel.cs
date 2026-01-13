using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VPortal.Infrastructure.Module.Domain.CliBinding;
using VPortal.ProxyClient.Domain.Auth;
using VPortal.ProxyClient.Domain.Shared.Auth;
using VPortal.ProxyClient.Domain.Windows.DomainServices;
using VPortal.ProxyClient.Host.Cli.Client;
using VPortal.ProxyClient.ViewModels.Commands;

namespace VPortal.ProxyClient.ViewModels.ViewModels.Registration
{
    public class RegistrationViewModel : ViewModel
    {
        private readonly ProxyClientHostCliClient hostCliClient;
        private readonly WindowsProxyClientDirectoryService directoryService;

        public RegistrationViewModel(
            ProxyClientHostCliClient hostCliClient,
            WindowsProxyClientDirectoryService directoryService)
            : base(directoryService)
        {
            this.hostCliClient = hostCliClient;
            this.directoryService = directoryService;
        }

        private bool showRegistrationKeyInput;
        public bool ShowRegistrationKeyInput
        {
            get => showRegistrationKeyInput;
            set => NotifySet(ref showRegistrationKeyInput, value);
        }

        private bool registerBtnEnabled;
        public bool RegisterBtnEnabled
        {
            get => registerBtnEnabled;
            set => NotifySet(ref registerBtnEnabled, value);
        }

        private bool registationInProcess;
        public bool RegistrationInProcess
        {
            get => registationInProcess;
            set
            {
                NotifySet(ref registationInProcess, value);
                Notify(nameof(RegistrationNotInProcess));
            }
        }

        public bool RegistrationNotInProcess => !RegistrationInProcess;

        private bool registrationErrorOccured;
        public bool RegistrationErrorOccured
        {
            get => registrationErrorOccured;
            set => NotifySet(ref registrationErrorOccured, value);
        }

        private string registrationKey;
        public string RegistrationKey
        {
            get => registrationKey;
            set
            {
                ShowRegistrationKeyInput = value?.Length > 0;
                NotifySet(ref registrationKey, value);
            }
        }

        private string currentRegistrationStage;
        public string CurrentRegistrationStage
        {
            get => currentRegistrationStage;
            set
            {
                ShowRegistrationKeyInput = value?.Length > 0;
                NotifySet(ref currentRegistrationStage, value);
            }
        }

        private ICommand registerProxy;
        public ICommand RegisterProxy => registerProxy ??= new AsyncCommand(async () =>
        {
            try
            {
                CurrentRegistrationStage = null;
                RegistrationInProcess = true;
                RegistrationErrorOccured = false;
                await foreach (var stage in hostCliClient.Register(RegistrationKey))
                {
                    CurrentRegistrationStage = stage.Trim();
                }
            }
            catch (Exception ex)
            {
                CurrentRegistrationStage = "Error occured";
                RegistrationErrorOccured = true;
                throw;
            }
            finally
            {
                RegistrationInProcess = false;
            }

            GoToMain.Execute(new object());
        });

        private ICommand init;
        public ICommand Init => init ??= new AsyncCommand(async () =>
        {
            var status = await hostCliClient.GetProxyStatus();
            if (status.RegistrationStage == ProxyClientRegistrationStages.Completed)
            {
                GoToMain.Execute(new object());
            }
            else if (status.RegistrationStage == ProxyClientRegistrationStages.NotRegistered)
            {
                ShowRegistrationKeyInput = true;
                RegisterBtnEnabled = false;
            }
            else
            {
                ShowRegistrationKeyInput = false;
                RegisterBtnEnabled = true;
            }
        });

        private ICommand goToMain;
        public ICommand GoToMain => goToMain ??= new AsyncCommand(async () =>
        {
            await Shell.Current.GoToAsync("//home");
        });
    }
}