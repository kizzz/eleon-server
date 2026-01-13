# EleonsoftProxy.Api.FileArchiveFavouriteApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**FileManagerFileArchiveFavouriteAddToFavourites**](FileArchiveFavouriteApi.md#filemanagerfilearchivefavouriteaddtofavourites) | **POST** /api/file-manager/file-archives-favourites/AddToFavourites |  |
| [**FileManagerFileArchiveFavouriteRemoveFromFavourites**](FileArchiveFavouriteApi.md#filemanagerfilearchivefavouriteremovefromfavourites) | **DELETE** /api/file-manager/file-archives-favourites/RemoveFromFavourites |  |

<a id="filemanagerfilearchivefavouriteaddtofavourites"></a>
# **FileManagerFileArchiveFavouriteAddToFavourites**
> bool FileManagerFileArchiveFavouriteAddToFavourites (FileManagerFileArchiveFavouriteDto fileManagerFileArchiveFavouriteDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveFavouriteAddToFavouritesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveFavouriteApi(config);
            var fileManagerFileArchiveFavouriteDto = new FileManagerFileArchiveFavouriteDto(); // FileManagerFileArchiveFavouriteDto |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileArchiveFavouriteAddToFavourites(fileManagerFileArchiveFavouriteDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveFavouriteApi.FileManagerFileArchiveFavouriteAddToFavourites: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveFavouriteAddToFavouritesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileArchiveFavouriteAddToFavouritesWithHttpInfo(fileManagerFileArchiveFavouriteDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveFavouriteApi.FileManagerFileArchiveFavouriteAddToFavouritesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **fileManagerFileArchiveFavouriteDto** | [**FileManagerFileArchiveFavouriteDto**](FileManagerFileArchiveFavouriteDto.md) |  | [optional]  |

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

<a id="filemanagerfilearchivefavouriteremovefromfavourites"></a>
# **FileManagerFileArchiveFavouriteRemoveFromFavourites**
> bool FileManagerFileArchiveFavouriteRemoveFromFavourites (Guid archiveId = null, string parentId = null, string userId = null, string fileId = null, string folderId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class FileManagerFileArchiveFavouriteRemoveFromFavouritesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new FileArchiveFavouriteApi(config);
            var archiveId = "archiveId_example";  // Guid |  (optional) 
            var parentId = "parentId_example";  // string |  (optional) 
            var userId = "userId_example";  // string |  (optional) 
            var fileId = "fileId_example";  // string |  (optional) 
            var folderId = "folderId_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.FileManagerFileArchiveFavouriteRemoveFromFavourites(archiveId, parentId, userId, fileId, folderId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling FileArchiveFavouriteApi.FileManagerFileArchiveFavouriteRemoveFromFavourites: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the FileManagerFileArchiveFavouriteRemoveFromFavouritesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.FileManagerFileArchiveFavouriteRemoveFromFavouritesWithHttpInfo(archiveId, parentId, userId, fileId, folderId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling FileArchiveFavouriteApi.FileManagerFileArchiveFavouriteRemoveFromFavouritesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **archiveId** | **Guid** |  | [optional]  |
| **parentId** | **string** |  | [optional]  |
| **userId** | **string** |  | [optional]  |
| **fileId** | **string** |  | [optional]  |
| **folderId** | **string** |  | [optional]  |

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

