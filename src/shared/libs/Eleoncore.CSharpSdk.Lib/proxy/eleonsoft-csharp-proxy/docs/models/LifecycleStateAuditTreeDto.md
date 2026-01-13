# EleonsoftProxy.Model.LifecycleStateAuditTreeDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**StatesGroupId** | **Guid** |  | [optional] 
**OrderIndex** | **int** |  | [optional] 
**IsActive** | **bool** |  | [optional] 
**StateName** | **string** |  | [optional] 
**IsMandatory** | **bool** |  | [optional] 
**IsReadOnly** | **bool** |  | [optional] 
**ApprovalType** | **int** |  | [optional] 
**CurrentActorOrderIndex** | **int** |  | [optional] 
**Status** | **EleoncoreLifecycleStatus** |  | [optional] 
**CurrentStatus** | [**LifecycleCurrentStatusDto**](LifecycleCurrentStatusDto.md) |  | [optional] 
**CreationTime** | **DateTime** |  | [optional] 
**Actors** | [**List&lt;LifecycleStateActorAuditTreeDto&gt;**](LifecycleStateActorAuditTreeDto.md) |  | [optional] 
**CurrentActor** | [**LifecycleStateActorAuditDto**](LifecycleStateActorAuditDto.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

