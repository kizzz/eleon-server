# EleonsoftProxy.Api.ClientIsolationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementClientIsolationGetUserIsolationSettings**](ClientIsolationApi.md#tenantmanagementclientisolationgetuserisolationsettings) | **GET** /api/TenantManagement/ClientIsolation/GetUserIsolationSettings |  |
| [**TenantManagementClientIsolationSetTenantIpIsolationSettings**](ClientIsolationApi.md#tenantmanagementclientisolationsettenantipisolationsettings) | **POST** /api/TenantManagement/ClientIsolation/SetTenantIpIsolationSettings |  |
| [**TenantManagementClientIsolationSetTenantIsolation**](ClientIsolationApi.md#tenantmanagementclientisolationsettenantisolation) | **POST** /api/TenantManagement/ClientIsolation/SetTenantIsolation |  |
| [**TenantManagementClientIsolationSetUserIsolation**](ClientIsolationApi.md#tenantmanagementclientisolationsetuserisolation) | **POST** /api/TenantManagement/ClientIsolation/SetUserIsolation |  |
| [**TenantManagementClientIsolationValidateClientIsolation**](ClientIsolationApi.md#tenantmanagementclientisolationvalidateclientisolation) | **POST** /api/TenantManagement/ClientIsolation/ValidateClientIsolation |  |

<a id="tenantmanagementclientisolationgetuserisolationsettings"></a>
# **TenantManagementClientIsolationGetUserIsolationSettings**
> TenantManagementUserIsolationSettingsDto TenantManagementClientIsolationGetUserIsolationSettings (Guid userId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementClientIsolationGetUserIsolationSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientIsolationApi(config);
            var userId = "userId_example";  // Guid |  (optional) 

            try
            {
                TenantManagementUserIsolationSettingsDto result = apiInstance.TenantManagementClientIsolationGetUserIsolationSettings(userId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationGetUserIsolationSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementClientIsolationGetUserIsolationSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementUserIsolationSettingsDto> response = apiInstance.TenantManagementClientIsolationGetUserIsolationSettingsWithHttpInfo(userId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationGetUserIsolationSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |

### Return type

[**TenantManagementUserIsolationSettingsDto**](TenantManagementUserIsolationSettingsDto.md)

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

<a id="tenantmanagementclientisolationsettenantipisolationsettings"></a>
# **TenantManagementClientIsolationSetTenantIpIsolationSettings**
> bool TenantManagementClientIsolationSetTenantIpIsolationSettings (TenantManagementSetIpIsolationRequestDto tenantManagementSetIpIsolationRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementClientIsolationSetTenantIpIsolationSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientIsolationApi(config);
            var tenantManagementSetIpIsolationRequestDto = new TenantManagementSetIpIsolationRequestDto(); // TenantManagementSetIpIsolationRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementClientIsolationSetTenantIpIsolationSettings(tenantManagementSetIpIsolationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetTenantIpIsolationSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementClientIsolationSetTenantIpIsolationSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementClientIsolationSetTenantIpIsolationSettingsWithHttpInfo(tenantManagementSetIpIsolationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetTenantIpIsolationSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetIpIsolationRequestDto** | [**TenantManagementSetIpIsolationRequestDto**](TenantManagementSetIpIsolationRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementclientisolationsettenantisolation"></a>
# **TenantManagementClientIsolationSetTenantIsolation**
> bool TenantManagementClientIsolationSetTenantIsolation (TenantManagementSetTenantIsolationRequestDto tenantManagementSetTenantIsolationRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementClientIsolationSetTenantIsolationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientIsolationApi(config);
            var tenantManagementSetTenantIsolationRequestDto = new TenantManagementSetTenantIsolationRequestDto(); // TenantManagementSetTenantIsolationRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementClientIsolationSetTenantIsolation(tenantManagementSetTenantIsolationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetTenantIsolation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementClientIsolationSetTenantIsolationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementClientIsolationSetTenantIsolationWithHttpInfo(tenantManagementSetTenantIsolationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetTenantIsolationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetTenantIsolationRequestDto** | [**TenantManagementSetTenantIsolationRequestDto**](TenantManagementSetTenantIsolationRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementclientisolationsetuserisolation"></a>
# **TenantManagementClientIsolationSetUserIsolation**
> bool TenantManagementClientIsolationSetUserIsolation (TenantManagementSetUserIsolationRequestDto tenantManagementSetUserIsolationRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementClientIsolationSetUserIsolationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientIsolationApi(config);
            var tenantManagementSetUserIsolationRequestDto = new TenantManagementSetUserIsolationRequestDto(); // TenantManagementSetUserIsolationRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementClientIsolationSetUserIsolation(tenantManagementSetUserIsolationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetUserIsolation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementClientIsolationSetUserIsolationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementClientIsolationSetUserIsolationWithHttpInfo(tenantManagementSetUserIsolationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationSetUserIsolationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetUserIsolationRequestDto** | [**TenantManagementSetUserIsolationRequestDto**](TenantManagementSetUserIsolationRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementclientisolationvalidateclientisolation"></a>
# **TenantManagementClientIsolationValidateClientIsolation**
> bool TenantManagementClientIsolationValidateClientIsolation (Object body = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementClientIsolationValidateClientIsolationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientIsolationApi(config);
            var body = null;  // Object |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementClientIsolationValidateClientIsolation(body);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationValidateClientIsolation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementClientIsolationValidateClientIsolationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementClientIsolationValidateClientIsolationWithHttpInfo(body);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientIsolationApi.TenantManagementClientIsolationValidateClientIsolationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **body** | **Object** |  | [optional]  |

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

