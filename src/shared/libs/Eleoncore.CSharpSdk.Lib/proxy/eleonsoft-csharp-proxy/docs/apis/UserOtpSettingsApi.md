# EleonsoftProxy.Api.UserOtpSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementUserOtpSettingsGetUserOtpSettings**](UserOtpSettingsApi.md#tenantmanagementuserotpsettingsgetuserotpsettings) | **GET** /api/Infrastructure/UserOtpSettings/GetUserOtpSettings |  |
| [**TenantManagementUserOtpSettingsSetUserOtpSettings**](UserOtpSettingsApi.md#tenantmanagementuserotpsettingssetuserotpsettings) | **POST** /api/Infrastructure/UserOtpSettings/SetUserOtpSettings |  |

<a id="tenantmanagementuserotpsettingsgetuserotpsettings"></a>
# **TenantManagementUserOtpSettingsGetUserOtpSettings**
> TenantManagementUserOtpSettingsDto TenantManagementUserOtpSettingsGetUserOtpSettings (Guid userId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementUserOtpSettingsGetUserOtpSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserOtpSettingsApi(config);
            var userId = "userId_example";  // Guid |  (optional) 

            try
            {
                TenantManagementUserOtpSettingsDto result = apiInstance.TenantManagementUserOtpSettingsGetUserOtpSettings(userId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserOtpSettingsApi.TenantManagementUserOtpSettingsGetUserOtpSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementUserOtpSettingsGetUserOtpSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementUserOtpSettingsDto> response = apiInstance.TenantManagementUserOtpSettingsGetUserOtpSettingsWithHttpInfo(userId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserOtpSettingsApi.TenantManagementUserOtpSettingsGetUserOtpSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |

### Return type

[**TenantManagementUserOtpSettingsDto**](TenantManagementUserOtpSettingsDto.md)

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

<a id="tenantmanagementuserotpsettingssetuserotpsettings"></a>
# **TenantManagementUserOtpSettingsSetUserOtpSettings**
> bool TenantManagementUserOtpSettingsSetUserOtpSettings (TenantManagementUserOtpSettingsDto tenantManagementUserOtpSettingsDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementUserOtpSettingsSetUserOtpSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserOtpSettingsApi(config);
            var tenantManagementUserOtpSettingsDto = new TenantManagementUserOtpSettingsDto(); // TenantManagementUserOtpSettingsDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementUserOtpSettingsSetUserOtpSettings(tenantManagementUserOtpSettingsDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserOtpSettingsApi.TenantManagementUserOtpSettingsSetUserOtpSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementUserOtpSettingsSetUserOtpSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementUserOtpSettingsSetUserOtpSettingsWithHttpInfo(tenantManagementUserOtpSettingsDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserOtpSettingsApi.TenantManagementUserOtpSettingsSetUserOtpSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUserOtpSettingsDto** | [**TenantManagementUserOtpSettingsDto**](TenantManagementUserOtpSettingsDto.md) |  | [optional]  |

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

