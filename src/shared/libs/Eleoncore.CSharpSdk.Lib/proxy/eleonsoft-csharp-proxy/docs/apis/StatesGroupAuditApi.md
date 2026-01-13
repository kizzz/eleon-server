# EleonsoftProxy.Api.StatesGroupAuditApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LifecycleStatesGroupAuditAdd**](StatesGroupAuditApi.md#lifecyclestatesgroupauditadd) | **POST** /api/Lifecycle/Audits/StatesGroup/Add |  |
| [**LifecycleStatesGroupAuditDeepCancel**](StatesGroupAuditApi.md#lifecyclestatesgroupauditdeepcancel) | **DELETE** /api/Lifecycle/Audits/StatesGroup/DeepCancel |  |
| [**LifecycleStatesGroupAuditGetById**](StatesGroupAuditApi.md#lifecyclestatesgroupauditgetbyid) | **GET** /api/Lifecycle/Audits/StatesGroup/Get |  |
| [**LifecycleStatesGroupAuditGetPendingApproval**](StatesGroupAuditApi.md#lifecyclestatesgroupauditgetpendingapproval) | **GET** /api/Lifecycle/Audits/StatesGroup/GetPendingApproval |  |
| [**LifecycleStatesGroupAuditRemove**](StatesGroupAuditApi.md#lifecyclestatesgroupauditremove) | **DELETE** /api/Lifecycle/Audits/StatesGroup/Remove |  |

<a id="lifecyclestatesgroupauditadd"></a>
# **LifecycleStatesGroupAuditAdd**
> bool LifecycleStatesGroupAuditAdd (LifecycleStatesGroupAuditDto lifecycleStatesGroupAuditDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupAuditAddExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupAuditApi(config);
            var lifecycleStatesGroupAuditDto = new LifecycleStatesGroupAuditDto(); // LifecycleStatesGroupAuditDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupAuditAdd(lifecycleStatesGroupAuditDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditAdd: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupAuditAddWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupAuditAddWithHttpInfo(lifecycleStatesGroupAuditDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditAddWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStatesGroupAuditDto** | [**LifecycleStatesGroupAuditDto**](LifecycleStatesGroupAuditDto.md) |  | [optional]  |

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

<a id="lifecyclestatesgroupauditdeepcancel"></a>
# **LifecycleStatesGroupAuditDeepCancel**
> bool LifecycleStatesGroupAuditDeepCancel (string docType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupAuditDeepCancelExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupAuditApi(config);
            var docType = "docType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupAuditDeepCancel(docType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditDeepCancel: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupAuditDeepCancelWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupAuditDeepCancelWithHttpInfo(docType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditDeepCancelWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **docType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |

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

<a id="lifecyclestatesgroupauditgetbyid"></a>
# **LifecycleStatesGroupAuditGetById**
> LifecycleStatesGroupAuditDto LifecycleStatesGroupAuditGetById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupAuditGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupAuditApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                LifecycleStatesGroupAuditDto result = apiInstance.LifecycleStatesGroupAuditGetById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupAuditGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleStatesGroupAuditDto> response = apiInstance.LifecycleStatesGroupAuditGetByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**LifecycleStatesGroupAuditDto**](LifecycleStatesGroupAuditDto.md)

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

<a id="lifecyclestatesgroupauditgetpendingapproval"></a>
# **LifecycleStatesGroupAuditGetPendingApproval**
> EleoncorePagedResultDtoOfLifecycleStatesGroupAuditPendingApprovalDto LifecycleStatesGroupAuditGetPendingApproval (string searchQuery = null, DateTime statusDateFilterStart = null, DateTime statusDateFilterEnd = null, List<string> objectTypeFilter = null, Guid userId = null, Guid statesGroupTemplateId = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupAuditGetPendingApprovalExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupAuditApi(config);
            var searchQuery = "searchQuery_example";  // string |  (optional) 
            var statusDateFilterStart = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var statusDateFilterEnd = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var objectTypeFilter = new List<string>(); // List<string> |  (optional) 
            var userId = "userId_example";  // Guid |  (optional) 
            var statesGroupTemplateId = "statesGroupTemplateId_example";  // Guid |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfLifecycleStatesGroupAuditPendingApprovalDto result = apiInstance.LifecycleStatesGroupAuditGetPendingApproval(searchQuery, statusDateFilterStart, statusDateFilterEnd, objectTypeFilter, userId, statesGroupTemplateId, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditGetPendingApproval: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupAuditGetPendingApprovalWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfLifecycleStatesGroupAuditPendingApprovalDto> response = apiInstance.LifecycleStatesGroupAuditGetPendingApprovalWithHttpInfo(searchQuery, statusDateFilterStart, statusDateFilterEnd, objectTypeFilter, userId, statesGroupTemplateId, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditGetPendingApprovalWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **searchQuery** | **string** |  | [optional]  |
| **statusDateFilterStart** | **DateTime** |  | [optional]  |
| **statusDateFilterEnd** | **DateTime** |  | [optional]  |
| **objectTypeFilter** | [**List&lt;string&gt;**](string.md) |  | [optional]  |
| **userId** | **Guid** |  | [optional]  |
| **statesGroupTemplateId** | **Guid** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfLifecycleStatesGroupAuditPendingApprovalDto**](EleoncorePagedResultDtoOfLifecycleStatesGroupAuditPendingApprovalDto.md)

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

<a id="lifecyclestatesgroupauditremove"></a>
# **LifecycleStatesGroupAuditRemove**
> bool LifecycleStatesGroupAuditRemove (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupAuditRemoveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupAuditApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupAuditRemove(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditRemove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupAuditRemoveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupAuditRemoveWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupAuditApi.LifecycleStatesGroupAuditRemoveWithHttpInfo: " + e.Message);
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

