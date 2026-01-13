# EleonsoftProxy.Api.NotificationLogApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**NotificatorNotificationLogGetNotificationLogList**](NotificationLogApi.md#notificatornotificationloggetnotificationloglist) | **GET** /api/Notificator/NotificationLogs/GetNotificationLogList |  |

<a id="notificatornotificationloggetnotificationloglist"></a>
# **NotificatorNotificationLogGetNotificationLogList**
> EleoncorePagedResultDtoOfNotificatorNotificationLogDto NotificatorNotificationLogGetNotificationLogList (List<EleoncoreNotificationType> typeFilter = null, DateTime fromDate = null, DateTime toDate = null, string applicationName = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorNotificationLogGetNotificationLogListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new NotificationLogApi(config);
            var typeFilter = new List<EleoncoreNotificationType>(); // List<EleoncoreNotificationType> |  (optional) 
            var fromDate = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var toDate = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var applicationName = "applicationName_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfNotificatorNotificationLogDto result = apiInstance.NotificatorNotificationLogGetNotificationLogList(typeFilter, fromDate, toDate, applicationName, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling NotificationLogApi.NotificatorNotificationLogGetNotificationLogList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorNotificationLogGetNotificationLogListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfNotificatorNotificationLogDto> response = apiInstance.NotificatorNotificationLogGetNotificationLogListWithHttpInfo(typeFilter, fromDate, toDate, applicationName, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling NotificationLogApi.NotificatorNotificationLogGetNotificationLogListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **typeFilter** | [**List&lt;EleoncoreNotificationType&gt;**](EleoncoreNotificationType.md) |  | [optional]  |
| **fromDate** | **DateTime** |  | [optional]  |
| **toDate** | **DateTime** |  | [optional]  |
| **applicationName** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfNotificatorNotificationLogDto**](EleoncorePagedResultDtoOfNotificatorNotificationLogDto.md)

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

