# EleonsoftProxy.Api.DocumentConversationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CollaborationDocumentConversationGetDocumentConversationInfo**](DocumentConversationApi.md#collaborationdocumentconversationgetdocumentconversationinfo) | **GET** /api/Collaboration/DocumentConversations/GetDocumentConversationInfo |  |
| [**CollaborationDocumentConversationSendDocumentChatMessages**](DocumentConversationApi.md#collaborationdocumentconversationsenddocumentchatmessages) | **POST** /api/Collaboration/DocumentConversations/SendDocumentChatMessages |  |

<a id="collaborationdocumentconversationgetdocumentconversationinfo"></a>
# **CollaborationDocumentConversationGetDocumentConversationInfo**
> CollaborationDocumentConversationInfoDto CollaborationDocumentConversationGetDocumentConversationInfo (string docType = null, string documentId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationDocumentConversationGetDocumentConversationInfoExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DocumentConversationApi(config);
            var docType = "docType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 

            try
            {
                CollaborationDocumentConversationInfoDto result = apiInstance.CollaborationDocumentConversationGetDocumentConversationInfo(docType, documentId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DocumentConversationApi.CollaborationDocumentConversationGetDocumentConversationInfo: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationDocumentConversationGetDocumentConversationInfoWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationDocumentConversationInfoDto> response = apiInstance.CollaborationDocumentConversationGetDocumentConversationInfoWithHttpInfo(docType, documentId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DocumentConversationApi.CollaborationDocumentConversationGetDocumentConversationInfoWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **docType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |

### Return type

[**CollaborationDocumentConversationInfoDto**](CollaborationDocumentConversationInfoDto.md)

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

<a id="collaborationdocumentconversationsenddocumentchatmessages"></a>
# **CollaborationDocumentConversationSendDocumentChatMessages**
> void CollaborationDocumentConversationSendDocumentChatMessages (List<ModuleCollectorDocumentChatMessageDto> moduleCollectorDocumentChatMessageDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationDocumentConversationSendDocumentChatMessagesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new DocumentConversationApi(config);
            var moduleCollectorDocumentChatMessageDto = new List<ModuleCollectorDocumentChatMessageDto>(); // List<ModuleCollectorDocumentChatMessageDto> |  (optional) 

            try
            {
                apiInstance.CollaborationDocumentConversationSendDocumentChatMessages(moduleCollectorDocumentChatMessageDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling DocumentConversationApi.CollaborationDocumentConversationSendDocumentChatMessages: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationDocumentConversationSendDocumentChatMessagesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.CollaborationDocumentConversationSendDocumentChatMessagesWithHttpInfo(moduleCollectorDocumentChatMessageDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling DocumentConversationApi.CollaborationDocumentConversationSendDocumentChatMessagesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **moduleCollectorDocumentChatMessageDto** | [**List&lt;ModuleCollectorDocumentChatMessageDto&gt;**](ModuleCollectorDocumentChatMessageDto.md) |  | [optional]  |

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

