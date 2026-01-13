# EleonsoftProxy.Model.TenantManagementTenantSettingsCacheValueDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**TenantSetting** | [**TenantSettingsTenantSettingDto**](TenantSettingsTenantSettingDto.md) |  | [optional] 
**UserIsolationSettings** | [**List&lt;TenantManagementUserIsolationSettingsDto&gt;**](TenantManagementUserIsolationSettingsDto.md) |  | [optional] 
**AdminIds** | **List&lt;Guid&gt;** |  | [optional] 
**IsActive** | **bool** |  | [optional] [readonly] 
**TenantUrls** | **List&lt;string&gt;** |  | [optional] [readonly] 
**TenantAppearanceSetting** | [**TenantManagementTenantAppearanceSettingDto**](TenantManagementTenantAppearanceSettingDto.md) |  | [optional] 
**TenantHostnames** | **List&lt;string&gt;** |  | [optional] [readonly] 
**TenantSecureHostnames** | **List&lt;string&gt;** |  | [optional] [readonly] 
**TenantNonSecureHostnames** | **List&lt;string&gt;** |  | [optional] [readonly] 
**TenantCertificate** | **string** |  | [optional] [readonly] 
**TenantWhitelistedIps** | **List&lt;string&gt;** |  | [optional] [readonly] 
**LoginProviders** | [**List&lt;TenantSettingsTenantExternalLoginProviderDto&gt;**](TenantSettingsTenantExternalLoginProviderDto.md) |  | [optional] [readonly] 
**TenantContentSecurityHosts** | **List&lt;string&gt;** |  | [optional] [readonly] 
**EnabledProviders** | [**List&lt;EleoncoreExternalLoginProviderType&gt;**](EleoncoreExternalLoginProviderType.md) |  | [optional] [readonly] 
**CertificatesByUsersLookup** | **Dictionary&lt;string, string&gt;** |  | [optional] [readonly] 
**UsersByCertificatesLookup** | **Dictionary&lt;string, Guid&gt;** |  | [optional] [readonly] 
**AdminUserIds** | **List&lt;Guid&gt;** |  | [optional] [readonly] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

