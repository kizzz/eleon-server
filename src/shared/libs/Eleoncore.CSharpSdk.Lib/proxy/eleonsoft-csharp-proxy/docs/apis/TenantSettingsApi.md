# EleonsoftProxy.Api.TenantSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreTenantSettingsGetTenantSettings**](TenantSettingsApi.md#coretenantsettingsgettenantsettings) | **GET** /api/Infrastructure/TenantSettings/GetTenantSettings |  |
| [**CoreTenantSettingsGetTenantSystemHealthSettings**](TenantSettingsApi.md#coretenantsettingsgettenantsystemhealthsettings) | **GET** /api/Infrastructure/TenantSettings/GetTenantSystemHealthSettings |  |
| [**CoreTenantSettingsSetExternalProviderSettings**](TenantSettingsApi.md#coretenantsettingssetexternalprovidersettings) | **POST** /api/Infrastructure/TenantSettings/SetExternalProviderSettings |  |
| [**CoreTenantSettingsUpdateTenantSystemHealthSettings**](TenantSettingsApi.md#coretenantsettingsupdatetenantsystemhealthsettings) | **POST** /api/Infrastructure/TenantSettings/UpdateTenantSystemHealthSettings |  |

<a id="coretenantsettingsgettenantsettings"></a>
# **CoreTenantSettingsGetTenantSettings**
> TenantSettingsTenantSettingDto CoreTenantSettingsGetTenantSettings (Guid tenantId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsGetTenantSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 

            try
            {
                TenantSettingsTenantSettingDto result = apiInstance.CoreTenantSettingsGetTenantSettings(tenantId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsGetTenantSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsGetTenantSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantSettingsTenantSettingDto> response = apiInstance.CoreTenantSettingsGetTenantSettingsWithHttpInfo(tenantId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsGetTenantSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |

### Return type

[**TenantSettingsTenantSettingDto**](TenantSettingsTenantSettingDto.md)

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

<a id="coretenantsettingsgettenantsystemhealthsettings"></a>
# **CoreTenantSettingsGetTenantSystemHealthSettings**
> EleonsoftModuleCollectorTenantSystemHealthSettingsDto CoreTenantSettingsGetTenantSystemHealthSettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsGetTenantSystemHealthSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsApi(config);

            try
            {
                EleonsoftModuleCollectorTenantSystemHealthSettingsDto result = apiInstance.CoreTenantSettingsGetTenantSystemHealthSettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsGetTenantSystemHealthSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsGetTenantSystemHealthSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleonsoftModuleCollectorTenantSystemHealthSettingsDto> response = apiInstance.CoreTenantSettingsGetTenantSystemHealthSettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsGetTenantSystemHealthSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**EleonsoftModuleCollectorTenantSystemHealthSettingsDto**](EleonsoftModuleCollectorTenantSystemHealthSettingsDto.md)

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

<a id="coretenantsettingssetexternalprovidersettings"></a>
# **CoreTenantSettingsSetExternalProviderSettings**
> bool CoreTenantSettingsSetExternalProviderSettings (TenantSettingsSetTenantProviderSettingsRequestDto tenantSettingsSetTenantProviderSettingsRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsSetExternalProviderSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsApi(config);
            var tenantSettingsSetTenantProviderSettingsRequestDto = new TenantSettingsSetTenantProviderSettingsRequestDto(); // TenantSettingsSetTenantProviderSettingsRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.CoreTenantSettingsSetExternalProviderSettings(tenantSettingsSetTenantProviderSettingsRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsSetExternalProviderSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsSetExternalProviderSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreTenantSettingsSetExternalProviderSettingsWithHttpInfo(tenantSettingsSetTenantProviderSettingsRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsSetExternalProviderSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantSettingsSetTenantProviderSettingsRequestDto** | [**TenantSettingsSetTenantProviderSettingsRequestDto**](TenantSettingsSetTenantProviderSettingsRequestDto.md) |  | [optional]  |

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

<a id="coretenantsettingsupdatetenantsystemhealthsettings"></a>
# **CoreTenantSettingsUpdateTenantSystemHealthSettings**
> bool CoreTenantSettingsUpdateTenantSystemHealthSettings (EleonsoftModuleCollectorTenantSystemHealthSettingsDto eleonsoftModuleCollectorTenantSystemHealthSettingsDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsUpdateTenantSystemHealthSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsApi(config);
            var eleonsoftModuleCollectorTenantSystemHealthSettingsDto = new EleonsoftModuleCollectorTenantSystemHealthSettingsDto(); // EleonsoftModuleCollectorTenantSystemHealthSettingsDto |  (optional) 

            try
            {
                bool result = apiInstance.CoreTenantSettingsUpdateTenantSystemHealthSettings(eleonsoftModuleCollectorTenantSystemHealthSettingsDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsUpdateTenantSystemHealthSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsUpdateTenantSystemHealthSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreTenantSettingsUpdateTenantSystemHealthSettingsWithHttpInfo(eleonsoftModuleCollectorTenantSystemHealthSettingsDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsApi.CoreTenantSettingsUpdateTenantSystemHealthSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **eleonsoftModuleCollectorTenantSystemHealthSettingsDto** | [**EleonsoftModuleCollectorTenantSystemHealthSettingsDto**](EleonsoftModuleCollectorTenantSystemHealthSettingsDto.md) |  | [optional]  |

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

