using VPortal.GatewayClient.UI.Windows.ViewModels.Samples;

namespace VPortal.GatewayClient.UI.Windows.Views;

public partial class Next24HrWidget
{
    public Next24HrWidget()
    {
        InitializeComponent();

        BindingContext = new WeatherViewModel();
    }
}
