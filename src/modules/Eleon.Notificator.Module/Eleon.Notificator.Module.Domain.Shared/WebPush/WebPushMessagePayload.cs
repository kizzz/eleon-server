namespace VPortal.Notificator.Module.WebPush
{
  public class WebPushMessagePayload
  {
    public WebPushMessageNotification Notification { get; set; }

    public class WebPushMessageNotification
    {
      public string Body { get; set; }
      public string Title { get; set; }
      public string Icon { get; set; }
    }
  }
}
