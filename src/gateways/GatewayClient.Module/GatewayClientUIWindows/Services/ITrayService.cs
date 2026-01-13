namespace VPortal.GatewayClient.UI.Windows.Services;

public interface ITrayService
{
    void Initialize();

    Action ClickHandler { get; set; }
}
