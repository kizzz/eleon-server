# EleonsoftProxy.Api.EleonsoftApplicationConfigurationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorEleonsoftApplicationConfigurationGetBase**](EleonsoftApplicationConfigurationApi.md#modulecollectoreleonsoftapplicationconfigurationgetbase) | **GET** /api/eleoncore-application-configuration/GetBase |  |

<a id="modulecollectoreleonsoftapplicationconfigurationgetbase"></a>
# **ModuleCollectorEleonsoftApplicationConfigurationGetBase**
> ModuleCollectorEleonsoftApplicationConfigurationDto ModuleCollectorEleonsoftApplicationConfigurationGetBase ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorEleonsoftApplicationConfigurationGetBaseExample
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
                ModuleCollectorEleonsoftApplicationConfigurationDto result = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGetBase();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGetBase: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorEleonsoftApplicationConfigurationGetBaseWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorEleonsoftApplicationConfigurationDto> response = apiInstance.ModuleCollectorEleonsoftApplicationConfigurationGetBaseWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling EleonsoftApplicationConfigurationApi.ModuleCollectorEleonsoftApplicationConfigurationGetBaseWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**ModuleCollectorEleonsoftApplicationConfigurationDto**](ModuleCollectorEleonsoftApplicationConfigurationDto.md)

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

