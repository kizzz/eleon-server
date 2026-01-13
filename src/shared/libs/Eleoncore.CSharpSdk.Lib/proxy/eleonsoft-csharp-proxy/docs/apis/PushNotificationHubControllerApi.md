# EleonsoftProxy.Api.PushNotificationHubControllerApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**EleonPushNotificationHubControllerPushNotificationDtoEcho**](PushNotificationHubControllerApi.md#eleonpushnotificationhubcontrollerpushnotificationdtoecho) | **GET** /api/Notificator/PushNotificationHub |  |

<a id="eleonpushnotificationhubcontrollerpushnotificationdtoecho"></a>
# **EleonPushNotificationHubControllerPushNotificationDtoEcho**
> EleonsoftModuleCollectorPushNotificationDto EleonPushNotificationHubControllerPushNotificationDtoEcho (DateTime creationTime = null, string content = null, bool isLocalizedData = null, List<string> dataParams = null, string applicationName = null, bool isRedirectEnabled = null, string redirectUrl = null, EleonsoftSdkNotificationPriority priority = null, bool isNewMessage = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EleonPushNotificationHubControllerPushNotificationDtoEchoExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationHubControllerApi(config);
            var creationTime = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var content = "content_example";  // string |  (optional) 
            var isLocalizedData = true;  // bool |  (optional) 
            var dataParams = new List<string>(); // List<string> |  (optional) 
            var applicationName = "applicationName_example";  // string |  (optional) 
            var isRedirectEnabled = true;  // bool |  (optional) 
            var redirectUrl = "redirectUrl_example";  // string |  (optional) 
            var priority = (EleonsoftSdkNotificationPriority) "0";  // EleonsoftSdkNotificationPriority |  (optional) 
            var isNewMessage = true;  // bool |  (optional) 

            try
            {
                EleonsoftModuleCollectorPushNotificationDto result = apiInstance.EleonPushNotificationHubControllerPushNotificationDtoEcho(creationTime, content, isLocalizedData, dataParams, applicationName, isRedirectEnabled, redirectUrl, priority, isNewMessage);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationHubControllerApi.EleonPushNotificationHubControllerPushNotificationDtoEcho: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EleonPushNotificationHubControllerPushNotificationDtoEchoWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorPushNotificationDto> response = apiInstance.EleonPushNotificationHubControllerPushNotificationDtoEchoWithHttpInfo(creationTime, content, isLocalizedData, dataParams, applicationName, isRedirectEnabled, redirectUrl, priority, isNewMessage);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationHubControllerApi.EleonPushNotificationHubControllerPushNotificationDtoEchoWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **creationTime** | **DateTime** |  | [optional]  |
| **content** | **string** |  | [optional]  |
| **isLocalizedData** | **bool** |  | [optional]  |
| **dataParams** | [**List&lt;string&gt;**](string.md) |  | [optional]  |
| **applicationName** | **string** |  | [optional]  |
| **isRedirectEnabled** | **bool** |  | [optional]  |
| **redirectUrl** | **string** |  | [optional]  |
| **priority** | **EleonsoftSdkNotificationPriority** |  | [optional]  |
| **isNewMessage** | **bool** |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorPushNotificationDto**](EleonsoftModuleCollectorPushNotificationDto.md)

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: Not defined
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

