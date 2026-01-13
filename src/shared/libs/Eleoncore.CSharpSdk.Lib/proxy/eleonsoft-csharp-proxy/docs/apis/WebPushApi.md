# EleonsoftProxy.Api.WebPushApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**NotificatorWebPushAddWebPushSubscription**](WebPushApi.md#notificatorwebpushaddwebpushsubscription) | **POST** /api/Notificator/WebPush/AddWebPushSubscription |  |

<a id="notificatorwebpushaddwebpushsubscription"></a>
# **NotificatorWebPushAddWebPushSubscription**
> bool NotificatorWebPushAddWebPushSubscription (NotificatorWebPushSubscriptionDto notificatorWebPushSubscriptionDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorWebPushAddWebPushSubscriptionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new WebPushApi(config);
            var notificatorWebPushSubscriptionDto = new NotificatorWebPushSubscriptionDto(); // NotificatorWebPushSubscriptionDto |  (optional) 

            try
            {
                bool result = apiInstance.NotificatorWebPushAddWebPushSubscription(notificatorWebPushSubscriptionDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling WebPushApi.NotificatorWebPushAddWebPushSubscription: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorWebPushAddWebPushSubscriptionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.NotificatorWebPushAddWebPushSubscriptionWithHttpInfo(notificatorWebPushSubscriptionDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling WebPushApi.NotificatorWebPushAddWebPushSubscriptionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **notificatorWebPushSubscriptionDto** | [**NotificatorWebPushSubscriptionDto**](NotificatorWebPushSubscriptionDto.md) |  | [optional]  |

### Return type

**bool**

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: application/json, text/json, application/*+json
 - **Accept**: text/plain, application/json, text/json


### HTTP response details
| Status code | Description | Response headers |
|-------------|-------------|------------------|
| **200** | OK |  -  |
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

