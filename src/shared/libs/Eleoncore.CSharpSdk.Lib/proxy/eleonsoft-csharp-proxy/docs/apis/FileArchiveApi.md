# EleonsoftProxy.Api.FileArchiveApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**FileManagerFileArchiveCreateFileArchive**](FileArchiveApi.md#filemanagerfilearchivecreatefilearchive) | **POST** /api/file-manager/file-archives/CreateFileArchive |  |
| [**FileManagerFileArchiveDeleteFileArchive**](FileArchiveApi.md#filemanagerfilearchivedeletefilearchive) | **DELETE** /api/file-manager/file-archives/DeleteFileArchive |  |
| [**FileManagerFileArchiveGetFileArchiveById**](FileArchiveApi.md#filemanagerfilearchivegetfilearchivebyid) | **GET** /api/file-manager/file-archives/GetFileArchiveById |  |
| [**FileManagerFileArchiveGetFileArchivesList**](FileArchiveApi.md#filemanagerfilearchivegetfilearchiveslist) | **GET** /api/file-manager/file-archives/GetFileArchivesList |  |
| [**FileManagerFileArchiveGetFileArchivesListByParams**](FileArchiveApi.md#filemanagerfilearchivegetfilearchiveslistbyparams) | **POST** /api/file-manager/file-archives/GetFileArchivesListByParams |  |
| [**FileManagerFileArchiveUpdateFileArchive**](FileArchiveApi.md#filemanagerfilearchiveupdatefilearchive) | **POST** /api/file-manager/file-archives/UpdateFileArchive |  |

<a id="filemanagerfilearchivecreatefilearchive"></a>
# **FileManagerFileArchiveCreateFileArchive**
> FileManagerFileArchiveDto FileManagerFileArchiveCreateFileArchive (FileManagerFileArchiveDto fileManagerFileArchiveDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveCreateFileArchiveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);
            var fileManagerFileArchiveDto = new FileManagerFileArchiveDto(); // FileManagerFileArchiveDto |  (optional) 

            try
            {
                FileManagerFileArchiveDto result = apiInstance.FileManagerFileArchiveCreateFileArchive(fileManagerFileArchiveDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveCreateFileArchive: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveCreateFileArchiveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileArchiveDto> response = apiInstance.FileManagerFileArchiveCreateFileArchiveWithHttpInfo(fileManagerFileArchiveDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveCreateFileArchiveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchiveDto** | [**FileManagerFileArchiveDto**](FileManagerFileArchiveDto.md) |  | [optional]  |

### Return type

[**FileManagerFileArchiveDto**](FileManagerFileArchiveDto.md)

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

<a id="filemanagerfilearchivedeletefilearchive"></a>
# **FileManagerFileArchiveDeleteFileArchive**
> bool FileManagerFileArchiveDeleteFileArchive (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveDeleteFileArchiveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileArchiveDeleteFileArchive(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveDeleteFileArchive: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveDeleteFileArchiveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileArchiveDeleteFileArchiveWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveDeleteFileArchiveWithHttpInfo: " + e.Message);
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

<a id="filemanagerfilearchivegetfilearchivebyid"></a>
# **FileManagerFileArchiveGetFileArchiveById**
> FileManagerFileArchiveDto FileManagerFileArchiveGetFileArchiveById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveGetFileArchiveByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                FileManagerFileArchiveDto result = apiInstance.FileManagerFileArchiveGetFileArchiveById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchiveById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveGetFileArchiveByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileArchiveDto> response = apiInstance.FileManagerFileArchiveGetFileArchiveByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchiveByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**FileManagerFileArchiveDto**](FileManagerFileArchiveDto.md)

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

<a id="filemanagerfilearchivegetfilearchiveslist"></a>
# **FileManagerFileArchiveGetFileArchivesList**
> List&lt;FileManagerFileArchiveDto&gt; FileManagerFileArchiveGetFileArchivesList ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveGetFileArchivesListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);

            try
            {
                List<FileManagerFileArchiveDto> result = apiInstance.FileManagerFileArchiveGetFileArchivesList();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchivesList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveGetFileArchivesListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<FileManagerFileArchiveDto>> response = apiInstance.FileManagerFileArchiveGetFileArchivesListWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchivesListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;FileManagerFileArchiveDto&gt;**](FileManagerFileArchiveDto.md)

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

<a id="filemanagerfilearchivegetfilearchiveslistbyparams"></a>
# **FileManagerFileArchiveGetFileArchivesListByParams**
> EleoncorePagedResultDtoOfFileManagerFileArchiveDto FileManagerFileArchiveGetFileArchivesListByParams (FileManagerFileArchiveListRequestDto fileManagerFileArchiveListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveGetFileArchivesListByParamsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);
            var fileManagerFileArchiveListRequestDto = new FileManagerFileArchiveListRequestDto(); // FileManagerFileArchiveListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfFileManagerFileArchiveDto result = apiInstance.FileManagerFileArchiveGetFileArchivesListByParams(fileManagerFileArchiveListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchivesListByParams: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveGetFileArchivesListByParamsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfFileManagerFileArchiveDto> response = apiInstance.FileManagerFileArchiveGetFileArchivesListByParamsWithHttpInfo(fileManagerFileArchiveListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveGetFileArchivesListByParamsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchiveListRequestDto** | [**FileManagerFileArchiveListRequestDto**](FileManagerFileArchiveListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfFileManagerFileArchiveDto**](EleoncorePagedResultDtoOfFileManagerFileArchiveDto.md)

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

<a id="filemanagerfilearchiveupdatefilearchive"></a>
# **FileManagerFileArchiveUpdateFileArchive**
> FileManagerFileArchiveDto FileManagerFileArchiveUpdateFileArchive (FileManagerFileArchiveDto fileManagerFileArchiveDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveUpdateFileArchiveExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveApi(config);
            var fileManagerFileArchiveDto = new FileManagerFileArchiveDto(); // FileManagerFileArchiveDto |  (optional) 

            try
            {
                FileManagerFileArchiveDto result = apiInstance.FileManagerFileArchiveUpdateFileArchive(fileManagerFileArchiveDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveUpdateFileArchive: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveUpdateFileArchiveWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileArchiveDto> response = apiInstance.FileManagerFileArchiveUpdateFileArchiveWithHttpInfo(fileManagerFileArchiveDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveApi.FileManagerFileArchiveUpdateFileArchiveWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchiveDto** | [**FileManagerFileArchiveDto**](FileManagerFileArchiveDto.md) |  | [optional]  |

### Return type

[**FileManagerFileArchiveDto**](FileManagerFileArchiveDto.md)

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

