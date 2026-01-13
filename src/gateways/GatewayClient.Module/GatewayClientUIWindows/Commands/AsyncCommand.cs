using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Volo.Abp;
using VPortal.GatewayClient.UI.Windows.Exceptions;

namespace VPortal.GatewayClient.ViewModels.Commands
{
    public class AsyncCommand : ICommand, IErrorProvider
    {
        public event EventHandler CanExecuteChanged;
        public event Action ErrorChanged;

        private Func<Task> _executeAsync;
        private Func<object, Task> _executeAsyncWithParameter;
        private bool _isExecuting;

        public AsyncCommand(Func<Task> executeAsync)
        {
            _executeAsync = executeAsync;
        }
        public AsyncCommand(Func<object, Task> executeAsync)
        {
            _executeAsyncWithParameter = executeAsync;
        }

        public bool CanExecute(object parameter) => !IsExecuting;

        public bool IsExecuting
        {
            get => _isExecuting;
            set
            {
                if (_isExecuting != value)
                {
                    _isExecuting = value;
                    CanExecuteChanged?.Invoke(this, new());
                }
            }
        }

        public string? LastError { get; private set; }

        public void Execute()
            => Execute(new object());

        public async void Execute(object parameter)
        {
            try
            {
                IsExecuting = true;

                if (LastError != null)
                {
                    LastError = null;
                    ErrorChanged?.Invoke();
                }

                if (_executeAsyncWithParameter != null)
                {
                    await _executeAsyncWithParameter(parameter);
                }
                else
                {
                    await _executeAsync();
                }
            }
            catch (BusinessException ex)
            {
                LastError = ex.Message;
                ErrorChanged?.Invoke();
            }
            finally
            {
                IsExecuting = false;
            }
        }
    }
}
