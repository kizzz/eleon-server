# EleonsoftProxy.Api.FileApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**FileManagerFileCopyEntry**](FileApi.md#filemanagerfilecopyentry) | **POST** /api/file-manager/files/CopyEntry |  |
| [**FileManagerFileCreateEntry**](FileApi.md#filemanagerfilecreateentry) | **POST** /api/file-manager/files/CreateEntry |  |
| [**FileManagerFileDeleteEntry**](FileApi.md#filemanagerfiledeleteentry) | **DELETE** /api/file-manager/files/DeleteEntry |  |
| [**FileManagerFileDeleteFileToken**](FileApi.md#filemanagerfiledeletefiletoken) | **DELETE** /api/file-manager/files/DeleteFileToken |  |
| [**FileManagerFileDownloadAll**](FileApi.md#filemanagerfiledownloadall) | **POST** /api/file-manager/files/DownloadAll |  |
| [**FileManagerFileDownloadFile**](FileApi.md#filemanagerfiledownloadfile) | **GET** /api/file-manager/files/DownloadFile |  |
| [**FileManagerFileDownloadFileByToken**](FileApi.md#filemanagerfiledownloadfilebytoken) | **GET** /api/file-manager/files/DownloadFileByToken |  |
| [**FileManagerFileFileViewer**](FileApi.md#filemanagerfilefileviewer) | **GET** /api/file-manager/files/FileViewer |  |
| [**FileManagerFileGetEntries**](FileApi.md#filemanagerfilegetentries) | **POST** /api/file-manager/files/GetEntries |  |
| [**FileManagerFileGetEntriesByIds**](FileApi.md#filemanagerfilegetentriesbyids) | **POST** /api/file-manager/files/GetEntriesByIds |  |
| [**FileManagerFileGetEntriesByParentId**](FileApi.md#filemanagerfilegetentriesbyparentid) | **GET** /api/file-manager/files/GetEntriesByParentId |  |
| [**FileManagerFileGetEntriesByParentIdPaged**](FileApi.md#filemanagerfilegetentriesbyparentidpaged) | **GET** /api/file-manager/files/GetEntriesByParentIdPaged |  |
| [**FileManagerFileGetEntryById**](FileApi.md#filemanagerfilegetentrybyid) | **GET** /api/file-manager/files/GetEntryById |  |
| [**FileManagerFileGetEntryHistory**](FileApi.md#filemanagerfilegetentryhistory) | **GET** /api/file-manager/files/GetEntryHistory |  |
| [**FileManagerFileGetEntryParentsById**](FileApi.md#filemanagerfilegetentryparentsbyid) | **GET** /api/file-manager/files/GetEntryParentsById |  |
| [**FileManagerFileGetFileByToken**](FileApi.md#filemanagerfilegetfilebytoken) | **GET** /api/file-manager/files/GetFileByToken |  |
| [**FileManagerFileGetFileToken**](FileApi.md#filemanagerfilegetfiletoken) | **GET** /api/file-manager/files/GetFileToken |  |
| [**FileManagerFileGetRootEntry**](FileApi.md#filemanagerfilegetrootentry) | **GET** /api/file-manager/files/GetRootEntry |  |
| [**FileManagerFileMoveAllEntries**](FileApi.md#filemanagerfilemoveallentries) | **POST** /api/file-manager/files/MoveAllEntries |  |
| [**FileManagerFileMoveEntry**](FileApi.md#filemanagerfilemoveentry) | **POST** /api/file-manager/files/MoveEntry |  |
| [**FileManagerFileReadTextFile**](FileApi.md#filemanagerfilereadtextfile) | **GET** /api/file-manager/files/ReadTextFile |  |
| [**FileManagerFileReadTextFileByToken**](FileApi.md#filemanagerfilereadtextfilebytoken) | **GET** /api/file-manager/files/ReadTextFileByToken |  |
| [**FileManagerFileRenameEntry**](FileApi.md#filemanagerfilerenameentry) | **PUT** /api/file-manager/files/RenameEntry |  |
| [**FileManagerFileRestoreEntry**](FileApi.md#filemanagerfilerestoreentry) | **POST** /api/file-manager/files/RestoreEntry |  |
| [**FileManagerFileSearchEntries**](FileApi.md#filemanagerfilesearchentries) | **GET** /api/file-manager/files/SearchEntries |  |
| [**FileManagerFileUploadFiles**](FileApi.md#filemanagerfileuploadfiles) | **POST** /api/file-manager/files/UploadFiles |  |

<a id="filemanagerfilecopyentry"></a>
# **FileManagerFileCopyEntry**
> bool FileManagerFileCopyEntry (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null, FileManagerCopyEntryDto fileManagerCopyEntryDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileCopyEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var fileManagerCopyEntryDto = new FileManagerCopyEntryDto(); // FileManagerCopyEntryDto |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileCopyEntry(archiveId, type, fileManagerCopyEntryDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileCopyEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileCopyEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileCopyEntryWithHttpInfo(archiveId, type, fileManagerCopyEntryDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileCopyEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **fileManagerCopyEntryDto** | [**FileManagerCopyEntryDto**](FileManagerCopyEntryDto.md) |  | [optional]  |

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

<a id="filemanagerfilecreateentry"></a>
# **FileManagerFileCreateEntry**
> FileManagerFileSystemEntryDto FileManagerFileCreateEntry (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null, FileManagerCreateEntryDto fileManagerCreateEntryDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileCreateEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var fileManagerCreateEntryDto = new FileManagerCreateEntryDto(); // FileManagerCreateEntryDto |  (optional) 

            try
            {
                FileManagerFileSystemEntryDto result = apiInstance.FileManagerFileCreateEntry(archiveId, type, fileManagerCreateEntryDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileCreateEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileCreateEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileSystemEntryDto> response = apiInstance.FileManagerFileCreateEntryWithHttpInfo(archiveId, type, fileManagerCreateEntryDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileCreateEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **fileManagerCreateEntryDto** | [**FileManagerCreateEntryDto**](FileManagerCreateEntryDto.md) |  | [optional]  |

### Return type

[**FileManagerFileSystemEntryDto**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfiledeleteentry"></a>
# **FileManagerFileDeleteEntry**
> bool FileManagerFileDeleteEntry (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileDeleteEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileDeleteEntry(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileDeleteEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileDeleteEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileDeleteEntryWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileDeleteEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfiledeletefiletoken"></a>
# **FileManagerFileDeleteFileToken**
> bool FileManagerFileDeleteFileToken (Guid token = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileDeleteFileTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var token = "token_example";  // Guid |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileDeleteFileToken(token, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileDeleteFileToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileDeleteFileTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileDeleteFileTokenWithHttpInfo(token, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileDeleteFileTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **token** | **Guid** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfiledownloadall"></a>
# **FileManagerFileDownloadAll**
> byte[] FileManagerFileDownloadAll (FileManagerDownloadAllDto fileManagerDownloadAllDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileDownloadAllExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var fileManagerDownloadAllDto = new FileManagerDownloadAllDto(); // FileManagerDownloadAllDto |  (optional) 

            try
            {
                byte[] result = apiInstance.FileManagerFileDownloadAll(fileManagerDownloadAllDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileDownloadAll: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileDownloadAllWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.FileManagerFileDownloadAllWithHttpInfo(fileManagerDownloadAllDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileDownloadAllWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerDownloadAllDto** | [**FileManagerDownloadAllDto**](FileManagerDownloadAllDto.md) |  | [optional]  |

### Return type

**byte[]**

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

<a id="filemanagerfiledownloadfile"></a>
# **FileManagerFileDownloadFile**
> byte[] FileManagerFileDownloadFile (string id = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileDownloadFileExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                byte[] result = apiInstance.FileManagerFileDownloadFile(id, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileDownloadFile: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileDownloadFileWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.FileManagerFileDownloadFileWithHttpInfo(id, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileDownloadFileWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

**byte[]**

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

<a id="filemanagerfiledownloadfilebytoken"></a>
# **FileManagerFileDownloadFileByToken**
> byte[] FileManagerFileDownloadFileByToken (string id = null, string token = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileDownloadFileByTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var token = "token_example";  // string |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                byte[] result = apiInstance.FileManagerFileDownloadFileByToken(id, token, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileDownloadFileByToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileDownloadFileByTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.FileManagerFileDownloadFileByTokenWithHttpInfo(id, token, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileDownloadFileByTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **token** | **string** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

**byte[]**

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

<a id="filemanagerfilefileviewer"></a>
# **FileManagerFileFileViewer**
> FileManagerFileSourceDto FileManagerFileFileViewer (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileFileViewerExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                FileManagerFileSourceDto result = apiInstance.FileManagerFileFileViewer(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileFileViewer: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileFileViewerWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileSourceDto> response = apiInstance.FileManagerFileFileViewerWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileFileViewerWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**FileManagerFileSourceDto**](FileManagerFileSourceDto.md)

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

<a id="filemanagerfilegetentries"></a>
# **FileManagerFileGetEntries**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileGetEntries (FileManagerEntryFilterDto fileManagerEntryFilterDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntriesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var fileManagerEntryFilterDto = new FileManagerEntryFilterDto(); // FileManagerEntryFilterDto |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileGetEntries(fileManagerEntryFilterDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntries: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileGetEntriesWithHttpInfo(fileManagerEntryFilterDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerEntryFilterDto** | [**FileManagerEntryFilterDto**](FileManagerEntryFilterDto.md) |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentriesbyids"></a>
# **FileManagerFileGetEntriesByIds**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileGetEntriesByIds (Guid archiveId = null, FileManagerEntryKind kind = null, EleonsoftModuleCollectorFileManagerType type = null, List<string> requestBody = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntriesByIdsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var kind = (FileManagerEntryKind) "0";  // FileManagerEntryKind |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var requestBody = new List<string>(); // List<string> |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileGetEntriesByIds(archiveId, kind, type, requestBody);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByIds: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntriesByIdsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileGetEntriesByIdsWithHttpInfo(archiveId, kind, type, requestBody);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByIdsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **kind** | **FileManagerEntryKind** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **requestBody** | [**List&lt;string&gt;**](string.md) |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentriesbyparentid"></a>
# **FileManagerFileGetEntriesByParentId**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileGetEntriesByParentId (string parentId = null, Guid archiveId = null, FileManagerEntryKind kind = null, List<EleoncoreFileStatus> fileStatuses = null, EleonsoftModuleCollectorFileManagerType type = null, bool recursive = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntriesByParentIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var parentId = "parentId_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var kind = (FileManagerEntryKind) "0";  // FileManagerEntryKind |  (optional) 
            var fileStatuses = new List<EleoncoreFileStatus>(); // List<EleoncoreFileStatus> |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var recursive = true;  // bool |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileGetEntriesByParentId(parentId, archiveId, kind, fileStatuses, type, recursive);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByParentId: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntriesByParentIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileGetEntriesByParentIdWithHttpInfo(parentId, archiveId, kind, fileStatuses, type, recursive);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByParentIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **parentId** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **kind** | **FileManagerEntryKind** |  | [optional]  |
| **fileStatuses** | [**List&lt;EleoncoreFileStatus&gt;**](EleoncoreFileStatus.md) |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **recursive** | **bool** |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentriesbyparentidpaged"></a>
# **FileManagerFileGetEntriesByParentIdPaged**
> EleoncorePagedResultDtoOfFileManagerFileSystemEntryDto FileManagerFileGetEntriesByParentIdPaged (string folderId = null, string sorting = null, int skipCount = null, int maxResultCount = null, Guid archiveId = null, FileManagerEntryKind kind = null, List<EleoncoreFileStatus> fileStatuses = null, EleonsoftModuleCollectorFileManagerType type = null, bool recursive = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntriesByParentIdPagedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var folderId = "folderId_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var kind = (FileManagerEntryKind) "0";  // FileManagerEntryKind |  (optional) 
            var fileStatuses = new List<EleoncoreFileStatus>(); // List<EleoncoreFileStatus> |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var recursive = true;  // bool |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfFileManagerFileSystemEntryDto result = apiInstance.FileManagerFileGetEntriesByParentIdPaged(folderId, sorting, skipCount, maxResultCount, archiveId, kind, fileStatuses, type, recursive);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByParentIdPaged: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntriesByParentIdPagedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfFileManagerFileSystemEntryDto> response = apiInstance.FileManagerFileGetEntriesByParentIdPagedWithHttpInfo(folderId, sorting, skipCount, maxResultCount, archiveId, kind, fileStatuses, type, recursive);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntriesByParentIdPagedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **folderId** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **kind** | **FileManagerEntryKind** |  | [optional]  |
| **fileStatuses** | [**List&lt;EleoncoreFileStatus&gt;**](EleoncoreFileStatus.md) |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **recursive** | **bool** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfFileManagerFileSystemEntryDto**](EleoncorePagedResultDtoOfFileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentrybyid"></a>
# **FileManagerFileGetEntryById**
> FileManagerFileSystemEntryDto FileManagerFileGetEntryById (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntryByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                FileManagerFileSystemEntryDto result = apiInstance.FileManagerFileGetEntryById(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntryByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileSystemEntryDto> response = apiInstance.FileManagerFileGetEntryByIdWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**FileManagerFileSystemEntryDto**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentryhistory"></a>
# **FileManagerFileGetEntryHistory**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileGetEntryHistory (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntryHistoryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileGetEntryHistory(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryHistory: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntryHistoryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileGetEntryHistoryWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryHistoryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilegetentryparentsbyid"></a>
# **FileManagerFileGetEntryParentsById**
> List&lt;FileManagerHierarchyFolderDto&gt; FileManagerFileGetEntryParentsById (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetEntryParentsByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                List<FileManagerHierarchyFolderDto> result = apiInstance.FileManagerFileGetEntryParentsById(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryParentsById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetEntryParentsByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerHierarchyFolderDto>> response = apiInstance.FileManagerFileGetEntryParentsByIdWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetEntryParentsByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**List&lt;FileManagerHierarchyFolderDto&gt;**](FileManagerHierarchyFolderDto.md)

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

<a id="filemanagerfilegetfilebytoken"></a>
# **FileManagerFileGetFileByToken**
> byte[] FileManagerFileGetFileByToken (string id = null, Guid token = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetFileByTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var token = "token_example";  // Guid |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                byte[] result = apiInstance.FileManagerFileGetFileByToken(id, token, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetFileByToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetFileByTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<byte[]> response = apiInstance.FileManagerFileGetFileByTokenWithHttpInfo(id, token, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetFileByTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **token** | **Guid** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

**byte[]**

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

<a id="filemanagerfilegetfiletoken"></a>
# **FileManagerFileGetFileToken**
> string FileManagerFileGetFileToken (string id = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetFileTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                string result = apiInstance.FileManagerFileGetFileToken(id, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetFileToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetFileTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.FileManagerFileGetFileTokenWithHttpInfo(id, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetFileTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfilegetrootentry"></a>
# **FileManagerFileGetRootEntry**
> FileManagerFileSystemEntryDto FileManagerFileGetRootEntry (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileGetRootEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                FileManagerFileSystemEntryDto result = apiInstance.FileManagerFileGetRootEntry(archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileGetRootEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileGetRootEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileSystemEntryDto> response = apiInstance.FileManagerFileGetRootEntryWithHttpInfo(archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileGetRootEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**FileManagerFileSystemEntryDto**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilemoveallentries"></a>
# **FileManagerFileMoveAllEntries**
> bool FileManagerFileMoveAllEntries (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null, FileManagerMoveAllEntriesDto fileManagerMoveAllEntriesDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileMoveAllEntriesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var fileManagerMoveAllEntriesDto = new FileManagerMoveAllEntriesDto(); // FileManagerMoveAllEntriesDto |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileMoveAllEntries(archiveId, type, fileManagerMoveAllEntriesDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileMoveAllEntries: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileMoveAllEntriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileMoveAllEntriesWithHttpInfo(archiveId, type, fileManagerMoveAllEntriesDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileMoveAllEntriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **fileManagerMoveAllEntriesDto** | [**FileManagerMoveAllEntriesDto**](FileManagerMoveAllEntriesDto.md) |  | [optional]  |

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

<a id="filemanagerfilemoveentry"></a>
# **FileManagerFileMoveEntry**
> bool FileManagerFileMoveEntry (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null, FileManagerMoveEntryDto fileManagerMoveEntryDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileMoveEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var fileManagerMoveEntryDto = new FileManagerMoveEntryDto(); // FileManagerMoveEntryDto |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileMoveEntry(archiveId, type, fileManagerMoveEntryDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileMoveEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileMoveEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileMoveEntryWithHttpInfo(archiveId, type, fileManagerMoveEntryDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileMoveEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **fileManagerMoveEntryDto** | [**FileManagerMoveEntryDto**](FileManagerMoveEntryDto.md) |  | [optional]  |

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

<a id="filemanagerfilereadtextfile"></a>
# **FileManagerFileReadTextFile**
> List&lt;string&gt; FileManagerFileReadTextFile (string id = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileReadTextFileExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                List<string> result = apiInstance.FileManagerFileReadTextFile(id, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileReadTextFile: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileReadTextFileWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.FileManagerFileReadTextFileWithHttpInfo(id, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileReadTextFileWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfilereadtextfilebytoken"></a>
# **FileManagerFileReadTextFileByToken**
> List&lt;string&gt; FileManagerFileReadTextFileByToken (string id = null, string token = null, bool isVersion = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileReadTextFileByTokenExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var token = "token_example";  // string |  (optional) 
            var isVersion = true;  // bool |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                List<string> result = apiInstance.FileManagerFileReadTextFileByToken(id, token, isVersion, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileReadTextFileByToken: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileReadTextFileByTokenWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<string>> response = apiInstance.FileManagerFileReadTextFileByTokenWithHttpInfo(id, token, isVersion, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileReadTextFileByTokenWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **token** | **string** |  | [optional]  |
| **isVersion** | **bool** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfilerenameentry"></a>
# **FileManagerFileRenameEntry**
> FileManagerFileSystemEntryDto FileManagerFileRenameEntry (Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null, FileManagerRenameEntryDto fileManagerRenameEntryDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileRenameEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 
            var fileManagerRenameEntryDto = new FileManagerRenameEntryDto(); // FileManagerRenameEntryDto |  (optional) 

            try
            {
                FileManagerFileSystemEntryDto result = apiInstance.FileManagerFileRenameEntry(archiveId, type, fileManagerRenameEntryDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileRenameEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileRenameEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileSystemEntryDto> response = apiInstance.FileManagerFileRenameEntryWithHttpInfo(archiveId, type, fileManagerRenameEntryDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileRenameEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |
| **fileManagerRenameEntryDto** | [**FileManagerRenameEntryDto**](FileManagerRenameEntryDto.md) |  | [optional]  |

### Return type

[**FileManagerFileSystemEntryDto**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfilerestoreentry"></a>
# **FileManagerFileRestoreEntry**
> bool FileManagerFileRestoreEntry (string id = null, Guid archiveId = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileRestoreEntryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var id = "id_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileRestoreEntry(id, archiveId, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileRestoreEntry: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileRestoreEntryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileRestoreEntryWithHttpInfo(id, archiveId, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileRestoreEntryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

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

<a id="filemanagerfilesearchentries"></a>
# **FileManagerFileSearchEntries**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileSearchEntries (string search = null, Guid archiveId = null, FileManagerEntryKind kind = null, EleonsoftModuleCollectorFileManagerType type = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileSearchEntriesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var search = "search_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var kind = (FileManagerEntryKind) "0";  // FileManagerEntryKind |  (optional) 
            var type = (EleonsoftModuleCollectorFileManagerType) "0";  // EleonsoftModuleCollectorFileManagerType |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileSearchEntries(search, archiveId, kind, type);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileSearchEntries: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileSearchEntriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileSearchEntriesWithHttpInfo(search, archiveId, kind, type);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileSearchEntriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **search** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |
| **kind** | **FileManagerEntryKind** |  | [optional]  |
| **type** | **EleonsoftModuleCollectorFileManagerType** |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

<a id="filemanagerfileuploadfiles"></a>
# **FileManagerFileUploadFiles**
> List&lt;FileManagerFileSystemEntryDto&gt; FileManagerFileUploadFiles (FileManagerFileUploadDto fileManagerFileUploadDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileUploadFilesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileApi(config);
            var fileManagerFileUploadDto = new FileManagerFileUploadDto(); // FileManagerFileUploadDto |  (optional) 

            try
            {
                List<FileManagerFileSystemEntryDto> result = apiInstance.FileManagerFileUploadFiles(fileManagerFileUploadDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileApi.FileManagerFileUploadFiles: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileUploadFilesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileSystemEntryDto>> response = apiInstance.FileManagerFileUploadFilesWithHttpInfo(fileManagerFileUploadDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileApi.FileManagerFileUploadFilesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileUploadDto** | [**FileManagerFileUploadDto**](FileManagerFileUploadDto.md) |  | [optional]  |

### Return type

[**List&lt;FileManagerFileSystemEntryDto&gt;**](FileManagerFileSystemEntryDto.md)

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

