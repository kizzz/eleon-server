# EleonsoftProxy.Api.PackageTemplateApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AccountingPackageTemplateGetPackageTemplateById**](PackageTemplateApi.md#accountingpackagetemplategetpackagetemplatebyid) | **GET** /api/account/packageTemplates/GetPackageTemplateById |  |
| [**AccountingPackageTemplateGetPackageTemplateList**](PackageTemplateApi.md#accountingpackagetemplategetpackagetemplatelist) | **POST** /api/account/packageTemplates/GetPackageTemplateList |  |
| [**AccountingPackageTemplateRemovePackageTemplate**](PackageTemplateApi.md#accountingpackagetemplateremovepackagetemplate) | **POST** /api/account/packageTemplates/RemovePackageTemplate |  |
| [**AccountingPackageTemplateUpdatePackageTemplate**](PackageTemplateApi.md#accountingpackagetemplateupdatepackagetemplate) | **POST** /api/account/packageTemplates/UpdatePackageTemplate |  |

<a id="accountingpackagetemplategetpackagetemplatebyid"></a>
# **AccountingPackageTemplateGetPackageTemplateById**
> AccountingPackageTemplateDto AccountingPackageTemplateGetPackageTemplateById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingPackageTemplateGetPackageTemplateByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PackageTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                AccountingPackageTemplateDto result = apiInstance.AccountingPackageTemplateGetPackageTemplateById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateGetPackageTemplateById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingPackageTemplateGetPackageTemplateByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AccountingPackageTemplateDto> response = apiInstance.AccountingPackageTemplateGetPackageTemplateByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateGetPackageTemplateByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**AccountingPackageTemplateDto**](AccountingPackageTemplateDto.md)

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

<a id="accountingpackagetemplategetpackagetemplatelist"></a>
# **AccountingPackageTemplateGetPackageTemplateList**
> EleoncorePagedResultDtoOfAccountingPackageTemplateDto AccountingPackageTemplateGetPackageTemplateList (AccountingPackageTemplateListRequestDto accountingPackageTemplateListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingPackageTemplateGetPackageTemplateListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PackageTemplateApi(config);
            var accountingPackageTemplateListRequestDto = new AccountingPackageTemplateListRequestDto(); // AccountingPackageTemplateListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfAccountingPackageTemplateDto result = apiInstance.AccountingPackageTemplateGetPackageTemplateList(accountingPackageTemplateListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateGetPackageTemplateList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingPackageTemplateGetPackageTemplateListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfAccountingPackageTemplateDto> response = apiInstance.AccountingPackageTemplateGetPackageTemplateListWithHttpInfo(accountingPackageTemplateListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateGetPackageTemplateListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **accountingPackageTemplateListRequestDto** | [**AccountingPackageTemplateListRequestDto**](AccountingPackageTemplateListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfAccountingPackageTemplateDto**](EleoncorePagedResultDtoOfAccountingPackageTemplateDto.md)

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

<a id="accountingpackagetemplateremovepackagetemplate"></a>
# **AccountingPackageTemplateRemovePackageTemplate**
> string AccountingPackageTemplateRemovePackageTemplate (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingPackageTemplateRemovePackageTemplateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PackageTemplateApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                string result = apiInstance.AccountingPackageTemplateRemovePackageTemplate(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateRemovePackageTemplate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingPackageTemplateRemovePackageTemplateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.AccountingPackageTemplateRemovePackageTemplateWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateRemovePackageTemplateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

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

<a id="accountingpackagetemplateupdatepackagetemplate"></a>
# **AccountingPackageTemplateUpdatePackageTemplate**
> AccountingPackageTemplateDto AccountingPackageTemplateUpdatePackageTemplate (AccountingPackageTemplateDto accountingPackageTemplateDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingPackageTemplateUpdatePackageTemplateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new PackageTemplateApi(config);
            var accountingPackageTemplateDto = new AccountingPackageTemplateDto(); // AccountingPackageTemplateDto |  (optional) 

            try
            {
                AccountingPackageTemplateDto result = apiInstance.AccountingPackageTemplateUpdatePackageTemplate(accountingPackageTemplateDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateUpdatePackageTemplate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingPackageTemplateUpdatePackageTemplateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AccountingPackageTemplateDto> response = apiInstance.AccountingPackageTemplateUpdatePackageTemplateWithHttpInfo(accountingPackageTemplateDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling PackageTemplateApi.AccountingPackageTemplateUpdatePackageTemplateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **accountingPackageTemplateDto** | [**AccountingPackageTemplateDto**](AccountingPackageTemplateDto.md) |  | [optional]  |

### Return type

[**AccountingPackageTemplateDto**](AccountingPackageTemplateDto.md)

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

