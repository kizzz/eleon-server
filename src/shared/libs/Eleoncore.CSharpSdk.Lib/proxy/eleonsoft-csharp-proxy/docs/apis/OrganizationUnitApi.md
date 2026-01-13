# EleonsoftProxy.Api.OrganizationUnitApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CoreOrganizationUnitAddMember**](OrganizationUnitApi.md#coreorganizationunitaddmember) | **POST** /api/Infrastructure/OrganizationUnit/AddMember |  |
| [**CoreOrganizationUnitAddMembers**](OrganizationUnitApi.md#coreorganizationunitaddmembers) | **POST** /api/Infrastructure/OrganizationUnit/AddMembers |  |
| [**CoreOrganizationUnitAddRole**](OrganizationUnitApi.md#coreorganizationunitaddrole) | **POST** /api/Infrastructure/OrganizationUnit/AddRole |  |
| [**CoreOrganizationUnitAddRoles**](OrganizationUnitApi.md#coreorganizationunitaddroles) | **POST** /api/Infrastructure/OrganizationUnit/AddRoles |  |
| [**CoreOrganizationUnitCheckOrganizationUnitParentsPermission**](OrganizationUnitApi.md#coreorganizationunitcheckorganizationunitparentspermission) | **GET** /api/Infrastructure/OrganizationUnit/CheckOrganizationUnitParentsPermission |  |
| [**CoreOrganizationUnitCheckOrganizationUnitPermission**](OrganizationUnitApi.md#coreorganizationunitcheckorganizationunitpermission) | **GET** /api/Infrastructure/OrganizationUnit/CheckOrganizationUnitPermission |  |
| [**CoreOrganizationUnitClone**](OrganizationUnitApi.md#coreorganizationunitclone) | **POST** /api/Infrastructure/OrganizationUnit/CloneAsync |  |
| [**CoreOrganizationUnitCreate**](OrganizationUnitApi.md#coreorganizationunitcreate) | **POST** /api/Infrastructure/OrganizationUnit/CreateAsync |  |
| [**CoreOrganizationUnitDelete**](OrganizationUnitApi.md#coreorganizationunitdelete) | **DELETE** /api/Infrastructure/OrganizationUnit/DeleteAsync |  |
| [**CoreOrganizationUnitGetAllUnitAndChildsMembers**](OrganizationUnitApi.md#coreorganizationunitgetallunitandchildsmembers) | **POST** /api/Infrastructure/OrganizationUnit/GetAllUnitAndChildsMembers |  |
| [**CoreOrganizationUnitGetAvailableForUser**](OrganizationUnitApi.md#coreorganizationunitgetavailableforuser) | **GET** /api/Infrastructure/OrganizationUnit/GetAvailableForUser |  |
| [**CoreOrganizationUnitGetById**](OrganizationUnitApi.md#coreorganizationunitgetbyid) | **GET** /api/Infrastructure/OrganizationUnit/GetById |  |
| [**CoreOrganizationUnitGetDocumentSeriaNumber**](OrganizationUnitApi.md#coreorganizationunitgetdocumentserianumber) | **GET** /api/Infrastructure/OrganizationUnit/GetDocumentSeriaNumber |  |
| [**CoreOrganizationUnitGetHighLevelOrgUnitList**](OrganizationUnitApi.md#coreorganizationunitgethighlevelorgunitlist) | **GET** /api/Infrastructure/OrganizationUnit/GetHighLevelOrgUnitListAsync |  |
| [**CoreOrganizationUnitGetList**](OrganizationUnitApi.md#coreorganizationunitgetlist) | **GET** /api/Infrastructure/OrganizationUnit/GetListAsync |  |
| [**CoreOrganizationUnitGetMembers**](OrganizationUnitApi.md#coreorganizationunitgetmembers) | **GET** /api/Infrastructure/OrganizationUnit/GetMembers |  |
| [**CoreOrganizationUnitGetRoleOrganizationUnits**](OrganizationUnitApi.md#coreorganizationunitgetroleorganizationunits) | **GET** /api/Infrastructure/OrganizationUnit/GetRoleOrganizationUnits |  |
| [**CoreOrganizationUnitGetRoles**](OrganizationUnitApi.md#coreorganizationunitgetroles) | **GET** /api/Infrastructure/OrganizationUnit/GetRoles |  |
| [**CoreOrganizationUnitGetUserOrganizationUnits**](OrganizationUnitApi.md#coreorganizationunitgetuserorganizationunits) | **GET** /api/Infrastructure/OrganizationUnit/GetUserOrganizationUnits |  |
| [**CoreOrganizationUnitMove**](OrganizationUnitApi.md#coreorganizationunitmove) | **POST** /api/Infrastructure/OrganizationUnit/MoveAsync |  |
| [**CoreOrganizationUnitRemoveMember**](OrganizationUnitApi.md#coreorganizationunitremovemember) | **DELETE** /api/Infrastructure/OrganizationUnit/RemoveMember |  |
| [**CoreOrganizationUnitRemoveRole**](OrganizationUnitApi.md#coreorganizationunitremoverole) | **DELETE** /api/Infrastructure/OrganizationUnit/RemoveRole |  |
| [**CoreOrganizationUnitSetRoleOrganizationUnits**](OrganizationUnitApi.md#coreorganizationunitsetroleorganizationunits) | **POST** /api/Infrastructure/OrganizationUnit/SetRoleOrganizationUnits |  |
| [**CoreOrganizationUnitSetUserOrganizationUnits**](OrganizationUnitApi.md#coreorganizationunitsetuserorganizationunits) | **POST** /api/Infrastructure/OrganizationUnit/SetUserOrganizationUnits |  |
| [**CoreOrganizationUnitUpdate**](OrganizationUnitApi.md#coreorganizationunitupdate) | **POST** /api/Infrastructure/OrganizationUnit/UpdateAsync |  |

<a id="coreorganizationunitaddmember"></a>
# **CoreOrganizationUnitAddMember**
> void CoreOrganizationUnitAddMember (Guid userId = null, Guid orgUnitId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitAddMemberExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var userId = "userId_example";  // Guid |  (optional) 
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitAddMember(userId, orgUnitId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddMember: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitAddMemberWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitAddMemberWithHttpInfo(userId, orgUnitId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddMemberWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |
| **orgUnitId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitaddmembers"></a>
# **CoreOrganizationUnitAddMembers**
> void CoreOrganizationUnitAddMembers (Guid orgUnitId = null, List<Guid> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitAddMembersExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 
            var requestBody = new List<Guid>(); // List<Guid> |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitAddMembers(orgUnitId, requestBody);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddMembers: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitAddMembersWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitAddMembersWithHttpInfo(orgUnitId, requestBody);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddMembersWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **orgUnitId** | **Guid** |  | [optional]  |
| **requestBody** | [**List&lt;Guid&gt;**](Guid.md) |  | [optional]  |

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

<a id="coreorganizationunitaddrole"></a>
# **CoreOrganizationUnitAddRole**
> void CoreOrganizationUnitAddRole (Guid roleId = null, Guid orgUnitId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitAddRoleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var roleId = "roleId_example";  // Guid |  (optional) 
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitAddRole(roleId, orgUnitId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddRole: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitAddRoleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitAddRoleWithHttpInfo(roleId, orgUnitId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddRoleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **roleId** | **Guid** |  | [optional]  |
| **orgUnitId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitaddroles"></a>
# **CoreOrganizationUnitAddRoles**
> void CoreOrganizationUnitAddRoles (Guid orgUnitId = null, List<Guid> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitAddRolesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 
            var requestBody = new List<Guid>(); // List<Guid> |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitAddRoles(orgUnitId, requestBody);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddRoles: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitAddRolesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitAddRolesWithHttpInfo(orgUnitId, requestBody);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitAddRolesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **orgUnitId** | **Guid** |  | [optional]  |
| **requestBody** | [**List&lt;Guid&gt;**](Guid.md) |  | [optional]  |

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

<a id="coreorganizationunitcheckorganizationunitparentspermission"></a>
# **CoreOrganizationUnitCheckOrganizationUnitParentsPermission**
> bool CoreOrganizationUnitCheckOrganizationUnitParentsPermission (Guid orgUnitId = null, string permission = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitCheckOrganizationUnitParentsPermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 
            var permission = "permission_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.CoreOrganizationUnitCheckOrganizationUnitParentsPermission(orgUnitId, permission);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCheckOrganizationUnitParentsPermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitCheckOrganizationUnitParentsPermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreOrganizationUnitCheckOrganizationUnitParentsPermissionWithHttpInfo(orgUnitId, permission);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCheckOrganizationUnitParentsPermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **orgUnitId** | **Guid** |  | [optional]  |
| **permission** | **string** |  | [optional]  |

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

<a id="coreorganizationunitcheckorganizationunitpermission"></a>
# **CoreOrganizationUnitCheckOrganizationUnitPermission**
> bool CoreOrganizationUnitCheckOrganizationUnitPermission (Guid orgUnitId = null, string permission = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitCheckOrganizationUnitPermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 
            var permission = "permission_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.CoreOrganizationUnitCheckOrganizationUnitPermission(orgUnitId, permission);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCheckOrganizationUnitPermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitCheckOrganizationUnitPermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreOrganizationUnitCheckOrganizationUnitPermissionWithHttpInfo(orgUnitId, permission);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCheckOrganizationUnitPermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **orgUnitId** | **Guid** |  | [optional]  |
| **permission** | **string** |  | [optional]  |

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

<a id="coreorganizationunitclone"></a>
# **CoreOrganizationUnitClone**
> TenantManagementCommonOrganizationUnitTreeNodeDto CoreOrganizationUnitClone (Guid id = null, string newName = null, bool withRoles = null, bool withMembers = null, bool withChildren = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitCloneExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var newName = "newName_example";  // string |  (optional) 
            var withRoles = true;  // bool |  (optional) 
            var withMembers = true;  // bool |  (optional) 
            var withChildren = true;  // bool |  (optional) 

            try
            {
                TenantManagementCommonOrganizationUnitTreeNodeDto result = apiInstance.CoreOrganizationUnitClone(id, newName, withRoles, withMembers, withChildren);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitClone: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitCloneWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementCommonOrganizationUnitTreeNodeDto> response = apiInstance.CoreOrganizationUnitCloneWithHttpInfo(id, newName, withRoles, withMembers, withChildren);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCloneWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **newName** | **string** |  | [optional]  |
| **withRoles** | **bool** |  | [optional]  |
| **withMembers** | **bool** |  | [optional]  |
| **withChildren** | **bool** |  | [optional]  |

### Return type

[**TenantManagementCommonOrganizationUnitTreeNodeDto**](TenantManagementCommonOrganizationUnitTreeNodeDto.md)

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

<a id="coreorganizationunitcreate"></a>
# **CoreOrganizationUnitCreate**
> TenantManagementCommonOrganizationUnitDto CoreOrganizationUnitCreate (TenantManagementCommonOrganizationUnitDto tenantManagementCommonOrganizationUnitDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitCreateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var tenantManagementCommonOrganizationUnitDto = new TenantManagementCommonOrganizationUnitDto(); // TenantManagementCommonOrganizationUnitDto |  (optional) 

            try
            {
                TenantManagementCommonOrganizationUnitDto result = apiInstance.CoreOrganizationUnitCreate(tenantManagementCommonOrganizationUnitDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCreate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitCreateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementCommonOrganizationUnitDto> response = apiInstance.CoreOrganizationUnitCreateWithHttpInfo(tenantManagementCommonOrganizationUnitDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitCreateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementCommonOrganizationUnitDto** | [**TenantManagementCommonOrganizationUnitDto**](TenantManagementCommonOrganizationUnitDto.md) |  | [optional]  |

### Return type

[**TenantManagementCommonOrganizationUnitDto**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitdelete"></a>
# **CoreOrganizationUnitDelete**
> void CoreOrganizationUnitDelete (Guid orgUnitId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitDeleteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitDelete(orgUnitId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitDelete: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitDeleteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitDeleteWithHttpInfo(orgUnitId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitDeleteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **orgUnitId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitgetallunitandchildsmembers"></a>
# **CoreOrganizationUnitGetAllUnitAndChildsMembers**
> EleoncorePagedResultDtoOfTenantManagementCommonUserDto CoreOrganizationUnitGetAllUnitAndChildsMembers (TenantManagementGetAllUnitAndChildsMembersRequestDto tenantManagementGetAllUnitAndChildsMembersRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetAllUnitAndChildsMembersExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var tenantManagementGetAllUnitAndChildsMembersRequestDto = new TenantManagementGetAllUnitAndChildsMembersRequestDto(); // TenantManagementGetAllUnitAndChildsMembersRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfTenantManagementCommonUserDto result = apiInstance.CoreOrganizationUnitGetAllUnitAndChildsMembers(tenantManagementGetAllUnitAndChildsMembersRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetAllUnitAndChildsMembers: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetAllUnitAndChildsMembersWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfTenantManagementCommonUserDto> response = apiInstance.CoreOrganizationUnitGetAllUnitAndChildsMembersWithHttpInfo(tenantManagementGetAllUnitAndChildsMembersRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetAllUnitAndChildsMembersWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementGetAllUnitAndChildsMembersRequestDto** | [**TenantManagementGetAllUnitAndChildsMembersRequestDto**](TenantManagementGetAllUnitAndChildsMembersRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfTenantManagementCommonUserDto**](EleoncorePagedResultDtoOfTenantManagementCommonUserDto.md)

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

<a id="coreorganizationunitgetavailableforuser"></a>
# **CoreOrganizationUnitGetAvailableForUser**
> List&lt;TenantManagementUserOrganizationUnitLookupDto&gt; CoreOrganizationUnitGetAvailableForUser (Guid userId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetAvailableForUserExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var userId = "userId_example";  // Guid |  (optional) 

            try
            {
                List<TenantManagementUserOrganizationUnitLookupDto> result = apiInstance.CoreOrganizationUnitGetAvailableForUser(userId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetAvailableForUser: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetAvailableForUserWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementUserOrganizationUnitLookupDto>> response = apiInstance.CoreOrganizationUnitGetAvailableForUserWithHttpInfo(userId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetAvailableForUserWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;TenantManagementUserOrganizationUnitLookupDto&gt;**](TenantManagementUserOrganizationUnitLookupDto.md)

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

<a id="coreorganizationunitgetbyid"></a>
# **CoreOrganizationUnitGetById**
> TenantManagementCommonOrganizationUnitDto CoreOrganizationUnitGetById (Guid id = null, bool includeSoftDeleted = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var includeSoftDeleted = false;  // bool |  (optional)  (default to false)

            try
            {
                TenantManagementCommonOrganizationUnitDto result = apiInstance.CoreOrganizationUnitGetById(id, includeSoftDeleted);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementCommonOrganizationUnitDto> response = apiInstance.CoreOrganizationUnitGetByIdWithHttpInfo(id, includeSoftDeleted);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **includeSoftDeleted** | **bool** |  | [optional] [default to false] |

### Return type

[**TenantManagementCommonOrganizationUnitDto**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitgetdocumentserianumber"></a>
# **CoreOrganizationUnitGetDocumentSeriaNumber**
> string CoreOrganizationUnitGetDocumentSeriaNumber (string documentObjectType = null, string prefix = null, string refId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetDocumentSeriaNumberExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var prefix = "prefix_example";  // string |  (optional) 
            var refId = "refId_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.CoreOrganizationUnitGetDocumentSeriaNumber(documentObjectType, prefix, refId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetDocumentSeriaNumber: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetDocumentSeriaNumberWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CoreOrganizationUnitGetDocumentSeriaNumberWithHttpInfo(documentObjectType, prefix, refId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetDocumentSeriaNumberWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **prefix** | **string** |  | [optional]  |
| **refId** | **string** |  | [optional]  |

### Return type

**string**

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

<a id="coreorganizationunitgethighlevelorgunitlist"></a>
# **CoreOrganizationUnitGetHighLevelOrgUnitList**
> List&lt;TenantManagementCommonOrganizationUnitDto&gt; CoreOrganizationUnitGetHighLevelOrgUnitList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetHighLevelOrgUnitListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);

            try
            {
                List<TenantManagementCommonOrganizationUnitDto> result = apiInstance.CoreOrganizationUnitGetHighLevelOrgUnitList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetHighLevelOrgUnitList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetHighLevelOrgUnitListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonOrganizationUnitDto>> response = apiInstance.CoreOrganizationUnitGetHighLevelOrgUnitListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetHighLevelOrgUnitListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementCommonOrganizationUnitDto&gt;**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitgetlist"></a>
# **CoreOrganizationUnitGetList**
> List&lt;TenantManagementCommonOrganizationUnitDto&gt; CoreOrganizationUnitGetList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);

            try
            {
                List<TenantManagementCommonOrganizationUnitDto> result = apiInstance.CoreOrganizationUnitGetList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonOrganizationUnitDto>> response = apiInstance.CoreOrganizationUnitGetListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementCommonOrganizationUnitDto&gt;**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitgetmembers"></a>
# **CoreOrganizationUnitGetMembers**
> List&lt;TenantManagementCommonUserDto&gt; CoreOrganizationUnitGetMembers (Guid id = null, Guid parentId = null, string code = null, string displayName = null, Guid tenantId = null, bool isEnabled = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetMembersExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var parentId = "parentId_example";  // Guid |  (optional) 
            var code = "code_example";  // string |  (optional) 
            var displayName = "displayName_example";  // string |  (optional) 
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var isEnabled = true;  // bool |  (optional) 

            try
            {
                List<TenantManagementCommonUserDto> result = apiInstance.CoreOrganizationUnitGetMembers(id, parentId, code, displayName, tenantId, isEnabled);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetMembers: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetMembersWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonUserDto>> response = apiInstance.CoreOrganizationUnitGetMembersWithHttpInfo(id, parentId, code, displayName, tenantId, isEnabled);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetMembersWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **parentId** | **Guid** |  | [optional]  |
| **code** | **string** |  | [optional]  |
| **displayName** | **string** |  | [optional]  |
| **tenantId** | **Guid** |  | [optional]  |
| **isEnabled** | **bool** |  | [optional]  |

### Return type

[**List&lt;TenantManagementCommonUserDto&gt;**](TenantManagementCommonUserDto.md)

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

<a id="coreorganizationunitgetroleorganizationunits"></a>
# **CoreOrganizationUnitGetRoleOrganizationUnits**
> List&lt;TenantManagementCommonOrganizationUnitDto&gt; CoreOrganizationUnitGetRoleOrganizationUnits (string roleName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetRoleOrganizationUnitsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var roleName = "roleName_example";  // string |  (optional) 

            try
            {
                List<TenantManagementCommonOrganizationUnitDto> result = apiInstance.CoreOrganizationUnitGetRoleOrganizationUnits(roleName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetRoleOrganizationUnits: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetRoleOrganizationUnitsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonOrganizationUnitDto>> response = apiInstance.CoreOrganizationUnitGetRoleOrganizationUnitsWithHttpInfo(roleName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetRoleOrganizationUnitsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **roleName** | **string** |  | [optional]  |

### Return type

[**List&lt;TenantManagementCommonOrganizationUnitDto&gt;**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitgetroles"></a>
# **CoreOrganizationUnitGetRoles**
> List&lt;TenantManagementCommonRoleDto&gt; CoreOrganizationUnitGetRoles (Guid id = null, Guid parentId = null, string code = null, string displayName = null, Guid tenantId = null, bool isEnabled = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetRolesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var parentId = "parentId_example";  // Guid |  (optional) 
            var code = "code_example";  // string |  (optional) 
            var displayName = "displayName_example";  // string |  (optional) 
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var isEnabled = true;  // bool |  (optional) 

            try
            {
                List<TenantManagementCommonRoleDto> result = apiInstance.CoreOrganizationUnitGetRoles(id, parentId, code, displayName, tenantId, isEnabled);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetRoles: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetRolesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonRoleDto>> response = apiInstance.CoreOrganizationUnitGetRolesWithHttpInfo(id, parentId, code, displayName, tenantId, isEnabled);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetRolesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **parentId** | **Guid** |  | [optional]  |
| **code** | **string** |  | [optional]  |
| **displayName** | **string** |  | [optional]  |
| **tenantId** | **Guid** |  | [optional]  |
| **isEnabled** | **bool** |  | [optional]  |

### Return type

[**List&lt;TenantManagementCommonRoleDto&gt;**](TenantManagementCommonRoleDto.md)

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

<a id="coreorganizationunitgetuserorganizationunits"></a>
# **CoreOrganizationUnitGetUserOrganizationUnits**
> List&lt;TenantManagementCommonOrganizationUnitDto&gt; CoreOrganizationUnitGetUserOrganizationUnits (Guid userId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitGetUserOrganizationUnitsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var userId = "userId_example";  // Guid |  (optional) 

            try
            {
                List<TenantManagementCommonOrganizationUnitDto> result = apiInstance.CoreOrganizationUnitGetUserOrganizationUnits(userId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetUserOrganizationUnits: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitGetUserOrganizationUnitsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementCommonOrganizationUnitDto>> response = apiInstance.CoreOrganizationUnitGetUserOrganizationUnitsWithHttpInfo(userId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitGetUserOrganizationUnitsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;TenantManagementCommonOrganizationUnitDto&gt;**](TenantManagementCommonOrganizationUnitDto.md)

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

<a id="coreorganizationunitmove"></a>
# **CoreOrganizationUnitMove**
> bool CoreOrganizationUnitMove (Guid id = null, Guid newParentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitMoveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var newParentId = "newParentId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.CoreOrganizationUnitMove(id, newParentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitMove: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitMoveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreOrganizationUnitMoveWithHttpInfo(id, newParentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitMoveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **newParentId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitremovemember"></a>
# **CoreOrganizationUnitRemoveMember**
> void CoreOrganizationUnitRemoveMember (Guid userId = null, Guid orgUnitId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitRemoveMemberExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var userId = "userId_example";  // Guid |  (optional) 
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitRemoveMember(userId, orgUnitId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitRemoveMember: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitRemoveMemberWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitRemoveMemberWithHttpInfo(userId, orgUnitId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitRemoveMemberWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **userId** | **Guid** |  | [optional]  |
| **orgUnitId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitremoverole"></a>
# **CoreOrganizationUnitRemoveRole**
> void CoreOrganizationUnitRemoveRole (Guid roleId = null, Guid orgUnitId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitRemoveRoleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var roleId = "roleId_example";  // Guid |  (optional) 
            var orgUnitId = "orgUnitId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CoreOrganizationUnitRemoveRole(roleId, orgUnitId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitRemoveRole: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitRemoveRoleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CoreOrganizationUnitRemoveRoleWithHttpInfo(roleId, orgUnitId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitRemoveRoleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **roleId** | **Guid** |  | [optional]  |
| **orgUnitId** | **Guid** |  | [optional]  |

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

<a id="coreorganizationunitsetroleorganizationunits"></a>
# **CoreOrganizationUnitSetRoleOrganizationUnits**
> bool CoreOrganizationUnitSetRoleOrganizationUnits (TenantManagementSetRoleOrganizationUnitsInput tenantManagementSetRoleOrganizationUnitsInput = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitSetRoleOrganizationUnitsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var tenantManagementSetRoleOrganizationUnitsInput = new TenantManagementSetRoleOrganizationUnitsInput(); // TenantManagementSetRoleOrganizationUnitsInput |  (optional) 

            try
            {
                bool result = apiInstance.CoreOrganizationUnitSetRoleOrganizationUnits(tenantManagementSetRoleOrganizationUnitsInput);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitSetRoleOrganizationUnits: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitSetRoleOrganizationUnitsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreOrganizationUnitSetRoleOrganizationUnitsWithHttpInfo(tenantManagementSetRoleOrganizationUnitsInput);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitSetRoleOrganizationUnitsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetRoleOrganizationUnitsInput** | [**TenantManagementSetRoleOrganizationUnitsInput**](TenantManagementSetRoleOrganizationUnitsInput.md) |  | [optional]  |

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

<a id="coreorganizationunitsetuserorganizationunits"></a>
# **CoreOrganizationUnitSetUserOrganizationUnits**
> bool CoreOrganizationUnitSetUserOrganizationUnits (TenantManagementSetUserOrganizationUnitsInput tenantManagementSetUserOrganizationUnitsInput = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitSetUserOrganizationUnitsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var tenantManagementSetUserOrganizationUnitsInput = new TenantManagementSetUserOrganizationUnitsInput(); // TenantManagementSetUserOrganizationUnitsInput |  (optional) 

            try
            {
                bool result = apiInstance.CoreOrganizationUnitSetUserOrganizationUnits(tenantManagementSetUserOrganizationUnitsInput);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitSetUserOrganizationUnits: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitSetUserOrganizationUnitsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CoreOrganizationUnitSetUserOrganizationUnitsWithHttpInfo(tenantManagementSetUserOrganizationUnitsInput);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitSetUserOrganizationUnitsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementSetUserOrganizationUnitsInput** | [**TenantManagementSetUserOrganizationUnitsInput**](TenantManagementSetUserOrganizationUnitsInput.md) |  | [optional]  |

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

<a id="coreorganizationunitupdate"></a>
# **CoreOrganizationUnitUpdate**
> TenantManagementCommonOrganizationUnitDto CoreOrganizationUnitUpdate (TenantManagementCommonOrganizationUnitDto tenantManagementCommonOrganizationUnitDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CoreOrganizationUnitUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new OrganizationUnitApi(config);
            var tenantManagementCommonOrganizationUnitDto = new TenantManagementCommonOrganizationUnitDto(); // TenantManagementCommonOrganizationUnitDto |  (optional) 

            try
            {
                TenantManagementCommonOrganizationUnitDto result = apiInstance.CoreOrganizationUnitUpdate(tenantManagementCommonOrganizationUnitDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CoreOrganizationUnitUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementCommonOrganizationUnitDto> response = apiInstance.CoreOrganizationUnitUpdateWithHttpInfo(tenantManagementCommonOrganizationUnitDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling OrganizationUnitApi.CoreOrganizationUnitUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementCommonOrganizationUnitDto** | [**TenantManagementCommonOrganizationUnitDto**](TenantManagementCommonOrganizationUnitDto.md) |  | [optional]  |

### Return type

[**TenantManagementCommonOrganizationUnitDto**](TenantManagementCommonOrganizationUnitDto.md)

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

