# EleonsoftProxy.Api.GoogleGeocodingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GoogleGoogleGeocodingGeocodeAddresses**](GoogleGeocodingApi.md#googlegooglegeocodinggeocodeaddresses) | **POST** /api/Google/Geocoding/GeocodeAddresses |  |

<a id="googlegooglegeocodinggeocodeaddresses"></a>
# **GoogleGoogleGeocodingGeocodeAddresses**
> List&lt;GoogleLatLng&gt; GoogleGoogleGeocodingGeocodeAddresses (List<string> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class GoogleGoogleGeocodingGeocodeAddressesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GoogleGeocodingApi(config);
            var requestBody = new List<string>(); // List<string> |  (optional) 

            try
            {
                List<GoogleLatLng> result = apiInstance.GoogleGoogleGeocodingGeocodeAddresses(requestBody);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GoogleGeocodingApi.GoogleGoogleGeocodingGeocodeAddresses: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GoogleGoogleGeocodingGeocodeAddressesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<GoogleLatLng>> response = apiInstance.GoogleGoogleGeocodingGeocodeAddressesWithHttpInfo(requestBody);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GoogleGeocodingApi.GoogleGoogleGeocodingGeocodeAddressesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **requestBody** | [**List&lt;string&gt;**](string.md) |  | [optional]  |

### Return type

[**List&lt;GoogleLatLng&gt;**](GoogleLatLng.md)

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

