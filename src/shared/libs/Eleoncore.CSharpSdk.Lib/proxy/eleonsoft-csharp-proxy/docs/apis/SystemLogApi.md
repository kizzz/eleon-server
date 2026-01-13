# EleonsoftProxy.Api.SystemLogApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**DocMessageLogSystemLogGetById**](SystemLogApi.md#docmessagelogsystemloggetbyid) | **GET** /api/SystemLog/SystemLog/GetById |  |
| [**DocMessageLogSystemLogGetList**](SystemLogApi.md#docmessagelogsystemloggetlist) | **GET** /api/SystemLog/SystemLog/GetList |  |
| [**DocMessageLogSystemLogGetTotalUnresolvedCount**](SystemLogApi.md#docmessagelogsystemloggettotalunresolvedcount) | **GET** /api/SystemLog/SystemLog/GetTotalUnresolvedCount |  |
| [**DocMessageLogSystemLogMarkAllReaded**](SystemLogApi.md#docmessagelogsystemlogmarkallreaded) | **POST** /api/SystemLog/SystemLog/MarkAllReaded |  |
| [**DocMessageLogSystemLogMarkReaded**](SystemLogApi.md#docmessagelogsystemlogmarkreaded) | **POST** /api/SystemLog/SystemLog/MarkReaded |  |
| [**DocMessageLogSystemLogWrite**](SystemLogApi.md#docmessagelogsystemlogwrite) | **POST** /api/SystemLog/SystemLog/Write |  |
| [**DocMessageLogSystemLogWriteMany**](SystemLogApi.md#docmessagelogsystemlogwritemany) | **POST** /api/SystemLog/SystemLog/WriteMany |  |

<a id="docmessagelogsystemloggetbyid"></a>
# **DocMessageLogSystemLogGetById**
> EleonsoftModuleCollectorFullSystemLogDto DocMessageLogSystemLogGetById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                EleonsoftModuleCollectorFullSystemLogDto result = apiInstance.DocMessageLogSystemLogGetById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorFullSystemLogDto> response = apiInstance.DocMessageLogSystemLogGetByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorFullSystemLogDto**](EleonsoftModuleCollectorFullSystemLogDto.md)

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

<a id="docmessagelogsystemloggetlist"></a>
# **DocMessageLogSystemLogGetList**
> EleoncorePagedResultDtoOfDocMessageLogSystemLogDto DocMessageLogSystemLogGetList (string searchQuery = null, string docIdFilter = null, EleonSystemLogLevel minLogLevelFilter = null, string initiatorFilter = null, string initiatorTypeFilter = null, string applicationNameFilter = null, DateTime creationFromDateFilter = null, DateTime creationToDateFilter = null, bool onlyUnread = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);
            var searchQuery = "searchQuery_example";  // string |  (optional) 
            var docIdFilter = "docIdFilter_example";  // string |  (optional) 
            var minLogLevelFilter = (EleonSystemLogLevel) "0";  // EleonSystemLogLevel |  (optional) 
            var initiatorFilter = "initiatorFilter_example";  // string |  (optional) 
            var initiatorTypeFilter = "initiatorTypeFilter_example";  // string |  (optional) 
            var applicationNameFilter = "applicationNameFilter_example";  // string |  (optional) 
            var creationFromDateFilter = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var creationToDateFilter = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var onlyUnread = true;  // bool |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfDocMessageLogSystemLogDto result = apiInstance.DocMessageLogSystemLogGetList(searchQuery, docIdFilter, minLogLevelFilter, initiatorFilter, initiatorTypeFilter, applicationNameFilter, creationFromDateFilter, creationToDateFilter, onlyUnread, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfDocMessageLogSystemLogDto> response = apiInstance.DocMessageLogSystemLogGetListWithHttpInfo(searchQuery, docIdFilter, minLogLevelFilter, initiatorFilter, initiatorTypeFilter, applicationNameFilter, creationFromDateFilter, creationToDateFilter, onlyUnread, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **searchQuery** | **string** |  | [optional]  |
| **docIdFilter** | **string** |  | [optional]  |
| **minLogLevelFilter** | **EleonSystemLogLevel** |  | [optional]  |
| **initiatorFilter** | **string** |  | [optional]  |
| **initiatorTypeFilter** | **string** |  | [optional]  |
| **applicationNameFilter** | **string** |  | [optional]  |
| **creationFromDateFilter** | **DateTime** |  | [optional]  |
| **creationToDateFilter** | **DateTime** |  | [optional]  |
| **onlyUnread** | **bool** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfDocMessageLogSystemLogDto**](EleoncorePagedResultDtoOfDocMessageLogSystemLogDto.md)

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

<a id="docmessagelogsystemloggettotalunresolvedcount"></a>
# **DocMessageLogSystemLogGetTotalUnresolvedCount**
> DocMessageLogUnresolvedSystemLogCountDto DocMessageLogSystemLogGetTotalUnresolvedCount ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogGetTotalUnresolvedCountExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);

            try
            {
                DocMessageLogUnresolvedSystemLogCountDto result = apiInstance.DocMessageLogSystemLogGetTotalUnresolvedCount();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetTotalUnresolvedCount: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogGetTotalUnresolvedCountWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<DocMessageLogUnresolvedSystemLogCountDto> response = apiInstance.DocMessageLogSystemLogGetTotalUnresolvedCountWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogGetTotalUnresolvedCountWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**DocMessageLogUnresolvedSystemLogCountDto**](DocMessageLogUnresolvedSystemLogCountDto.md)

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

<a id="docmessagelogsystemlogmarkallreaded"></a>
# **DocMessageLogSystemLogMarkAllReaded**
> void DocMessageLogSystemLogMarkAllReaded ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogMarkAllReadedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);

            try
            {
                apiInstance.DocMessageLogSystemLogMarkAllReaded();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogMarkAllReaded: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogMarkAllReadedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.DocMessageLogSystemLogMarkAllReadedWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogMarkAllReadedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
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

<a id="docmessagelogsystemlogmarkreaded"></a>
# **DocMessageLogSystemLogMarkReaded**
> bool DocMessageLogSystemLogMarkReaded (EleonsoftModuleCollectorMarkSystemLogsReadedRequestDto eleonsoftModuleCollectorMarkSystemLogsReadedRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogMarkReadedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);
            var eleonsoftModuleCollectorMarkSystemLogsReadedRequestDto = new EleonsoftModuleCollectorMarkSystemLogsReadedRequestDto(); // EleonsoftModuleCollectorMarkSystemLogsReadedRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.DocMessageLogSystemLogMarkReaded(eleonsoftModuleCollectorMarkSystemLogsReadedRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogMarkReaded: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogMarkReadedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.DocMessageLogSystemLogMarkReadedWithHttpInfo(eleonsoftModuleCollectorMarkSystemLogsReadedRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogMarkReadedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorMarkSystemLogsReadedRequestDto** | [**EleonsoftModuleCollectorMarkSystemLogsReadedRequestDto**](EleonsoftModuleCollectorMarkSystemLogsReadedRequestDto.md) |  | [optional]  |

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

<a id="docmessagelogsystemlogwrite"></a>
# **DocMessageLogSystemLogWrite**
> bool DocMessageLogSystemLogWrite (EleonsoftModuleCollectorCreateSystemLogDto eleonsoftModuleCollectorCreateSystemLogDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogWriteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);
            var eleonsoftModuleCollectorCreateSystemLogDto = new EleonsoftModuleCollectorCreateSystemLogDto(); // EleonsoftModuleCollectorCreateSystemLogDto |  (optional) 

            try
            {
                bool result = apiInstance.DocMessageLogSystemLogWrite(eleonsoftModuleCollectorCreateSystemLogDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogWrite: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogWriteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.DocMessageLogSystemLogWriteWithHttpInfo(eleonsoftModuleCollectorCreateSystemLogDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogWriteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorCreateSystemLogDto** | [**EleonsoftModuleCollectorCreateSystemLogDto**](EleonsoftModuleCollectorCreateSystemLogDto.md) |  | [optional]  |

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

<a id="docmessagelogsystemlogwritemany"></a>
# **DocMessageLogSystemLogWriteMany**
> bool DocMessageLogSystemLogWriteMany (List<EleonsoftModuleCollectorCreateSystemLogDto> eleonsoftModuleCollectorCreateSystemLogDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class DocMessageLogSystemLogWriteManyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SystemLogApi(config);
            var eleonsoftModuleCollectorCreateSystemLogDto = new List<EleonsoftModuleCollectorCreateSystemLogDto>(); // List<EleonsoftModuleCollectorCreateSystemLogDto> |  (optional) 

            try
            {
                bool result = apiInstance.DocMessageLogSystemLogWriteMany(eleonsoftModuleCollectorCreateSystemLogDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogWriteMany: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the DocMessageLogSystemLogWriteManyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.DocMessageLogSystemLogWriteManyWithHttpInfo(eleonsoftModuleCollectorCreateSystemLogDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SystemLogApi.DocMessageLogSystemLogWriteManyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorCreateSystemLogDto** | [**List&lt;EleonsoftModuleCollectorCreateSystemLogDto&gt;**](EleonsoftModuleCollectorCreateSystemLogDto.md) |  | [optional]  |

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

