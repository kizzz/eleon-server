using VPortal.GatewayClient.ViewModels.ViewModels.Registration;

namespace VPortal.GatewayClient.UI.Windows.Pages;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage()
    {
        BindingContext = (Application.Current as App)!.Services.GetRequiredService<RegistrationViewModel>();
        InitializeComponent();
	}
}
