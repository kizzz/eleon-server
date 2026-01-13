# EleonsoftProxy.Api.ControlDelegationApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**TenantManagementControlDelegationAddControlDelegation**](ControlDelegationApi.md#tenantmanagementcontroldelegationaddcontroldelegation) | **POST** /api/TenantManagement/ControlDelegation/AddControlDelegation |  |
| [**TenantManagementControlDelegationGetActiveControlDelegationsByUser**](ControlDelegationApi.md#tenantmanagementcontroldelegationgetactivecontroldelegationsbyuser) | **GET** /api/TenantManagement/ControlDelegation/GetActiveControlDelegationsByUser |  |
| [**TenantManagementControlDelegationGetActiveControlDelegationsToUser**](ControlDelegationApi.md#tenantmanagementcontroldelegationgetactivecontroldelegationstouser) | **GET** /api/TenantManagement/ControlDelegation/GetActiveControlDelegationsToUser |  |
| [**TenantManagementControlDelegationGetControlDelegation**](ControlDelegationApi.md#tenantmanagementcontroldelegationgetcontroldelegation) | **GET** /api/TenantManagement/ControlDelegation/GetControlDelegation |  |
| [**TenantManagementControlDelegationGetControlDelegationsByUser**](ControlDelegationApi.md#tenantmanagementcontroldelegationgetcontroldelegationsbyuser) | **GET** /api/TenantManagement/ControlDelegation/GetControlDelegationsByUser |  |
| [**TenantManagementControlDelegationSetControlDelegationActiveState**](ControlDelegationApi.md#tenantmanagementcontroldelegationsetcontroldelegationactivestate) | **POST** /api/TenantManagement/ControlDelegation/SetControlDelegationActiveState |  |
| [**TenantManagementControlDelegationUpdateControlDelegation**](ControlDelegationApi.md#tenantmanagementcontroldelegationupdatecontroldelegation) | **POST** /api/TenantManagement/ControlDelegation/UpdateControlDelegation |  |

<a id="tenantmanagementcontroldelegationaddcontroldelegation"></a>
# **TenantManagementControlDelegationAddControlDelegation**
> bool TenantManagementControlDelegationAddControlDelegation (TenantManagementCreateControlDelegationRequestDto tenantManagementCreateControlDelegationRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationAddControlDelegationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);
            var tenantManagementCreateControlDelegationRequestDto = new TenantManagementCreateControlDelegationRequestDto(); // TenantManagementCreateControlDelegationRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementControlDelegationAddControlDelegation(tenantManagementCreateControlDelegationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationAddControlDelegation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationAddControlDelegationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementControlDelegationAddControlDelegationWithHttpInfo(tenantManagementCreateControlDelegationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationAddControlDelegationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementCreateControlDelegationRequestDto** | [**TenantManagementCreateControlDelegationRequestDto**](TenantManagementCreateControlDelegationRequestDto.md) |  | [optional]  |

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

<a id="tenantmanagementcontroldelegationgetactivecontroldelegationsbyuser"></a>
# **TenantManagementControlDelegationGetActiveControlDelegationsByUser**
> List&lt;TenantManagementControlDelegationDto&gt; TenantManagementControlDelegationGetActiveControlDelegationsByUser ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationGetActiveControlDelegationsByUserExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);

            try
            {
                List<TenantManagementControlDelegationDto> result = apiInstance.TenantManagementControlDelegationGetActiveControlDelegationsByUser();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetActiveControlDelegationsByUser: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationGetActiveControlDelegationsByUserWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementControlDelegationDto>> response = apiInstance.TenantManagementControlDelegationGetActiveControlDelegationsByUserWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetActiveControlDelegationsByUserWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementControlDelegationDto&gt;**](TenantManagementControlDelegationDto.md)

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

<a id="tenantmanagementcontroldelegationgetactivecontroldelegationstouser"></a>
# **TenantManagementControlDelegationGetActiveControlDelegationsToUser**
> List&lt;TenantManagementControlDelegationDto&gt; TenantManagementControlDelegationGetActiveControlDelegationsToUser ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationGetActiveControlDelegationsToUserExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);

            try
            {
                List<TenantManagementControlDelegationDto> result = apiInstance.TenantManagementControlDelegationGetActiveControlDelegationsToUser();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetActiveControlDelegationsToUser: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationGetActiveControlDelegationsToUserWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<TenantManagementControlDelegationDto>> response = apiInstance.TenantManagementControlDelegationGetActiveControlDelegationsToUserWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetActiveControlDelegationsToUserWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;TenantManagementControlDelegationDto&gt;**](TenantManagementControlDelegationDto.md)

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

<a id="tenantmanagementcontroldelegationgetcontroldelegation"></a>
# **TenantManagementControlDelegationGetControlDelegation**
> TenantManagementControlDelegationDto TenantManagementControlDelegationGetControlDelegation (Guid delegationId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationGetControlDelegationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);
            var delegationId = "delegationId_example";  // Guid |  (optional) 

            try
            {
                TenantManagementControlDelegationDto result = apiInstance.TenantManagementControlDelegationGetControlDelegation(delegationId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetControlDelegation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationGetControlDelegationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<TenantManagementControlDelegationDto> response = apiInstance.TenantManagementControlDelegationGetControlDelegationWithHttpInfo(delegationId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetControlDelegationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **delegationId** | **Guid** |  | [optional]  |

### Return type

[**TenantManagementControlDelegationDto**](TenantManagementControlDelegationDto.md)

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

<a id="tenantmanagementcontroldelegationgetcontroldelegationsbyuser"></a>
# **TenantManagementControlDelegationGetControlDelegationsByUser**
> EleoncorePagedResultDtoOfTenantManagementControlDelegationDto TenantManagementControlDelegationGetControlDelegationsByUser (int skip = null, int take = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationGetControlDelegationsByUserExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);
            var skip = 56;  // int |  (optional) 
            var take = 56;  // int |  (optional) 

            try
            {
                EleoncorePagedResultDtoOfTenantManagementControlDelegationDto result = apiInstance.TenantManagementControlDelegationGetControlDelegationsByUser(skip, take);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetControlDelegationsByUser: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationGetControlDelegationsByUserWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<EleoncorePagedResultDtoOfTenantManagementControlDelegationDto> response = apiInstance.TenantManagementControlDelegationGetControlDelegationsByUserWithHttpInfo(skip, take);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationGetControlDelegationsByUserWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **skip** | **int** |  | [optional]  |
| **take** | **int** |  | [optional]  |

### Return type

[**EleoncorePagedResultDtoOfTenantManagementControlDelegationDto**](EleoncorePagedResultDtoOfTenantManagementControlDelegationDto.md)

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

<a id="tenantmanagementcontroldelegationsetcontroldelegationactivestate"></a>
# **TenantManagementControlDelegationSetControlDelegationActiveState**
> bool TenantManagementControlDelegationSetControlDelegationActiveState (Guid delegationId = null, bool isActive = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationSetControlDelegationActiveStateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);
            var delegationId = "delegationId_example";  // Guid |  (optional) 
            var isActive = true;  // bool |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementControlDelegationSetControlDelegationActiveState(delegationId, isActive);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationSetControlDelegationActiveState: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationSetControlDelegationActiveStateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementControlDelegationSetControlDelegationActiveStateWithHttpInfo(delegationId, isActive);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationSetControlDelegationActiveStateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **delegationId** | **Guid** |  | [optional]  |
| **isActive** | **bool** |  | [optional]  |

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

<a id="tenantmanagementcontroldelegationupdatecontroldelegation"></a>
# **TenantManagementControlDelegationUpdateControlDelegation**
> bool TenantManagementControlDelegationUpdateControlDelegation (TenantManagementUpdateControlDelegationRequestDto tenantManagementUpdateControlDelegationRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleonsoftProxy.Api;
using EleonsoftProxy.Client;
using EleonsoftProxy.Model;

namespace Example
{
    public class TenantManagementControlDelegationUpdateControlDelegationExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ControlDelegationApi(config);
            var tenantManagementUpdateControlDelegationRequestDto = new TenantManagementUpdateControlDelegationRequestDto(); // TenantManagementUpdateControlDelegationRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.TenantManagementControlDelegationUpdateControlDelegation(tenantManagementUpdateControlDelegationRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationUpdateControlDelegation: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the TenantManagementControlDelegationUpdateControlDelegationWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.TenantManagementControlDelegationUpdateControlDelegationWithHttpInfo(tenantManagementUpdateControlDelegationRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ControlDelegationApi.TenantManagementControlDelegationUpdateControlDelegationWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantManagementUpdateControlDelegationRequestDto** | [**TenantManagementUpdateControlDelegationRequestDto**](TenantManagementUpdateControlDelegationRequestDto.md) |  | [optional]  |

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

