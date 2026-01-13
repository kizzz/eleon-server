# EleoncoreProxy.Api.ApplicationConnectionStringApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementApplicationConnectionStringAddConnectionString**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringaddconnectionstring) | **POST** /api/SitesManagement/ApplicationConnectionString/AddConnectionString |  |
| [**SitesManagementApplicationConnectionStringGet**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringget) | **GET** /api/SitesManagement/ApplicationConnectionString/Get |  |
| [**SitesManagementApplicationConnectionStringGetConnectionStrings**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringgetconnectionstrings) | **GET** /api/SitesManagement/ApplicationConnectionString/GetConnectionStrings |  |
| [**SitesManagementApplicationConnectionStringRemoveConnectionString**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringremoveconnectionstring) | **POST** /api/SitesManagement/ApplicationConnectionString/RemoveConnectionString |  |
| [**SitesManagementApplicationConnectionStringSetConnectionString**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringsetconnectionstring) | **POST** /api/SitesManagement/ApplicationConnectionString/SetConnectionString |  |
| [**SitesManagementApplicationConnectionStringUpdateConnectionString**](ApplicationConnectionStringApi.md#sitesmanagementapplicationconnectionstringupdateconnectionstring) | **POST** /api/SitesManagement/ApplicationConnectionString/UpdateConnectionString |  |

<a id="sitesmanagementapplicationconnectionstringaddconnectionstring"></a>
# **SitesManagementApplicationConnectionStringAddConnectionString**
> bool SitesManagementApplicationConnectionStringAddConnectionString (SitesManagementCreateConnectionStringRequestDto sitesManagementCreateConnectionStringRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringAddConnectionStringExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var sitesManagementCreateConnectionStringRequestDto = new SitesManagementCreateConnectionStringRequestDto(); // SitesManagementCreateConnectionStringRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementApplicationConnectionStringAddConnectionString(sitesManagementCreateConnectionStringRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringAddConnectionString: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringAddConnectionStringWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementApplicationConnectionStringAddConnectionStringWithHttpInfo(sitesManagementCreateConnectionStringRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringAddConnectionStringWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCreateConnectionStringRequestDto** | [**SitesManagementCreateConnectionStringRequestDto**](SitesManagementCreateConnectionStringRequestDto.md) |  | [optional]  |

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

<a id="sitesmanagementapplicationconnectionstringget"></a>
# **SitesManagementApplicationConnectionStringGet**
> SitesManagementConnectionStringDto SitesManagementApplicationConnectionStringGet (Guid tenantId = null, string applicationName = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringGetExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 
            var applicationName = "applicationName_example";  // string |  (optional) 

            try
            {
                SitesManagementConnectionStringDto result = apiInstance.SitesManagementApplicationConnectionStringGet(tenantId, applicationName);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringGet: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringGetWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementConnectionStringDto> response = apiInstance.SitesManagementApplicationConnectionStringGetWithHttpInfo(tenantId, applicationName);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringGetWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |
| **applicationName** | **string** |  | [optional]  |

### Return type

[**SitesManagementConnectionStringDto**](SitesManagementConnectionStringDto.md)

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

<a id="sitesmanagementapplicationconnectionstringgetconnectionstrings"></a>
# **SitesManagementApplicationConnectionStringGetConnectionStrings**
> List&lt;SitesManagementConnectionStringDto&gt; SitesManagementApplicationConnectionStringGetConnectionStrings (Guid tenantId = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringGetConnectionStringsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var tenantId = "tenantId_example";  // Guid |  (optional) 

            try
            {
                List<SitesManagementConnectionStringDto> result = apiInstance.SitesManagementApplicationConnectionStringGetConnectionStrings(tenantId);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringGetConnectionStrings: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringGetConnectionStringsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementConnectionStringDto>> response = apiInstance.SitesManagementApplicationConnectionStringGetConnectionStringsWithHttpInfo(tenantId);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringGetConnectionStringsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **tenantId** | **Guid** |  | [optional]  |

### Return type

[**List&lt;SitesManagementConnectionStringDto&gt;**](SitesManagementConnectionStringDto.md)

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

<a id="sitesmanagementapplicationconnectionstringremoveconnectionstring"></a>
# **SitesManagementApplicationConnectionStringRemoveConnectionString**
> bool SitesManagementApplicationConnectionStringRemoveConnectionString (SitesManagementRemoveConnectionStringRequestDto sitesManagementRemoveConnectionStringRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringRemoveConnectionStringExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var sitesManagementRemoveConnectionStringRequestDto = new SitesManagementRemoveConnectionStringRequestDto(); // SitesManagementRemoveConnectionStringRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementApplicationConnectionStringRemoveConnectionString(sitesManagementRemoveConnectionStringRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringRemoveConnectionString: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringRemoveConnectionStringWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementApplicationConnectionStringRemoveConnectionStringWithHttpInfo(sitesManagementRemoveConnectionStringRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringRemoveConnectionStringWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementRemoveConnectionStringRequestDto** | [**SitesManagementRemoveConnectionStringRequestDto**](SitesManagementRemoveConnectionStringRequestDto.md) |  | [optional]  |

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

<a id="sitesmanagementapplicationconnectionstringsetconnectionstring"></a>
# **SitesManagementApplicationConnectionStringSetConnectionString**
> void SitesManagementApplicationConnectionStringSetConnectionString (SitesManagementSetConnectionStringRequestDto sitesManagementSetConnectionStringRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringSetConnectionStringExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var sitesManagementSetConnectionStringRequestDto = new SitesManagementSetConnectionStringRequestDto(); // SitesManagementSetConnectionStringRequestDto |  (optional) 

            try
            {
                apiInstance.SitesManagementApplicationConnectionStringSetConnectionString(sitesManagementSetConnectionStringRequestDto);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringSetConnectionString: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringSetConnectionStringWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementApplicationConnectionStringSetConnectionStringWithHttpInfo(sitesManagementSetConnectionStringRequestDto);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringSetConnectionStringWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementSetConnectionStringRequestDto** | [**SitesManagementSetConnectionStringRequestDto**](SitesManagementSetConnectionStringRequestDto.md) |  | [optional]  |

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

<a id="sitesmanagementapplicationconnectionstringupdateconnectionstring"></a>
# **SitesManagementApplicationConnectionStringUpdateConnectionString**
> bool SitesManagementApplicationConnectionStringUpdateConnectionString (SitesManagementUpdateConnectionStringRequestDto sitesManagementUpdateConnectionStringRequestDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementApplicationConnectionStringUpdateConnectionStringExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new ApplicationConnectionStringApi(config);
            var sitesManagementUpdateConnectionStringRequestDto = new SitesManagementUpdateConnectionStringRequestDto(); // SitesManagementUpdateConnectionStringRequestDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementApplicationConnectionStringUpdateConnectionString(sitesManagementUpdateConnectionStringRequestDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringUpdateConnectionString: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementApplicationConnectionStringUpdateConnectionStringWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementApplicationConnectionStringUpdateConnectionStringWithHttpInfo(sitesManagementUpdateConnectionStringRequestDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling ApplicationConnectionStringApi.SitesManagementApplicationConnectionStringUpdateConnectionStringWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementUpdateConnectionStringRequestDto** | [**SitesManagementUpdateConnectionStringRequestDto**](SitesManagementUpdateConnectionStringRequestDto.md) |  | [optional]  |

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

