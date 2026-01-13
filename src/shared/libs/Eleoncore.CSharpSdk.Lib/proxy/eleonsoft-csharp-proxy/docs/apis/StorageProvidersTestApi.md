# EleonsoftProxy.Api.StorageProvidersTestApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**StorageStorageProvidersTestTestStorageProvider**](StorageProvidersTestApi.md#storagestorageproviderstestteststorageprovider) | **POST** /api/Storage/StorageProvidersTest/TestStorageProvider |  |

<a id="storagestorageproviderstestteststorageprovider"></a>
# **StorageStorageProvidersTestTestStorageProvider**
> bool StorageStorageProvidersTestTestStorageProvider (SharedModuleStorageProviderDto sharedModuleStorageProviderDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class StorageStorageProvidersTestTestStorageProviderExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new StorageProvidersTestApi(config);
            var sharedModuleStorageProviderDto = new SharedModuleStorageProviderDto(); // SharedModuleStorageProviderDto |  (optional) 

            try
            {
                bool result = apiInstance.StorageStorageProvidersTestTestStorageProvider(sharedModuleStorageProviderDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling StorageProvidersTestApi.StorageStorageProvidersTestTestStorageProvider: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the StorageStorageProvidersTestTestStorageProviderWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.StorageStorageProvidersTestTestStorageProviderWithHttpInfo(sharedModuleStorageProviderDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling StorageProvidersTestApi.StorageStorageProvidersTestTestStorageProviderWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sharedModuleStorageProviderDto** | [**SharedModuleStorageProviderDto**](SharedModuleStorageProviderDto.md) |  | [optional]  |

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

