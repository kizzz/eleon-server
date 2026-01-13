# EleonsoftProxy.Api.GoogleRoutesApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**ModuleCollectorGoogleRoutesOptimizeRoute**](GoogleRoutesApi.md#modulecollectorgoogleroutesoptimizeroute) | **POST** /api/Google/Geocoding/OptimizeRoute |  |

<a id="modulecollectorgoogleroutesoptimizeroute"></a>
# **ModuleCollectorGoogleRoutesOptimizeRoute**
> ModuleCollectorOptimizedToursDto ModuleCollectorGoogleRoutesOptimizeRoute (ModuleCollectorOptimizeToursRequestDto moduleCollectorOptimizeToursRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class ModuleCollectorGoogleRoutesOptimizeRouteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GoogleRoutesApi(config);
            var moduleCollectorOptimizeToursRequestDto = new ModuleCollectorOptimizeToursRequestDto(); // ModuleCollectorOptimizeToursRequestDto |  (optional) 

            try
            {
                ModuleCollectorOptimizedToursDto result = apiInstance.ModuleCollectorGoogleRoutesOptimizeRoute(moduleCollectorOptimizeToursRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GoogleRoutesApi.ModuleCollectorGoogleRoutesOptimizeRoute: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the ModuleCollectorGoogleRoutesOptimizeRouteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<ModuleCollectorOptimizedToursDto> response = apiInstance.ModuleCollectorGoogleRoutesOptimizeRouteWithHttpInfo(moduleCollectorOptimizeToursRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GoogleRoutesApi.ModuleCollectorGoogleRoutesOptimizeRouteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorOptimizeToursRequestDto** | [**ModuleCollectorOptimizeToursRequestDto**](ModuleCollectorOptimizeToursRequestDto.md) |  | [optional]  |

### Return type

[**ModuleCollectorOptimizedToursDto**](ModuleCollectorOptimizedToursDto.md)

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

