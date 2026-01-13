# EleonsoftProxy.Api.BaseApplicationConfigurationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorBaseApplicationConfigurationGetBase**](BaseApplicationConfigurationApi.md#modulecollectorbaseapplicationconfigurationgetbase) | **GET** /api/tenant-management/application-configuration/GetBase |  |

<a id="modulecollectorbaseapplicationconfigurationgetbase"></a>
# **ModuleCollectorBaseApplicationConfigurationGetBase**
> ModuleCollectorEleoncoreApplicationConfigurationDto ModuleCollectorBaseApplicationConfigurationGetBase ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorBaseApplicationConfigurationGetBaseExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new BaseApplicationConfigurationApi(config);

            try
            {
                ModuleCollectorEleoncoreApplicationConfigurationDto result = apiInstance.ModuleCollectorBaseApplicationConfigurationGetBase();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling BaseApplicationConfigurationApi.ModuleCollectorBaseApplicationConfigurationGetBase: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorBaseApplicationConfigurationGetBaseWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleoncoreApplicationConfigurationDto> response = apiInstance.ModuleCollectorBaseApplicationConfigurationGetBaseWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling BaseApplicationConfigurationApi.ModuleCollectorBaseApplicationConfigurationGetBaseWithHttpInfo: " + e.Message);
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

