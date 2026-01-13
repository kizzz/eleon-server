using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Volo.Abp.DependencyInjection;
using VPortal.GatewayClient.Domain.Windows.DomainServices;
using VPortal.GatewayClient.UI.Windows.Services;
using VPortal.GatewayClient.ViewModels.Commands;

namespace VPortal.GatewayClient.ViewModels.ViewModels
{
    public abstract class ViewModel : INotifyPropertyChanged, ITransientDependency
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly WindowsGatewayClientDirectoryService directoryService;

        public ViewModel(WindowsGatewayClientDirectoryService directoryService)
        {
            this.directoryService = directoryService;
        }

        private AsyncCommand quit;
        public AsyncCommand Quit => AsyncCmd(ref quit, async () =>
        {
            Application.Current.Quit();
        });

        private AsyncCommand openConfigurationLogs;
        public AsyncCommand OpenConfigurationLogs => AsyncCmd(ref openConfigurationLogs, async () =>
        {
            directoryService.OpenConfigurationLogs();
        });

        private AsyncCommand openHostLogs;
        public AsyncCommand OpenHostLogs => AsyncCmd(ref openHostLogs, async () =>
        {
            directoryService.OpenHostLogs();
        });

        protected void Notify(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        protected T NotifySet<T>(ref T originalValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            originalValue = newValue;
            Notify(propertyName);
            return newValue;
        }

        protected AsyncCommand AsyncCmd(ref AsyncCommand cmdCache, Func<Task> callback, [CallerMemberName] string propertyName = null)
            => cmdCache ??= new AsyncCommand(callback);

        protected AsyncCommand AsyncCmd(ref AsyncCommand cmdCache, Func<object, Task> callback, [CallerMemberName] string propertyName = null)
            => cmdCache ??= new AsyncCommand(callback);
    }
}
