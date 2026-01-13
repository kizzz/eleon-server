# EleonsoftProxy.Api.DomainSettingsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementDomainSettingsAddCorporateDomain**](DomainSettingsApi.md#tenantmanagementdomainsettingsaddcorporatedomain) | **POST** /api/TenantManagement/DomainSettings/AddCorporateDomain |  |
| [**TenantManagementDomainSettingsAddCorporateDomainForTenant**](DomainSettingsApi.md#tenantmanagementdomainsettingsaddcorporatedomainfortenant) | **POST** /api/TenantManagement/DomainSettings/AddCorporateDomainForTenant |  |
| [**TenantManagementDomainSettingsGetCurrentTenantHostnames**](DomainSettingsApi.md#tenantmanagementdomainsettingsgetcurrenttenanthostnames) | **GET** /api/TenantManagement/DomainSettings/GetCurrentTenantHostnames |  |
| [**TenantManagementDomainSettingsGetHostnamesByApplication**](DomainSettingsApi.md#tenantmanagementdomainsettingsgethostnamesbyapplication) | **GET** /api/TenantManagement/DomainSettings/GetHostnamesByApplication |  |
| [**TenantManagementDomainSettingsGetHostnamesForTenant**](DomainSettingsApi.md#tenantmanagementdomainsettingsgethostnamesfortenant) | **GET** /api/TenantManagement/DomainSettings/GetHostnamesForTenant |  |
| [**TenantManagementDomainSettingsRemoveCorporateDomain**](DomainSettingsApi.md#tenantmanagementdomainsettingsremovecorporatedomain) | **POST** /api/TenantManagement/DomainSettings/RemoveCorporateDomain |  |
| [**TenantManagementDomainSettingsRemoveCorporateDomainForTenant**](DomainSettingsApi.md#tenantmanagementdomainsettingsremovecorporatedomainfortenant) | **POST** /api/TenantManagement/DomainSettings/RemoveCorporateDomainForTenant |  |
| [**TenantManagementDomainSettingsUpdateCorporateDomain**](DomainSettingsApi.md#tenantmanagementdomainsettingsupdatecorporatedomain) | **POST** /api/TenantManagement/DomainSettings/UpdateCorporateDomain |  |
| [**TenantManagementDomainSettingsUpdateCorporateDomainForTenant**](DomainSettingsApi.md#tenantmanagementdomainsettingsupdatecorporatedomainfortenant) | **POST** /api/TenantManagement/DomainSettings/UpdateCorporateDomainForTenant |  |
| [**TenantManagementDomainSettingsUpdateDomainApplication**](DomainSettingsApi.md#tenantmanagementdomainsettingsupdatedomainapplication) | **POST** /api/TenantManagement/DomainSettings/UpdateDomainApplication |  |

<a id="tenantmanagementdomainsettingsaddcorporatedomain"></a>
# **TenantManagementDomainSettingsAddCorporateDomain**
> bool TenantManagementDomainSettingsAddCorporateDomain (TenantManagementCreateCorporateDomainRequestDto tenantManagementCreateCorporateDomainRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsAddCorporateDomainExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantManagementCreateCorporateDomainRequestDto = new TenantManagementCreateCorporateDomainRequestDto(); // TenantManagementCreateCorporateDomainRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsAddCorporateDomain(tenantManagementCreateCorporateDomainRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsAddCorporateDomain: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsAddCorporateDomainWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsAddCorporateDomainWithHttpInfo(tenantManagementCreateCorporateDomainRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsAddCorporateDomainWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementCreateCorporateDomainRequestDto** | [**TenantManagementCreateCorporateDomainRequestDto**](TenantManagementCreateCorporateDomainRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsaddcorporatedomainfortenant"></a>
# **TenantManagementDomainSettingsAddCorporateDomainForTenant**
> bool TenantManagementDomainSettingsAddCorporateDomainForTenant (Guid tenantId = null, TenantManagementCreateCorporateDomainRequestDto tenantManagementCreateCorporateDomainRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsAddCorporateDomainForTenantExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var tenantManagementCreateCorporateDomainRequestDto = new TenantManagementCreateCorporateDomainRequestDto(); // TenantManagementCreateCorporateDomainRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsAddCorporateDomainForTenant(tenantId, tenantManagementCreateCorporateDomainRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsAddCorporateDomainForTenant: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsAddCorporateDomainForTenantWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsAddCorporateDomainForTenantWithHttpInfo(tenantId, tenantManagementCreateCorporateDomainRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsAddCorporateDomainForTenantWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |
| **tenantManagementCreateCorporateDomainRequestDto** | [**TenantManagementCreateCorporateDomainRequestDto**](TenantManagementCreateCorporateDomainRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsgetcurrenttenanthostnames"></a>
# **TenantManagementDomainSettingsGetCurrentTenantHostnames**
> List&lt;TenantSettingsTenantHostnameDto&gt; TenantManagementDomainSettingsGetCurrentTenantHostnames ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsGetCurrentTenantHostnamesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);

            try
            {
                List<TenantSettingsTenantHostnameDto> result = apiInstance.TenantManagementDomainSettingsGetCurrentTenantHostnames();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetCurrentTenantHostnames: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsGetCurrentTenantHostnamesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantSettingsTenantHostnameDto>> response = apiInstance.TenantManagementDomainSettingsGetCurrentTenantHostnamesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetCurrentTenantHostnamesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantSettingsTenantHostnameDto&gt;**](TenantSettingsTenantHostnameDto.md)

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

<a id="tenantmanagementdomainsettingsgethostnamesbyapplication"></a>
# **TenantManagementDomainSettingsGetHostnamesByApplication**
> List&lt;TenantSettingsTenantHostnameDto&gt; TenantManagementDomainSettingsGetHostnamesByApplication (Guid applicationId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsGetHostnamesByApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var applicationId = "applicationId_example";  // Guid |  (optional) 

            try
            {
                List<TenantSettingsTenantHostnameDto> result = apiInstance.TenantManagementDomainSettingsGetHostnamesByApplication(applicationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetHostnamesByApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsGetHostnamesByApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantSettingsTenantHostnameDto>> response = apiInstance.TenantManagementDomainSettingsGetHostnamesByApplicationWithHttpInfo(applicationId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetHostnamesByApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;TenantSettingsTenantHostnameDto&gt;**](TenantSettingsTenantHostnameDto.md)

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

<a id="tenantmanagementdomainsettingsgethostnamesfortenant"></a>
# **TenantManagementDomainSettingsGetHostnamesForTenant**
> List&lt;TenantSettingsTenantHostnameDto&gt; TenantManagementDomainSettingsGetHostnamesForTenant (Guid tenantId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsGetHostnamesForTenantExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 

            try
            {
                List<TenantSettingsTenantHostnameDto> result = apiInstance.TenantManagementDomainSettingsGetHostnamesForTenant(tenantId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetHostnamesForTenant: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsGetHostnamesForTenantWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantSettingsTenantHostnameDto>> response = apiInstance.TenantManagementDomainSettingsGetHostnamesForTenantWithHttpInfo(tenantId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsGetHostnamesForTenantWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;TenantSettingsTenantHostnameDto&gt;**](TenantSettingsTenantHostnameDto.md)

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

<a id="tenantmanagementdomainsettingsremovecorporatedomain"></a>
# **TenantManagementDomainSettingsRemoveCorporateDomain**
> bool TenantManagementDomainSettingsRemoveCorporateDomain (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsRemoveCorporateDomainExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsRemoveCorporateDomain(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsRemoveCorporateDomain: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsRemoveCorporateDomainWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsRemoveCorporateDomainWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsRemoveCorporateDomainWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsremovecorporatedomainfortenant"></a>
# **TenantManagementDomainSettingsRemoveCorporateDomainForTenant**
> bool TenantManagementDomainSettingsRemoveCorporateDomainForTenant (Guid tenantId = null, Guid domainId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsRemoveCorporateDomainForTenantExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var domainId = "domainId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsRemoveCorporateDomainForTenant(tenantId, domainId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsRemoveCorporateDomainForTenant: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsRemoveCorporateDomainForTenantWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsRemoveCorporateDomainForTenantWithHttpInfo(tenantId, domainId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsRemoveCorporateDomainForTenantWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |
| **domainId** | **Guid** |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsupdatecorporatedomain"></a>
# **TenantManagementDomainSettingsUpdateCorporateDomain**
> bool TenantManagementDomainSettingsUpdateCorporateDomain (TenantManagementUpdateCorporateDomainRequestDto tenantManagementUpdateCorporateDomainRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsUpdateCorporateDomainExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantManagementUpdateCorporateDomainRequestDto = new TenantManagementUpdateCorporateDomainRequestDto(); // TenantManagementUpdateCorporateDomainRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsUpdateCorporateDomain(tenantManagementUpdateCorporateDomainRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateCorporateDomain: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsUpdateCorporateDomainWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsUpdateCorporateDomainWithHttpInfo(tenantManagementUpdateCorporateDomainRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateCorporateDomainWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUpdateCorporateDomainRequestDto** | [**TenantManagementUpdateCorporateDomainRequestDto**](TenantManagementUpdateCorporateDomainRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsupdatecorporatedomainfortenant"></a>
# **TenantManagementDomainSettingsUpdateCorporateDomainForTenant**
> bool TenantManagementDomainSettingsUpdateCorporateDomainForTenant (Guid tenantId = null, TenantManagementUpdateCorporateDomainRequestDto tenantManagementUpdateCorporateDomainRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsUpdateCorporateDomainForTenantExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var tenantManagementUpdateCorporateDomainRequestDto = new TenantManagementUpdateCorporateDomainRequestDto(); // TenantManagementUpdateCorporateDomainRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsUpdateCorporateDomainForTenant(tenantId, tenantManagementUpdateCorporateDomainRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateCorporateDomainForTenant: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsUpdateCorporateDomainForTenantWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsUpdateCorporateDomainForTenantWithHttpInfo(tenantId, tenantManagementUpdateCorporateDomainRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateCorporateDomainForTenantWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |
| **tenantManagementUpdateCorporateDomainRequestDto** | [**TenantManagementUpdateCorporateDomainRequestDto**](TenantManagementUpdateCorporateDomainRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementdomainsettingsupdatedomainapplication"></a>
# **TenantManagementDomainSettingsUpdateDomainApplication**
> bool TenantManagementDomainSettingsUpdateDomainApplication (Guid domainId = null, Guid appId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementDomainSettingsUpdateDomainApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DomainSettingsApi(config);
            var domainId = "domainId_example";  // Guid |  (optional) 
            var appId = "appId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementDomainSettingsUpdateDomainApplication(domainId, appId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateDomainApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementDomainSettingsUpdateDomainApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementDomainSettingsUpdateDomainApplicationWithHttpInfo(domainId, appId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DomainSettingsApi.TenantManagementDomainSettingsUpdateDomainApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **domainId** | **Guid** |  | [optional]  |
| **appId** | **Guid** |  | [optional]  |

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

