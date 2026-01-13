using VPortal.ProxyClient.ViewModels.ViewModels.Registration;

namespace VPortal.ProxyClient.UI.Windows.Pages;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage()
    {
        BindingContext = (Application.Current as App)!.Services.GetRequiredService<RegistrationViewModel>();
        InitializeComponent();
	}
}