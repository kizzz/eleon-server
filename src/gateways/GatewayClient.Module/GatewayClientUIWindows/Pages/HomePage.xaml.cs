using VPortal.GatewayClient.ViewModels.ViewModels.Main;

namespace VPortal.GatewayClient.UI.Windows.Pages;

public partial class HomePage : ContentPage
{
	protected object thisMenuBarWorkaround;

    public HomePage()
	{
		InitializeComponent();
		BindingContext = (Application.Current as App)!.Services.GetRequiredService<MainViewModel>();
    }
}
