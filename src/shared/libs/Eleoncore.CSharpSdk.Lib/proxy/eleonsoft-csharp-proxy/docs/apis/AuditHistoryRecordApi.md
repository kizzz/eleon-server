# EleonsoftProxy.Api.AuditHistoryRecordApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AuditorAuditHistoryRecordGetDocumentHistory**](AuditHistoryRecordApi.md#auditoraudithistoryrecordgetdocumenthistory) | **GET** /api/Auditor/History/GetModuleFieldSets |  |

<a id="auditoraudithistoryrecordgetdocumenthistory"></a>
# **AuditorAuditHistoryRecordGetDocumentHistory**
> EleoncorePagedResultDtoOfEleoncoreDocumentVersionEntity AuditorAuditHistoryRecordGetDocumentHistory (string documentObjectType = null, string documentId = null, DateTime fromDateFilter = null, DateTime toDateFilter = null, string sorting = null, int skipCount = null, int maxResultCount = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AuditorAuditHistoryRecordGetDocumentHistoryExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AuditHistoryRecordApi(config);
            var documentObjectType = "documentObjectType_example";  // string |  (optional) 
            var documentId = "documentId_example";  // string |  (optional) 
            var fromDateFilter = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var toDateFilter = DateTime.Parse("2013-10-20T19:20:30+01:00");  // DateTime |  (optional) 
            var sorting = "sorting_example";  // string |  (optional) 
            var skipCount = 56;  // int |  (optional) 
            var maxResultCount = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfEleoncoreDocumentVersionEntity result = apiInstance.AuditorAuditHistoryRecordGetDocumentHistory(documentObjectType, documentId, fromDateFilter, toDateFilter, sorting, skipCount, maxResultCount);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AuditHistoryRecordApi.AuditorAuditHistoryRecordGetDocumentHistory: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AuditorAuditHistoryRecordGetDocumentHistoryWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfEleoncoreDocumentVersionEntity> response = apiInstance.AuditorAuditHistoryRecordGetDocumentHistoryWithHttpInfo(documentObjectType, documentId, fromDateFilter, toDateFilter, sorting, skipCount, maxResultCount);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AuditHistoryRecordApi.AuditorAuditHistoryRecordGetDocumentHistoryWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **documentObjectType** | **string** |  | [optional]  |
| **documentId** | **string** |  | [optional]  |
| **fromDateFilter** | **DateTime** |  | [optional]  |
| **toDateFilter** | **DateTime** |  | [optional]  |
| **sorting** | **string** |  | [optional]  |
| **skipCount** | **int** |  | [optional]  |
| **maxResultCount** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfEleoncoreDocumentVersionEntity**](EleoncorePagedResultDtoOfEleoncoreDocumentVersionEntity.md)

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

