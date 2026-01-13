# EleonsoftProxy.Api.StorageProvidersApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**StorageStorageProvidersCreateStorageProvider**](StorageProvidersApi.md#storagestorageproviderscreatestorageprovider) | **POST** /api/Storage/StorageProviders/CreateStorageProvider |  |
| [**StorageStorageProvidersGetPossibleSettings**](StorageProvidersApi.md#storagestorageprovidersgetpossiblesettings) | **GET** /api/Storage/StorageProviders/GetPossibleSettings |  |
| [**StorageStorageProvidersGetStorageProvider**](StorageProvidersApi.md#storagestorageprovidersgetstorageprovider) | **GET** /api/Storage/StorageProviders/GetStorageProvider |  |
| [**StorageStorageProvidersGetStorageProviderTypesList**](StorageProvidersApi.md#storagestorageprovidersgetstorageprovidertypeslist) | **GET** /api/Storage/StorageProviders/GetStorageProviderTypesList |  |
| [**StorageStorageProvidersGetStorageProvidersList**](StorageProvidersApi.md#storagestorageprovidersgetstorageproviderslist) | **GET** /api/Storage/StorageProviders/GetStorageProvidersList |  |
| [**StorageStorageProvidersRemoveStorageProvider**](StorageProvidersApi.md#storagestorageprovidersremovestorageprovider) | **POST** /api/Storage/StorageProviders/RemoveStorageProvider |  |
| [**StorageStorageProvidersSaveStorageProvider**](StorageProvidersApi.md#storagestorageproviderssavestorageprovider) | **POST** /api/Storage/StorageProviders/SaveStorageProvider |  |

<a id="storagestorageproviderscreatestorageprovider"></a>
# **StorageStorageProvidersCreateStorageProvider**
> SharedModuleStorageProviderDto StorageStorageProvidersCreateStorageProvider (EleonCreateStorageProviderDto eleonCreateStorageProviderDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersCreateStorageProviderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);
            var eleonCreateStorageProviderDto = new EleonCreateStorageProviderDto(); // EleonCreateStorageProviderDto |  (optional) 

            try
            {
                SharedModuleStorageProviderDto result = apiInstance.StorageStorageProvidersCreateStorageProvider(eleonCreateStorageProviderDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersCreateStorageProvider: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersCreateStorageProviderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SharedModuleStorageProviderDto> response = apiInstance.StorageStorageProvidersCreateStorageProviderWithHttpInfo(eleonCreateStorageProviderDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersCreateStorageProviderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonCreateStorageProviderDto** | [**EleonCreateStorageProviderDto**](EleonCreateStorageProviderDto.md) |  | [optional]  |

### Return type

[**SharedModuleStorageProviderDto**](SharedModuleStorageProviderDto.md)

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

<a id="storagestorageprovidersgetpossiblesettings"></a>
# **StorageStorageProvidersGetPossibleSettings**
> List&lt;StoragePossibleStorageProviderSettingsDto&gt; StorageStorageProvidersGetPossibleSettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersGetPossibleSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);

            try
            {
                List<StoragePossibleStorageProviderSettingsDto> result = apiInstance.StorageStorageProvidersGetPossibleSettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetPossibleSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersGetPossibleSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<StoragePossibleStorageProviderSettingsDto>> response = apiInstance.StorageStorageProvidersGetPossibleSettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetPossibleSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;StoragePossibleStorageProviderSettingsDto&gt;**](StoragePossibleStorageProviderSettingsDto.md)

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

<a id="storagestorageprovidersgetstorageprovider"></a>
# **StorageStorageProvidersGetStorageProvider**
> SharedModuleStorageProviderDto StorageStorageProvidersGetStorageProvider (string storageProviderId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersGetStorageProviderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);
            var storageProviderId = "storageProviderId_example";  // string |  (optional) 

            try
            {
                SharedModuleStorageProviderDto result = apiInstance.StorageStorageProvidersGetStorageProvider(storageProviderId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProvider: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersGetStorageProviderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SharedModuleStorageProviderDto> response = apiInstance.StorageStorageProvidersGetStorageProviderWithHttpInfo(storageProviderId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProviderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **storageProviderId** | **string** |  | [optional]  |

### Return type

[**SharedModuleStorageProviderDto**](SharedModuleStorageProviderDto.md)

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

<a id="storagestorageprovidersgetstorageprovidertypeslist"></a>
# **StorageStorageProvidersGetStorageProviderTypesList**
> List&lt;EleonStorageProviderTypeDto&gt; StorageStorageProvidersGetStorageProviderTypesList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersGetStorageProviderTypesListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);

            try
            {
                List<EleonStorageProviderTypeDto> result = apiInstance.StorageStorageProvidersGetStorageProviderTypesList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProviderTypesList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersGetStorageProviderTypesListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<EleonStorageProviderTypeDto>> response = apiInstance.StorageStorageProvidersGetStorageProviderTypesListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProviderTypesListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;EleonStorageProviderTypeDto&gt;**](EleonStorageProviderTypeDto.md)

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

<a id="storagestorageprovidersgetstorageproviderslist"></a>
# **StorageStorageProvidersGetStorageProvidersList**
> List&lt;SharedModuleStorageProviderDto&gt; StorageStorageProvidersGetStorageProvidersList (string searchQuery = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersGetStorageProvidersListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);
            var searchQuery = "searchQuery_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                List<SharedModuleStorageProviderDto> result = apiInstance.StorageStorageProvidersGetStorageProvidersList(searchQuery, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProvidersList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersGetStorageProvidersListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SharedModuleStorageProviderDto>> response = apiInstance.StorageStorageProvidersGetStorageProvidersListWithHttpInfo(searchQuery, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersGetStorageProvidersListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **searchQuery** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**List&lt;SharedModuleStorageProviderDto&gt;**](SharedModuleStorageProviderDto.md)

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

<a id="storagestorageprovidersremovestorageprovider"></a>
# **StorageStorageProvidersRemoveStorageProvider**
> bool StorageStorageProvidersRemoveStorageProvider (string storageProviderId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersRemoveStorageProviderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);
            var storageProviderId = "storageProviderId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.StorageStorageProvidersRemoveStorageProvider(storageProviderId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersRemoveStorageProvider: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersRemoveStorageProviderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.StorageStorageProvidersRemoveStorageProviderWithHttpInfo(storageProviderId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersRemoveStorageProviderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **storageProviderId** | **string** |  | [optional]  |

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

<a id="storagestorageproviderssavestorageprovider"></a>
# **StorageStorageProvidersSaveStorageProvider**
> EleonsoftModuleCollectorStorageProviderSaveResponseDto StorageStorageProvidersSaveStorageProvider (SharedModuleStorageProviderDto sharedModuleStorageProviderDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersSaveStorageProviderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersApi(config);
            var sharedModuleStorageProviderDto = new SharedModuleStorageProviderDto(); // SharedModuleStorageProviderDto |  (optional) 

            try
            {
                EleonsoftModuleCollectorStorageProviderSaveResponseDto result = apiInstance.StorageStorageProvidersSaveStorageProvider(sharedModuleStorageProviderDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersSaveStorageProvider: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersSaveStorageProviderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorStorageProviderSaveResponseDto> response = apiInstance.StorageStorageProvidersSaveStorageProviderWithHttpInfo(sharedModuleStorageProviderDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersApi.StorageStorageProvidersSaveStorageProviderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sharedModuleStorageProviderDto** | [**SharedModuleStorageProviderDto**](SharedModuleStorageProviderDto.md) |  | [optional]  |

### Return type

[**EleonsoftModuleCollectorStorageProviderSaveResponseDto**](EleonsoftModuleCollectorStorageProviderSaveResponseDto.md)

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

