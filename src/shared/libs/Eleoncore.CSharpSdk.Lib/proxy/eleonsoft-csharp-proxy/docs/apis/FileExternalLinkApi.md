# EleonsoftProxy.Api.FileExternalLinkApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**FileManagerFileExternalLinkCancelChanges**](FileExternalLinkApi.md#filemanagerfileexternallinkcancelchanges) | **DELETE** /api/file-manager/file-external-link/CancelChanges |  |
| [**FileManagerFileExternalLinkCancelChangesByFile**](FileExternalLinkApi.md#filemanagerfileexternallinkcancelchangesbyfile) | **POST** /api/file-manager/file-external-link/CancelChangesByFile |  |
| [**FileManagerFileExternalLinkCreateOrUpdateReviewer**](FileExternalLinkApi.md#filemanagerfileexternallinkcreateorupdatereviewer) | **PUT** /api/file-manager/file-external-link/CreateOrUpdateReviewer |  |
| [**FileManagerFileExternalLinkDeleteExternalLinkSetting**](FileExternalLinkApi.md#filemanagerfileexternallinkdeleteexternallinksetting) | **GET** /api/file-manager/file-external-link/DeleteExternalLinkSetting |  |
| [**FileManagerFileExternalLinkDeleteReviewer**](FileExternalLinkApi.md#filemanagerfileexternallinkdeletereviewer) | **DELETE** /api/file-manager/file-external-link/DeleteReviewer |  |
| [**FileManagerFileExternalLinkDirectLogin**](FileExternalLinkApi.md#filemanagerfileexternallinkdirectlogin) | **POST** /api/file-manager/file-external-link/DirectLoginAsync |  |
| [**FileManagerFileExternalLinkGetFileExternalLinkSetting**](FileExternalLinkApi.md#filemanagerfileexternallinkgetfileexternallinksetting) | **GET** /api/file-manager/file-external-link/GetFileExternalLinkSetting |  |
| [**FileManagerFileExternalLinkGetLoginInfo**](FileExternalLinkApi.md#filemanagerfileexternallinkgetlogininfo) | **GET** /api/file-manager/file-external-link/GetLoginInfoAsync |  |
| [**FileManagerFileExternalLinkSaveChanges**](FileExternalLinkApi.md#filemanagerfileexternallinksavechanges) | **POST** /api/file-manager/file-external-link/SaveChanges |  |
| [**FileManagerFileExternalLinkSaveChangesByFile**](FileExternalLinkApi.md#filemanagerfileexternallinksavechangesbyfile) | **POST** /api/file-manager/file-external-link/SaveChangesByFile |  |
| [**FileManagerFileExternalLinkUpdateExternalLinkSetting**](FileExternalLinkApi.md#filemanagerfileexternallinkupdateexternallinksetting) | **PUT** /api/file-manager/file-external-link/UpdateExternalLinkSetting |  |

<a id="filemanagerfileexternallinkcancelchanges"></a>
# **FileManagerFileExternalLinkCancelChanges**
> bool FileManagerFileExternalLinkCancelChanges (Guid linkId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkCancelChangesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var linkId = "linkId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkCancelChanges(linkId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCancelChanges: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkCancelChangesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkCancelChangesWithHttpInfo(linkId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCancelChangesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **linkId** | **Guid** |  | [optional]  |

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

<a id="filemanagerfileexternallinkcancelchangesbyfile"></a>
# **FileManagerFileExternalLinkCancelChangesByFile**
> bool FileManagerFileExternalLinkCancelChangesByFile (Guid archiveId = null, string fileId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkCancelChangesByFileExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var fileId = "fileId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkCancelChangesByFile(archiveId, fileId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCancelChangesByFile: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkCancelChangesByFileWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkCancelChangesByFileWithHttpInfo(archiveId, fileId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCancelChangesByFileWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **fileId** | **string** |  | [optional]  |

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

<a id="filemanagerfileexternallinkcreateorupdatereviewer"></a>
# **FileManagerFileExternalLinkCreateOrUpdateReviewer**
> FileManagerFileExternalLinkReviewerDto FileManagerFileExternalLinkCreateOrUpdateReviewer (FileManagerCreateOrUpdateReviewerDto fileManagerCreateOrUpdateReviewerDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkCreateOrUpdateReviewerExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var fileManagerCreateOrUpdateReviewerDto = new FileManagerCreateOrUpdateReviewerDto(); // FileManagerCreateOrUpdateReviewerDto |  (optional) 

            try
            {
                FileManagerFileExternalLinkReviewerDto result = apiInstance.FileManagerFileExternalLinkCreateOrUpdateReviewer(fileManagerCreateOrUpdateReviewerDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCreateOrUpdateReviewer: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkCreateOrUpdateReviewerWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileExternalLinkReviewerDto> response = apiInstance.FileManagerFileExternalLinkCreateOrUpdateReviewerWithHttpInfo(fileManagerCreateOrUpdateReviewerDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkCreateOrUpdateReviewerWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerCreateOrUpdateReviewerDto** | [**FileManagerCreateOrUpdateReviewerDto**](FileManagerCreateOrUpdateReviewerDto.md) |  | [optional]  |

### Return type

[**FileManagerFileExternalLinkReviewerDto**](FileManagerFileExternalLinkReviewerDto.md)

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

<a id="filemanagerfileexternallinkdeleteexternallinksetting"></a>
# **FileManagerFileExternalLinkDeleteExternalLinkSetting**
> bool FileManagerFileExternalLinkDeleteExternalLinkSetting (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkDeleteExternalLinkSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkDeleteExternalLinkSetting(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDeleteExternalLinkSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkDeleteExternalLinkSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkDeleteExternalLinkSettingWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDeleteExternalLinkSettingWithHttpInfo: " + e.Message);
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

<a id="filemanagerfileexternallinkdeletereviewer"></a>
# **FileManagerFileExternalLinkDeleteReviewer**
> bool FileManagerFileExternalLinkDeleteReviewer (Guid reviewerId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkDeleteReviewerExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var reviewerId = "reviewerId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkDeleteReviewer(reviewerId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDeleteReviewer: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkDeleteReviewerWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkDeleteReviewerWithHttpInfo(reviewerId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDeleteReviewerWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **reviewerId** | **Guid** |  | [optional]  |

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

<a id="filemanagerfileexternallinkdirectlogin"></a>
# **FileManagerFileExternalLinkDirectLogin**
> string FileManagerFileExternalLinkDirectLogin (Guid id = null, string password = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkDirectLoginExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var id = "id_example";  // Guid |  (optional) 
            var password = "password_example";  // string |  (optional) 

            try
            {
                string result = apiInstance.FileManagerFileExternalLinkDirectLogin(id, password);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDirectLogin: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkDirectLoginWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.FileManagerFileExternalLinkDirectLoginWithHttpInfo(id, password);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkDirectLoginWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |
| **password** | **string** |  | [optional]  |

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

<a id="filemanagerfileexternallinkgetfileexternallinksetting"></a>
# **FileManagerFileExternalLinkGetFileExternalLinkSetting**
> FileManagerFileExternalLinkDto FileManagerFileExternalLinkGetFileExternalLinkSetting (string fileId = null, Guid archiveId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkGetFileExternalLinkSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var fileId = "fileId_example";  // string |  (optional) 
            var archiveId = "archiveId_example";  // Guid |  (optional) 

            try
            {
                FileManagerFileExternalLinkDto result = apiInstance.FileManagerFileExternalLinkGetFileExternalLinkSetting(fileId, archiveId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkGetFileExternalLinkSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkGetFileExternalLinkSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileExternalLinkDto> response = apiInstance.FileManagerFileExternalLinkGetFileExternalLinkSettingWithHttpInfo(fileId, archiveId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkGetFileExternalLinkSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileId** | **string** |  | [optional]  |
| **archiveId** | **Guid** |  | [optional]  |

### Return type

[**FileManagerFileExternalLinkDto**](FileManagerFileExternalLinkDto.md)

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

<a id="filemanagerfileexternallinkgetlogininfo"></a>
# **FileManagerFileExternalLinkGetLoginInfo**
> FileManagerFileExternalLinkReviewerInfoDto FileManagerFileExternalLinkGetLoginInfo (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkGetLoginInfoExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                FileManagerFileExternalLinkReviewerInfoDto result = apiInstance.FileManagerFileExternalLinkGetLoginInfo(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkGetLoginInfo: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkGetLoginInfoWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileExternalLinkReviewerInfoDto> response = apiInstance.FileManagerFileExternalLinkGetLoginInfoWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkGetLoginInfoWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**FileManagerFileExternalLinkReviewerInfoDto**](FileManagerFileExternalLinkReviewerInfoDto.md)

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

<a id="filemanagerfileexternallinksavechanges"></a>
# **FileManagerFileExternalLinkSaveChanges**
> bool FileManagerFileExternalLinkSaveChanges (Guid linkId = null, bool deleteAfterChanges = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkSaveChangesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var linkId = "linkId_example";  // Guid |  (optional) 
            var deleteAfterChanges = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkSaveChanges(linkId, deleteAfterChanges);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkSaveChanges: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkSaveChangesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkSaveChangesWithHttpInfo(linkId, deleteAfterChanges);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkSaveChangesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **linkId** | **Guid** |  | [optional]  |
| **deleteAfterChanges** | **bool** |  | [optional]  |

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

<a id="filemanagerfileexternallinksavechangesbyfile"></a>
# **FileManagerFileExternalLinkSaveChangesByFile**
> bool FileManagerFileExternalLinkSaveChangesByFile (Guid archiveId = null, string fileId = null, bool deleteAfterChanges = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkSaveChangesByFileExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var fileId = "fileId_example";  // string |  (optional) 
            var deleteAfterChanges = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileExternalLinkSaveChangesByFile(archiveId, fileId, deleteAfterChanges);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkSaveChangesByFile: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkSaveChangesByFileWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileExternalLinkSaveChangesByFileWithHttpInfo(archiveId, fileId, deleteAfterChanges);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkSaveChangesByFileWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **fileId** | **string** |  | [optional]  |
| **deleteAfterChanges** | **bool** |  | [optional]  |

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

<a id="filemanagerfileexternallinkupdateexternallinksetting"></a>
# **FileManagerFileExternalLinkUpdateExternalLinkSetting**
> FileManagerFileExternalLinkDto FileManagerFileExternalLinkUpdateExternalLinkSetting (FileManagerFileExternalLinkDto fileManagerFileExternalLinkDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileExternalLinkUpdateExternalLinkSettingExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileExternalLinkApi(config);
            var fileManagerFileExternalLinkDto = new FileManagerFileExternalLinkDto(); // FileManagerFileExternalLinkDto |  (optional) 

            try
            {
                FileManagerFileExternalLinkDto result = apiInstance.FileManagerFileExternalLinkUpdateExternalLinkSetting(fileManagerFileExternalLinkDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkUpdateExternalLinkSetting: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileExternalLinkUpdateExternalLinkSettingWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<FileManagerFileExternalLinkDto> response = apiInstance.FileManagerFileExternalLinkUpdateExternalLinkSettingWithHttpInfo(fileManagerFileExternalLinkDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileExternalLinkApi.FileManagerFileExternalLinkUpdateExternalLinkSettingWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileExternalLinkDto** | [**FileManagerFileExternalLinkDto**](FileManagerFileExternalLinkDto.md) |  | [optional]  |

### Return type

[**FileManagerFileExternalLinkDto**](FileManagerFileExternalLinkDto.md)

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

