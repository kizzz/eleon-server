# EleonsoftProxy.Api.PushNotificationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**NotificatorPushNotificationAcknowledgeNotificationRead**](PushNotificationApi.md#notificatorpushnotificationacknowledgenotificationread) | **POST** /api/Notificator/PushNotifications/AcknowledgeNotificationRead |  |
| [**NotificatorPushNotificationAcknowledgeNotificationsBulkRead**](PushNotificationApi.md#notificatorpushnotificationacknowledgenotificationsbulkread) | **POST** /api/Notificator/PushNotifications/AcknowledgeNotificationsBulkRead |  |
| [**NotificatorPushNotificationGetRecentNotifications**](PushNotificationApi.md#notificatorpushnotificationgetrecentnotifications) | **GET** /api/Notificator/PushNotifications/GetRecentNotifications |  |
| [**NotificatorPushNotificationGetTotalUnreadNotifications**](PushNotificationApi.md#notificatorpushnotificationgettotalunreadnotifications) | **GET** /api/Notificator/PushNotifications/GetTotalUnreadNotifications |  |
| [**NotificatorPushNotificationGetUnreadNotifications**](PushNotificationApi.md#notificatorpushnotificationgetunreadnotifications) | **GET** /api/Notificator/PushNotifications/GetUnreadNotifications |  |

<a id="notificatorpushnotificationacknowledgenotificationread"></a>
# **NotificatorPushNotificationAcknowledgeNotificationRead**
> bool NotificatorPushNotificationAcknowledgeNotificationRead (Guid notificationLogId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorPushNotificationAcknowledgeNotificationReadExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationApi(config);
            var notificationLogId = "notificationLogId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.NotificatorPushNotificationAcknowledgeNotificationRead(notificationLogId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationAcknowledgeNotificationRead: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorPushNotificationAcknowledgeNotificationReadWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.NotificatorPushNotificationAcknowledgeNotificationReadWithHttpInfo(notificationLogId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationAcknowledgeNotificationReadWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **notificationLogId** | **Guid** |  | [optional]  |

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

<a id="notificatorpushnotificationacknowledgenotificationsbulkread"></a>
# **NotificatorPushNotificationAcknowledgeNotificationsBulkRead**
> bool NotificatorPushNotificationAcknowledgeNotificationsBulkRead (List<Guid> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorPushNotificationAcknowledgeNotificationsBulkReadExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationApi(config);
            var requestBody = new List<Guid>(); // List<Guid> |  (optional) 

            try
            {
                bool result = apiInstance.NotificatorPushNotificationAcknowledgeNotificationsBulkRead(requestBody);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationAcknowledgeNotificationsBulkRead: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorPushNotificationAcknowledgeNotificationsBulkReadWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.NotificatorPushNotificationAcknowledgeNotificationsBulkReadWithHttpInfo(requestBody);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationAcknowledgeNotificationsBulkReadWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **requestBody** | [**List&lt;Guid&gt;**](Guid.md) |  | [optional]  |

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

<a id="notificatorpushnotificationgetrecentnotifications"></a>
# **NotificatorPushNotificationGetRecentNotifications**
> List&lt;NotificatorNotificationLogDto&gt; NotificatorPushNotificationGetRecentNotifications (string applicationName = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorPushNotificationGetRecentNotificationsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationApi(config);
            var applicationName = "applicationName_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                List<NotificatorNotificationLogDto> result = apiInstance.NotificatorPushNotificationGetRecentNotifications(applicationName, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetRecentNotifications: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorPushNotificationGetRecentNotificationsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<NotificatorNotificationLogDto>> response = apiInstance.NotificatorPushNotificationGetRecentNotificationsWithHttpInfo(applicationName, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetRecentNotificationsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationName** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**List&lt;NotificatorNotificationLogDto&gt;**](NotificatorNotificationLogDto.md)

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

<a id="notificatorpushnotificationgettotalunreadnotifications"></a>
# **NotificatorPushNotificationGetTotalUnreadNotifications**
> int NotificatorPushNotificationGetTotalUnreadNotifications (string applicationName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorPushNotificationGetTotalUnreadNotificationsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationApi(config);
            var applicationName = "applicationName_example";  // string |  (optional) 

            try
            {
                int result = apiInstance.NotificatorPushNotificationGetTotalUnreadNotifications(applicationName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetTotalUnreadNotifications: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorPushNotificationGetTotalUnreadNotificationsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<int> response = apiInstance.NotificatorPushNotificationGetTotalUnreadNotificationsWithHttpInfo(applicationName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetTotalUnreadNotificationsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationName** | **string** |  | [optional]  |

### Return type

**int**

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

<a id="notificatorpushnotificationgetunreadnotifications"></a>
# **NotificatorPushNotificationGetUnreadNotifications**
> EleoncorePagedResultDtoOfNotificatorNotificationLogDto NotificatorPushNotificationGetUnreadNotifications (string applicationName = null, int skip = null, int take = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class NotificatorPushNotificationGetUnreadNotificationsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PushNotificationApi(config);
            var applicationName = "applicationName_example";  // string |  (optional) 
            var skip = 56;  // int |  (optional) 
            var take = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfNotificatorNotificationLogDto result = apiInstance.NotificatorPushNotificationGetUnreadNotifications(applicationName, skip, take);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetUnreadNotifications: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the NotificatorPushNotificationGetUnreadNotificationsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfNotificatorNotificationLogDto> response = apiInstance.NotificatorPushNotificationGetUnreadNotificationsWithHttpInfo(applicationName, skip, take);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PushNotificationApi.NotificatorPushNotificationGetUnreadNotificationsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationName** | **string** |  | [optional]  |
| **skip** | **int** |  | [optional]  |
| **take** | **int** |  | [optional]  |

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

