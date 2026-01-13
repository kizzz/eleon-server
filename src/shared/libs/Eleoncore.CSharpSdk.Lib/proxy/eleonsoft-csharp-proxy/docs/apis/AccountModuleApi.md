# EleonsoftProxy.Api.AccountModuleApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**AccountingAccountModuleGetAccountModuleList**](AccountModuleApi.md#accountingaccountmodulegetaccountmodulelist) | **POST** /api/account/accountModules/GetAccountModuleList |  |
| [**AccountingAccountModuleGetModuleById**](AccountModuleApi.md#accountingaccountmodulegetmodulebyid) | **GET** /api/account/accountModules/GetModuleById |  |
| [**AccountingAccountModuleRemoveAccountModule**](AccountModuleApi.md#accountingaccountmoduleremoveaccountmodule) | **POST** /api/account/accountModules/RemoveAccountModule |  |
| [**AccountingAccountModuleUpdateModule**](AccountModuleApi.md#accountingaccountmoduleupdatemodule) | **POST** /api/account/accountModules/UpdateModule |  |

<a id="accountingaccountmodulegetaccountmodulelist"></a>
# **AccountingAccountModuleGetAccountModuleList**
> EleoncorePagedResultDtoOfAccountingModuleDto AccountingAccountModuleGetAccountModuleList (AccountingModuleListRequestDto accountingModuleListRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingAccountModuleGetAccountModuleListExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AccountModuleApi(config);
            var accountingModuleListRequestDto = new AccountingModuleListRequestDto(); // AccountingModuleListRequestDto |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfAccountingModuleDto result = apiInstance.AccountingAccountModuleGetAccountModuleList(accountingModuleListRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleGetAccountModuleList: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingAccountModuleGetAccountModuleListWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfAccountingModuleDto> response = apiInstance.AccountingAccountModuleGetAccountModuleListWithHttpInfo(accountingModuleListRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleGetAccountModuleListWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **accountingModuleListRequestDto** | [**AccountingModuleListRequestDto**](AccountingModuleListRequestDto.md) |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfAccountingModuleDto**](EleoncorePagedResultDtoOfAccountingModuleDto.md)

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

<a id="accountingaccountmodulegetmodulebyid"></a>
# **AccountingAccountModuleGetModuleById**
> AccountingModuleDto AccountingAccountModuleGetModuleById (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingAccountModuleGetModuleByIdExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AccountModuleApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                AccountingModuleDto result = apiInstance.AccountingAccountModuleGetModuleById(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleGetModuleById: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingAccountModuleGetModuleByIdWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AccountingModuleDto> response = apiInstance.AccountingAccountModuleGetModuleByIdWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleGetModuleByIdWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **id** | **Guid** |  | [optional]  |

### Return type

[**AccountingModuleDto**](AccountingModuleDto.md)

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

<a id="accountingaccountmoduleremoveaccountmodule"></a>
# **AccountingAccountModuleRemoveAccountModule**
> string AccountingAccountModuleRemoveAccountModule (Guid id = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingAccountModuleRemoveAccountModuleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AccountModuleApi(config);
            var id = "id_example";  // Guid |  (optional) 

            try
            {
                string result = apiInstance.AccountingAccountModuleRemoveAccountModule(id);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleRemoveAccountModule: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingAccountModuleRemoveAccountModuleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<string> response = apiInstance.AccountingAccountModuleRemoveAccountModuleWithHttpInfo(id);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleRemoveAccountModuleWithHttpInfo: " + e.Message);
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

<a id="accountingaccountmoduleupdatemodule"></a>
# **AccountingAccountModuleUpdateModule**
> AccountingModuleDto AccountingAccountModuleUpdateModule (AccountingModuleDto accountingModuleDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class AccountingAccountModuleUpdateModuleExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new AccountModuleApi(config);
            var accountingModuleDto = new AccountingModuleDto(); // AccountingModuleDto |  (optional) 

            try
            {
                AccountingModuleDto result = apiInstance.AccountingAccountModuleUpdateModule(accountingModuleDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleUpdateModule: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the AccountingAccountModuleUpdateModuleWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<AccountingModuleDto> response = apiInstance.AccountingAccountModuleUpdateModuleWithHttpInfo(accountingModuleDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling AccountModuleApi.AccountingAccountModuleUpdateModuleWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **accountingModuleDto** | [**AccountingModuleDto**](AccountingModuleDto.md) |  | [optional]  |

### Return type

[**AccountingModuleDto**](AccountingModuleDto.md)

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

