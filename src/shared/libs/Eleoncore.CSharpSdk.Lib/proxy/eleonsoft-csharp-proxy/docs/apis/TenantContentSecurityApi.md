# EleonsoftProxy.Api.TenantContentSecurityApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementTenantContentSecurityAddTenantContentSecurityHost**](TenantContentSecurityApi.md#tenantmanagementtenantcontentsecurityaddtenantcontentsecurityhost) | **POST** /api/TenantManagement/ContentSecurity/AddTenantContentSecurityHost |  |
| [**TenantManagementTenantContentSecurityRemoveTenantContentSecurityHost**](TenantContentSecurityApi.md#tenantmanagementtenantcontentsecurityremovetenantcontentsecurityhost) | **POST** /api/TenantManagement/ContentSecurity/RemoveTenantContentSecurityHost |  |
| [**TenantManagementTenantContentSecurityUpdateTenantContentSecurityHost**](TenantContentSecurityApi.md#tenantmanagementtenantcontentsecurityupdatetenantcontentsecurityhost) | **POST** /api/TenantManagement/ContentSecurity/UpdateTenantContentSecurityHost |  |

<a id="tenantmanagementtenantcontentsecurityaddtenantcontentsecurityhost"></a>
# **TenantManagementTenantContentSecurityAddTenantContentSecurityHost**
> bool TenantManagementTenantContentSecurityAddTenantContentSecurityHost (TenantManagementAddTenantContentSecurityHostDto tenantManagementAddTenantContentSecurityHostDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementTenantContentSecurityAddTenantContentSecurityHostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantContentSecurityApi(config);
            var tenantManagementAddTenantContentSecurityHostDto = new TenantManagementAddTenantContentSecurityHostDto(); // TenantManagementAddTenantContentSecurityHostDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementTenantContentSecurityAddTenantContentSecurityHost(tenantManagementAddTenantContentSecurityHostDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityAddTenantContentSecurityHost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementTenantContentSecurityAddTenantContentSecurityHostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementTenantContentSecurityAddTenantContentSecurityHostWithHttpInfo(tenantManagementAddTenantContentSecurityHostDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityAddTenantContentSecurityHostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementAddTenantContentSecurityHostDto** | [**TenantManagementAddTenantContentSecurityHostDto**](TenantManagementAddTenantContentSecurityHostDto.md) |  | [optional]  |

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

<a id="tenantmanagementtenantcontentsecurityremovetenantcontentsecurityhost"></a>
# **TenantManagementTenantContentSecurityRemoveTenantContentSecurityHost**
> bool TenantManagementTenantContentSecurityRemoveTenantContentSecurityHost (TenantManagementRemoveTenantContentSecurityHostDto tenantManagementRemoveTenantContentSecurityHostDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementTenantContentSecurityRemoveTenantContentSecurityHostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantContentSecurityApi(config);
            var tenantManagementRemoveTenantContentSecurityHostDto = new TenantManagementRemoveTenantContentSecurityHostDto(); // TenantManagementRemoveTenantContentSecurityHostDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementTenantContentSecurityRemoveTenantContentSecurityHost(tenantManagementRemoveTenantContentSecurityHostDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityRemoveTenantContentSecurityHost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementTenantContentSecurityRemoveTenantContentSecurityHostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementTenantContentSecurityRemoveTenantContentSecurityHostWithHttpInfo(tenantManagementRemoveTenantContentSecurityHostDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityRemoveTenantContentSecurityHostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementRemoveTenantContentSecurityHostDto** | [**TenantManagementRemoveTenantContentSecurityHostDto**](TenantManagementRemoveTenantContentSecurityHostDto.md) |  | [optional]  |

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

<a id="tenantmanagementtenantcontentsecurityupdatetenantcontentsecurityhost"></a>
# **TenantManagementTenantContentSecurityUpdateTenantContentSecurityHost**
> bool TenantManagementTenantContentSecurityUpdateTenantContentSecurityHost (TenantManagementUpdateTenantContentSecurityHostDto tenantManagementUpdateTenantContentSecurityHostDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementTenantContentSecurityUpdateTenantContentSecurityHostExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new TenantContentSecurityApi(config);
            var tenantManagementUpdateTenantContentSecurityHostDto = new TenantManagementUpdateTenantContentSecurityHostDto(); // TenantManagementUpdateTenantContentSecurityHostDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementTenantContentSecurityUpdateTenantContentSecurityHost(tenantManagementUpdateTenantContentSecurityHostDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityUpdateTenantContentSecurityHost: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementTenantContentSecurityUpdateTenantContentSecurityHostWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementTenantContentSecurityUpdateTenantContentSecurityHostWithHttpInfo(tenantManagementUpdateTenantContentSecurityHostDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling TenantContentSecurityApi.TenantManagementTenantContentSecurityUpdateTenantContentSecurityHostWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUpdateTenantContentSecurityHostDto** | [**TenantManagementUpdateTenantContentSecurityHostDto**](TenantManagementUpdateTenantContentSecurityHostDto.md) |  | [optional]  |

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

