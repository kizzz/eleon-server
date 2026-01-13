# EleonsoftProxy.Api.TenantSettingsCacheApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreTenantSettingsCacheGetApplicationUrls**](TenantSettingsCacheApi.md#coretenantsettingscachegetapplicationurls) | **GET** /api/Infrastructure/TenantSettingsCache/GetApplicationUrls |  |
| [**CoreTenantSettingsCacheGetInactiveTenants**](TenantSettingsCacheApi.md#coretenantsettingscachegetinactivetenants) | **GET** /api/Infrastructure/TenantSettingsCache/GetInactiveTenants |  |
| [**CoreTenantSettingsCacheGetTenantByUrl**](TenantSettingsCacheApi.md#coretenantsettingscachegettenantbyurl) | **GET** /api/Infrastructure/TenantSettingsCache/GetTenantByUrl |  |
| [**CoreTenantSettingsCacheGetTenantSettings**](TenantSettingsCacheApi.md#coretenantsettingscachegettenantsettings) | **GET** /api/Infrastructure/TenantSettingsCache/GetTenantSettings |  |

<a id="coretenantsettingscachegetapplicationurls"></a>
# **CoreTenantSettingsCacheGetApplicationUrls**
> List&lt;string&gt; CoreTenantSettingsCacheGetApplicationUrls ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsCacheGetApplicationUrlsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsCacheApi(config);

            try
            {
                List<string> result = apiInstance.CoreTenantSettingsCacheGetApplicationUrls();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetApplicationUrls: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsCacheGetApplicationUrlsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.CoreTenantSettingsCacheGetApplicationUrlsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetApplicationUrlsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**List<string>**

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

<a id="coretenantsettingscachegetinactivetenants"></a>
# **CoreTenantSettingsCacheGetInactiveTenants**
> List&lt;Guid&gt; CoreTenantSettingsCacheGetInactiveTenants ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsCacheGetInactiveTenantsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsCacheApi(config);

            try
            {
                List<Guid> result = apiInstance.CoreTenantSettingsCacheGetInactiveTenants();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetInactiveTenants: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsCacheGetInactiveTenantsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<Guid>> response = apiInstance.CoreTenantSettingsCacheGetInactiveTenantsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetInactiveTenantsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**List<Guid>**

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

<a id="coretenantsettingscachegettenantbyurl"></a>
# **CoreTenantSettingsCacheGetTenantByUrl**
> TenantManagementTenantFoundDto CoreTenantSettingsCacheGetTenantByUrl (string url = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsCacheGetTenantByUrlExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsCacheApi(config);
            var url = "url_example";  // string |  (optional) 

            try
            {
                TenantManagementTenantFoundDto result = apiInstance.CoreTenantSettingsCacheGetTenantByUrl(url);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantByUrl: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsCacheGetTenantByUrlWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementTenantFoundDto> response = apiInstance.CoreTenantSettingsCacheGetTenantByUrlWithHttpInfo(url);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantByUrlWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **url** | **string** |  | [optional]  |

### Return type

[**TenantManagementTenantFoundDto**](TenantManagementTenantFoundDto.md)

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

<a id="coretenantsettingscachegettenantsettings"></a>
# **CoreTenantSettingsCacheGetTenantSettings**
> TenantManagementTenantSettingsCacheValueDto CoreTenantSettingsCacheGetTenantSettings (Guid tenantId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreTenantSettingsCacheGetTenantSettingsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantSettingsCacheApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 

            try
            {
                TenantManagementTenantSettingsCacheValueDto result = apiInstance.CoreTenantSettingsCacheGetTenantSettings(tenantId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantSettings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreTenantSettingsCacheGetTenantSettingsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementTenantSettingsCacheValueDto> response = apiInstance.CoreTenantSettingsCacheGetTenantSettingsWithHttpInfo(tenantId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantSettingsCacheApi.CoreTenantSettingsCacheGetTenantSettingsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |

### Return type

[**TenantManagementTenantSettingsCacheValueDto**](TenantManagementTenantSettingsCacheValueDto.md)

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

