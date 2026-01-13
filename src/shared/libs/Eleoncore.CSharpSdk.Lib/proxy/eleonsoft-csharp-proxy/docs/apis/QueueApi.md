# EleonsoftProxy.Api.QueueApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**EventManagementModuleQueueClear**](QueueApi.md#eventmanagementmodulequeueclear) | **POST** /api/EventManagement/Queues/Clear |  |
| [**EventManagementModuleQueueCreate**](QueueApi.md#eventmanagementmodulequeuecreate) | **POST** /api/EventManagement/Queues/Create |  |
| [**EventManagementModuleQueueDelete**](QueueApi.md#eventmanagementmodulequeuedelete) | **DELETE** /api/EventManagement/Queues/Delete |  |
| [**EventManagementModuleQueueEnsureCreated**](QueueApi.md#eventmanagementmodulequeueensurecreated) | **POST** /api/EventManagement/Queues/EnsureCreated |  |
| [**EventManagementModuleQueueGet**](QueueApi.md#eventmanagementmodulequeueget) | **GET** /api/EventManagement/Queues/Get |  |
| [**EventManagementModuleQueueGetAll**](QueueApi.md#eventmanagementmodulequeuegetall) | **GET** /api/EventManagement/Queues/GetAll |  |
| [**EventManagementModuleQueueGetList**](QueueApi.md#eventmanagementmodulequeuegetlist) | **GET** /api/EventManagement/Queues/GetList |  |
| [**EventManagementModuleQueueUpdate**](QueueApi.md#eventmanagementmodulequeueupdate) | **POST** /api/EventManagement/Queues/Update |  |

<a id="eventmanagementmodulequeueclear"></a>
# **EventManagementModuleQueueClear**
> void EventManagementModuleQueueClear (ModuleCollectorQueueRequestDto moduleCollectorQueueRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueClearExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var moduleCollectorQueueRequestDto = new ModuleCollectorQueueRequestDto(); // ModuleCollectorQueueRequestDto |  (optional) 

            try
            {
                apiInstance.EventManagementModuleQueueClear(moduleCollectorQueueRequestDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueClear: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueClearWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.EventManagementModuleQueueClearWithHttpInfo(moduleCollectorQueueRequestDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueClearWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorQueueRequestDto** | [**ModuleCollectorQueueRequestDto**](ModuleCollectorQueueRequestDto.md) |  | [optional]  |

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

<a id="eventmanagementmodulequeuecreate"></a>
# **EventManagementModuleQueueCreate**
> EventManagementModuleQueueDto EventManagementModuleQueueCreate (EventManagementModuleCreateQueueRequestDto eventManagementModuleCreateQueueRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var eventManagementModuleCreateQueueRequestDto = new EventManagementModuleCreateQueueRequestDto(); // EventManagementModuleCreateQueueRequestDto |  (optional) 

            try
            {
                EventManagementModuleQueueDto result = apiInstance.EventManagementModuleQueueCreate(eventManagementModuleCreateQueueRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventManagementModuleQueueDto> response = apiInstance.EventManagementModuleQueueCreateWithHttpInfo(eventManagementModuleCreateQueueRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventManagementModuleCreateQueueRequestDto** | [**EventManagementModuleCreateQueueRequestDto**](EventManagementModuleCreateQueueRequestDto.md) |  | [optional]  |

### Return type

[**EventManagementModuleQueueDto**](EventManagementModuleQueueDto.md)

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

<a id="eventmanagementmodulequeuedelete"></a>
# **EventManagementModuleQueueDelete**
> void EventManagementModuleQueueDelete (string queueName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var queueName = "queueName_example";  // string |  (optional) 

            try
            {
                apiInstance.EventManagementModuleQueueDelete(queueName);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.EventManagementModuleQueueDeleteWithHttpInfo(queueName);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **queueName** | **string** |  | [optional]  |

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

<a id="eventmanagementmodulequeueensurecreated"></a>
# **EventManagementModuleQueueEnsureCreated**
> EventManagementModuleQueueDto EventManagementModuleQueueEnsureCreated (EventManagementModuleCreateQueueRequestDto eventManagementModuleCreateQueueRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueEnsureCreatedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var eventManagementModuleCreateQueueRequestDto = new EventManagementModuleCreateQueueRequestDto(); // EventManagementModuleCreateQueueRequestDto |  (optional) 

            try
            {
                EventManagementModuleQueueDto result = apiInstance.EventManagementModuleQueueEnsureCreated(eventManagementModuleCreateQueueRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueEnsureCreated: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueEnsureCreatedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventManagementModuleQueueDto> response = apiInstance.EventManagementModuleQueueEnsureCreatedWithHttpInfo(eventManagementModuleCreateQueueRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueEnsureCreatedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventManagementModuleCreateQueueRequestDto** | [**EventManagementModuleCreateQueueRequestDto**](EventManagementModuleCreateQueueRequestDto.md) |  | [optional]  |

### Return type

[**EventManagementModuleQueueDto**](EventManagementModuleQueueDto.md)

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

<a id="eventmanagementmodulequeueget"></a>
# **EventManagementModuleQueueGet**
> EventManagementModuleQueueDto EventManagementModuleQueueGet (string queueName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var queueName = "queueName_example";  // string |  (optional) 

            try
            {
                EventManagementModuleQueueDto result = apiInstance.EventManagementModuleQueueGet(queueName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventManagementModuleQueueDto> response = apiInstance.EventManagementModuleQueueGetWithHttpInfo(queueName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **queueName** | **string** |  | [optional]  |

### Return type

[**EventManagementModuleQueueDto**](EventManagementModuleQueueDto.md)

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

<a id="eventmanagementmodulequeuegetall"></a>
# **EventManagementModuleQueueGetAll**
> List&lt;EventManagementModuleQueueDto&gt; EventManagementModuleQueueGetAll ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);

            try
            {
                List<EventManagementModuleQueueDto> result = apiInstance.EventManagementModuleQueueGetAll();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGetAll: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueGetAllWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<EventManagementModuleQueueDto>> response = apiInstance.EventManagementModuleQueueGetAllWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGetAllWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;EventManagementModuleQueueDto&gt;**](EventManagementModuleQueueDto.md)

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

<a id="eventmanagementmodulequeuegetlist"></a>
# **EventManagementModuleQueueGetList**
> EleoncorePagedResultDtoOfEventManagementModuleQueueDto EventManagementModuleQueueGetList (string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEventManagementModuleQueueDto result = apiInstance.EventManagementModuleQueueGetList(sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEventManagementModuleQueueDto> response = apiInstance.EventManagementModuleQueueGetListWithHttpInfo(sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEventManagementModuleQueueDto**](EleoncorePagedResultDtoOfEventManagementModuleQueueDto.md)

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

<a id="eventmanagementmodulequeueupdate"></a>
# **EventManagementModuleQueueUpdate**
> EventManagementModuleQueueDto EventManagementModuleQueueUpdate (EventManagementModuleUpdateQueueRequestDto eventManagementModuleUpdateQueueRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class EventManagementModuleQueueUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new QueueApi(config);
            var eventManagementModuleUpdateQueueRequestDto = new EventManagementModuleUpdateQueueRequestDto(); // EventManagementModuleUpdateQueueRequestDto |  (optional) 

            try
            {
                EventManagementModuleQueueDto result = apiInstance.EventManagementModuleQueueUpdate(eventManagementModuleUpdateQueueRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the EventManagementModuleQueueUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EventManagementModuleQueueDto> response = apiInstance.EventManagementModuleQueueUpdateWithHttpInfo(eventManagementModuleUpdateQueueRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling QueueApi.EventManagementModuleQueueUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eventManagementModuleUpdateQueueRequestDto** | [**EventManagementModuleUpdateQueueRequestDto**](EventManagementModuleUpdateQueueRequestDto.md) |  | [optional]  |

### Return type

[**EventManagementModuleQueueDto**](EventManagementModuleQueueDto.md)

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

