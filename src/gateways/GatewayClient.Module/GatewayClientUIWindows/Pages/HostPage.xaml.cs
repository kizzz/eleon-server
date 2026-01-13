using VPortal.GatewayClient.ViewModels.ViewModels.Main;

namespace VPortal.GatewayClient.UI.Windows.Pages;

public partial class HostPage : ContentPage
{
	public HostPage()
	{
		InitializeComponent();
        BindingContext = (Application.Current as App)!.Services.GetRequiredService<HostViewModel>();
    }
}
