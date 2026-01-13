# EleonsoftProxy.Api.LifecycleManagerApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LifecycleLifecycleManagerApprove**](LifecycleManagerApi.md#lifecyclelifecyclemanagerapprove) | **POST** /api/Lifecycle/Manager/Approve |  |
| [**LifecycleLifecycleManagerCanApprove**](LifecycleManagerApi.md#lifecyclelifecyclemanagercanapprove) | **GET** /api/Lifecycle/Manager/CanApprove |  |
| [**LifecycleLifecycleManagerCanReview**](LifecycleManagerApi.md#lifecyclelifecyclemanagercanreview) | **GET** /api/Lifecycle/Manager/CanReview |  |
| [**LifecycleLifecycleManagerGetDocumentIdsByFilter**](LifecycleManagerApi.md#lifecyclelifecyclemanagergetdocumentidsbyfilter) | **GET** /api/Lifecycle/Manager/GetDocumentIdsByFilter |  |
| [**LifecycleLifecycleManagerGetLifecycleStatus**](LifecycleManagerApi.md#lifecyclelifecyclemanagergetlifecyclestatus) | **GET** /api/Lifecycle/Manager/GetLifecycleStatus |  |
| [**LifecycleLifecycleManagerGetTrace**](LifecycleManagerApi.md#lifecyclelifecyclemanagergettrace) | **GET** /api/Lifecycle/Manager/GetTrace |  |
| [**LifecycleLifecycleManagerGetViewPermission**](LifecycleManagerApi.md#lifecyclelifecyclemanagergetviewpermission) | **GET** /api/Lifecycle/Manager/GetViewPermission |  |
| [**LifecycleLifecycleManagerReject**](LifecycleManagerApi.md#lifecyclelifecyclemanagerreject) | **POST** /api/Lifecycle/Manager/Reject |  |
| [**LifecycleLifecycleManagerStartExistingLifecycle**](LifecycleManagerApi.md#lifecyclelifecyclemanagerstartexistinglifecycle) | **POST** /api/Lifecycle/Manager/StartExistingLifecycle |  |
| [**LifecycleLifecycleManagerStartNewLifecycle**](LifecycleManagerApi.md#lifecyclelifecyclemanagerstartnewlifecycle) | **POST** /api/Lifecycle/Manager/StartNewLifecycle |  |

<a id="lifecyclelifecyclemanagerapprove"></a>
# **LifecycleLifecycleManagerApprove**
> bool LifecycleLifecycleManagerApprove (string documentObjectType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerApproveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleLifecycleManagerApprove(documentObjectType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerApprove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerApproveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleLifecycleManagerApproveWithHttpInfo(documentObjectType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerApproveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
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

<a id="lifecyclelifecyclemanagercanapprove"></a>
# **LifecycleLifecycleManagerCanApprove**
> bool LifecycleLifecycleManagerCanApprove (string documentObjectType = null, string docId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerCanApproveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var docId = "docId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleLifecycleManagerCanApprove(documentObjectType, docId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerCanApprove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerCanApproveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleLifecycleManagerCanApproveWithHttpInfo(documentObjectType, docId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerCanApproveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **docId** | **string** |  | [optional]  |

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

<a id="lifecyclelifecyclemanagercanreview"></a>
# **LifecycleLifecycleManagerCanReview**
> bool LifecycleLifecycleManagerCanReview (string documentObjectType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerCanReviewExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleLifecycleManagerCanReview(documentObjectType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerCanReview: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerCanReviewWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleLifecycleManagerCanReviewWithHttpInfo(documentObjectType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerCanReviewWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
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

<a id="lifecyclelifecyclemanagergetdocumentidsbyfilter"></a>
# **LifecycleLifecycleManagerGetDocumentIdsByFilter**
> List&lt;string&gt; LifecycleLifecycleManagerGetDocumentIdsByFilter (string documentObjectType = null, Guid userId = null, List<string> roles = null, List<EleoncoreLifecycleStatus> lifecycleStatuses = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerGetDocumentIdsByFilterExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var userId = "userId_example";  // Guid |  (optional) 
            var roles = new List<string>(); // List<string> |  (optional) 
            var lifecycleStatuses = new List<EleoncoreLifecycleStatus>(); // List<EleoncoreLifecycleStatus> |  (optional) 

            try
            {
                List<string> result = apiInstance.LifecycleLifecycleManagerGetDocumentIdsByFilter(documentObjectType, userId, roles, lifecycleStatuses);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetDocumentIdsByFilter: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerGetDocumentIdsByFilterWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.LifecycleLifecycleManagerGetDocumentIdsByFilterWithHttpInfo(documentObjectType, userId, roles, lifecycleStatuses);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetDocumentIdsByFilterWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **userId** | **Guid** |  | [optional]  |
| **roles** | [**List&lt;string&gt;**](string.md) |  | [optional]  |
| **lifecycleStatuses** | [**List&lt;EleoncoreLifecycleStatus&gt;**](EleoncoreLifecycleStatus.md) |  | [optional]  |

### Return type

**List<string>**

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

<a id="lifecyclelifecyclemanagergetlifecyclestatus"></a>
# **LifecycleLifecycleManagerGetLifecycleStatus**
> LifecycleLifecycleStatusDto LifecycleLifecycleManagerGetLifecycleStatus (string documentObjectType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerGetLifecycleStatusExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                LifecycleLifecycleStatusDto result = apiInstance.LifecycleLifecycleManagerGetLifecycleStatus(documentObjectType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetLifecycleStatus: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerGetLifecycleStatusWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleLifecycleStatusDto> response = apiInstance.LifecycleLifecycleManagerGetLifecycleStatusWithHttpInfo(documentObjectType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetLifecycleStatusWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |

### Return type

[**LifecycleLifecycleStatusDto**](LifecycleLifecycleStatusDto.md)

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

<a id="lifecyclelifecyclemanagergettrace"></a>
# **LifecycleLifecycleManagerGetTrace**
> LifecycleStatesGroupAuditTreeDto LifecycleLifecycleManagerGetTrace (string documentObjectType = null, string docId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerGetTraceExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var docId = "docId_example";  // string |  (optional) 

            try
            {
                LifecycleStatesGroupAuditTreeDto result = apiInstance.LifecycleLifecycleManagerGetTrace(documentObjectType, docId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetTrace: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerGetTraceWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleStatesGroupAuditTreeDto> response = apiInstance.LifecycleLifecycleManagerGetTraceWithHttpInfo(documentObjectType, docId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetTraceWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **docId** | **string** |  | [optional]  |

### Return type

[**LifecycleStatesGroupAuditTreeDto**](LifecycleStatesGroupAuditTreeDto.md)

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

<a id="lifecyclelifecyclemanagergetviewpermission"></a>
# **LifecycleLifecycleManagerGetViewPermission**
> bool LifecycleLifecycleManagerGetViewPermission (Guid initiatorId = null, string documentObjectType = null, string documentId = null, bool review = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerGetViewPermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var initiatorId = "initiatorId_example";  // Guid |  (optional) 
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 
            var review = true;  // bool |  (optional)  (default to true)

            try
            {
                bool result = apiInstance.LifecycleLifecycleManagerGetViewPermission(initiatorId, documentObjectType, documentId, review);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetViewPermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerGetViewPermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleLifecycleManagerGetViewPermissionWithHttpInfo(initiatorId, documentObjectType, documentId, review);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerGetViewPermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **initiatorId** | **Guid** |  | [optional]  |
| **documentObjectType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |
| **review** | **bool** |  | [optional] [default to true] |

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

<a id="lifecyclelifecyclemanagerreject"></a>
# **LifecycleLifecycleManagerReject**
> bool LifecycleLifecycleManagerReject (string documentObjectType = null, string documentId = null, string reason = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerRejectExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 
            var reason = "reason_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleLifecycleManagerReject(documentObjectType, documentId, reason);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerReject: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerRejectWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleLifecycleManagerRejectWithHttpInfo(documentObjectType, documentId, reason);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerRejectWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |
| **reason** | **string** |  | [optional]  |

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

<a id="lifecyclelifecyclemanagerstartexistinglifecycle"></a>
# **LifecycleLifecycleManagerStartExistingLifecycle**
> LifecycleStatesGroupAuditTreeDto LifecycleLifecycleManagerStartExistingLifecycle (string documentObjectType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerStartExistingLifecycleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                LifecycleStatesGroupAuditTreeDto result = apiInstance.LifecycleLifecycleManagerStartExistingLifecycle(documentObjectType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerStartExistingLifecycle: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerStartExistingLifecycleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleStatesGroupAuditTreeDto> response = apiInstance.LifecycleLifecycleManagerStartExistingLifecycleWithHttpInfo(documentObjectType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerStartExistingLifecycleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |

### Return type

[**LifecycleStatesGroupAuditTreeDto**](LifecycleStatesGroupAuditTreeDto.md)

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

<a id="lifecyclelifecyclemanagerstartnewlifecycle"></a>
# **LifecycleLifecycleManagerStartNewLifecycle**
> LifecycleStatesGroupAuditTreeDto LifecycleLifecycleManagerStartNewLifecycle (LifecycleStartNewLifecycleRequestDto lifecycleStartNewLifecycleRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleLifecycleManagerStartNewLifecycleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new LifecycleManagerApi(config);
            var lifecycleStartNewLifecycleRequestDto = new LifecycleStartNewLifecycleRequestDto(); // LifecycleStartNewLifecycleRequestDto |  (optional) 

            try
            {
                LifecycleStatesGroupAuditTreeDto result = apiInstance.LifecycleLifecycleManagerStartNewLifecycle(lifecycleStartNewLifecycleRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerStartNewLifecycle: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleLifecycleManagerStartNewLifecycleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleStatesGroupAuditTreeDto> response = apiInstance.LifecycleLifecycleManagerStartNewLifecycleWithHttpInfo(lifecycleStartNewLifecycleRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling LifecycleManagerApi.LifecycleLifecycleManagerStartNewLifecycleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStartNewLifecycleRequestDto** | [**LifecycleStartNewLifecycleRequestDto**](LifecycleStartNewLifecycleRequestDto.md) |  | [optional]  |

### Return type

[**LifecycleStatesGroupAuditTreeDto**](LifecycleStatesGroupAuditTreeDto.md)

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

