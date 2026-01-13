# EleoncoreProxy.Api.EleonsoftApplicationConfigurationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorEleonsoftApplicationConfigurationGet**](EleonsoftApplicationConfigurationApi.md#modulecollectoreleonsoftapplicationconfigurationget) | **GET** /api/sites-management/eleoncore-application-configuration/Get |  |
| [**ModuleCollectorEleonsoftApplicationConfigurationGetByAppId**](EleonsoftApplicationConfigurationApi.md#modulecollectoreleonsoftapplicationconfigurationgetbyappid) | **GET** /api/sites-management/eleoncore-application-configuration/GetByAppId |  |

<a id="modulecollectoreleonsoftapplicationconfigurationget"></a>
# **ModuleCollectorEleonsoftApplicationConfigurationGet**
> ModuleCollectorEleoncoreApplicationConfigurationDto ModuleCollectorEleonsoftApplicationConfigurationGet ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class ModuleCollectorEleonsoftApplicationConfigurationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EleonsoftApplicationConfigurationApi(config);

            try
            {
                ModuleCollectorEleoncoreApplicationConfigurationDto result = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGet();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorEleonsoftApplicationConfigurationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleoncoreApplicationConfigurationDto> response = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGetWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGetWithHttpInfo: " + e.Message);
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

<a id="modulecollectoreleonsoftapplicationconfigurationgetbyappid"></a>
# **ModuleCollectorEleonsoftApplicationConfigurationGetByAppId**
> ModuleCollectorEleoncoreApplicationConfigurationDto ModuleCollectorEleonsoftApplicationConfigurationGetByAppId (string appId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class ModuleCollectorEleonsoftApplicationConfigurationGetByAppIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new EleonsoftApplicationConfigurationApi(config);
            var appId = "appId_example";  // string |  (optional) 

            try
            {
                ModuleCollectorEleoncoreApplicationConfigurationDto result = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGetByAppId(appId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGetByAppId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorEleonsoftApplicationConfigurationGetByAppIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleoncoreApplicationConfigurationDto> response = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGetByAppIdWithHttpInfo(appId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGetByAppIdWithHttpInfo: " + e.Message);
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

