# EleonsoftProxy.Api.ChatRoomApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CollaborationChatRoomClose**](ChatRoomApi.md#collaborationchatroomclose) | **POST** /api/Collaboration/ChatRooms/Close |  |
| [**CollaborationChatRoomCreateChat**](ChatRoomApi.md#collaborationchatroomcreatechat) | **POST** /api/Collaboration/ChatRooms/CreateGroupChat |  |
| [**CollaborationChatRoomGetChatsList**](ChatRoomApi.md#collaborationchatroomgetchatslist) | **GET** /api/Collaboration/ChatRooms/GetChatsList |  |
| [**CollaborationChatRoomRenameChat**](ChatRoomApi.md#collaborationchatroomrenamechat) | **POST** /api/Collaboration/ChatRooms/RenameChat |  |
| [**CollaborationChatRoomUpdate**](ChatRoomApi.md#collaborationchatroomupdate) | **POST** /api/Collaboration/ChatRooms/Update |  |

<a id="collaborationchatroomclose"></a>
# **CollaborationChatRoomClose**
> void CollaborationChatRoomClose (Guid chatId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatRoomCloseExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatRoomApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 

            try
            {
                apiInstance.CollaborationChatRoomClose(chatId);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomClose: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatRoomCloseWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CollaborationChatRoomCloseWithHttpInfo(chatId);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomCloseWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |

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

<a id="collaborationchatroomcreatechat"></a>
# **CollaborationChatRoomCreateChat**
> CollaborationChatRoomDto CollaborationChatRoomCreateChat (CollaborationCreateChatRequestDto collaborationCreateChatRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatRoomCreateChatExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatRoomApi(config);
            var collaborationCreateChatRequestDto = new CollaborationCreateChatRequestDto(); // CollaborationCreateChatRequestDto |  (optional) 

            try
            {
                CollaborationChatRoomDto result = apiInstance.CollaborationChatRoomCreateChat(collaborationCreateChatRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomCreateChat: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatRoomCreateChatWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationChatRoomDto> response = apiInstance.CollaborationChatRoomCreateChatWithHttpInfo(collaborationCreateChatRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomCreateChatWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **collaborationCreateChatRequestDto** | [**CollaborationCreateChatRequestDto**](CollaborationCreateChatRequestDto.md) |  | [optional]  |

### Return type

[**CollaborationChatRoomDto**](CollaborationChatRoomDto.md)

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

<a id="collaborationchatroomgetchatslist"></a>
# **CollaborationChatRoomGetChatsList**
> EleoncorePagedResultDtoOfCollaborationChatRoomDto CollaborationChatRoomGetChatsList (string nameFilter = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatRoomGetChatsListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatRoomApi(config);
            var nameFilter = "nameFilter_example";  // string |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfCollaborationChatRoomDto result = apiInstance.CollaborationChatRoomGetChatsList(nameFilter, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomGetChatsList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatRoomGetChatsListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfCollaborationChatRoomDto> response = apiInstance.CollaborationChatRoomGetChatsListWithHttpInfo(nameFilter, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomGetChatsListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **nameFilter** | **string** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfCollaborationChatRoomDto**](EleoncorePagedResultDtoOfCollaborationChatRoomDto.md)

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

<a id="collaborationchatroomrenamechat"></a>
# **CollaborationChatRoomRenameChat**
> bool CollaborationChatRoomRenameChat (Guid chatId = null, string newName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatRoomRenameChatExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatRoomApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 
            var newName = "newName_example";  // string |  (optional) 

            try
            {
                bool result = apiInstance.CollaborationChatRoomRenameChat(chatId, newName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomRenameChat: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatRoomRenameChatWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CollaborationChatRoomRenameChatWithHttpInfo(chatId, newName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomRenameChatWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |
| **newName** | **string** |  | [optional]  |

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

<a id="collaborationchatroomupdate"></a>
# **CollaborationChatRoomUpdate**
> CollaborationChatRoomDto CollaborationChatRoomUpdate (ModuleCollectorUpdateChatRequestDto moduleCollectorUpdateChatRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatRoomUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatRoomApi(config);
            var moduleCollectorUpdateChatRequestDto = new ModuleCollectorUpdateChatRequestDto(); // ModuleCollectorUpdateChatRequestDto |  (optional) 

            try
            {
                CollaborationChatRoomDto result = apiInstance.CollaborationChatRoomUpdate(moduleCollectorUpdateChatRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatRoomUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationChatRoomDto> response = apiInstance.CollaborationChatRoomUpdateWithHttpInfo(moduleCollectorUpdateChatRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatRoomApi.CollaborationChatRoomUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorUpdateChatRequestDto** | [**ModuleCollectorUpdateChatRequestDto**](ModuleCollectorUpdateChatRequestDto.md) |  | [optional]  |

### Return type

[**CollaborationChatRoomDto**](CollaborationChatRoomDto.md)

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

