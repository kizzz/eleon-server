# EleoncoreProxy.Api.ClientApplicationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementClientApplicationAddBulkModulesToApplication**](ClientApplicationApi.md#sitesmanagementclientapplicationaddbulkmodulestoapplication) | **POST** /api/CoreInfrastructure/ClientApplications/AddBulkModulesToApplication |  |
| [**SitesManagementClientApplicationAddModuleToApplication**](ClientApplicationApi.md#sitesmanagementclientapplicationaddmoduletoapplication) | **POST** /api/CoreInfrastructure/ClientApplications/AddModuleToApplication |  |
| [**SitesManagementClientApplicationCreate**](ClientApplicationApi.md#sitesmanagementclientapplicationcreate) | **POST** /api/CoreInfrastructure/ClientApplications/CreateAsync |  |
| [**SitesManagementClientApplicationDelete**](ClientApplicationApi.md#sitesmanagementclientapplicationdelete) | **DELETE** /api/CoreInfrastructure/ClientApplications/{id} |  |
| [**SitesManagementClientApplicationGet**](ClientApplicationApi.md#sitesmanagementclientapplicationget) | **GET** /api/CoreInfrastructure/ClientApplications/{id} |  |
| [**SitesManagementClientApplicationGetAll**](ClientApplicationApi.md#sitesmanagementclientapplicationgetall) | **GET** /api/CoreInfrastructure/ClientApplications/GetAllAsync |  |
| [**SitesManagementClientApplicationGetAppsSettingsBySiteId**](ClientApplicationApi.md#sitesmanagementclientapplicationgetappssettingsbysiteid) | **GET** /api/CoreInfrastructure/ClientApplications/GetAppsSettingsBySiteId |  |
| [**SitesManagementClientApplicationGetByTenantId**](ClientApplicationApi.md#sitesmanagementclientapplicationgetbytenantid) | **GET** /api/CoreInfrastructure/ClientApplications/GetByTenantIdAsync |  |
| [**SitesManagementClientApplicationGetDefaultApplication**](ClientApplicationApi.md#sitesmanagementclientapplicationgetdefaultapplication) | **GET** /api/CoreInfrastructure/ClientApplications/GetDefaultApplication |  |
| [**SitesManagementClientApplicationGetEnabledApplications**](ClientApplicationApi.md#sitesmanagementclientapplicationgetenabledapplications) | **GET** /api/CoreInfrastructure/ClientApplications/enabled-applications |  |
| [**SitesManagementClientApplicationGetLocations**](ClientApplicationApi.md#sitesmanagementclientapplicationgetlocations) | **GET** /api/CoreInfrastructure/ClientApplications/GetLocations |  |
| [**SitesManagementClientApplicationGetLocationsBySiteId**](ClientApplicationApi.md#sitesmanagementclientapplicationgetlocationsbysiteid) | **GET** /api/CoreInfrastructure/ClientApplications/GetLocationsBySiteId |  |
| [**SitesManagementClientApplicationGetSetting**](ClientApplicationApi.md#sitesmanagementclientapplicationgetsetting) | **GET** /api/CoreInfrastructure/ClientApplications/GetSetting |  |
| [**SitesManagementClientApplicationGetSiteByHostname**](ClientApplicationApi.md#sitesmanagementclientapplicationgetsitebyhostname) | **GET** /api/CoreInfrastructure/ClientApplications/GetSiteByHostname |  |
| [**SitesManagementClientApplicationRemoveModuleFromApplication**](ClientApplicationApi.md#sitesmanagementclientapplicationremovemodulefromapplication) | **DELETE** /api/CoreInfrastructure/ClientApplications/RemoveModuleToApplication |  |
| [**SitesManagementClientApplicationUpdate**](ClientApplicationApi.md#sitesmanagementclientapplicationupdate) | **PUT** /api/CoreInfrastructure/ClientApplications/{id} |  |
| [**SitesManagementClientApplicationUseDedicatedDatabase**](ClientApplicationApi.md#sitesmanagementclientapplicationusededicateddatabase) | **PUT** /api/CoreInfrastructure/ClientApplications/{id}/UseDedicatedDatabase |  |

<a id="sitesmanagementclientapplicationaddbulkmodulestoapplication"></a>
# **SitesManagementClientApplicationAddBulkModulesToApplication**
> bool SitesManagementClientApplicationAddBulkModulesToApplication (List<SitesManagementApplicationModuleDto> sitesManagementApplicationModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationAddBulkModulesToApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var sitesManagementApplicationModuleDto = new List<SitesManagementApplicationModuleDto>(); // List<SitesManagementApplicationModuleDto> |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementClientApplicationAddBulkModulesToApplication(sitesManagementApplicationModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationAddBulkModulesToApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationAddBulkModulesToApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementClientApplicationAddBulkModulesToApplicationWithHttpInfo(sitesManagementApplicationModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationAddBulkModulesToApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementApplicationModuleDto** | [**List&lt;SitesManagementApplicationModuleDto&gt;**](SitesManagementApplicationModuleDto.md) |  | [optional]  |

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

<a id="sitesmanagementclientapplicationaddmoduletoapplication"></a>
# **SitesManagementClientApplicationAddModuleToApplication**
> bool SitesManagementClientApplicationAddModuleToApplication (SitesManagementApplicationModuleDto sitesManagementApplicationModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationAddModuleToApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var sitesManagementApplicationModuleDto = new SitesManagementApplicationModuleDto(); // SitesManagementApplicationModuleDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementClientApplicationAddModuleToApplication(sitesManagementApplicationModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationAddModuleToApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationAddModuleToApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementClientApplicationAddModuleToApplicationWithHttpInfo(sitesManagementApplicationModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationAddModuleToApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementApplicationModuleDto** | [**SitesManagementApplicationModuleDto**](SitesManagementApplicationModuleDto.md) |  | [optional]  |

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

<a id="sitesmanagementclientapplicationcreate"></a>
# **SitesManagementClientApplicationCreate**
> SitesManagementClientApplicationDto SitesManagementClientApplicationCreate (SitesManagementCreateClientApplicationDto sitesManagementCreateClientApplicationDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var sitesManagementCreateClientApplicationDto = new SitesManagementCreateClientApplicationDto(); // SitesManagementCreateClientApplicationDto |  (optional) 

            try
            {
                SitesManagementClientApplicationDto result = apiInstance.SitesManagementClientApplicationCreate(sitesManagementCreateClientApplicationDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementClientApplicationDto> response = apiInstance.SitesManagementClientApplicationCreateWithHttpInfo(sitesManagementCreateClientApplicationDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCreateClientApplicationDto** | [**SitesManagementCreateClientApplicationDto**](SitesManagementCreateClientApplicationDto.md) |  | [optional]  |

### Return type

[**SitesManagementClientApplicationDto**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationdelete"></a>
# **SitesManagementClientApplicationDelete**
> void SitesManagementClientApplicationDelete (Guid id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var id = "id_example";  // Guid | 

            try
            {
                apiInstance.SitesManagementClientApplicationDelete(id);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementClientApplicationDeleteWithHttpInfo(id);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |

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

<a id="sitesmanagementclientapplicationget"></a>
# **SitesManagementClientApplicationGet**
> SitesManagementClientApplicationDto SitesManagementClientApplicationGet (Guid id)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var id = "id_example";  // Guid | 

            try
            {
                SitesManagementClientApplicationDto result = apiInstance.SitesManagementClientApplicationGet(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementClientApplicationDto> response = apiInstance.SitesManagementClientApplicationGetWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |

### Return type

[**SitesManagementClientApplicationDto**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationgetall"></a>
# **SitesManagementClientApplicationGetAll**
> List&lt;SitesManagementFullClientApplicationDto&gt; SitesManagementClientApplicationGetAll ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);

            try
            {
                List<SitesManagementFullClientApplicationDto> result = apiInstance.SitesManagementClientApplicationGetAll();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetAll: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetAllWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementFullClientApplicationDto>> response = apiInstance.SitesManagementClientApplicationGetAllWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetAllWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementFullClientApplicationDto&gt;**](SitesManagementFullClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationgetappssettingsbysiteid"></a>
# **SitesManagementClientApplicationGetAppsSettingsBySiteId**
> SitesManagementModuleSettingsDto SitesManagementClientApplicationGetAppsSettingsBySiteId (Guid siteId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetAppsSettingsBySiteIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var siteId = "siteId_example";  // Guid |  (optional) 

            try
            {
                SitesManagementModuleSettingsDto result = apiInstance.SitesManagementClientApplicationGetAppsSettingsBySiteId(siteId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetAppsSettingsBySiteId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetAppsSettingsBySiteIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementModuleSettingsDto> response = apiInstance.SitesManagementClientApplicationGetAppsSettingsBySiteIdWithHttpInfo(siteId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetAppsSettingsBySiteIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **siteId** | **Guid** |  | [optional]  |

### Return type

[**SitesManagementModuleSettingsDto**](SitesManagementModuleSettingsDto.md)

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

<a id="sitesmanagementclientapplicationgetbytenantid"></a>
# **SitesManagementClientApplicationGetByTenantId**
> List&lt;SitesManagementClientApplicationDto&gt; SitesManagementClientApplicationGetByTenantId (Guid tenantId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetByTenantIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 

            try
            {
                List<SitesManagementClientApplicationDto> result = apiInstance.SitesManagementClientApplicationGetByTenantId(tenantId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetByTenantId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetByTenantIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementClientApplicationDto>> response = apiInstance.SitesManagementClientApplicationGetByTenantIdWithHttpInfo(tenantId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetByTenantIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;SitesManagementClientApplicationDto&gt;**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationgetdefaultapplication"></a>
# **SitesManagementClientApplicationGetDefaultApplication**
> SitesManagementClientApplicationDto SitesManagementClientApplicationGetDefaultApplication ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetDefaultApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);

            try
            {
                SitesManagementClientApplicationDto result = apiInstance.SitesManagementClientApplicationGetDefaultApplication();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetDefaultApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetDefaultApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementClientApplicationDto> response = apiInstance.SitesManagementClientApplicationGetDefaultApplicationWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetDefaultApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**SitesManagementClientApplicationDto**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationgetenabledapplications"></a>
# **SitesManagementClientApplicationGetEnabledApplications**
> List&lt;SitesManagementClientApplicationDto&gt; SitesManagementClientApplicationGetEnabledApplications ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetEnabledApplicationsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);

            try
            {
                List<SitesManagementClientApplicationDto> result = apiInstance.SitesManagementClientApplicationGetEnabledApplications();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetEnabledApplications: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetEnabledApplicationsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementClientApplicationDto>> response = apiInstance.SitesManagementClientApplicationGetEnabledApplicationsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetEnabledApplicationsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementClientApplicationDto&gt;**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationgetlocations"></a>
# **SitesManagementClientApplicationGetLocations**
> List&lt;ModuleCollectorLocation&gt; SitesManagementClientApplicationGetLocations ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetLocationsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);

            try
            {
                List<ModuleCollectorLocation> result = apiInstance.SitesManagementClientApplicationGetLocations();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetLocations: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetLocationsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ModuleCollectorLocation>> response = apiInstance.SitesManagementClientApplicationGetLocationsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetLocationsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;ModuleCollectorLocation&gt;**](ModuleCollectorLocation.md)

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

<a id="sitesmanagementclientapplicationgetlocationsbysiteid"></a>
# **SitesManagementClientApplicationGetLocationsBySiteId**
> List&lt;ModuleCollectorLocation&gt; SitesManagementClientApplicationGetLocationsBySiteId (Guid siteId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetLocationsBySiteIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var siteId = "siteId_example";  // Guid |  (optional) 

            try
            {
                List<ModuleCollectorLocation> result = apiInstance.SitesManagementClientApplicationGetLocationsBySiteId(siteId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetLocationsBySiteId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetLocationsBySiteIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<ModuleCollectorLocation>> response = apiInstance.SitesManagementClientApplicationGetLocationsBySiteIdWithHttpInfo(siteId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetLocationsBySiteIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **siteId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;ModuleCollectorLocation&gt;**](ModuleCollectorLocation.md)

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

<a id="sitesmanagementclientapplicationgetsetting"></a>
# **SitesManagementClientApplicationGetSetting**
> SitesManagementModuleSettingsDto SitesManagementClientApplicationGetSetting ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);

            try
            {
                SitesManagementModuleSettingsDto result = apiInstance.SitesManagementClientApplicationGetSetting();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementModuleSettingsDto> response = apiInstance.SitesManagementClientApplicationGetSettingWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**SitesManagementModuleSettingsDto**](SitesManagementModuleSettingsDto.md)

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

<a id="sitesmanagementclientapplicationgetsitebyhostname"></a>
# **SitesManagementClientApplicationGetSiteByHostname**
> SitesManagementClientApplicationDto SitesManagementClientApplicationGetSiteByHostname (string hostname = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationGetSiteByHostnameExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var hostname = "hostname_example";  // string |  (optional) 

            try
            {
                SitesManagementClientApplicationDto result = apiInstance.SitesManagementClientApplicationGetSiteByHostname(hostname);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetSiteByHostname: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationGetSiteByHostnameWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementClientApplicationDto> response = apiInstance.SitesManagementClientApplicationGetSiteByHostnameWithHttpInfo(hostname);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationGetSiteByHostnameWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **hostname** | **string** |  | [optional]  |

### Return type

[**SitesManagementClientApplicationDto**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationremovemodulefromapplication"></a>
# **SitesManagementClientApplicationRemoveModuleFromApplication**
> bool SitesManagementClientApplicationRemoveModuleFromApplication (Guid applicationId = null, Guid moduleId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationRemoveModuleFromApplicationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var applicationId = "applicationId_example";  // Guid |  (optional) 
            var moduleId = "moduleId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementClientApplicationRemoveModuleFromApplication(applicationId, moduleId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationRemoveModuleFromApplication: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationRemoveModuleFromApplicationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementClientApplicationRemoveModuleFromApplicationWithHttpInfo(applicationId, moduleId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationRemoveModuleFromApplicationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **applicationId** | **Guid** |  | [optional]  |
| **moduleId** | **Guid** |  | [optional]  |

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

<a id="sitesmanagementclientapplicationupdate"></a>
# **SitesManagementClientApplicationUpdate**
> SitesManagementClientApplicationDto SitesManagementClientApplicationUpdate (Guid id, SitesManagementUpdateClientApplicationDto sitesManagementUpdateClientApplicationDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var id = "id_example";  // Guid | 
            var sitesManagementUpdateClientApplicationDto = new SitesManagementUpdateClientApplicationDto(); // SitesManagementUpdateClientApplicationDto |  (optional) 

            try
            {
                SitesManagementClientApplicationDto result = apiInstance.SitesManagementClientApplicationUpdate(id, sitesManagementUpdateClientApplicationDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementClientApplicationDto> response = apiInstance.SitesManagementClientApplicationUpdateWithHttpInfo(id, sitesManagementUpdateClientApplicationDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |
| **sitesManagementUpdateClientApplicationDto** | [**SitesManagementUpdateClientApplicationDto**](SitesManagementUpdateClientApplicationDto.md) |  | [optional]  |

### Return type

[**SitesManagementClientApplicationDto**](SitesManagementClientApplicationDto.md)

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

<a id="sitesmanagementclientapplicationusededicateddatabase"></a>
# **SitesManagementClientApplicationUseDedicatedDatabase**
> void SitesManagementClientApplicationUseDedicatedDatabase (Guid id, bool useDedicatedDb = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementClientApplicationUseDedicatedDatabaseExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ClientApplicationApi(config);
            var id = "id_example";  // Guid | 
            var useDedicatedDb = true;  // bool |  (optional) 

            try
            {
                apiInstance.SitesManagementClientApplicationUseDedicatedDatabase(id, useDedicatedDb);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationUseDedicatedDatabase: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementClientApplicationUseDedicatedDatabaseWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementClientApplicationUseDedicatedDatabaseWithHttpInfo(id, useDedicatedDb);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ClientApplicationApi.SitesManagementClientApplicationUseDedicatedDatabaseWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  |  |
| **useDedicatedDb** | **bool** |  | [optional]  |

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

