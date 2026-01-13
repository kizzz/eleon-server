# EleoncoreProxy.Api.GatewayClientApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementGatewayClientConfirmGatewayRegistration**](GatewayClientApi.md#gatewaymanagementgatewayclientconfirmgatewayregistration) | **POST** /api/GatewayManagement/GatewayClient/ConfirmGatewayRegistration |  |
| [**GatewayManagementGatewayClientGetCurrentGateway**](GatewayClientApi.md#gatewaymanagementgatewayclientgetcurrentgateway) | **GET** /api/GatewayManagement/GatewayClient/GetCurrentGateway |  |
| [**GatewayManagementGatewayClientGetCurrentGatewayWorkspace**](GatewayClientApi.md#gatewaymanagementgatewayclientgetcurrentgatewayworkspace) | **GET** /api/GatewayManagement/GatewayClient/GetCurrentGatewayWorkspace |  |
| [**GatewayManagementGatewayClientRegisterGateway**](GatewayClientApi.md#gatewaymanagementgatewayclientregistergateway) | **POST** /api/GatewayManagement/GatewayClient/RegisterGateway |  |
| [**GatewayManagementGatewayClientSetGatewayHealthStatus**](GatewayClientApi.md#gatewaymanagementgatewayclientsetgatewayhealthstatus) | **POST** /api/GatewayManagement/GatewayClient/SetGatewayHealthStatus |  |

<a id="gatewaymanagementgatewayclientconfirmgatewayregistration"></a>
# **GatewayManagementGatewayClientConfirmGatewayRegistration**
> bool GatewayManagementGatewayClientConfirmGatewayRegistration ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayClientConfirmGatewayRegistrationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayClientApi(config);

            try
            {
                bool result = apiInstance.GatewayManagementGatewayClientConfirmGatewayRegistration();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientConfirmGatewayRegistration: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayClientConfirmGatewayRegistrationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.GatewayManagementGatewayClientConfirmGatewayRegistrationWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientConfirmGatewayRegistrationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**bool**

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

<a id="gatewaymanagementgatewayclientgetcurrentgateway"></a>
# **GatewayManagementGatewayClientGetCurrentGateway**
> GatewayManagementGatewayDto GatewayManagementGatewayClientGetCurrentGateway ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayClientGetCurrentGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayClientApi(config);

            try
            {
                GatewayManagementGatewayDto result = apiInstance.GatewayManagementGatewayClientGetCurrentGateway();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientGetCurrentGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayClientGetCurrentGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayDto> response = apiInstance.GatewayManagementGatewayClientGetCurrentGatewayWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientGetCurrentGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**GatewayManagementGatewayDto**](GatewayManagementGatewayDto.md)

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

<a id="gatewaymanagementgatewayclientgetcurrentgatewayworkspace"></a>
# **GatewayManagementGatewayClientGetCurrentGatewayWorkspace**
> GatewayManagementGatewayWorkspaceDto GatewayManagementGatewayClientGetCurrentGatewayWorkspace (string workspaceName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayClientGetCurrentGatewayWorkspaceExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayClientApi(config);
            var workspaceName = "workspaceName_example";  // string |  (optional) 

            try
            {
                GatewayManagementGatewayWorkspaceDto result = apiInstance.GatewayManagementGatewayClientGetCurrentGatewayWorkspace(workspaceName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientGetCurrentGatewayWorkspace: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayClientGetCurrentGatewayWorkspaceWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayWorkspaceDto> response = apiInstance.GatewayManagementGatewayClientGetCurrentGatewayWorkspaceWithHttpInfo(workspaceName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientGetCurrentGatewayWorkspaceWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **workspaceName** | **string** |  | [optional]  |

### Return type

[**GatewayManagementGatewayWorkspaceDto**](GatewayManagementGatewayWorkspaceDto.md)

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

<a id="gatewaymanagementgatewayclientregistergateway"></a>
# **GatewayManagementGatewayClientRegisterGateway**
> GatewayManagementGatewayRegistrationResultDto GatewayManagementGatewayClientRegisterGateway (GatewayManagementRegisterGatewayRequestDto gatewayManagementRegisterGatewayRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayClientRegisterGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayClientApi(config);
            var gatewayManagementRegisterGatewayRequestDto = new GatewayManagementRegisterGatewayRequestDto(); // GatewayManagementRegisterGatewayRequestDto |  (optional) 

            try
            {
                GatewayManagementGatewayRegistrationResultDto result = apiInstance.GatewayManagementGatewayClientRegisterGateway(gatewayManagementRegisterGatewayRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientRegisterGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayClientRegisterGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayRegistrationResultDto> response = apiInstance.GatewayManagementGatewayClientRegisterGatewayWithHttpInfo(gatewayManagementRegisterGatewayRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientRegisterGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementRegisterGatewayRequestDto** | [**GatewayManagementRegisterGatewayRequestDto**](GatewayManagementRegisterGatewayRequestDto.md) |  | [optional]  |

### Return type

[**GatewayManagementGatewayRegistrationResultDto**](GatewayManagementGatewayRegistrationResultDto.md)

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

<a id="gatewaymanagementgatewayclientsetgatewayhealthstatus"></a>
# **GatewayManagementGatewayClientSetGatewayHealthStatus**
> void GatewayManagementGatewayClientSetGatewayHealthStatus (GatewayManagementSetGatewayHealthStatusRequestDto gatewayManagementSetGatewayHealthStatusRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayClientSetGatewayHealthStatusExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayClientApi(config);
            var gatewayManagementSetGatewayHealthStatusRequestDto = new GatewayManagementSetGatewayHealthStatusRequestDto(); // GatewayManagementSetGatewayHealthStatusRequestDto |  (optional) 

            try
            {
                apiInstance.GatewayManagementGatewayClientSetGatewayHealthStatus(gatewayManagementSetGatewayHealthStatusRequestDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientSetGatewayHealthStatus: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayClientSetGatewayHealthStatusWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementGatewayClientSetGatewayHealthStatusWithHttpInfo(gatewayManagementSetGatewayHealthStatusRequestDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayClientApi.GatewayManagementGatewayClientSetGatewayHealthStatusWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementSetGatewayHealthStatusRequestDto** | [**GatewayManagementSetGatewayHealthStatusRequestDto**](GatewayManagementSetGatewayHealthStatusRequestDto.md) |  | [optional]  |

### Return type

void (empty response body)

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

