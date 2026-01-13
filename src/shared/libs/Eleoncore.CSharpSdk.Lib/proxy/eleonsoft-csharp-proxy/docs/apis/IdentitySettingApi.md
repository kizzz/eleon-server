# EleonsoftProxy.Api.IdentitySettingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementIdentitySettingGetIdentitySettings**](IdentitySettingApi.md#tenantmanagementidentitysettinggetidentitysettings) | **GET** /api/Infrastructure/IdentitySettings/GetIdentitySettings |  |
| [**TenantManagementIdentitySettingSetIdentitySettings**](IdentitySettingApi.md#tenantmanagementidentitysettingsetidentitysettings) | **POST** /api/Infrastructure/IdentitySettings/SetIdentitySettings |  |

<a id="tenantmanagementidentitysettinggetidentitysettings"></a>
# **TenantManagementIdentitySettingGetIdentitySettings**
> List&lt;TenantManagementIdentitySettingDto&gt; TenantManagementIdentitySettingGetIdentitySettings ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementIdentitySettingGetIdentitySettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new IdentitySettingApi(config);

            try
            {
                List<TenantManagementIdentitySettingDto> result = apiInstance.TenantManagementIdentitySettingGetIdentitySettings();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling IdentitySettingApi.TenantManagementIdentitySettingGetIdentitySettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementIdentitySettingGetIdentitySettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementIdentitySettingDto>> response = apiInstance.TenantManagementIdentitySettingGetIdentitySettingsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling IdentitySettingApi.TenantManagementIdentitySettingGetIdentitySettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementIdentitySettingDto&gt;**](TenantManagementIdentitySettingDto.md)

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

<a id="tenantmanagementidentitysettingsetidentitysettings"></a>
# **TenantManagementIdentitySettingSetIdentitySettings**
> bool TenantManagementIdentitySettingSetIdentitySettings (TenantManagementSetIdentitySettingsRequest tenantManagementSetIdentitySettingsRequest = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementIdentitySettingSetIdentitySettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new IdentitySettingApi(config);
            var tenantManagementSetIdentitySettingsRequest = new TenantManagementSetIdentitySettingsRequest(); // TenantManagementSetIdentitySettingsRequest |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementIdentitySettingSetIdentitySettings(tenantManagementSetIdentitySettingsRequest);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling IdentitySettingApi.TenantManagementIdentitySettingSetIdentitySettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementIdentitySettingSetIdentitySettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementIdentitySettingSetIdentitySettingsWithHttpInfo(tenantManagementSetIdentitySettingsRequest);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling IdentitySettingApi.TenantManagementIdentitySettingSetIdentitySettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetIdentitySettingsRequest** | [**TenantManagementSetIdentitySettingsRequest**](TenantManagementSetIdentitySettingsRequest.md) |  | [optional]  |

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

