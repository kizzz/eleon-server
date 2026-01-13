# EleonsoftProxy.Api.UserChatSettingApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**CollaborationUserChatSettingSetChatArchived**](UserChatSettingApi.md#collaborationuserchatsettingsetchatarchived) | **POST** /api/Collaboration/UserChatSetting/SetChatArchived |  |
| [**CollaborationUserChatSettingSetChatMute**](UserChatSettingApi.md#collaborationuserchatsettingsetchatmute) | **POST** /api/Collaboration/UserChatSetting/SetChatMute |  |

<a id="collaborationuserchatsettingsetchatarchived"></a>
# **CollaborationUserChatSettingSetChatArchived**
> bool CollaborationUserChatSettingSetChatArchived (Guid chatId = null, bool isArchived = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationUserChatSettingSetChatArchivedExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserChatSettingApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 
            var isArchived = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.CollaborationUserChatSettingSetChatArchived(chatId, isArchived);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserChatSettingApi.CollaborationUserChatSettingSetChatArchived: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationUserChatSettingSetChatArchivedWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CollaborationUserChatSettingSetChatArchivedWithHttpInfo(chatId, isArchived);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserChatSettingApi.CollaborationUserChatSettingSetChatArchivedWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |
| **isArchived** | **bool** |  | [optional]  |

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

<a id="collaborationuserchatsettingsetchatmute"></a>
# **CollaborationUserChatSettingSetChatMute**
> bool CollaborationUserChatSettingSetChatMute (Guid chatId = null, bool isMuted = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class CollaborationUserChatSettingSetChatMuteExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new UserChatSettingApi(config);
            var chatId = "chatId_example";  // Guid |  (optional) 
            var isMuted = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.CollaborationUserChatSettingSetChatMute(chatId, isMuted);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling UserChatSettingApi.CollaborationUserChatSettingSetChatMute: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the CollaborationUserChatSettingSetChatMuteWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.CollaborationUserChatSettingSetChatMuteWithHttpInfo(chatId, isMuted);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling UserChatSettingApi.CollaborationUserChatSettingSetChatMuteWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **chatId** | **Guid** |  | [optional]  |
| **isMuted** | **bool** |  | [optional]  |

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

