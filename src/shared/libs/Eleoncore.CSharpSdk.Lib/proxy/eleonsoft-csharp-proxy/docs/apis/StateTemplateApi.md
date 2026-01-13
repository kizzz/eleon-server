# EleonsoftProxy.Api.StateTemplateApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LifecycleStateTemplateAdd**](StateTemplateApi.md#lifecyclestatetemplateadd) | **POST** /api/Lifecycle/Templates/State/Add |  |
| [**LifecycleStateTemplateEnable**](StateTemplateApi.md#lifecyclestatetemplateenable) | **PUT** /api/Lifecycle/Templates/State/Enable |  |
| [**LifecycleStateTemplateGetAll**](StateTemplateApi.md#lifecyclestatetemplategetall) | **GET** /api/Lifecycle/Templates/State/GetAll |  |
| [**LifecycleStateTemplateRemove**](StateTemplateApi.md#lifecyclestatetemplateremove) | **DELETE** /api/Lifecycle/Templates/State/Remove |  |
| [**LifecycleStateTemplateUpdateApprovalType**](StateTemplateApi.md#lifecyclestatetemplateupdateapprovaltype) | **PUT** /api/Lifecycle/Templates/State/UpdateApprovalType |  |
| [**LifecycleStateTemplateUpdateName**](StateTemplateApi.md#lifecyclestatetemplateupdatename) | **PUT** /api/Lifecycle/Templates/State/UpdateName |  |
| [**LifecycleStateTemplateUpdateOrderIndex**](StateTemplateApi.md#lifecyclestatetemplateupdateorderindex) | **PUT** /api/Lifecycle/Templates/State/UpdateOrderIndex |  |
| [**LifecycleStateTemplateUpdateOrderIndexes**](StateTemplateApi.md#lifecyclestatetemplateupdateorderindexes) | **PUT** /api/Lifecycle/Templates/State/UpdateOrderIndexes |  |

<a id="lifecyclestatetemplateadd"></a>
# **LifecycleStateTemplateAdd**
> bool LifecycleStateTemplateAdd (LifecycleStateTemplateDto lifecycleStateTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateAddExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var lifecycleStateTemplateDto = new LifecycleStateTemplateDto(); // LifecycleStateTemplateDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateAdd(lifecycleStateTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateAdd: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateAddWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateAddWithHttpInfo(lifecycleStateTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateAddWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStateTemplateDto** | [**LifecycleStateTemplateDto**](LifecycleStateTemplateDto.md) |  | [optional]  |

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

<a id="lifecyclestatetemplateenable"></a>
# **LifecycleStateTemplateEnable**
> bool LifecycleStateTemplateEnable (LifecycleStateSwitchDto lifecycleStateSwitchDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateEnableExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var lifecycleStateSwitchDto = new LifecycleStateSwitchDto(); // LifecycleStateSwitchDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateEnable(lifecycleStateSwitchDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateEnable: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateEnableWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateEnableWithHttpInfo(lifecycleStateSwitchDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateEnableWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStateSwitchDto** | [**LifecycleStateSwitchDto**](LifecycleStateSwitchDto.md) |  | [optional]  |

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

<a id="lifecyclestatetemplategetall"></a>
# **LifecycleStateTemplateGetAll**
> List&lt;LifecycleStateTemplateDto&gt; LifecycleStateTemplateGetAll (Guid groupId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var groupId = "groupId_example";  // Guid |  (optional) 

            try
            {
                List<LifecycleStateTemplateDto> result = apiInstance.LifecycleStateTemplateGetAll(groupId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateGetAll: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateGetAllWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<LifecycleStateTemplateDto>> response = apiInstance.LifecycleStateTemplateGetAllWithHttpInfo(groupId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateGetAllWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **groupId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;LifecycleStateTemplateDto&gt;**](LifecycleStateTemplateDto.md)

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

<a id="lifecyclestatetemplateremove"></a>
# **LifecycleStateTemplateRemove**
> bool LifecycleStateTemplateRemove (Guid groupId = null, Guid stateId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateRemoveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var groupId = "groupId_example";  // Guid |  (optional) 
            var stateId = "stateId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateRemove(groupId, stateId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateRemove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateRemoveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateRemoveWithHttpInfo(groupId, stateId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateRemoveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **groupId** | **Guid** |  | [optional]  |
| **stateId** | **Guid** |  | [optional]  |

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

<a id="lifecyclestatetemplateupdateapprovaltype"></a>
# **LifecycleStateTemplateUpdateApprovalType**
> bool LifecycleStateTemplateUpdateApprovalType (LifecycleUpdateApprovalTypeDto lifecycleUpdateApprovalTypeDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateUpdateApprovalTypeExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var lifecycleUpdateApprovalTypeDto = new LifecycleUpdateApprovalTypeDto(); // LifecycleUpdateApprovalTypeDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateUpdateApprovalType(lifecycleUpdateApprovalTypeDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateApprovalType: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateUpdateApprovalTypeWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateUpdateApprovalTypeWithHttpInfo(lifecycleUpdateApprovalTypeDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateApprovalTypeWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleUpdateApprovalTypeDto** | [**LifecycleUpdateApprovalTypeDto**](LifecycleUpdateApprovalTypeDto.md) |  | [optional]  |

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

<a id="lifecyclestatetemplateupdatename"></a>
# **LifecycleStateTemplateUpdateName**
> bool LifecycleStateTemplateUpdateName (Guid id = null, string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateUpdateNameExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var name = "name_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateUpdateName(id, name);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateName: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateUpdateNameWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateUpdateNameWithHttpInfo(id, name);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateNameWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **name** | **string** |  | [optional]  |

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

<a id="lifecyclestatetemplateupdateorderindex"></a>
# **LifecycleStateTemplateUpdateOrderIndex**
> bool LifecycleStateTemplateUpdateOrderIndex (LifecycleUpdateOrderIndexDto lifecycleUpdateOrderIndexDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateUpdateOrderIndexExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var lifecycleUpdateOrderIndexDto = new LifecycleUpdateOrderIndexDto(); // LifecycleUpdateOrderIndexDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateUpdateOrderIndex(lifecycleUpdateOrderIndexDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateOrderIndex: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateUpdateOrderIndexWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateUpdateOrderIndexWithHttpInfo(lifecycleUpdateOrderIndexDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateOrderIndexWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleUpdateOrderIndexDto** | [**LifecycleUpdateOrderIndexDto**](LifecycleUpdateOrderIndexDto.md) |  | [optional]  |

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

<a id="lifecyclestatetemplateupdateorderindexes"></a>
# **LifecycleStateTemplateUpdateOrderIndexes**
> bool LifecycleStateTemplateUpdateOrderIndexes (Dictionary<string, int> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStateTemplateUpdateOrderIndexesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StateTemplateApi(config);
            var requestBody = new Dictionary<string, int>(); // Dictionary<string, int> |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStateTemplateUpdateOrderIndexes(requestBody);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateOrderIndexes: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStateTemplateUpdateOrderIndexesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStateTemplateUpdateOrderIndexesWithHttpInfo(requestBody);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StateTemplateApi.LifecycleStateTemplateUpdateOrderIndexesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **requestBody** | [**Dictionary&lt;string, int&gt;**](int.md) |  | [optional]  |

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

