# EleoncoreProxy.Api.GatewayManagementApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**GatewayManagementGatewayManagementAcceptPendingGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementacceptpendinggateway) | **POST** /api/GatewayManagement/GatewayManagement/AcceptPendingGateway |  |
| [**GatewayManagementGatewayManagementAddGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementaddgateway) | **POST** /api/GatewayManagement/GatewayManagement/AddGateway |  |
| [**GatewayManagementGatewayManagementCancelOngoingGatewayRegistration**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementcancelongoinggatewayregistration) | **POST** /api/GatewayManagement/GatewayManagement/CancelOngoingGatewayRegistration |  |
| [**GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKey**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementgetcurrentgatewayregistrationkey) | **GET** /api/GatewayManagement/GatewayManagement/GetCurrentGatewayRegistrationKey |  |
| [**GatewayManagementGatewayManagementGetGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementgetgateway) | **GET** /api/GatewayManagement/GatewayManagement/GetGateway |  |
| [**GatewayManagementGatewayManagementGetGatewayList**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementgetgatewaylist) | **GET** /api/GatewayManagement/GatewayManagement/GetGatewayList |  |
| [**GatewayManagementGatewayManagementRejectPendingGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementrejectpendinggateway) | **POST** /api/GatewayManagement/GatewayManagement/RejectPendingGateway |  |
| [**GatewayManagementGatewayManagementRemoveGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementremovegateway) | **POST** /api/GatewayManagement/GatewayManagement/RemoveGateway |  |
| [**GatewayManagementGatewayManagementRequestGatewayRegistration**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementrequestgatewayregistration) | **POST** /api/GatewayManagement/GatewayManagement/RequestGatewayRegistration |  |
| [**GatewayManagementGatewayManagementUpdateGateway**](GatewayManagementApi.md#gatewaymanagementgatewaymanagementupdategateway) | **POST** /api/GatewayManagement/GatewayManagement/UpdateGateway |  |

<a id="gatewaymanagementgatewaymanagementacceptpendinggateway"></a>
# **GatewayManagementGatewayManagementAcceptPendingGateway**
> void GatewayManagementGatewayManagementAcceptPendingGateway (GatewayManagementAcceptPendingGatewayRequestDto gatewayManagementAcceptPendingGatewayRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementAcceptPendingGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayManagementAcceptPendingGatewayRequestDto = new GatewayManagementAcceptPendingGatewayRequestDto(); // GatewayManagementAcceptPendingGatewayRequestDto |  (optional) 

            try
            {
                apiInstance.GatewayManagementGatewayManagementAcceptPendingGateway(gatewayManagementAcceptPendingGatewayRequestDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementAcceptPendingGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementAcceptPendingGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementGatewayManagementAcceptPendingGatewayWithHttpInfo(gatewayManagementAcceptPendingGatewayRequestDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementAcceptPendingGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementAcceptPendingGatewayRequestDto** | [**GatewayManagementAcceptPendingGatewayRequestDto**](GatewayManagementAcceptPendingGatewayRequestDto.md) |  | [optional]  |

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

<a id="gatewaymanagementgatewaymanagementaddgateway"></a>
# **GatewayManagementGatewayManagementAddGateway**
> string GatewayManagementGatewayManagementAddGateway (GatewayManagementGatewayDto gatewayManagementGatewayDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementAddGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayManagementGatewayDto = new GatewayManagementGatewayDto(); // GatewayManagementGatewayDto |  (optional) 

            try
            {
                string result = apiInstance.GatewayManagementGatewayManagementAddGateway(gatewayManagementGatewayDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementAddGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementAddGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.GatewayManagementGatewayManagementAddGatewayWithHttpInfo(gatewayManagementGatewayDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementAddGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementGatewayDto** | [**GatewayManagementGatewayDto**](GatewayManagementGatewayDto.md) |  | [optional]  |

### Return type

**string**

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

<a id="gatewaymanagementgatewaymanagementcancelongoinggatewayregistration"></a>
# **GatewayManagementGatewayManagementCancelOngoingGatewayRegistration**
> bool GatewayManagementGatewayManagementCancelOngoingGatewayRegistration (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementCancelOngoingGatewayRegistrationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.GatewayManagementGatewayManagementCancelOngoingGatewayRegistration(gatewayId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementCancelOngoingGatewayRegistration: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementCancelOngoingGatewayRegistrationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.GatewayManagementGatewayManagementCancelOngoingGatewayRegistrationWithHttpInfo(gatewayId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementCancelOngoingGatewayRegistrationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

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

<a id="gatewaymanagementgatewaymanagementgetcurrentgatewayregistrationkey"></a>
# **GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKey**
> GatewayManagementGatewayRegistrationKeyDto GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKey (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKeyExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                GatewayManagementGatewayRegistrationKeyDto result = apiInstance.GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKey(gatewayId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKey: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKeyWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayRegistrationKeyDto> response = apiInstance.GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKeyWithHttpInfo(gatewayId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetCurrentGatewayRegistrationKeyWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

### Return type

[**GatewayManagementGatewayRegistrationKeyDto**](GatewayManagementGatewayRegistrationKeyDto.md)

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

<a id="gatewaymanagementgatewaymanagementgetgateway"></a>
# **GatewayManagementGatewayManagementGetGateway**
> GatewayManagementGatewayDto GatewayManagementGatewayManagementGetGateway (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementGetGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                GatewayManagementGatewayDto result = apiInstance.GatewayManagementGatewayManagementGetGateway(gatewayId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementGetGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayDto> response = apiInstance.GatewayManagementGatewayManagementGetGatewayWithHttpInfo(gatewayId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

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

<a id="gatewaymanagementgatewaymanagementgetgatewaylist"></a>
# **GatewayManagementGatewayManagementGetGatewayList**
> List&lt;GatewayManagementGatewayDto&gt; GatewayManagementGatewayManagementGetGatewayList (EleoncoreGatewayStatus statusFilter = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementGetGatewayListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var statusFilter = (EleoncoreGatewayStatus) "1";  // EleoncoreGatewayStatus |  (optional) 

            try
            {
                List<GatewayManagementGatewayDto> result = apiInstance.GatewayManagementGatewayManagementGetGatewayList(statusFilter);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetGatewayList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementGetGatewayListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<GatewayManagementGatewayDto>> response = apiInstance.GatewayManagementGatewayManagementGetGatewayListWithHttpInfo(statusFilter);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementGetGatewayListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **statusFilter** | **EleoncoreGatewayStatus** |  | [optional]  |

### Return type

[**List&lt;GatewayManagementGatewayDto&gt;**](GatewayManagementGatewayDto.md)

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

<a id="gatewaymanagementgatewaymanagementrejectpendinggateway"></a>
# **GatewayManagementGatewayManagementRejectPendingGateway**
> void GatewayManagementGatewayManagementRejectPendingGateway (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementRejectPendingGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.GatewayManagementGatewayManagementRejectPendingGateway(gatewayId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRejectPendingGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementRejectPendingGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.GatewayManagementGatewayManagementRejectPendingGatewayWithHttpInfo(gatewayId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRejectPendingGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

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

<a id="gatewaymanagementgatewaymanagementremovegateway"></a>
# **GatewayManagementGatewayManagementRemoveGateway**
> bool GatewayManagementGatewayManagementRemoveGateway (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementRemoveGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.GatewayManagementGatewayManagementRemoveGateway(gatewayId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRemoveGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementRemoveGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.GatewayManagementGatewayManagementRemoveGatewayWithHttpInfo(gatewayId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRemoveGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

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

<a id="gatewaymanagementgatewaymanagementrequestgatewayregistration"></a>
# **GatewayManagementGatewayManagementRequestGatewayRegistration**
> GatewayManagementGatewayRegistrationKeyDto GatewayManagementGatewayManagementRequestGatewayRegistration (Guid gatewayId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementRequestGatewayRegistrationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayId = "gatewayId_example";  // Guid |  (optional) 

            try
            {
                GatewayManagementGatewayRegistrationKeyDto result = apiInstance.GatewayManagementGatewayManagementRequestGatewayRegistration(gatewayId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRequestGatewayRegistration: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementRequestGatewayRegistrationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<GatewayManagementGatewayRegistrationKeyDto> response = apiInstance.GatewayManagementGatewayManagementRequestGatewayRegistrationWithHttpInfo(gatewayId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementRequestGatewayRegistrationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayId** | **Guid** |  | [optional]  |

### Return type

[**GatewayManagementGatewayRegistrationKeyDto**](GatewayManagementGatewayRegistrationKeyDto.md)

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

<a id="gatewaymanagementgatewaymanagementupdategateway"></a>
# **GatewayManagementGatewayManagementUpdateGateway**
> bool GatewayManagementGatewayManagementUpdateGateway (GatewayManagementUpdateGatewayRequestDto gatewayManagementUpdateGatewayRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class GatewayManagementGatewayManagementUpdateGatewayExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new GatewayManagementApi(config);
            var gatewayManagementUpdateGatewayRequestDto = new GatewayManagementUpdateGatewayRequestDto(); // GatewayManagementUpdateGatewayRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.GatewayManagementGatewayManagementUpdateGateway(gatewayManagementUpdateGatewayRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementUpdateGateway: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the GatewayManagementGatewayManagementUpdateGatewayWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.GatewayManagementGatewayManagementUpdateGatewayWithHttpInfo(gatewayManagementUpdateGatewayRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling GatewayManagementApi.GatewayManagementGatewayManagementUpdateGatewayWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **gatewayManagementUpdateGatewayRequestDto** | [**GatewayManagementUpdateGatewayRequestDto**](GatewayManagementUpdateGatewayRequestDto.md) |  | [optional]  |

### Return type

**bool**

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

