using VPortal.ProxyClient.UI.Windows.ViewModels.Samples;

namespace VPortal.ProxyClient.UI.Windows.Views;

public partial class Next24HrWidget
{
    public Next24HrWidget()
    {
        InitializeComponent();

        BindingContext = new WeatherViewModel();
    }
}
