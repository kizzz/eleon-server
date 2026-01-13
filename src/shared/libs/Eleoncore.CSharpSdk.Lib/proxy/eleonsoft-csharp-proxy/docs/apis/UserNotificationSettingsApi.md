# EleonsoftProxy.Api.UserNotificationSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**NotificatorUserNotificationSettingsGetUserNotificationSettings**](UserNotificationSettingsApi.md#notificatorusernotificationsettingsgetusernotificationsettings) | **GET** /api/Notificator/NotificationSettings/GetUserNotificationSettings |  |
| [**NotificatorUserNotificationSettingsSetUserNotificationSettings**](UserNotificationSettingsApi.md#notificatorusernotificationsettingssetusernotificationsettings) | **POST** /api/Notificator/NotificationSettings/SetUserNotificationSettings |  |

<a id="notificatorusernotificationsettingsgetusernotificationsettings"></a>
# **NotificatorUserNotificationSettingsGetUserNotificationSettings**
> List&lt;NotificatorUserNotificationSettingsDto&gt; NotificatorUserNotificationSettingsGetUserNotificationSettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorUserNotificationSettingsGetUserNotificationSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserNotificationSettingsApi(config);

            try
            {
                List<NotificatorUserNotificationSettingsDto> result = apiInstance.NotificatorUserNotificationSettingsGetUserNotificationSettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserNotificationSettingsApi.NotificatorUserNotificationSettingsGetUserNotificationSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorUserNotificationSettingsGetUserNotificationSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<NotificatorUserNotificationSettingsDto>> response = apiInstance.NotificatorUserNotificationSettingsGetUserNotificationSettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserNotificationSettingsApi.NotificatorUserNotificationSettingsGetUserNotificationSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;NotificatorUserNotificationSettingsDto&gt;**](NotificatorUserNotificationSettingsDto.md)

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

<a id="notificatorusernotificationsettingssetusernotificationsettings"></a>
# **NotificatorUserNotificationSettingsSetUserNotificationSettings**
> bool NotificatorUserNotificationSettingsSetUserNotificationSettings (EleoncoreNotificationSourceType sourceType = null, bool sendNative = null, bool sendEmail = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorUserNotificationSettingsSetUserNotificationSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserNotificationSettingsApi(config);
            var sourceType = (EleoncoreNotificationSourceType) "0";  // EleoncoreNotificationSourceType |  (optional) 
            var sendNative = true;  // bool |  (optional) 
            var sendEmail = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.NotificatorUserNotificationSettingsSetUserNotificationSettings(sourceType, sendNative, sendEmail);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserNotificationSettingsApi.NotificatorUserNotificationSettingsSetUserNotificationSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorUserNotificationSettingsSetUserNotificationSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.NotificatorUserNotificationSettingsSetUserNotificationSettingsWithHttpInfo(sourceType, sendNative, sendEmail);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserNotificationSettingsApi.NotificatorUserNotificationSettingsSetUserNotificationSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sourceType** | **EleoncoreNotificationSourceType** |  | [optional]  |
| **sendNative** | **bool** |  | [optional]  |
| **sendEmail** | **bool** |  | [optional]  |

### Return type

**bool**

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

