# EleonsoftProxy.Api.TenantAppearanceApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementTenantAppearanceGetTenantAppearanceSettings**](TenantAppearanceApi.md#tenantmanagementtenantappearancegettenantappearancesettings) | **GET** /api/Infrastructure/TenantAppearance/GetTenantAppearanceSettings |  |
| [**TenantManagementTenantAppearanceUpdateTenantAppearanceSettings**](TenantAppearanceApi.md#tenantmanagementtenantappearanceupdatetenantappearancesettings) | **POST** /api/Infrastructure/TenantAppearance/UpdateTenantAppearanceSettings |  |

<a id="tenantmanagementtenantappearancegettenantappearancesettings"></a>
# **TenantManagementTenantAppearanceGetTenantAppearanceSettings**
> TenantManagementTenantAppearanceSettingDto TenantManagementTenantAppearanceGetTenantAppearanceSettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementTenantAppearanceGetTenantAppearanceSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantAppearanceApi(config);

            try
            {
                TenantManagementTenantAppearanceSettingDto result = apiInstance.TenantManagementTenantAppearanceGetTenantAppearanceSettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantAppearanceApi.TenantManagementTenantAppearanceGetTenantAppearanceSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementTenantAppearanceGetTenantAppearanceSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementTenantAppearanceSettingDto> response = apiInstance.TenantManagementTenantAppearanceGetTenantAppearanceSettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantAppearanceApi.TenantManagementTenantAppearanceGetTenantAppearanceSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**TenantManagementTenantAppearanceSettingDto**](TenantManagementTenantAppearanceSettingDto.md)

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

<a id="tenantmanagementtenantappearanceupdatetenantappearancesettings"></a>
# **TenantManagementTenantAppearanceUpdateTenantAppearanceSettings**
> bool TenantManagementTenantAppearanceUpdateTenantAppearanceSettings (TenantManagementUpdateTenantAppearanceSettingRequest tenantManagementUpdateTenantAppearanceSettingRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementTenantAppearanceUpdateTenantAppearanceSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantAppearanceApi(config);
            var tenantManagementUpdateTenantAppearanceSettingRequest = new TenantManagementUpdateTenantAppearanceSettingRequest(); // TenantManagementUpdateTenantAppearanceSettingRequest |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementTenantAppearanceUpdateTenantAppearanceSettings(tenantManagementUpdateTenantAppearanceSettingRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantAppearanceApi.TenantManagementTenantAppearanceUpdateTenantAppearanceSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementTenantAppearanceUpdateTenantAppearanceSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementTenantAppearanceUpdateTenantAppearanceSettingsWithHttpInfo(tenantManagementUpdateTenantAppearanceSettingRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantAppearanceApi.TenantManagementTenantAppearanceUpdateTenantAppearanceSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUpdateTenantAppearanceSettingRequest** | [**TenantManagementUpdateTenantAppearanceSettingRequest**](TenantManagementUpdateTenantAppearanceSettingRequest.md) |  | [optional]  |

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

