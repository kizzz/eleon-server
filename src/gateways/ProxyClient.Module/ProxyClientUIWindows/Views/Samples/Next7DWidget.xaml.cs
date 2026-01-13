using VPortal.ProxyClient.UI.Windows.ViewModels.Samples;

namespace VPortal.ProxyClient.UI.Windows.Views;

public partial class Next7DWidget
{
    public Next7DWidget()
    {
        InitializeComponent();

        BindingContext = new WeatherViewModel();
    }
}
