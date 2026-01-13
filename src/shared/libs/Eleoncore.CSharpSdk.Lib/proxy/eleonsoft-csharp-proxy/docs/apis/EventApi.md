# EleonsoftProxy.Api.EventApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**EventManagementModuleEventDownloadMessage**](EventApi.md#eventmanagementmoduleeventdownloadmessage) | **GET** /api/EventManagement/Events/DownloadMessage |  |
| [**EventManagementModuleEventGetList**](EventApi.md#eventmanagementmoduleeventgetlist) | **GET** /api/EventManagement/Events/GetList |  |
| [**EventManagementModuleEventPublish**](EventApi.md#eventmanagementmoduleeventpublish) | **POST** /api/EventManagement/Events/Publish |  |
| [**EventManagementModuleEventPublishError**](EventApi.md#eventmanagementmoduleeventpublisherror) | **POST** /api/EventManagement/Events/PublishError |  |
| [**EventManagementModuleEventReceiveMany**](EventApi.md#eventmanagementmoduleeventreceivemany) | **GET** /api/EventManagement/Events/RecieveMany |  |

<a id="eventmanagementmoduleeventdownloadmessage"></a>
# **EventManagementModuleEventDownloadMessage**
> EventManagementModuleFullEventDto EventManagementModuleEventDownloadMessage (Guid messageId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleEventDownloadMessageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventApi(config);
            var messageId = "messageId_example";  // Guid |  (optional) 

            try
            {
                EventManagementModuleFullEventDto result = apiInstance.EventManagementModuleEventDownloadMessage(messageId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.EventManagementModuleEventDownloadMessage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleEventDownloadMessageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventManagementModuleFullEventDto> response = apiInstance.EventManagementModuleEventDownloadMessageWithHttpInfo(messageId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.EventManagementModuleEventDownloadMessageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **messageId** | **Guid** |  | [optional]  |

### Return type

[**EventManagementModuleFullEventDto**](EventManagementModuleFullEventDto.md)

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

<a id="eventmanagementmoduleeventgetlist"></a>
# **EventManagementModuleEventGetList**
> EleoncorePagedResultDtoOfEventManagementModuleEventDto EventManagementModuleEventGetList (Guid queueId = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleEventGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventApi(config);
            var queueId = "queueId_example";  // Guid |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEventManagementModuleEventDto result = apiInstance.EventManagementModuleEventGetList(queueId, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.EventManagementModuleEventGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleEventGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEventManagementModuleEventDto> response = apiInstance.EventManagementModuleEventGetListWithHttpInfo(queueId, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.EventManagementModuleEventGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **queueId** | **Guid** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEventManagementModuleEventDto**](EleoncorePagedResultDtoOfEventManagementModuleEventDto.md)

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

<a id="eventmanagementmoduleeventpublish"></a>
# **EventManagementModuleEventPublish**
> void EventManagementModuleEventPublish (EventManagementModulePublishMessageRequestDto eventManagementModulePublishMessageRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleEventPublishExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventApi(config);
            var eventManagementModulePublishMessageRequestDto = new EventManagementModulePublishMessageRequestDto(); // EventManagementModulePublishMessageRequestDto |  (optional) 

            try
            {
                apiInstance.EventManagementModuleEventPublish(eventManagementModulePublishMessageRequestDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.EventManagementModuleEventPublish: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleEventPublishWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.EventManagementModuleEventPublishWithHttpInfo(eventManagementModulePublishMessageRequestDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.EventManagementModuleEventPublishWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventManagementModulePublishMessageRequestDto** | [**EventManagementModulePublishMessageRequestDto**](EventManagementModulePublishMessageRequestDto.md) |  | [optional]  |

### Return type

void (empty response body)

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

<a id="eventmanagementmoduleeventpublisherror"></a>
# **EventManagementModuleEventPublishError**
> void EventManagementModuleEventPublishError (string message = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleEventPublishErrorExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventApi(config);
            var message = "message_example";  // string |  (optional) 

            try
            {
                apiInstance.EventManagementModuleEventPublishError(message);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.EventManagementModuleEventPublishError: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleEventPublishErrorWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.EventManagementModuleEventPublishErrorWithHttpInfo(message);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.EventManagementModuleEventPublishErrorWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **message** | **string** |  | [optional]  |

### Return type

void (empty response body)

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

<a id="eventmanagementmoduleeventreceivemany"></a>
# **EventManagementModuleEventReceiveMany**
> ModuleCollectorRecieveMessagesResponseDto EventManagementModuleEventReceiveMany (string queueName = null, int maxCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleEventReceiveManyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EventApi(config);
            var queueName = "queueName_example";  // string |  (optional) 
            var maxCount = 100;  // int |  (optional)  (default to 100)

            try
            {
                ModuleCollectorRecieveMessagesResponseDto result = apiInstance.EventManagementModuleEventReceiveMany(queueName, maxCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EventApi.EventManagementModuleEventReceiveMany: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleEventReceiveManyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorRecieveMessagesResponseDto> response = apiInstance.EventManagementModuleEventReceiveManyWithHttpInfo(queueName, maxCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EventApi.EventManagementModuleEventReceiveManyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **queueName** | **string** |  | [optional]  |
| **maxCount** | **int** |  | [optional] [default to 100] |

### Return type

[**ModuleCollectorRecieveMessagesResponseDto**](ModuleCollectorRecieveMessagesResponseDto.md)

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

