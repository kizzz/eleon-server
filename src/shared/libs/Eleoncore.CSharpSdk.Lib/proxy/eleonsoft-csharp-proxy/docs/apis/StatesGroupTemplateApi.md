# EleonsoftProxy.Api.StatesGroupTemplateApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**LifecycleStatesGroupTemplateAdd**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplateadd) | **POST** /api/Lifecycle/Templates/StatesGroup/Add |  |
| [**LifecycleStatesGroupTemplateEnable**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplateenable) | **PUT** /api/Lifecycle/Templates/StatesGroup/Enable |  |
| [**LifecycleStatesGroupTemplateGet**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplateget) | **GET** /api/Lifecycle/Templates/StatesGroup/Get |  |
| [**LifecycleStatesGroupTemplateGetList**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplategetlist) | **GET** /api/Lifecycle/Templates/StatesGroup/GetList |  |
| [**LifecycleStatesGroupTemplateRemove**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplateremove) | **DELETE** /api/Lifecycle/Templates/StatesGroup/Remove |  |
| [**LifecycleStatesGroupTemplateRename**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplaterename) | **POST** /api/Lifecycle/Templates/StatesGroup/Rename |  |
| [**LifecycleStatesGroupTemplateUpdate**](StatesGroupTemplateApi.md#lifecyclestatesgrouptemplateupdate) | **POST** /api/Lifecycle/Templates/StatesGroup/Update |  |

<a id="lifecyclestatesgrouptemplateadd"></a>
# **LifecycleStatesGroupTemplateAdd**
> bool LifecycleStatesGroupTemplateAdd (LifecycleStatesGroupTemplateDto lifecycleStatesGroupTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateAddExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var lifecycleStatesGroupTemplateDto = new LifecycleStatesGroupTemplateDto(); // LifecycleStatesGroupTemplateDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupTemplateAdd(lifecycleStatesGroupTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateAdd: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateAddWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupTemplateAddWithHttpInfo(lifecycleStatesGroupTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateAddWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStatesGroupTemplateDto** | [**LifecycleStatesGroupTemplateDto**](LifecycleStatesGroupTemplateDto.md) |  | [optional]  |

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

<a id="lifecyclestatesgrouptemplateenable"></a>
# **LifecycleStatesGroupTemplateEnable**
> bool LifecycleStatesGroupTemplateEnable (LifecycleStatesGroupSwitchDto lifecycleStatesGroupSwitchDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateEnableExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var lifecycleStatesGroupSwitchDto = new LifecycleStatesGroupSwitchDto(); // LifecycleStatesGroupSwitchDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupTemplateEnable(lifecycleStatesGroupSwitchDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateEnable: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateEnableWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupTemplateEnableWithHttpInfo(lifecycleStatesGroupSwitchDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateEnableWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStatesGroupSwitchDto** | [**LifecycleStatesGroupSwitchDto**](LifecycleStatesGroupSwitchDto.md) |  | [optional]  |

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

<a id="lifecyclestatesgrouptemplateget"></a>
# **LifecycleStatesGroupTemplateGet**
> LifecycleFullStatesGroupTemplateDto LifecycleStatesGroupTemplateGet (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                LifecycleFullStatesGroupTemplateDto result = apiInstance.LifecycleStatesGroupTemplateGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<LifecycleFullStatesGroupTemplateDto> response = apiInstance.LifecycleStatesGroupTemplateGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**LifecycleFullStatesGroupTemplateDto**](LifecycleFullStatesGroupTemplateDto.md)

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

<a id="lifecyclestatesgrouptemplategetlist"></a>
# **LifecycleStatesGroupTemplateGetList**
> EleoncorePagedResultDtoOfLifecycleStatesGroupTemplateDto LifecycleStatesGroupTemplateGetList (string documentObjectType = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfLifecycleStatesGroupTemplateDto result = apiInstance.LifecycleStatesGroupTemplateGetList(documentObjectType, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfLifecycleStatesGroupTemplateDto> response = apiInstance.LifecycleStatesGroupTemplateGetListWithHttpInfo(documentObjectType, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfLifecycleStatesGroupTemplateDto**](EleoncorePagedResultDtoOfLifecycleStatesGroupTemplateDto.md)

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

<a id="lifecyclestatesgrouptemplateremove"></a>
# **LifecycleStatesGroupTemplateRemove**
> bool LifecycleStatesGroupTemplateRemove (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateRemoveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupTemplateRemove(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateRemove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateRemoveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupTemplateRemoveWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateRemoveWithHttpInfo: " + e.Message);
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

<a id="lifecyclestatesgrouptemplaterename"></a>
# **LifecycleStatesGroupTemplateRename**
> bool LifecycleStatesGroupTemplateRename (Guid id = null, string newName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateRenameExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var newName = "newName_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupTemplateRename(id, newName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateRename: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateRenameWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupTemplateRenameWithHttpInfo(id, newName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateRenameWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **newName** | **string** |  | [optional]  |

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

<a id="lifecyclestatesgrouptemplateupdate"></a>
# **LifecycleStatesGroupTemplateUpdate**
> bool LifecycleStatesGroupTemplateUpdate (LifecycleStatesGroupTemplateDto lifecycleStatesGroupTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class LifecycleStatesGroupTemplateUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StatesGroupTemplateApi(config);
            var lifecycleStatesGroupTemplateDto = new LifecycleStatesGroupTemplateDto(); // LifecycleStatesGroupTemplateDto |  (optional) 

            try
            {
                bool result = apiInstance.LifecycleStatesGroupTemplateUpdate(lifecycleStatesGroupTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the LifecycleStatesGroupTemplateUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.LifecycleStatesGroupTemplateUpdateWithHttpInfo(lifecycleStatesGroupTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StatesGroupTemplateApi.LifecycleStatesGroupTemplateUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **lifecycleStatesGroupTemplateDto** | [**LifecycleStatesGroupTemplateDto**](LifecycleStatesGroupTemplateDto.md) |  | [optional]  |

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

