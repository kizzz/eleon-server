# EleonsoftProxy.Model.AccountingAccountListRequestDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MaxResultCount** | **int** |  | [optional] 
**SkipCount** | **int** |  | [optional] 
**Sorting** | **string** |  | [optional] 
**SearchQuery** | **string** |  | [optional] 
**RequestType** | **AccountingAccountListRequestType** |  | [optional] 
**CreationDateFilterStart** | **DateTime** |  | [optional] 
**CreationDateFilterEnd** | **DateTime** |  | [optional] 
**InitiatorNameFilter** | **string** |  | [optional] 
**AccountStatusFilter** | [**List&lt;EleoncoreAccountStatus&gt;**](EleoncoreAccountStatus.md) |  | [optional] 
**DocumentStatusFilter** | [**List&lt;EleoncoreDocumentStatuses&gt;**](EleoncoreDocumentStatuses.md) |  | [optional] 
**ActorTypeFilter** | **EleoncoreLifecycleActorTypes** |  | [optional] 
**ActorRefIdFilter** | **string** |  | [optional] 
**ApprovalNeededFilter** | **bool** |  | [optional] 
**AccountTypeFilter** | [**List&lt;EleoncoreAccountType&gt;**](EleoncoreAccountType.md) |  | [optional] 
**OrganizationUnitFilter** | **List&lt;Guid&gt;** |  | [optional] 
**ResellerTypeFilter** | [**List&lt;EleoncoreResellerType&gt;**](EleoncoreResellerType.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

