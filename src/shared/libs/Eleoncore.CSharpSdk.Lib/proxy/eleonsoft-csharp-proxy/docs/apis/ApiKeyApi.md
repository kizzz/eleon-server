# EleonsoftProxy.Api.ApiKeyApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreApiKeyAddIdentityApiKey**](ApiKeyApi.md#coreapikeyaddidentityapikey) | **POST** /api/Identity/ApiKeys/AddIdentityApiKey |  |
| [**CoreApiKeyAddSdkKey**](ApiKeyApi.md#coreapikeyaddsdkkey) | **POST** /api/Identity/ApiKeys/AddSdkKey |  |
| [**CoreApiKeyGetApiKeys**](ApiKeyApi.md#coreapikeygetapikeys) | **POST** /api/Identity/ApiKeys/GetApiKeys |  |
| [**CoreApiKeyGetById**](ApiKeyApi.md#coreapikeygetbyid) | **GET** /api/Identity/ApiKeys/GetById |  |
| [**CoreApiKeyRemoveApiKey**](ApiKeyApi.md#coreapikeyremoveapikey) | **POST** /api/Identity/ApiKeys/RemoveApiKey |  |
| [**CoreApiKeyUpdate**](ApiKeyApi.md#coreapikeyupdate) | **POST** /api/Identity/ApiKeys/Update |  |

<a id="coreapikeyaddidentityapikey"></a>
# **CoreApiKeyAddIdentityApiKey**
> ModuleCollectorIdentityApiKeyDto CoreApiKeyAddIdentityApiKey (ModuleCollectorCreateApiKeyDto moduleCollectorCreateApiKeyDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyAddIdentityApiKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var moduleCollectorCreateApiKeyDto = new ModuleCollectorCreateApiKeyDto(); // ModuleCollectorCreateApiKeyDto |  (optional) 

            try
            {
                ModuleCollectorIdentityApiKeyDto result = apiInstance.CoreApiKeyAddIdentityApiKey(moduleCollectorCreateApiKeyDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyAddIdentityApiKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyAddIdentityApiKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorIdentityApiKeyDto> response = apiInstance.CoreApiKeyAddIdentityApiKeyWithHttpInfo(moduleCollectorCreateApiKeyDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyAddIdentityApiKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorCreateApiKeyDto** | [**ModuleCollectorCreateApiKeyDto**](ModuleCollectorCreateApiKeyDto.md) |  | [optional]  |

### Return type

[**ModuleCollectorIdentityApiKeyDto**](ModuleCollectorIdentityApiKeyDto.md)

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

<a id="coreapikeyaddsdkkey"></a>
# **CoreApiKeyAddSdkKey**
> ModuleCollectorIdentityApiKeyDto CoreApiKeyAddSdkKey (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyAddSdkKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                ModuleCollectorIdentityApiKeyDto result = apiInstance.CoreApiKeyAddSdkKey(name);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyAddSdkKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyAddSdkKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorIdentityApiKeyDto> response = apiInstance.CoreApiKeyAddSdkKeyWithHttpInfo(name);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyAddSdkKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

### Return type

[**ModuleCollectorIdentityApiKeyDto**](ModuleCollectorIdentityApiKeyDto.md)

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

<a id="coreapikeygetapikeys"></a>
# **CoreApiKeyGetApiKeys**
> List&lt;ModuleCollectorIdentityApiKeyDto&gt; CoreApiKeyGetApiKeys (ModuleCollectorApiKeyRequestDto moduleCollectorApiKeyRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyGetApiKeysExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var moduleCollectorApiKeyRequestDto = new ModuleCollectorApiKeyRequestDto(); // ModuleCollectorApiKeyRequestDto |  (optional) 

            try
            {
                List<ModuleCollectorIdentityApiKeyDto> result = apiInstance.CoreApiKeyGetApiKeys(moduleCollectorApiKeyRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyGetApiKeys: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyGetApiKeysWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ModuleCollectorIdentityApiKeyDto>> response = apiInstance.CoreApiKeyGetApiKeysWithHttpInfo(moduleCollectorApiKeyRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyGetApiKeysWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorApiKeyRequestDto** | [**ModuleCollectorApiKeyRequestDto**](ModuleCollectorApiKeyRequestDto.md) |  | [optional]  |

### Return type

[**List&lt;ModuleCollectorIdentityApiKeyDto&gt;**](ModuleCollectorIdentityApiKeyDto.md)

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

<a id="coreapikeygetbyid"></a>
# **CoreApiKeyGetById**
> ModuleCollectorIdentityApiKeyDto CoreApiKeyGetById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                ModuleCollectorIdentityApiKeyDto result = apiInstance.CoreApiKeyGetById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorIdentityApiKeyDto> response = apiInstance.CoreApiKeyGetByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**ModuleCollectorIdentityApiKeyDto**](ModuleCollectorIdentityApiKeyDto.md)

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

<a id="coreapikeyremoveapikey"></a>
# **CoreApiKeyRemoveApiKey**
> void CoreApiKeyRemoveApiKey (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyRemoveApiKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreApiKeyRemoveApiKey(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyRemoveApiKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyRemoveApiKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreApiKeyRemoveApiKeyWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyRemoveApiKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

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

<a id="coreapikeyupdate"></a>
# **CoreApiKeyUpdate**
> ModuleCollectorIdentityApiKeyDto CoreApiKeyUpdate (ModuleCollectorUpdateApiKeyDto moduleCollectorUpdateApiKeyDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreApiKeyUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApiKeyApi(config);
            var moduleCollectorUpdateApiKeyDto = new ModuleCollectorUpdateApiKeyDto(); // ModuleCollectorUpdateApiKeyDto |  (optional) 

            try
            {
                ModuleCollectorIdentityApiKeyDto result = apiInstance.CoreApiKeyUpdate(moduleCollectorUpdateApiKeyDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreApiKeyUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorIdentityApiKeyDto> response = apiInstance.CoreApiKeyUpdateWithHttpInfo(moduleCollectorUpdateApiKeyDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApiKeyApi.CoreApiKeyUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorUpdateApiKeyDto** | [**ModuleCollectorUpdateApiKeyDto**](ModuleCollectorUpdateApiKeyDto.md) |  | [optional]  |

### Return type

[**ModuleCollectorIdentityApiKeyDto**](ModuleCollectorIdentityApiKeyDto.md)

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

