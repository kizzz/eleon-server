# EleoncoreProxy.Api.EleoncoreApplicationConfigurationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorEleoncoreApplicationConfigurationGet**](EleoncoreApplicationConfigurationApi.md#modulecollectoreleoncoreapplicationconfigurationget) | **GET** /api/sites-management/eleoncore-application-configuration/Get |  |
| [**ModuleCollectorEleoncoreApplicationConfigurationGetByAppId**](EleoncoreApplicationConfigurationApi.md#modulecollectoreleoncoreapplicationconfigurationgetbyappid) | **GET** /api/sites-management/eleoncore-application-configuration/GetByAppId |  |

<a id="modulecollectoreleoncoreapplicationconfigurationget"></a>
# **ModuleCollectorEleoncoreApplicationConfigurationGet**
> ModuleCollectorEleoncoreApplicationConfigurationDto ModuleCollectorEleoncoreApplicationConfigurationGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class ModuleCollectorEleoncoreApplicationConfigurationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EleoncoreApplicationConfigurationApi(config);

            try
            {
                ModuleCollectorEleoncoreApplicationConfigurationDto result = apiInstance.ModuleCollectorEleoncoreApplicationConfigurationGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EleoncoreApplicationConfigurationApi.ModuleCollectorEleoncoreApplicationConfigurationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorEleoncoreApplicationConfigurationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleoncoreApplicationConfigurationDto> response = apiInstance.ModuleCollectorEleoncoreApplicationConfigurationGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EleoncoreApplicationConfigurationApi.ModuleCollectorEleoncoreApplicationConfigurationGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**ModuleCollectorEleoncoreApplicationConfigurationDto**](ModuleCollectorEleoncoreApplicationConfigurationDto.md)

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

<a id="modulecollectoreleoncoreapplicationconfigurationgetbyappid"></a>
# **ModuleCollectorEleoncoreApplicationConfigurationGetByAppId**
> ModuleCollectorEleoncoreApplicationConfigurationDto ModuleCollectorEleoncoreApplicationConfigurationGetByAppId (string appId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class ModuleCollectorEleoncoreApplicationConfigurationGetByAppIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EleoncoreApplicationConfigurationApi(config);
            var appId = "appId_example";  // string |  (optional) 

            try
            {
                ModuleCollectorEleoncoreApplicationConfigurationDto result = apiInstance.ModuleCollectorEleoncoreApplicationConfigurationGetByAppId(appId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EleoncoreApplicationConfigurationApi.ModuleCollectorEleoncoreApplicationConfigurationGetByAppId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorEleoncoreApplicationConfigurationGetByAppIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleoncoreApplicationConfigurationDto> response = apiInstance.ModuleCollectorEleoncoreApplicationConfigurationGetByAppIdWithHttpInfo(appId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EleoncoreApplicationConfigurationApi.ModuleCollectorEleoncoreApplicationConfigurationGetByAppIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **appId** | **string** |  | [optional]  |

### Return type

[**ModuleCollectorEleoncoreApplicationConfigurationDto**](ModuleCollectorEleoncoreApplicationConfigurationDto.md)

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

