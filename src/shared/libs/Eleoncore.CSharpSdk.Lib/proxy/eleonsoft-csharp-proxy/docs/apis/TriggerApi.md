# EleonsoftProxy.Api.TriggerApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**JobSchedulerTriggerAdd**](TriggerApi.md#jobschedulertriggeradd) | **POST** /api/JobScheduler/Triggers/Add |  |
| [**JobSchedulerTriggerDelete**](TriggerApi.md#jobschedulertriggerdelete) | **DELETE** /api/JobScheduler/Triggers/Delete |  |
| [**JobSchedulerTriggerGetById**](TriggerApi.md#jobschedulertriggergetbyid) | **GET** /api/JobScheduler/Triggers/GetTriggerById |  |
| [**JobSchedulerTriggerGetList**](TriggerApi.md#jobschedulertriggergetlist) | **GET** /api/JobScheduler/Triggers/GetList |  |
| [**JobSchedulerTriggerSetTriggerIsEnabled**](TriggerApi.md#jobschedulertriggersettriggerisenabled) | **POST** /api/JobScheduler/Triggers/SetTriggerIsEnabled |  |
| [**JobSchedulerTriggerUpdate**](TriggerApi.md#jobschedulertriggerupdate) | **POST** /api/JobScheduler/Triggers/Update |  |

<a id="jobschedulertriggeradd"></a>
# **JobSchedulerTriggerAdd**
> JobSchedulerTriggerDto JobSchedulerTriggerAdd (JobSchedulerTriggerDto jobSchedulerTriggerDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerAddExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var jobSchedulerTriggerDto = new JobSchedulerTriggerDto(); // JobSchedulerTriggerDto |  (optional) 

            try
            {
                JobSchedulerTriggerDto result = apiInstance.JobSchedulerTriggerAdd(jobSchedulerTriggerDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerAdd: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerAddWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<JobSchedulerTriggerDto> response = apiInstance.JobSchedulerTriggerAddWithHttpInfo(jobSchedulerTriggerDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerAddWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **jobSchedulerTriggerDto** | [**JobSchedulerTriggerDto**](JobSchedulerTriggerDto.md) |  | [optional]  |

### Return type

[**JobSchedulerTriggerDto**](JobSchedulerTriggerDto.md)

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

<a id="jobschedulertriggerdelete"></a>
# **JobSchedulerTriggerDelete**
> bool JobSchedulerTriggerDelete (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.JobSchedulerTriggerDelete(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.JobSchedulerTriggerDeleteWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

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

<a id="jobschedulertriggergetbyid"></a>
# **JobSchedulerTriggerGetById**
> JobSchedulerTriggerDto JobSchedulerTriggerGetById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                JobSchedulerTriggerDto result = apiInstance.JobSchedulerTriggerGetById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<JobSchedulerTriggerDto> response = apiInstance.JobSchedulerTriggerGetByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**JobSchedulerTriggerDto**](JobSchedulerTriggerDto.md)

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

<a id="jobschedulertriggergetlist"></a>
# **JobSchedulerTriggerGetList**
> List&lt;JobSchedulerTriggerDto&gt; JobSchedulerTriggerGetList (bool isEnabledFilter = null, Guid taskId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var isEnabledFilter = true;  // bool |  (optional) 
            var taskId = "taskId_example";  // Guid |  (optional) 

            try
            {
                List<JobSchedulerTriggerDto> result = apiInstance.JobSchedulerTriggerGetList(isEnabledFilter, taskId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<JobSchedulerTriggerDto>> response = apiInstance.JobSchedulerTriggerGetListWithHttpInfo(isEnabledFilter, taskId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **isEnabledFilter** | **bool** |  | [optional]  |
| **taskId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;JobSchedulerTriggerDto&gt;**](JobSchedulerTriggerDto.md)

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

<a id="jobschedulertriggersettriggerisenabled"></a>
# **JobSchedulerTriggerSetTriggerIsEnabled**
> bool JobSchedulerTriggerSetTriggerIsEnabled (Guid triggerId = null, bool isEnabled = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerSetTriggerIsEnabledExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var triggerId = "triggerId_example";  // Guid |  (optional) 
            var isEnabled = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.JobSchedulerTriggerSetTriggerIsEnabled(triggerId, isEnabled);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerSetTriggerIsEnabled: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerSetTriggerIsEnabledWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.JobSchedulerTriggerSetTriggerIsEnabledWithHttpInfo(triggerId, isEnabled);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerSetTriggerIsEnabledWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **triggerId** | **Guid** |  | [optional]  |
| **isEnabled** | **bool** |  | [optional]  |

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

<a id="jobschedulertriggerupdate"></a>
# **JobSchedulerTriggerUpdate**
> JobSchedulerTriggerDto JobSchedulerTriggerUpdate (JobSchedulerTriggerDto jobSchedulerTriggerDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class JobSchedulerTriggerUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TriggerApi(config);
            var jobSchedulerTriggerDto = new JobSchedulerTriggerDto(); // JobSchedulerTriggerDto |  (optional) 

            try
            {
                JobSchedulerTriggerDto result = apiInstance.JobSchedulerTriggerUpdate(jobSchedulerTriggerDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the JobSchedulerTriggerUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<JobSchedulerTriggerDto> response = apiInstance.JobSchedulerTriggerUpdateWithHttpInfo(jobSchedulerTriggerDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TriggerApi.JobSchedulerTriggerUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **jobSchedulerTriggerDto** | [**JobSchedulerTriggerDto**](JobSchedulerTriggerDto.md) |  | [optional]  |

### Return type

[**JobSchedulerTriggerDto**](JobSchedulerTriggerDto.md)

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

