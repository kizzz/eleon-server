# EleonsoftProxy.Model.TenantSettingsTenantSettingDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TenantId** | **Guid** |  | [optional] 
**TenantIsolationEnabled** | **bool** |  | [optional] 
**TenantCertificateHash** | **string** |  | [optional] 
**IpIsolationEnabled** | **bool** |  | [optional] 
**Status** | **EleoncoreTenantStatus** |  | [optional] 
**Hostnames** | [**List&lt;TenantSettingsTenantHostnameDto&gt;**](TenantSettingsTenantHostnameDto.md) |  | [optional] 
**ExternalProviders** | [**List&lt;TenantSettingsTenantExternalLoginProviderDto&gt;**](TenantSettingsTenantExternalLoginProviderDto.md) |  | [optional] 
**WhitelistedIps** | [**List&lt;TenantSettingsTenantWhitelistedIpDto&gt;**](TenantSettingsTenantWhitelistedIpDto.md) |  | [optional] 
**ContentSecurityHosts** | [**List&lt;TenantSettingsTenantContentSecurityHostDto&gt;**](TenantSettingsTenantContentSecurityHostDto.md) |  | [optional] 
**AppearanceSettings** | [**TenantManagementTenantAppearanceSettingDto**](TenantManagementTenantAppearanceSettingDto.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

