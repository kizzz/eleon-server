# EleonsoftProxy.Api.ChatInteractionApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CollaborationChatInteractionAckMessageReceived**](ChatInteractionApi.md#collaborationchatinteractionackmessagereceived) | **POST** /api/Collaboration/ChatInteraction/AckMessageReceived |  |
| [**CollaborationChatInteractionGetChatById**](ChatInteractionApi.md#collaborationchatinteractiongetchatbyid) | **GET** /api/Collaboration/ChatInteraction/GetChatById |  |
| [**CollaborationChatInteractionGetChatMessages**](ChatInteractionApi.md#collaborationchatinteractiongetchatmessages) | **GET** /api/Collaboration/ChatInteraction/GetChatMessages |  |
| [**CollaborationChatInteractionGetLastChats**](ChatInteractionApi.md#collaborationchatinteractiongetlastchats) | **POST** /api/Collaboration/ChatInteraction/GetLastChats |  |
| [**CollaborationChatInteractionOpenChat**](ChatInteractionApi.md#collaborationchatinteractionopenchat) | **POST** /api/Collaboration/ChatInteraction/OpenChat |  |
| [**CollaborationChatInteractionRetreiveDocumentMessageContent**](ChatInteractionApi.md#collaborationchatinteractionretreivedocumentmessagecontent) | **GET** /api/Collaboration/ChatInteraction/RetreiveDocumentMessageContent |  |
| [**CollaborationChatInteractionSendDocumentMessage**](ChatInteractionApi.md#collaborationchatinteractionsenddocumentmessage) | **POST** /api/Collaboration/ChatInteraction/SendDocumentMessage |  |
| [**CollaborationChatInteractionSendTextMessage**](ChatInteractionApi.md#collaborationchatinteractionsendtextmessage) | **POST** /api/Collaboration/ChatInteraction/SendTextMessage |  |

<a id="collaborationchatinteractionackmessagereceived"></a>
# **CollaborationChatInteractionAckMessageReceived**
> bool CollaborationChatInteractionAckMessageReceived (Guid messageId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionAckMessageReceivedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var messageId = "messageId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.CollaborationChatInteractionAckMessageReceived(messageId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionAckMessageReceived: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionAckMessageReceivedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CollaborationChatInteractionAckMessageReceivedWithHttpInfo(messageId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionAckMessageReceivedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **messageId** | **Guid** |  | [optional]  |

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

<a id="collaborationchatinteractiongetchatbyid"></a>
# **CollaborationChatInteractionGetChatById**
> CollaborationUserChatInfoDto CollaborationChatInteractionGetChatById (Guid chatId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionGetChatByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 

            try
            {
                CollaborationUserChatInfoDto result = apiInstance.CollaborationChatInteractionGetChatById(chatId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetChatById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionGetChatByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationUserChatInfoDto> response = apiInstance.CollaborationChatInteractionGetChatByIdWithHttpInfo(chatId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetChatByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |

### Return type

[**CollaborationUserChatInfoDto**](CollaborationUserChatInfoDto.md)

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

<a id="collaborationchatinteractiongetchatmessages"></a>
# **CollaborationChatInteractionGetChatMessages**
> List&lt;CollaborationChatMessageDto&gt; CollaborationChatInteractionGetChatMessages (Guid chatId = null, int skip = null, int take = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionGetChatMessagesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 
            var skip = 56;  // int |  (optional) 
            var take = 56;  // int |  (optional) 

            try
            {
                List<CollaborationChatMessageDto> result = apiInstance.CollaborationChatInteractionGetChatMessages(chatId, skip, take);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetChatMessages: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionGetChatMessagesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<CollaborationChatMessageDto>> response = apiInstance.CollaborationChatInteractionGetChatMessagesWithHttpInfo(chatId, skip, take);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetChatMessagesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |
| **skip** | **int** |  | [optional]  |
| **take** | **int** |  | [optional]  |

### Return type

[**List&lt;CollaborationChatMessageDto&gt;**](CollaborationChatMessageDto.md)

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

<a id="collaborationchatinteractiongetlastchats"></a>
# **CollaborationChatInteractionGetLastChats**
> EleoncorePagedResultDtoOfCollaborationUserChatInfoDto CollaborationChatInteractionGetLastChats (CollaborationLastChatsRequestDto collaborationLastChatsRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionGetLastChatsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var collaborationLastChatsRequestDto = new CollaborationLastChatsRequestDto(); // CollaborationLastChatsRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfCollaborationUserChatInfoDto result = apiInstance.CollaborationChatInteractionGetLastChats(collaborationLastChatsRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetLastChats: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionGetLastChatsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfCollaborationUserChatInfoDto> response = apiInstance.CollaborationChatInteractionGetLastChatsWithHttpInfo(collaborationLastChatsRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionGetLastChatsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **collaborationLastChatsRequestDto** | [**CollaborationLastChatsRequestDto**](CollaborationLastChatsRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfCollaborationUserChatInfoDto**](EleoncorePagedResultDtoOfCollaborationUserChatInfoDto.md)

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

<a id="collaborationchatinteractionopenchat"></a>
# **CollaborationChatInteractionOpenChat**
> List&lt;CollaborationChatMessageDto&gt; CollaborationChatInteractionOpenChat (Guid chatId = null, int limit = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionOpenChatExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 
            var limit = 56;  // int |  (optional) 

            try
            {
                List<CollaborationChatMessageDto> result = apiInstance.CollaborationChatInteractionOpenChat(chatId, limit);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionOpenChat: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionOpenChatWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<CollaborationChatMessageDto>> response = apiInstance.CollaborationChatInteractionOpenChatWithHttpInfo(chatId, limit);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionOpenChatWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |
| **limit** | **int** |  | [optional]  |

### Return type

[**List&lt;CollaborationChatMessageDto&gt;**](CollaborationChatMessageDto.md)

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

<a id="collaborationchatinteractionretreivedocumentmessagecontent"></a>
# **CollaborationChatInteractionRetreiveDocumentMessageContent**
> string CollaborationChatInteractionRetreiveDocumentMessageContent (Guid messageId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionRetreiveDocumentMessageContentExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var messageId = "messageId_example";  // Guid |  (optional) 

            try
            {
                string result = apiInstance.CollaborationChatInteractionRetreiveDocumentMessageContent(messageId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionRetreiveDocumentMessageContent: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionRetreiveDocumentMessageContentWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.CollaborationChatInteractionRetreiveDocumentMessageContentWithHttpInfo(messageId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionRetreiveDocumentMessageContentWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **messageId** | **Guid** |  | [optional]  |

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

<a id="collaborationchatinteractionsenddocumentmessage"></a>
# **CollaborationChatInteractionSendDocumentMessage**
> CollaborationChatMessageDto CollaborationChatInteractionSendDocumentMessage (CollaborationSendDocumentMessageRequestDto collaborationSendDocumentMessageRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionSendDocumentMessageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var collaborationSendDocumentMessageRequestDto = new CollaborationSendDocumentMessageRequestDto(); // CollaborationSendDocumentMessageRequestDto |  (optional) 

            try
            {
                CollaborationChatMessageDto result = apiInstance.CollaborationChatInteractionSendDocumentMessage(collaborationSendDocumentMessageRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionSendDocumentMessage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionSendDocumentMessageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationChatMessageDto> response = apiInstance.CollaborationChatInteractionSendDocumentMessageWithHttpInfo(collaborationSendDocumentMessageRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionSendDocumentMessageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **collaborationSendDocumentMessageRequestDto** | [**CollaborationSendDocumentMessageRequestDto**](CollaborationSendDocumentMessageRequestDto.md) |  | [optional]  |

### Return type

[**CollaborationChatMessageDto**](CollaborationChatMessageDto.md)

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

<a id="collaborationchatinteractionsendtextmessage"></a>
# **CollaborationChatInteractionSendTextMessage**
> CollaborationChatMessageDto CollaborationChatInteractionSendTextMessage (CollaborationSendTextMessageRequestDto collaborationSendTextMessageRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationChatInteractionSendTextMessageExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ChatInteractionApi(config);
            var collaborationSendTextMessageRequestDto = new CollaborationSendTextMessageRequestDto(); // CollaborationSendTextMessageRequestDto |  (optional) 

            try
            {
                CollaborationChatMessageDto result = apiInstance.CollaborationChatInteractionSendTextMessage(collaborationSendTextMessageRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionSendTextMessage: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationChatInteractionSendTextMessageWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationChatMessageDto> response = apiInstance.CollaborationChatInteractionSendTextMessageWithHttpInfo(collaborationSendTextMessageRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ChatInteractionApi.CollaborationChatInteractionSendTextMessageWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **collaborationSendTextMessageRequestDto** | [**CollaborationSendTextMessageRequestDto**](CollaborationSendTextMessageRequestDto.md) |  | [optional]  |

### Return type

[**CollaborationChatMessageDto**](CollaborationChatMessageDto.md)

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

