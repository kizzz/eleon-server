# EleonsoftProxy.Api.PermissionsApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementPermissionsGet**](PermissionsApi.md#tenantmanagementpermissionsget) | **GET** /api/TenantManagement/Permissions/GetAsync |  |
| [**TenantManagementPermissionsGetByGroup**](PermissionsApi.md#tenantmanagementpermissionsgetbygroup) | **GET** /api/TenantManagement/Permissions/GetByGroupAsync |  |
| [**TenantManagementPermissionsUpdate**](PermissionsApi.md#tenantmanagementpermissionsupdate) | **POST** /api/TenantManagement/Permissions/UpdateAsync |  |

<a id="tenantmanagementpermissionsget"></a>
# **TenantManagementPermissionsGet**
> EleoncoreGetPermissionListResultDto TenantManagementPermissionsGet (string providerName = null, string providerKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementPermissionsGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PermissionsApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 

            try
            {
                EleoncoreGetPermissionListResultDto result = apiInstance.TenantManagementPermissionsGet(providerName, providerKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementPermissionsGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreGetPermissionListResultDto> response = apiInstance.TenantManagementPermissionsGetWithHttpInfo(providerName, providerKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |

### Return type

[**EleoncoreGetPermissionListResultDto**](EleoncoreGetPermissionListResultDto.md)

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

<a id="tenantmanagementpermissionsgetbygroup"></a>
# **TenantManagementPermissionsGetByGroup**
> EleoncoreGetPermissionListResultDto TenantManagementPermissionsGetByGroup (string groupName = null, string providerName = null, string providerKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementPermissionsGetByGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PermissionsApi(config);
            var groupName = "groupName_example";  // string |  (optional) 
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 

            try
            {
                EleoncoreGetPermissionListResultDto result = apiInstance.TenantManagementPermissionsGetByGroup(groupName, providerName, providerKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsGetByGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementPermissionsGetByGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncoreGetPermissionListResultDto> response = apiInstance.TenantManagementPermissionsGetByGroupWithHttpInfo(groupName, providerName, providerKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsGetByGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **groupName** | **string** |  | [optional]  |
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |

### Return type

[**EleoncoreGetPermissionListResultDto**](EleoncoreGetPermissionListResultDto.md)

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

<a id="tenantmanagementpermissionsupdate"></a>
# **TenantManagementPermissionsUpdate**
> void TenantManagementPermissionsUpdate (string providerName = null, string providerKey = null, EleoncoreUpdatePermissionsDto eleoncoreUpdatePermissionsDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementPermissionsUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PermissionsApi(config);
            var providerName = "providerName_example";  // string |  (optional) 
            var providerKey = "providerKey_example";  // string |  (optional) 
            var eleoncoreUpdatePermissionsDto = new EleoncoreUpdatePermissionsDto(); // EleoncoreUpdatePermissionsDto |  (optional) 

            try
            {
                apiInstance.TenantManagementPermissionsUpdate(providerName, providerKey, eleoncoreUpdatePermissionsDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementPermissionsUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.TenantManagementPermissionsUpdateWithHttpInfo(providerName, providerKey, eleoncoreUpdatePermissionsDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PermissionsApi.TenantManagementPermissionsUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **providerName** | **string** |  | [optional]  |
| **providerKey** | **string** |  | [optional]  |
| **eleoncoreUpdatePermissionsDto** | [**EleoncoreUpdatePermissionsDto**](EleoncoreUpdatePermissionsDto.md) |  | [optional]  |

### Return type

void (empty response body)

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

