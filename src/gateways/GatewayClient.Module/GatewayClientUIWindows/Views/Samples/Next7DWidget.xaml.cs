using VPortal.GatewayClient.UI.Windows.ViewModels.Samples;

namespace VPortal.GatewayClient.UI.Windows.Views;

public partial class Next7DWidget
{
    public Next7DWidget()
    {
        InitializeComponent();

        BindingContext = new WeatherViewModel();
    }
}
