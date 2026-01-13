using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading;
using VPortal.ProxyClient.UI.Windows.Extensions;
using VPortal.ProxyClient.UI.Windows.Pages;

namespace VPortal.ProxyClient.UI.Windows;

public partial class App : Application
{
    private IHost appHost;
    private CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
    
    public App()
    {
        InitializeComponent();

        CreateHost();

        App.Current.UserAppTheme = AppTheme.Dark;

        //Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        if (window != null)
        {
            //window.Title = "VPortal Proxy Configuration";
        }

        return window;
    }

    public IServiceProvider Services => appHost.Services;

    protected override async void OnStart()
    {
        // TODO: Handle graceful shutdown, optionally move to MAUI App class, use https://www.syncfusion.com/blogs/post/configuring-life-cycle-events-in-net-maui-apps.aspx
        await appHost.InitializeAndStart(CancellationTokenSource.Token);
        base.OnStart();
    }

    private void CreateHost()
    {
        appHost = new HostBuilder()
            .UseLogger()
            .UseConfiguration()
            .UseAutofac()
            .ConfigureServices((hostContext, services) =>
            {
                services.AddApplication<ProxyClientUIWindowsModule>();
            })
            .Build();
    }

    private async void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
    {
        try { 
            await Shell.Current.GoToAsync($"///settings");
        }catch (Exception ex) {
            Debug.WriteLine($"err: {ex.Message}");
        }
    }
}
