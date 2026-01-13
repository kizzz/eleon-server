# EleoncoreProxy.Api.GatewayHttpForwardingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementGatewayHttpForwardingGetForwardedRequest**](GatewayHttpForwardingApi.md#gatewaymanagementgatewayhttpforwardinggetforwardedrequest) | **GET** /api/GatewayManagement/GatewayHttpForwarding/GetForwardedRequest |  |
| [**GatewayManagementGatewayHttpForwardingSendForwardedResponse**](GatewayHttpForwardingApi.md#gatewaymanagementgatewayhttpforwardingsendforwardedresponse) | **POST** /api/GatewayManagement/GatewayHttpForwarding/SendForwardedResponse |  |

<a id="gatewaymanagementgatewayhttpforwardinggetforwardedrequest"></a>
# **GatewayManagementGatewayHttpForwardingGetForwardedRequest**
> System.IO.Stream GatewayManagementGatewayHttpForwardingGetForwardedRequest (Guid requestId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayHttpForwardingGetForwardedRequestExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayHttpForwardingApi(config);
            var requestId = "requestId_example";  // Guid |  (optional) 

            try
            {
                System.IO.Stream result = apiInstance.GatewayManagementGatewayHttpForwardingGetForwardedRequest(requestId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayHttpForwardingApi.GatewayManagementGatewayHttpForwardingGetForwardedRequest: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayHttpForwardingGetForwardedRequestWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<System.IO.Stream> response = apiInstance.GatewayManagementGatewayHttpForwardingGetForwardedRequestWithHttpInfo(requestId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayHttpForwardingApi.GatewayManagementGatewayHttpForwardingGetForwardedRequestWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **requestId** | **Guid** |  | [optional]  |

### Return type

**System.IO.Stream**

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

<a id="gatewaymanagementgatewayhttpforwardingsendforwardedresponse"></a>
# **GatewayManagementGatewayHttpForwardingSendForwardedResponse**
> bool GatewayManagementGatewayHttpForwardingSendForwardedResponse (Guid responseId = null, System.IO.Stream responseContent = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayHttpForwardingSendForwardedResponseExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayHttpForwardingApi(config);
            var responseId = "responseId_example";  // Guid |  (optional) 
            var responseContent = new System.IO.MemoryStream(System.IO.File.ReadAllBytes("/path/to/file.txt"));  // System.IO.Stream |  (optional) 

            try
            {
                bool result = apiInstance.GatewayManagementGatewayHttpForwardingSendForwardedResponse(responseId, responseContent);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayHttpForwardingApi.GatewayManagementGatewayHttpForwardingSendForwardedResponse: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayHttpForwardingSendForwardedResponseWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.GatewayManagementGatewayHttpForwardingSendForwardedResponseWithHttpInfo(responseId, responseContent);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayHttpForwardingApi.GatewayManagementGatewayHttpForwardingSendForwardedResponseWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **responseId** | **Guid** |  | [optional]  |
| **responseContent** | **System.IO.Stream****System.IO.Stream** |  | [optional]  |

### Return type

**bool**

### Authorization

[oauth2](../README.md#oauth2)

### HTTP request headers

 - **Content-Type**: multipart/form-data
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

