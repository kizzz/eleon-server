using VPortal.ProxyClient.ViewModels.ViewModels.Main;

namespace VPortal.ProxyClient.UI.Windows.Pages;

public partial class HomePage : ContentPage
{
	protected object thisMenuBarWorkaround;

    public HomePage()
	{
		InitializeComponent();
		BindingContext = (Application.Current as App)!.Services.GetRequiredService<MainViewModel>();
    }
}