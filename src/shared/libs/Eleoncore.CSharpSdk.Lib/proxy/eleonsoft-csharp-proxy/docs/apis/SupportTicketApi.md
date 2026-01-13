# EleonsoftProxy.Api.SupportTicketApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CollaborationSupportTicketCloseSupportTicket**](SupportTicketApi.md#collaborationsupportticketclosesupportticket) | **POST** /api/Collaboration/SupportTickets/CloseSupportTicket |  |
| [**CollaborationSupportTicketCreateSupportTicket**](SupportTicketApi.md#collaborationsupportticketcreatesupportticket) | **POST** /api/Collaboration/SupportTickets/CreateSupportTicket |  |

<a id="collaborationsupportticketclosesupportticket"></a>
# **CollaborationSupportTicketCloseSupportTicket**
> bool CollaborationSupportTicketCloseSupportTicket (Guid ticketChatRoomId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationSupportTicketCloseSupportTicketExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SupportTicketApi(config);
            var ticketChatRoomId = "ticketChatRoomId_example";  // Guid |  (optional) 

            try
            {
                bool result = apiInstance.CollaborationSupportTicketCloseSupportTicket(ticketChatRoomId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SupportTicketApi.CollaborationSupportTicketCloseSupportTicket: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationSupportTicketCloseSupportTicketWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CollaborationSupportTicketCloseSupportTicketWithHttpInfo(ticketChatRoomId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SupportTicketApi.CollaborationSupportTicketCloseSupportTicketWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **ticketChatRoomId** | **Guid** |  | [optional]  |

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

<a id="collaborationsupportticketcreatesupportticket"></a>
# **CollaborationSupportTicketCreateSupportTicket**
> CollaborationChatRoomDto CollaborationSupportTicketCreateSupportTicket (CollaborationCreateSupportTicketRequestDto collaborationCreateSupportTicketRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationSupportTicketCreateSupportTicketExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new SupportTicketApi(config);
            var collaborationCreateSupportTicketRequestDto = new CollaborationCreateSupportTicketRequestDto(); // CollaborationCreateSupportTicketRequestDto |  (optional) 

            try
            {
                CollaborationChatRoomDto result = apiInstance.CollaborationSupportTicketCreateSupportTicket(collaborationCreateSupportTicketRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling SupportTicketApi.CollaborationSupportTicketCreateSupportTicket: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationSupportTicketCreateSupportTicketWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<CollaborationChatRoomDto> response = apiInstance.CollaborationSupportTicketCreateSupportTicketWithHttpInfo(collaborationCreateSupportTicketRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling SupportTicketApi.CollaborationSupportTicketCreateSupportTicketWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **collaborationCreateSupportTicketRequestDto** | [**CollaborationCreateSupportTicketRequestDto**](CollaborationCreateSupportTicketRequestDto.md) |  | [optional]  |

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

