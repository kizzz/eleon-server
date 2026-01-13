# EleonsoftProxy.Api.CustomPermissionsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementCustomPermissionsCreateBulkForMicroservice**](CustomPermissionsApi.md#sitesmanagementcustompermissionscreatebulkformicroservice) | **POST** /api/SitesManagement/CustomPermissions/CreateBulkForMicroserviceAsync |  |
| [**SitesManagementCustomPermissionsCreateGroup**](CustomPermissionsApi.md#sitesmanagementcustompermissionscreategroup) | **POST** /api/SitesManagement/CustomPermissions/CreateGroup |  |
| [**SitesManagementCustomPermissionsCreatePermission**](CustomPermissionsApi.md#sitesmanagementcustompermissionscreatepermission) | **POST** /api/SitesManagement/CustomPermissions/CreatePermission |  |
| [**SitesManagementCustomPermissionsDeleteGroup**](CustomPermissionsApi.md#sitesmanagementcustompermissionsdeletegroup) | **DELETE** /api/SitesManagement/CustomPermissions/DeleteGroup |  |
| [**SitesManagementCustomPermissionsDeletePermission**](CustomPermissionsApi.md#sitesmanagementcustompermissionsdeletepermission) | **DELETE** /api/SitesManagement/CustomPermissions/DeletePermission |  |
| [**SitesManagementCustomPermissionsGetPermissionDynamicGroupCategories**](CustomPermissionsApi.md#sitesmanagementcustompermissionsgetpermissiondynamicgroupcategories) | **GET** /api/SitesManagement/CustomPermissions/GetPermissionGroups |  |
| [**SitesManagementCustomPermissionsGetPermissionsDynamic**](CustomPermissionsApi.md#sitesmanagementcustompermissionsgetpermissionsdynamic) | **GET** /api/SitesManagement/CustomPermissions/GetPermissions |  |
| [**SitesManagementCustomPermissionsUpdateGroup**](CustomPermissionsApi.md#sitesmanagementcustompermissionsupdategroup) | **PUT** /api/SitesManagement/CustomPermissions/UpdateGroup |  |
| [**SitesManagementCustomPermissionsUpdatePermission**](CustomPermissionsApi.md#sitesmanagementcustompermissionsupdatepermission) | **PUT** /api/SitesManagement/CustomPermissions/UpdatePermission |  |

<a id="sitesmanagementcustompermissionscreatebulkformicroservice"></a>
# **SitesManagementCustomPermissionsCreateBulkForMicroservice**
> bool SitesManagementCustomPermissionsCreateBulkForMicroservice (ModuleCollectorCustomPermissionsForMicroserviceDto moduleCollectorCustomPermissionsForMicroserviceDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsCreateBulkForMicroserviceExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var moduleCollectorCustomPermissionsForMicroserviceDto = new ModuleCollectorCustomPermissionsForMicroserviceDto(); // ModuleCollectorCustomPermissionsForMicroserviceDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementCustomPermissionsCreateBulkForMicroservice(moduleCollectorCustomPermissionsForMicroserviceDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreateBulkForMicroservice: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsCreateBulkForMicroserviceWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementCustomPermissionsCreateBulkForMicroserviceWithHttpInfo(moduleCollectorCustomPermissionsForMicroserviceDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreateBulkForMicroserviceWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorCustomPermissionsForMicroserviceDto** | [**ModuleCollectorCustomPermissionsForMicroserviceDto**](ModuleCollectorCustomPermissionsForMicroserviceDto.md) |  | [optional]  |

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

<a id="sitesmanagementcustompermissionscreategroup"></a>
# **SitesManagementCustomPermissionsCreateGroup**
> SitesManagementCustomPermissionGroupDto SitesManagementCustomPermissionsCreateGroup (SitesManagementCustomPermissionGroupDto sitesManagementCustomPermissionGroupDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsCreateGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var sitesManagementCustomPermissionGroupDto = new SitesManagementCustomPermissionGroupDto(); // SitesManagementCustomPermissionGroupDto |  (optional) 

            try
            {
                SitesManagementCustomPermissionGroupDto result = apiInstance.SitesManagementCustomPermissionsCreateGroup(sitesManagementCustomPermissionGroupDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreateGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsCreateGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomPermissionGroupDto> response = apiInstance.SitesManagementCustomPermissionsCreateGroupWithHttpInfo(sitesManagementCustomPermissionGroupDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreateGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomPermissionGroupDto** | [**SitesManagementCustomPermissionGroupDto**](SitesManagementCustomPermissionGroupDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomPermissionGroupDto**](SitesManagementCustomPermissionGroupDto.md)

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

<a id="sitesmanagementcustompermissionscreatepermission"></a>
# **SitesManagementCustomPermissionsCreatePermission**
> SitesManagementCustomPermissionDto SitesManagementCustomPermissionsCreatePermission (SitesManagementCustomPermissionDto sitesManagementCustomPermissionDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsCreatePermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var sitesManagementCustomPermissionDto = new SitesManagementCustomPermissionDto(); // SitesManagementCustomPermissionDto |  (optional) 

            try
            {
                SitesManagementCustomPermissionDto result = apiInstance.SitesManagementCustomPermissionsCreatePermission(sitesManagementCustomPermissionDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreatePermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsCreatePermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomPermissionDto> response = apiInstance.SitesManagementCustomPermissionsCreatePermissionWithHttpInfo(sitesManagementCustomPermissionDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsCreatePermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomPermissionDto** | [**SitesManagementCustomPermissionDto**](SitesManagementCustomPermissionDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomPermissionDto**](SitesManagementCustomPermissionDto.md)

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

<a id="sitesmanagementcustompermissionsdeletegroup"></a>
# **SitesManagementCustomPermissionsDeleteGroup**
> void SitesManagementCustomPermissionsDeleteGroup (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsDeleteGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                apiInstance.SitesManagementCustomPermissionsDeleteGroup(name);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsDeleteGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsDeleteGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementCustomPermissionsDeleteGroupWithHttpInfo(name);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsDeleteGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

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

<a id="sitesmanagementcustompermissionsdeletepermission"></a>
# **SitesManagementCustomPermissionsDeletePermission**
> void SitesManagementCustomPermissionsDeletePermission (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsDeletePermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                apiInstance.SitesManagementCustomPermissionsDeletePermission(name);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsDeletePermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsDeletePermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementCustomPermissionsDeletePermissionWithHttpInfo(name);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsDeletePermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

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

<a id="sitesmanagementcustompermissionsgetpermissiondynamicgroupcategories"></a>
# **SitesManagementCustomPermissionsGetPermissionDynamicGroupCategories**
> List&lt;SitesManagementCustomPermissionGroupDto&gt; SitesManagementCustomPermissionsGetPermissionDynamicGroupCategories ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsGetPermissionDynamicGroupCategoriesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);

            try
            {
                List<SitesManagementCustomPermissionGroupDto> result = apiInstance.SitesManagementCustomPermissionsGetPermissionDynamicGroupCategories();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsGetPermissionDynamicGroupCategories: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsGetPermissionDynamicGroupCategoriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomPermissionGroupDto>> response = apiInstance.SitesManagementCustomPermissionsGetPermissionDynamicGroupCategoriesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsGetPermissionDynamicGroupCategoriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomPermissionGroupDto&gt;**](SitesManagementCustomPermissionGroupDto.md)

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

<a id="sitesmanagementcustompermissionsgetpermissionsdynamic"></a>
# **SitesManagementCustomPermissionsGetPermissionsDynamic**
> List&lt;SitesManagementCustomPermissionDto&gt; SitesManagementCustomPermissionsGetPermissionsDynamic ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsGetPermissionsDynamicExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);

            try
            {
                List<SitesManagementCustomPermissionDto> result = apiInstance.SitesManagementCustomPermissionsGetPermissionsDynamic();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsGetPermissionsDynamic: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsGetPermissionsDynamicWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomPermissionDto>> response = apiInstance.SitesManagementCustomPermissionsGetPermissionsDynamicWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsGetPermissionsDynamicWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomPermissionDto&gt;**](SitesManagementCustomPermissionDto.md)

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

<a id="sitesmanagementcustompermissionsupdategroup"></a>
# **SitesManagementCustomPermissionsUpdateGroup**
> SitesManagementCustomPermissionGroupDto SitesManagementCustomPermissionsUpdateGroup (SitesManagementCustomPermissionGroupDto sitesManagementCustomPermissionGroupDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsUpdateGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var sitesManagementCustomPermissionGroupDto = new SitesManagementCustomPermissionGroupDto(); // SitesManagementCustomPermissionGroupDto |  (optional) 

            try
            {
                SitesManagementCustomPermissionGroupDto result = apiInstance.SitesManagementCustomPermissionsUpdateGroup(sitesManagementCustomPermissionGroupDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsUpdateGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsUpdateGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomPermissionGroupDto> response = apiInstance.SitesManagementCustomPermissionsUpdateGroupWithHttpInfo(sitesManagementCustomPermissionGroupDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsUpdateGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomPermissionGroupDto** | [**SitesManagementCustomPermissionGroupDto**](SitesManagementCustomPermissionGroupDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomPermissionGroupDto**](SitesManagementCustomPermissionGroupDto.md)

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

<a id="sitesmanagementcustompermissionsupdatepermission"></a>
# **SitesManagementCustomPermissionsUpdatePermission**
> SitesManagementCustomPermissionDto SitesManagementCustomPermissionsUpdatePermission (SitesManagementCustomPermissionDto sitesManagementCustomPermissionDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class SitesManagementCustomPermissionsUpdatePermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomPermissionsApi(config);
            var sitesManagementCustomPermissionDto = new SitesManagementCustomPermissionDto(); // SitesManagementCustomPermissionDto |  (optional) 

            try
            {
                SitesManagementCustomPermissionDto result = apiInstance.SitesManagementCustomPermissionsUpdatePermission(sitesManagementCustomPermissionDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsUpdatePermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomPermissionsUpdatePermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomPermissionDto> response = apiInstance.SitesManagementCustomPermissionsUpdatePermissionWithHttpInfo(sitesManagementCustomPermissionDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomPermissionsApi.SitesManagementCustomPermissionsUpdatePermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomPermissionDto** | [**SitesManagementCustomPermissionDto**](SitesManagementCustomPermissionDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomPermissionDto**](SitesManagementCustomPermissionDto.md)

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

