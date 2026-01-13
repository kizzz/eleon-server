using Microsoft.UI.Xaml;
using VPortal.ProxyClient.Domain.Windows.DomainServices;
using VPortal.ProxyClient.ViewModels.Commands;

namespace VPortal.ProxyClient.UI.Windows.Views.ExceptionHandling;

public partial class ErrorViewer : HorizontalStackLayout
{
    private readonly WindowsProxyClientDirectoryService directoryService;

    public ErrorViewer()
    {
        InitializeComponent();
        this.directoryService = (Microsoft.Maui.Controls.Application.Current as App).Services.GetService<WindowsProxyClientDirectoryService>();
    }

    public static readonly BindableProperty CommandProperty = BindableProperty.Create
        (
             nameof(Command),
             typeof(AsyncCommand),
             typeof(ErrorViewer),
             null,
             propertyChanged: (bindable, oldValue, newValue) =>
             {
                 var viewer = bindable as ErrorViewer;
                 var updatedCmd = newValue as AsyncCommand;
                 updatedCmd.ErrorChanged += () => OnErrorChanged(viewer, updatedCmd?.LastError);
                 OnErrorChanged(viewer, updatedCmd?.LastError);
             }
        );

    private static void OnErrorChanged(ErrorViewer viewer, string newError)
    {
        viewer.OpenDetailsBtn.IsVisible = newError != null;
        viewer.LastError = newError;
    }

    public AsyncCommand Command
    {
        get { return (AsyncCommand)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    public string LastError { get; set; }

    async void OnDisplayDetails(object sender, EventArgs eventArgs)
    {
        bool openLogs = await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert("Error", LastError, "Open Logs", "Ok");
        if (openLogs)
        {
            directoryService.OpenHostLogs();
        }
    }
}