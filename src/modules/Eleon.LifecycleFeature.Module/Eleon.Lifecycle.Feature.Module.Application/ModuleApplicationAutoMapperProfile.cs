using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Common.Module.Constants;
using VPortal.Lifecycle.Feature.Module.Audits;
using VPortal.Lifecycle.Feature.Module.Conditions;
using VPortal.Lifecycle.Feature.Module.Dto.Audits;
using VPortal.Lifecycle.Feature.Module.Dto.Templates;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Entities.Conditions;
using VPortal.Lifecycle.Feature.Module.Entities.Templates;
using VPortal.Lifecycle.Feature.Module.LifecycleManager;
using VPortal.Lifecycle.Feature.Module.ValueObjects;

namespace VPortal.Lifecycle.Feature.Module;

public class ModuleApplicationAutoMapperProfile : Profile
{
  public ModuleApplicationAutoMapperProfile()
  {
    // CreateMap<StateActorTaskListSettingAuditEntity, StateActorTaskListSettingAuditDto>().ReverseMap();
    CreateMap<StateTemplateEntity, FullStateTemplateDto>();
    CreateMap<StatesGroupTemplateEntity, FullStatesGroupTemplateDto>();
    CreateMap<StateActorAuditEntity, StateActorAuditDto>().ReverseMap();
    CreateMap<StateAuditEntity, StateAuditDto>().ReverseMap();
    CreateMap<StatesGroupAuditEntity, StatesGroupAuditDto>().ReverseMap();
    CreateMap<StatesGroupAuditEntity, StatesGroupAuditReportDto>()
      .ForMember(
        x => x.StatusDate,
        opt => opt.MapFrom(src => src.CurrentState != null && src.CurrentState.CurrentActor != null ? src.CurrentState.CurrentActor.StatusDate : (DateTime?)null)
      )
      .ForMember(x => x.Role, opt => opt.Ignore());

    CreateMap<StateActorTaskListSettingTemplateEntity, StateActorTaskListSettingTemplateDto>()
      .ReverseMap();
    CreateMap<StateActorTemplateEntity, StateActorTemplateDto>().ReverseMap();
    CreateMap<StatesGroupTemplateEntity, StatesGroupTemplateDto>().ReverseMap();
    CreateMap<StateTemplateEntity, StateTemplateDto>().ReverseMap();

    CreateMap<StateActorAuditEntity, StateActorAuditTreeDto>().ReverseMap();
    CreateMap<StateAuditEntity, StateAuditTreeDto>().ReverseMap();
    CreateMap<StatesGroupAuditEntity, StatesGroupAuditTreeDto>().ReverseMap();
    CreateMap<KeyValuePair<LifecycleFinishedStatus, string>, CurrentStatusDto>().ReverseMap();

    CreateMap<LifecycleStatusValueObject, LifecycleStatusDto>().ReverseMap();

    // Condition mappings
    CreateMap<ConditionDto, ConditionEntity>()
      .ConstructUsing(src =>
        src.Id != Guid.Empty
          ? new ConditionEntity(src.Id) // Use provided ID for updates
          : new ConditionEntity(Guid.NewGuid())
      ) // Generate new ID for creates
      .ForMember(dest => dest.ConditionTargetType, opt => opt.MapFrom(src => src.ConditionType))
      .ForMember(dest => dest.ConditionType, opt => opt.MapFrom(src => LifecycleConditionType.And)) // Default value
      .ForMember(
        dest => dest.ConditionResultType,
        opt => opt.MapFrom(src => LifecycleConditionResultType.SkipOnSuccess)
      ) // Default value
      .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => true)) // Default value
      .ForMember(dest => dest.Id, opt => opt.Ignore()) // Already set by constructor
      .ForMember(dest => dest.TenantId, opt => opt.Ignore()) // Will be set by ABP
      .ForMember(
        dest => dest.Rules,
        opt =>
          opt.MapFrom(src =>
            src.Rules != null
              ? src
                .Rules.Select(r => new RuleEntity(Guid.NewGuid())
                {
                  Function = r.Function,
                  FunctionType = r.FunctionType,
                  IsEnabled = true,
                })
                .ToList()
              : new List<RuleEntity>()
          )
      ); // Map Rules, convert RuleDto to RuleEntity, default to empty list if null

    CreateMap<ConditionEntity, ConditionDto>()
      .ForMember(dest => dest.ConditionType, opt => opt.MapFrom(src => src.ConditionTargetType))
      .ForMember(dest => dest.Rules, opt => opt.MapFrom(src => src.Rules ?? new List<RuleEntity>()))
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)); // Map Id so RemoveCondition can use it

    // Rule mappings
    CreateMap<RuleDto, RuleEntity>()
      .ConstructUsing(src => new RuleEntity(Guid.NewGuid())) // Use constructor with generated ID
      .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => true)) // Default value
      .ForMember(dest => dest.Id, opt => opt.Ignore()) // Already set by constructor
      .ForMember(dest => dest.TenantId, opt => opt.Ignore()); // Will be set by ABP

    CreateMap<RuleEntity, RuleDto>();
  }
}
