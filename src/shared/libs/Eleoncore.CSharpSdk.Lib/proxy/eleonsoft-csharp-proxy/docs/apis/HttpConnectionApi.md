# EleonsoftProxy.Api.HttpConnectionApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementHttpConnectionCheckHttpConnection**](HttpConnectionApi.md#gatewaymanagementhttpconnectioncheckhttpconnection) | **GET** /api/connection/check |  |

<a id="gatewaymanagementhttpconnectioncheckhttpconnection"></a>
# **GatewayManagementHttpConnectionCheckHttpConnection**
> void GatewayManagementHttpConnectionCheckHttpConnection ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class GatewayManagementHttpConnectionCheckHttpConnectionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new HttpConnectionApi(config);

            try
            {
                apiInstance.GatewayManagementHttpConnectionCheckHttpConnection();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling HttpConnectionApi.GatewayManagementHttpConnectionCheckHttpConnection: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementHttpConnectionCheckHttpConnectionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementHttpConnectionCheckHttpConnectionWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling HttpConnectionApi.GatewayManagementHttpConnectionCheckHttpConnectionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
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
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

