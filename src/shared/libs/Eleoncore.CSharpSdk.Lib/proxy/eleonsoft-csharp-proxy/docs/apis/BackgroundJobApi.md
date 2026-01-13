# EleonsoftProxy.Api.BackgroundJobApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**BackgroundJobsBackgroundJobCancelBackgroundJob**](BackgroundJobApi.md#backgroundjobsbackgroundjobcancelbackgroundjob) | **POST** /api/BackgroundJob/BackgroundJobs/CancelBackgroundJob |  |
| [**BackgroundJobsBackgroundJobComplete**](BackgroundJobApi.md#backgroundjobsbackgroundjobcomplete) | **POST** /api/BackgroundJob/BackgroundJobs/Complete |  |
| [**BackgroundJobsBackgroundJobCreate**](BackgroundJobApi.md#backgroundjobsbackgroundjobcreate) | **POST** /api/BackgroundJob/BackgroundJobs/Create |  |
| [**BackgroundJobsBackgroundJobGetBackgroundJobById**](BackgroundJobApi.md#backgroundjobsbackgroundjobgetbackgroundjobbyid) | **GET** /api/BackgroundJob/BackgroundJobs/GetBackgroundJobById |  |
| [**BackgroundJobsBackgroundJobGetBackgroundJobList**](BackgroundJobApi.md#backgroundjobsbackgroundjobgetbackgroundjoblist) | **GET** /api/BackgroundJob/BackgroundJobs/GetBackgroundJobList |  |
| [**BackgroundJobsBackgroundJobMarkExecutionStarted**](BackgroundJobApi.md#backgroundjobsbackgroundjobmarkexecutionstarted) | **POST** /api/BackgroundJob/BackgroundJobs/MarkExecutionStarted |  |
| [**BackgroundJobsBackgroundJobRetryBackgroundJob**](BackgroundJobApi.md#backgroundjobsbackgroundjobretrybackgroundjob) | **POST** /api/BackgroundJob/BackgroundJobs/RetryBackgroundJob |  |

<a id="backgroundjobsbackgroundjobcancelbackgroundjob"></a>
# **BackgroundJobsBackgroundJobCancelBackgroundJob**
> bool BackgroundJobsBackgroundJobCancelBackgroundJob (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobCancelBackgroundJobExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.BackgroundJobsBackgroundJobCancelBackgroundJob(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobCancelBackgroundJob: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobCancelBackgroundJobWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.BackgroundJobsBackgroundJobCancelBackgroundJobWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobCancelBackgroundJobWithHttpInfo: " + e.Message);
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

<a id="backgroundjobsbackgroundjobcomplete"></a>
# **BackgroundJobsBackgroundJobComplete**
> BackgroundJobsBackgroundJobDto BackgroundJobsBackgroundJobComplete (BackgroundJobsBackgroundJobExecutionCompleteDto backgroundJobsBackgroundJobExecutionCompleteDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobCompleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var backgroundJobsBackgroundJobExecutionCompleteDto = new BackgroundJobsBackgroundJobExecutionCompleteDto(); // BackgroundJobsBackgroundJobExecutionCompleteDto |  (optional) 

            try
            {
                BackgroundJobsBackgroundJobDto result = apiInstance.BackgroundJobsBackgroundJobComplete(backgroundJobsBackgroundJobExecutionCompleteDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobComplete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobCompleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<BackgroundJobsBackgroundJobDto> response = apiInstance.BackgroundJobsBackgroundJobCompleteWithHttpInfo(backgroundJobsBackgroundJobExecutionCompleteDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobCompleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **backgroundJobsBackgroundJobExecutionCompleteDto** | [**BackgroundJobsBackgroundJobExecutionCompleteDto**](BackgroundJobsBackgroundJobExecutionCompleteDto.md) |  | [optional]  |

### Return type

[**BackgroundJobsBackgroundJobDto**](BackgroundJobsBackgroundJobDto.md)

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

<a id="backgroundjobsbackgroundjobcreate"></a>
# **BackgroundJobsBackgroundJobCreate**
> BackgroundJobsBackgroundJobDto BackgroundJobsBackgroundJobCreate (BackgroundJobsCreateBackgroundJobDto backgroundJobsCreateBackgroundJobDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var backgroundJobsCreateBackgroundJobDto = new BackgroundJobsCreateBackgroundJobDto(); // BackgroundJobsCreateBackgroundJobDto |  (optional) 

            try
            {
                BackgroundJobsBackgroundJobDto result = apiInstance.BackgroundJobsBackgroundJobCreate(backgroundJobsCreateBackgroundJobDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<BackgroundJobsBackgroundJobDto> response = apiInstance.BackgroundJobsBackgroundJobCreateWithHttpInfo(backgroundJobsCreateBackgroundJobDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **backgroundJobsCreateBackgroundJobDto** | [**BackgroundJobsCreateBackgroundJobDto**](BackgroundJobsCreateBackgroundJobDto.md) |  | [optional]  |

### Return type

[**BackgroundJobsBackgroundJobDto**](BackgroundJobsBackgroundJobDto.md)

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

<a id="backgroundjobsbackgroundjobgetbackgroundjobbyid"></a>
# **BackgroundJobsBackgroundJobGetBackgroundJobById**
> BackgroundJobsFullBackgroundJobDto BackgroundJobsBackgroundJobGetBackgroundJobById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobGetBackgroundJobByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                BackgroundJobsFullBackgroundJobDto result = apiInstance.BackgroundJobsBackgroundJobGetBackgroundJobById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobGetBackgroundJobById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobGetBackgroundJobByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<BackgroundJobsFullBackgroundJobDto> response = apiInstance.BackgroundJobsBackgroundJobGetBackgroundJobByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobGetBackgroundJobByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**BackgroundJobsFullBackgroundJobDto**](BackgroundJobsFullBackgroundJobDto.md)

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

<a id="backgroundjobsbackgroundjobgetbackgroundjoblist"></a>
# **BackgroundJobsBackgroundJobGetBackgroundJobList**
> EleoncorePagedResultDtoOfBackgroundJobsBackgroundJobHeaderDto BackgroundJobsBackgroundJobGetBackgroundJobList (string searchQuery = null, List<EleoncoreBackgroundJobStatus> statusFilter = null, List<string> objectTypeFilter = null, List<string> typeFilter = null, DateTime creationDateFilterStart = null, DateTime creationDateFilterEnd = null, DateTime lastExecutionDateFilterStart = null, DateTime lastExecutionDateFilterEnd = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobGetBackgroundJobListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var searchQuery = "searchQuery_example";  // string |  (optional) 
            var statusFilter = new List<EleoncoreBackgroundJobStatus>(); // List<EleoncoreBackgroundJobStatus> |  (optional) 
            var objectTypeFilter = new List<string>(); // List<string> |  (optional) 
            var typeFilter = new List<string>(); // List<string> |  (optional) 
            var creationDateFilterStart = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var creationDateFilterEnd = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var lastExecutionDateFilterStart = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var lastExecutionDateFilterEnd = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfBackgroundJobsBackgroundJobHeaderDto result = apiInstance.BackgroundJobsBackgroundJobGetBackgroundJobList(searchQuery, statusFilter, objectTypeFilter, typeFilter, creationDateFilterStart, creationDateFilterEnd, lastExecutionDateFilterStart, lastExecutionDateFilterEnd, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobGetBackgroundJobList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobGetBackgroundJobListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfBackgroundJobsBackgroundJobHeaderDto> response = apiInstance.BackgroundJobsBackgroundJobGetBackgroundJobListWithHttpInfo(searchQuery, statusFilter, objectTypeFilter, typeFilter, creationDateFilterStart, creationDateFilterEnd, lastExecutionDateFilterStart, lastExecutionDateFilterEnd, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobGetBackgroundJobListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **searchQuery** | **string** |  | [optional]  |
| **statusFilter** | [**List&lt;EleoncoreBackgroundJobStatus&gt;**](EleoncoreBackgroundJobStatus.md) |  | [optional]  |
| **objectTypeFilter** | [**List&lt;string&gt;**](string.md) |  | [optional]  |
| **typeFilter** | [**List&lt;string&gt;**](string.md) |  | [optional]  |
| **creationDateFilterStart** | **DateTime** |  | [optional]  |
| **creationDateFilterEnd** | **DateTime** |  | [optional]  |
| **lastExecutionDateFilterStart** | **DateTime** |  | [optional]  |
| **lastExecutionDateFilterEnd** | **DateTime** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfBackgroundJobsBackgroundJobHeaderDto**](EleoncorePagedResultDtoOfBackgroundJobsBackgroundJobHeaderDto.md)

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

<a id="backgroundjobsbackgroundjobmarkexecutionstarted"></a>
# **BackgroundJobsBackgroundJobMarkExecutionStarted**
> bool BackgroundJobsBackgroundJobMarkExecutionStarted (Guid jobId = null, Guid executionId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobMarkExecutionStartedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var jobId = "jobId_example";  // Guid |  (optional) 
            var executionId = "executionId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.BackgroundJobsBackgroundJobMarkExecutionStarted(jobId, executionId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobMarkExecutionStarted: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobMarkExecutionStartedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.BackgroundJobsBackgroundJobMarkExecutionStartedWithHttpInfo(jobId, executionId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobMarkExecutionStartedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **jobId** | **Guid** |  | [optional]  |
| **executionId** | **Guid** |  | [optional]  |

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

<a id="backgroundjobsbackgroundjobretrybackgroundjob"></a>
# **BackgroundJobsBackgroundJobRetryBackgroundJob**
> bool BackgroundJobsBackgroundJobRetryBackgroundJob (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class BackgroundJobsBackgroundJobRetryBackgroundJobExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BackgroundJobApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.BackgroundJobsBackgroundJobRetryBackgroundJob(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobRetryBackgroundJob: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the BackgroundJobsBackgroundJobRetryBackgroundJobWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.BackgroundJobsBackgroundJobRetryBackgroundJobWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BackgroundJobApi.BackgroundJobsBackgroundJobRetryBackgroundJobWithHttpInfo: " + e.Message);
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

