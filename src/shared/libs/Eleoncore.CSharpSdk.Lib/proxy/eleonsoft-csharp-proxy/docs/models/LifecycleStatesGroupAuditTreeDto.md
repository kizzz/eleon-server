# EleonsoftProxy.Model.LifecycleStatesGroupAuditTreeDto

## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Id** | **Guid** |  | [optional] 
**DocumentId** | **string** |  | [optional] 
**DocumentObjectType** | **string** |  | [optional] 
**GroupName** | **string** |  | [optional] 
**CurrentStateOrderIndex** | **int** |  | [optional] 
**CreatorId** | **Guid** |  | [optional] 
**Status** | **EleoncoreLifecycleStatus** |  | [optional] 
**StatesGroupTemplateId** | **Guid** |  | [optional] 
**CurrentStatus** | [**LifecycleCurrentStatusDto**](LifecycleCurrentStatusDto.md) |  | [optional] 
**States** | [**List&lt;LifecycleStateAuditTreeDto&gt;**](LifecycleStateAuditTreeDto.md) |  | [optional] 
**CurrentState** | [**LifecycleStateAuditTreeDto**](LifecycleStateAuditTreeDto.md) |  | [optional] 

[[Back to Model list]](../../README.md#documentation-for-models) [[Back to API list]](../../README.md#documentation-for-api-endpoints) [[Back to README]](../../README.md)

