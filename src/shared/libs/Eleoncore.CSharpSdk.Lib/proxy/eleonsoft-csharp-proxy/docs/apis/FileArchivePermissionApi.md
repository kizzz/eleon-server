# EleonsoftProxy.Api.FileArchivePermissionApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**FileManagerFileArchivePermissionDeletePermissions**](FileArchivePermissionApi.md#filemanagerfilearchivepermissiondeletepermissions) | **POST** /api/file-manager/file-archives-permissions/DeletePermissions |  |
| [**FileManagerFileArchivePermissionGetList**](FileArchivePermissionApi.md#filemanagerfilearchivepermissiongetlist) | **GET** /api/file-manager/file-archives-permissions/GetList |  |
| [**FileManagerFileArchivePermissionGetPermissionOrDefault**](FileArchivePermissionApi.md#filemanagerfilearchivepermissiongetpermissionordefault) | **GET** /api/file-manager/file-archives-permissions/GetPermissionOrDefault |  |
| [**FileManagerFileArchivePermissionGetPermissionWithoutDefault**](FileArchivePermissionApi.md#filemanagerfilearchivepermissiongetpermissionwithoutdefault) | **GET** /api/file-manager/file-archives-permissions/GetPermissionWithoutDefault |  |
| [**FileManagerFileArchivePermissionUpdatePermission**](FileArchivePermissionApi.md#filemanagerfilearchivepermissionupdatepermission) | **PUT** /api/file-manager/file-archives-permissions/UpdatePermission |  |

<a id="filemanagerfilearchivepermissiondeletepermissions"></a>
# **FileManagerFileArchivePermissionDeletePermissions**
> bool FileManagerFileArchivePermissionDeletePermissions (FileManagerFileArchivePermissionDto fileManagerFileArchivePermissionDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchivePermissionDeletePermissionsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchivePermissionApi(config);
            var fileManagerFileArchivePermissionDto = new FileManagerFileArchivePermissionDto(); // FileManagerFileArchivePermissionDto |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileArchivePermissionDeletePermissions(fileManagerFileArchivePermissionDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionDeletePermissions: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchivePermissionDeletePermissionsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileArchivePermissionDeletePermissionsWithHttpInfo(fileManagerFileArchivePermissionDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionDeletePermissionsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchivePermissionDto** | [**FileManagerFileArchivePermissionDto**](FileManagerFileArchivePermissionDto.md) |  | [optional]  |

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

<a id="filemanagerfilearchivepermissiongetlist"></a>
# **FileManagerFileArchivePermissionGetList**
> List&lt;FileManagerFileArchivePermissionDto&gt; FileManagerFileArchivePermissionGetList (Guid archiveId = null, string folderId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchivePermissionGetListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchivePermissionApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var folderId = "folderId_example";  // string |  (optional) 

            try
            {
                List<FileManagerFileArchivePermissionDto> result = apiInstance.FileManagerFileArchivePermissionGetList(archiveId, folderId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchivePermissionGetListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileArchivePermissionDto>> response = apiInstance.FileManagerFileArchivePermissionGetListWithHttpInfo(archiveId, folderId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **folderId** | **string** |  | [optional]  |

### Return type

[**List&lt;FileManagerFileArchivePermissionDto&gt;**](FileManagerFileArchivePermissionDto.md)

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

<a id="filemanagerfilearchivepermissiongetpermissionordefault"></a>
# **FileManagerFileArchivePermissionGetPermissionOrDefault**
> List&lt;EleoncoreFileManagerPermissionType&gt; FileManagerFileArchivePermissionGetPermissionOrDefault (Guid archiveId = null, string folderId = null, ModuleCollectorPermissionActorType actorType = null, string actorId = null, string uniqueKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchivePermissionGetPermissionOrDefaultExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchivePermissionApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var folderId = "folderId_example";  // string |  (optional) 
            var actorType = (ModuleCollectorPermissionActorType) "0";  // ModuleCollectorPermissionActorType |  (optional) 
            var actorId = "actorId_example";  // string |  (optional) 
            var uniqueKey = "uniqueKey_example";  // string |  (optional) 

            try
            {
                List<EleoncoreFileManagerPermissionType> result = apiInstance.FileManagerFileArchivePermissionGetPermissionOrDefault(archiveId, folderId, actorType, actorId, uniqueKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetPermissionOrDefault: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchivePermissionGetPermissionOrDefaultWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<EleoncoreFileManagerPermissionType>> response = apiInstance.FileManagerFileArchivePermissionGetPermissionOrDefaultWithHttpInfo(archiveId, folderId, actorType, actorId, uniqueKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetPermissionOrDefaultWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **folderId** | **string** |  | [optional]  |
| **actorType** | **ModuleCollectorPermissionActorType** |  | [optional]  |
| **actorId** | **string** |  | [optional]  |
| **uniqueKey** | **string** |  | [optional]  |

### Return type

[**List&lt;EleoncoreFileManagerPermissionType&gt;**](EleoncoreFileManagerPermissionType.md)

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

<a id="filemanagerfilearchivepermissiongetpermissionwithoutdefault"></a>
# **FileManagerFileArchivePermissionGetPermissionWithoutDefault**
> FileManagerFileArchivePermissionDto FileManagerFileArchivePermissionGetPermissionWithoutDefault (Guid archiveId = null, string folderId = null, ModuleCollectorPermissionActorType actorType = null, string actorId = null, string uniqueKey = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchivePermissionGetPermissionWithoutDefaultExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchivePermissionApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var folderId = "folderId_example";  // string |  (optional) 
            var actorType = (ModuleCollectorPermissionActorType) "0";  // ModuleCollectorPermissionActorType |  (optional) 
            var actorId = "actorId_example";  // string |  (optional) 
            var uniqueKey = "uniqueKey_example";  // string |  (optional) 

            try
            {
                FileManagerFileArchivePermissionDto result = apiInstance.FileManagerFileArchivePermissionGetPermissionWithoutDefault(archiveId, folderId, actorType, actorId, uniqueKey);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetPermissionWithoutDefault: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchivePermissionGetPermissionWithoutDefaultWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileArchivePermissionDto> response = apiInstance.FileManagerFileArchivePermissionGetPermissionWithoutDefaultWithHttpInfo(archiveId, folderId, actorType, actorId, uniqueKey);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionGetPermissionWithoutDefaultWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **folderId** | **string** |  | [optional]  |
| **actorType** | **ModuleCollectorPermissionActorType** |  | [optional]  |
| **actorId** | **string** |  | [optional]  |
| **uniqueKey** | **string** |  | [optional]  |

### Return type

[**FileManagerFileArchivePermissionDto**](FileManagerFileArchivePermissionDto.md)

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

<a id="filemanagerfilearchivepermissionupdatepermission"></a>
# **FileManagerFileArchivePermissionUpdatePermission**
> FileManagerFileArchivePermissionDto FileManagerFileArchivePermissionUpdatePermission (FileManagerFileArchivePermissionDto fileManagerFileArchivePermissionDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchivePermissionUpdatePermissionExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchivePermissionApi(config);
            var fileManagerFileArchivePermissionDto = new FileManagerFileArchivePermissionDto(); // FileManagerFileArchivePermissionDto |  (optional) 

            try
            {
                FileManagerFileArchivePermissionDto result = apiInstance.FileManagerFileArchivePermissionUpdatePermission(fileManagerFileArchivePermissionDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionUpdatePermission: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchivePermissionUpdatePermissionWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileArchivePermissionDto> response = apiInstance.FileManagerFileArchivePermissionUpdatePermissionWithHttpInfo(fileManagerFileArchivePermissionDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchivePermissionApi.FileManagerFileArchivePermissionUpdatePermissionWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchivePermissionDto** | [**FileManagerFileArchivePermissionDto**](FileManagerFileArchivePermissionDto.md) |  | [optional]  |

### Return type

[**FileManagerFileArchivePermissionDto**](FileManagerFileArchivePermissionDto.md)

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

