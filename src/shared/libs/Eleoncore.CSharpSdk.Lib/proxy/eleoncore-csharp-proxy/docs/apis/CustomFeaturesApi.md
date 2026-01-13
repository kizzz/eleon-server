# EleoncoreProxy.Api.CustomFeaturesApi

All URIs are relative to *http://localhost*

| Method | HTTP request | Description |
|--------|--------------|-------------|
| [**SitesManagementCustomFeaturesCreateBulkForMicroservice**](CustomFeaturesApi.md#sitesmanagementcustomfeaturescreatebulkformicroservice) | **POST** /api/SitesManagement/CustomFeatures/CreateBulkForMicroserviceAsync |  |
| [**SitesManagementCustomFeaturesCreateFeature**](CustomFeaturesApi.md#sitesmanagementcustomfeaturescreatefeature) | **POST** /api/SitesManagement/CustomFeatures/CreateFeature |  |
| [**SitesManagementCustomFeaturesCreateGroup**](CustomFeaturesApi.md#sitesmanagementcustomfeaturescreategroup) | **POST** /api/SitesManagement/CustomFeatures/CreateGroup |  |
| [**SitesManagementCustomFeaturesDeleteFeature**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesdeletefeature) | **DELETE** /api/SitesManagement/CustomFeatures/DeleteFeature |  |
| [**SitesManagementCustomFeaturesDeleteGroup**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesdeletegroup) | **DELETE** /api/SitesManagement/CustomFeatures/DeleteGroup |  |
| [**SitesManagementCustomFeaturesGetAllFeatures**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesgetallfeatures) | **GET** /api/SitesManagement/CustomFeatures/GetAllFeatures |  |
| [**SitesManagementCustomFeaturesGetAllGroups**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesgetallgroups) | **GET** /api/SitesManagement/CustomFeatures/GetAllGroups |  |
| [**SitesManagementCustomFeaturesGetFeatureDynamicGroupCategories**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesgetfeaturedynamicgroupcategories) | **GET** /api/SitesManagement/CustomFeatures/GetFeatureGroups |  |
| [**SitesManagementCustomFeaturesGetFeaturesDynamic**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesgetfeaturesdynamic) | **GET** /api/SitesManagement/CustomFeatures/GetFeatures |  |
| [**SitesManagementCustomFeaturesGetSupportedValueTypes**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesgetsupportedvaluetypes) | **GET** /api/SitesManagement/CustomFeatures/GetSupportedValueTypes |  |
| [**SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdate**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesrequestfeaturespermissionsupdate) | **POST** /api/SitesManagement/CustomFeatures/RequestFeaturesPermissionsUpdate |  |
| [**SitesManagementCustomFeaturesUpdateFeature**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesupdatefeature) | **PUT** /api/SitesManagement/CustomFeatures/UpdateFeature |  |
| [**SitesManagementCustomFeaturesUpdateGroup**](CustomFeaturesApi.md#sitesmanagementcustomfeaturesupdategroup) | **PUT** /api/SitesManagement/CustomFeatures/UpdateGroup |  |

<a id="sitesmanagementcustomfeaturescreatebulkformicroservice"></a>
# **SitesManagementCustomFeaturesCreateBulkForMicroservice**
> bool SitesManagementCustomFeaturesCreateBulkForMicroservice (SitesManagementCustomFeatureForMicroserviceDto sitesManagementCustomFeatureForMicroserviceDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesCreateBulkForMicroserviceExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var sitesManagementCustomFeatureForMicroserviceDto = new SitesManagementCustomFeatureForMicroserviceDto(); // SitesManagementCustomFeatureForMicroserviceDto |  (optional) 

            try
            {
                bool result = apiInstance.SitesManagementCustomFeaturesCreateBulkForMicroservice(sitesManagementCustomFeatureForMicroserviceDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateBulkForMicroservice: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesCreateBulkForMicroserviceWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<bool> response = apiInstance.SitesManagementCustomFeaturesCreateBulkForMicroserviceWithHttpInfo(sitesManagementCustomFeatureForMicroserviceDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateBulkForMicroserviceWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomFeatureForMicroserviceDto** | [**SitesManagementCustomFeatureForMicroserviceDto**](SitesManagementCustomFeatureForMicroserviceDto.md) |  | [optional]  |

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

<a id="sitesmanagementcustomfeaturescreatefeature"></a>
# **SitesManagementCustomFeaturesCreateFeature**
> SitesManagementCustomFeatureDto SitesManagementCustomFeaturesCreateFeature (SitesManagementCustomFeatureDto sitesManagementCustomFeatureDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesCreateFeatureExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var sitesManagementCustomFeatureDto = new SitesManagementCustomFeatureDto(); // SitesManagementCustomFeatureDto |  (optional) 

            try
            {
                SitesManagementCustomFeatureDto result = apiInstance.SitesManagementCustomFeaturesCreateFeature(sitesManagementCustomFeatureDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateFeature: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesCreateFeatureWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomFeatureDto> response = apiInstance.SitesManagementCustomFeaturesCreateFeatureWithHttpInfo(sitesManagementCustomFeatureDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateFeatureWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomFeatureDto** | [**SitesManagementCustomFeatureDto**](SitesManagementCustomFeatureDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomFeatureDto**](SitesManagementCustomFeatureDto.md)

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

<a id="sitesmanagementcustomfeaturescreategroup"></a>
# **SitesManagementCustomFeaturesCreateGroup**
> SitesManagementCustomFeatureGroupDto SitesManagementCustomFeaturesCreateGroup (SitesManagementCustomFeatureGroupDto sitesManagementCustomFeatureGroupDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesCreateGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var sitesManagementCustomFeatureGroupDto = new SitesManagementCustomFeatureGroupDto(); // SitesManagementCustomFeatureGroupDto |  (optional) 

            try
            {
                SitesManagementCustomFeatureGroupDto result = apiInstance.SitesManagementCustomFeaturesCreateGroup(sitesManagementCustomFeatureGroupDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesCreateGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomFeatureGroupDto> response = apiInstance.SitesManagementCustomFeaturesCreateGroupWithHttpInfo(sitesManagementCustomFeatureGroupDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesCreateGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomFeatureGroupDto** | [**SitesManagementCustomFeatureGroupDto**](SitesManagementCustomFeatureGroupDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomFeatureGroupDto**](SitesManagementCustomFeatureGroupDto.md)

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

<a id="sitesmanagementcustomfeaturesdeletefeature"></a>
# **SitesManagementCustomFeaturesDeleteFeature**
> void SitesManagementCustomFeaturesDeleteFeature (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesDeleteFeatureExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                apiInstance.SitesManagementCustomFeaturesDeleteFeature(name);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesDeleteFeature: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesDeleteFeatureWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementCustomFeaturesDeleteFeatureWithHttpInfo(name);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesDeleteFeatureWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

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

<a id="sitesmanagementcustomfeaturesdeletegroup"></a>
# **SitesManagementCustomFeaturesDeleteGroup**
> void SitesManagementCustomFeaturesDeleteGroup (string name = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesDeleteGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var name = "name_example";  // string |  (optional) 

            try
            {
                apiInstance.SitesManagementCustomFeaturesDeleteGroup(name);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesDeleteGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesDeleteGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementCustomFeaturesDeleteGroupWithHttpInfo(name);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesDeleteGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **name** | **string** |  | [optional]  |

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

<a id="sitesmanagementcustomfeaturesgetallfeatures"></a>
# **SitesManagementCustomFeaturesGetAllFeatures**
> List&lt;SitesManagementCustomFeatureDto&gt; SitesManagementCustomFeaturesGetAllFeatures ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesGetAllFeaturesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                List<SitesManagementCustomFeatureDto> result = apiInstance.SitesManagementCustomFeaturesGetAllFeatures();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetAllFeatures: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesGetAllFeaturesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomFeatureDto>> response = apiInstance.SitesManagementCustomFeaturesGetAllFeaturesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetAllFeaturesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomFeatureDto&gt;**](SitesManagementCustomFeatureDto.md)

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

<a id="sitesmanagementcustomfeaturesgetallgroups"></a>
# **SitesManagementCustomFeaturesGetAllGroups**
> List&lt;SitesManagementCustomFeatureGroupDto&gt; SitesManagementCustomFeaturesGetAllGroups ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesGetAllGroupsExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                List<SitesManagementCustomFeatureGroupDto> result = apiInstance.SitesManagementCustomFeaturesGetAllGroups();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetAllGroups: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesGetAllGroupsWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomFeatureGroupDto>> response = apiInstance.SitesManagementCustomFeaturesGetAllGroupsWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetAllGroupsWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomFeatureGroupDto&gt;**](SitesManagementCustomFeatureGroupDto.md)

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

<a id="sitesmanagementcustomfeaturesgetfeaturedynamicgroupcategories"></a>
# **SitesManagementCustomFeaturesGetFeatureDynamicGroupCategories**
> List&lt;SitesManagementCustomFeatureGroupDto&gt; SitesManagementCustomFeaturesGetFeatureDynamicGroupCategories ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesGetFeatureDynamicGroupCategoriesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                List<SitesManagementCustomFeatureGroupDto> result = apiInstance.SitesManagementCustomFeaturesGetFeatureDynamicGroupCategories();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetFeatureDynamicGroupCategories: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesGetFeatureDynamicGroupCategoriesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomFeatureGroupDto>> response = apiInstance.SitesManagementCustomFeaturesGetFeatureDynamicGroupCategoriesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetFeatureDynamicGroupCategoriesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomFeatureGroupDto&gt;**](SitesManagementCustomFeatureGroupDto.md)

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

<a id="sitesmanagementcustomfeaturesgetfeaturesdynamic"></a>
# **SitesManagementCustomFeaturesGetFeaturesDynamic**
> List&lt;SitesManagementCustomFeatureDto&gt; SitesManagementCustomFeaturesGetFeaturesDynamic ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesGetFeaturesDynamicExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                List<SitesManagementCustomFeatureDto> result = apiInstance.SitesManagementCustomFeaturesGetFeaturesDynamic();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetFeaturesDynamic: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesGetFeaturesDynamicWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<List<SitesManagementCustomFeatureDto>> response = apiInstance.SitesManagementCustomFeaturesGetFeaturesDynamicWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetFeaturesDynamicWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

[**List&lt;SitesManagementCustomFeatureDto&gt;**](SitesManagementCustomFeatureDto.md)

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

<a id="sitesmanagementcustomfeaturesgetsupportedvaluetypes"></a>
# **SitesManagementCustomFeaturesGetSupportedValueTypes**
> Dictionary&lt;string, string&gt; SitesManagementCustomFeaturesGetSupportedValueTypes ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesGetSupportedValueTypesExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                Dictionary<string, string> result = apiInstance.SitesManagementCustomFeaturesGetSupportedValueTypes();
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetSupportedValueTypes: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesGetSupportedValueTypesWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<Dictionary<string, string>> response = apiInstance.SitesManagementCustomFeaturesGetSupportedValueTypesWithHttpInfo();
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesGetSupportedValueTypesWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
### Return type

**Dictionary<string, string>**

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

<a id="sitesmanagementcustomfeaturesrequestfeaturespermissionsupdate"></a>
# **SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdate**
> void SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdate ()



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdateExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);

            try
            {
                apiInstance.SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdate();
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdate: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdateWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    apiInstance.SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdateWithHttpInfo();
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesRequestFeaturesPermissionsUpdateWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters
This endpoint does not need any parameter.
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

<a id="sitesmanagementcustomfeaturesupdatefeature"></a>
# **SitesManagementCustomFeaturesUpdateFeature**
> SitesManagementCustomFeatureDto SitesManagementCustomFeaturesUpdateFeature (SitesManagementCustomFeatureDto sitesManagementCustomFeatureDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesUpdateFeatureExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var sitesManagementCustomFeatureDto = new SitesManagementCustomFeatureDto(); // SitesManagementCustomFeatureDto |  (optional) 

            try
            {
                SitesManagementCustomFeatureDto result = apiInstance.SitesManagementCustomFeaturesUpdateFeature(sitesManagementCustomFeatureDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesUpdateFeature: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesUpdateFeatureWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomFeatureDto> response = apiInstance.SitesManagementCustomFeaturesUpdateFeatureWithHttpInfo(sitesManagementCustomFeatureDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesUpdateFeatureWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomFeatureDto** | [**SitesManagementCustomFeatureDto**](SitesManagementCustomFeatureDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomFeatureDto**](SitesManagementCustomFeatureDto.md)

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

<a id="sitesmanagementcustomfeaturesupdategroup"></a>
# **SitesManagementCustomFeaturesUpdateGroup**
> SitesManagementCustomFeatureGroupDto SitesManagementCustomFeaturesUpdateGroup (SitesManagementCustomFeatureGroupDto sitesManagementCustomFeatureGroupDto = null)



### Example
```csharp
using System.Collections.Generic;
using System.Diagnostics;
using EleoncoreProxy.Api;
using EleoncoreProxy.Client;
using EleoncoreProxy.Model;

namespace Example
{
    public class SitesManagementCustomFeaturesUpdateGroupExample
    {
        public static void Main()
        {
            Configuration config = new Configuration();
            config.BasePath = "http://localhost";
            // Configure OAuth2 access token for authorization: oauth2
            config.AccessToken = "YOUR_ACCESS_TOKEN";

            var apiInstance = new CustomFeaturesApi(config);
            var sitesManagementCustomFeatureGroupDto = new SitesManagementCustomFeatureGroupDto(); // SitesManagementCustomFeatureGroupDto |  (optional) 

            try
            {
                SitesManagementCustomFeatureGroupDto result = apiInstance.SitesManagementCustomFeaturesUpdateGroup(sitesManagementCustomFeatureGroupDto);
                Debug.WriteLine(result);
            }
            catch (ApiException  e)
            {
                Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesUpdateGroup: " + e.Message);
                Debug.Print("Status Code: " + e.ErrorCode);
                Debug.Print(e.StackTrace);
            }
        }
    }
}
```

#### Using the SitesManagementCustomFeaturesUpdateGroupWithHttpInfo variant
This returns an ApiResponse object which contains the response data, status code and headers.

```csharp
try
{
    ApiResponse<SitesManagementCustomFeatureGroupDto> response = apiInstance.SitesManagementCustomFeaturesUpdateGroupWithHttpInfo(sitesManagementCustomFeatureGroupDto);
    Debug.Write("Status Code: " + response.StatusCode);
    Debug.Write("Response Headers: " + response.Headers);
    Debug.Write("Response Body: " + response.Data);
}
catch (ApiException e)
{
    Debug.Print("Exception when calling CustomFeaturesApi.SitesManagementCustomFeaturesUpdateGroupWithHttpInfo: " + e.Message);
    Debug.Print("Status Code: " + e.ErrorCode);
    Debug.Print(e.StackTrace);
}
```

### Parameters

| Name | Type | Description | Notes |
|------|------|-------------|-------|
| **sitesManagementCustomFeatureGroupDto** | [**SitesManagementCustomFeatureGroupDto**](SitesManagementCustomFeatureGroupDto.md) |  | [optional]  |

### Return type

[**SitesManagementCustomFeatureGroupDto**](SitesManagementCustomFeatureGroupDto.md)

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

