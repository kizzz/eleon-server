using Lib.Net.Http.WebPush;
using Lib.Net.Http.WebPush.Authentication;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Entities;

namespace VPortal.Notificator.Module.WebPush
{
  public class WebPushClientManager : ISingletonDependency
  {
    private readonly IHttpClientFactory httpClientFactory;
    private readonly IConfiguration configuration;

    private PushServiceClient _pushServiceClient;
    private PushServiceClient PushServiceClient => _pushServiceClient ??= CreatePushServiceClient();

    private VapidAuthentication _vapidAuthentication;
    private VapidAuthentication VapidAuthentication => _vapidAuthentication ??= GetVapidAuthentication();


    public WebPushClientManager(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
      this.httpClientFactory = httpClientFactory;
      this.configuration = configuration;
    }

    public async Task PushNotification(WebPushSubscriptionEntity sub, string msg)
    {
      var pushSub = MapToPushSub(sub);
      var payload = CreateWebPushMessagePayload(msg);
      var pushMsg = new PushMessage(JsonConvert.SerializeObject(payload));
      await PushServiceClient.RequestPushMessageDeliveryAsync(pushSub, pushMsg, VapidAuthentication);
    }

    private WebPushMessagePayload CreateWebPushMessagePayload(string msg)
        => new()
        {
          Notification = new()
          {
            Body = msg,
            Icon = null,
            Title = "VPortal Notification",
          },
        };

    private PushServiceClient CreatePushServiceClient()
    {
      var httpClient = httpClientFactory.CreateClient();
      return new PushServiceClient(httpClient);
    }

    private PushSubscription MapToPushSub(WebPushSubscriptionEntity sub)
    {
      var pushSub = new PushSubscription()
      {
        Endpoint = sub.Endpoint,
      };

      pushSub.SetKey(PushEncryptionKeyName.P256DH, sub.P256Dh);
      pushSub.SetKey(PushEncryptionKeyName.Auth, sub.Auth);
      return pushSub;
    }

    private VapidAuthentication GetVapidAuthentication()
    {
      var publicKey = configuration.GetValue<string>("WebPush:PublicKey");
      var privateKey = configuration.GetValue<string>("WebPush:PrivateKey");
      if (publicKey.IsNullOrWhiteSpace() || privateKey.IsNullOrWhiteSpace())
      {
        throw new Exception("VAPID keys are not properly configured. Please, set up WebPush configuration.");
      }

      return new VapidAuthentication(publicKey, privateKey);
    }
  }
}
