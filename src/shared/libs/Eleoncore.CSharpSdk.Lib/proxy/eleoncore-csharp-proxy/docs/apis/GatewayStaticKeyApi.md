# EleoncoreProxy.Api.GatewayStaticKeyApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementGatewayStaticKeyGetStaticKey**](GatewayStaticKeyApi.md#gatewaymanagementgatewaystatickeygetstatickey) | **GET** /api/GatewayManagement/GatewayStaticKey/GetStaticKey |  |
| [**GatewayManagementGatewayStaticKeySetStaticKeyEnabled**](GatewayStaticKeyApi.md#gatewaymanagementgatewaystatickeysetstatickeyenabled) | **POST** /api/GatewayManagement/GatewayStaticKey/SetStaticKeyEnabled |  |

<a id="gatewaymanagementgatewaystatickeygetstatickey"></a>
# **GatewayManagementGatewayStaticKeyGetStaticKey**
> string GatewayManagementGatewayStaticKeyGetStaticKey ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayStaticKeyGetStaticKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayStaticKeyApi(config);

            try
            {
                string result = apiInstance.GatewayManagementGatewayStaticKeyGetStaticKey();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayStaticKeyApi.GatewayManagementGatewayStaticKeyGetStaticKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayStaticKeyGetStaticKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.GatewayManagementGatewayStaticKeyGetStaticKeyWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayStaticKeyApi.GatewayManagementGatewayStaticKeyGetStaticKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**string**

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

<a id="gatewaymanagementgatewaystatickeysetstatickeyenabled"></a>
# **GatewayManagementGatewayStaticKeySetStaticKeyEnabled**
> void GatewayManagementGatewayStaticKeySetStaticKeyEnabled (bool shouldBeEnabled = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayStaticKeySetStaticKeyEnabledExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayStaticKeyApi(config);
            var shouldBeEnabled = true;  // bool |  (optional) 

            try
            {
                apiInstance.GatewayManagementGatewayStaticKeySetStaticKeyEnabled(shouldBeEnabled);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayStaticKeyApi.GatewayManagementGatewayStaticKeySetStaticKeyEnabled: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayStaticKeySetStaticKeyEnabledWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementGatewayStaticKeySetStaticKeyEnabledWithHttpInfo(shouldBeEnabled);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayStaticKeyApi.GatewayManagementGatewayStaticKeySetStaticKeyEnabledWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **shouldBeEnabled** | **bool** |  | [optional]  |

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
| **200** | OK |  -  |
| **403** | Forbidden |  -  |
| **401** | Unauthorized |  -  |
| **400** | Bad Request |  -  |
| **404** | Not Found |  -  |
| **501** | Not Implemented |  -  |
| **500** | Internal Server Error |  -  |

[[Back to top]](#) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to Model list]](../../README.md#documentation-for-models) [[Back to README]](../../README.md)

