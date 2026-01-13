# EleonsoftProxy.Api.PermissionGroupApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementPermissionGroupGetPermissionGroups**](PermissionGroupApi.md#tenantmanagementpermissiongroupgetpermissiongroups) | **GET** /api/TenantManagement/PermissionGroups/GetPermissionGroups |  |

<a id="tenantmanagementpermissiongroupgetpermissiongroups"></a>
# **TenantManagementPermissionGroupGetPermissionGroups**
> List&lt;TenantManagementPermissionGroupCategory&gt; TenantManagementPermissionGroupGetPermissionGroups ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementPermissionGroupGetPermissionGroupsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PermissionGroupApi(config);

            try
            {
                List<TenantManagementPermissionGroupCategory> result = apiInstance.TenantManagementPermissionGroupGetPermissionGroups();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PermissionGroupApi.TenantManagementPermissionGroupGetPermissionGroups: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementPermissionGroupGetPermissionGroupsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementPermissionGroupCategory>> response = apiInstance.TenantManagementPermissionGroupGetPermissionGroupsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PermissionGroupApi.TenantManagementPermissionGroupGetPermissionGroupsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementPermissionGroupCategory&gt;**](TenantManagementPermissionGroupCategory.md)

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

