using VPortal.ProxyClient.ViewModels.ViewModels.Main;

namespace VPortal.ProxyClient.UI.Windows.Pages;

public partial class HostPage : ContentPage
{
	public HostPage()
	{
		InitializeComponent();
        BindingContext = (Application.Current as App)!.Services.GetRequiredService<HostViewModel>();
    }
}