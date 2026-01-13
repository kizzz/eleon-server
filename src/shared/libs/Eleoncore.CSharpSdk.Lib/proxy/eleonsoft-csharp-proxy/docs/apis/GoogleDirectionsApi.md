# EleonsoftProxy.Api.GoogleDirectionsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GoogleGoogleDirectionsGetDirections**](GoogleDirectionsApi.md#googlegoogledirectionsgetdirections) | **POST** /api/Google/Direction/GetDirections |  |

<a id="googlegoogledirectionsgetdirections"></a>
# **GoogleGoogleDirectionsGetDirections**
> GoogleDirectionsPath GoogleGoogleDirectionsGetDirections (List<GoogleLatLng> googleLatLng = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class GoogleGoogleDirectionsGetDirectionsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GoogleDirectionsApi(config);
            var googleLatLng = new List<GoogleLatLng>(); // List<GoogleLatLng> |  (optional) 

            try
            {
                GoogleDirectionsPath result = apiInstance.GoogleGoogleDirectionsGetDirections(googleLatLng);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GoogleDirectionsApi.GoogleGoogleDirectionsGetDirections: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GoogleGoogleDirectionsGetDirectionsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GoogleDirectionsPath> response = apiInstance.GoogleGoogleDirectionsGetDirectionsWithHttpInfo(googleLatLng);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GoogleDirectionsApi.GoogleGoogleDirectionsGetDirectionsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **googleLatLng** | [**List&lt;GoogleLatLng&gt;**](GoogleLatLng.md) |  | [optional]  |

### Return type

[**GoogleDirectionsPath**](GoogleDirectionsPath.md)

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

