using AutoMapper;
using System;
using System.Linq;
using Volo.Abp.AutoMapper;
using VPortal.JobScheduler.Module.Actions;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Tasks;
using VPortal.JobScheduler.Module.Triggers;

namespace VPortal.JobScheduler.Module;

public class JobSchedulerApplicationAutoMapperProfile : Profile
{
  public JobSchedulerApplicationAutoMapperProfile()
  {
    CreateMap<ActionEntity, ActionDto>()
        .ForMember(dest => dest.ParentActionIds, opt => opt.MapFrom((src) => src.ParentActions != null ? src.ParentActions.Select(x => x.ParentActionId).ToList() : new List<Guid>()))
        .ForMember(dest => dest.RetryInterval, opt => opt.MapFrom(src => (int?)(src.RetryInterval ?? TimeSpan.FromSeconds(0)).TotalSeconds))
        .ReverseMap()
        .Ignore(x => x.Task)
        .ForMember(dest => dest.RetryInterval, opt => opt.MapFrom(src => TimeSpan.FromSeconds(src.RetryInterval ?? 0)))
        .ForMember(x => x.ParentActions, opt => opt.MapFrom(src => src.ParentActionIds != null ? src.ParentActionIds.Select(x => new ActionParentEntity(src.Id, x)).ToList() : new List<ActionParentEntity>()))
        .ForMember(i => i.Id, opt => opt.Condition(i => i.Id != Guid.Empty));

    CreateMap<TriggerEntity, TriggerDto>()
        .ReverseMap()
        .ForMember(i => i.Id, opt => opt.Condition(i => i.Id != Guid.Empty));
    CreateMap<TriggerEntity, TriggerDescriptionDto>();

    CreateMap<TaskExecutionEntity, TaskExecutionDto>()
        .ReverseMap()
        .Ignore(x => x.ActionExecutions);
    CreateMap<ActionExecutionEntity, ActionExecutionDto>()
        .ForMember(x => x.ParentActionExecutionIds, opt => opt.MapFrom((entity, dto) => entity.ParentActionExecutions != null ? entity.ParentActionExecutions.Select(x => x.ParentActionExecutionId).ToList() : new List<Guid>()));

    //CreateMap<TaskEntity, TaskDto>()
    //    .ForMember(x => x.TimeoutSeconds, opt => opt.MapFrom((entity, dto) => entity.Timeout?.TotalSeconds))
    //    .ForMember(x => x.RestartAfterFailIntervalSeconds, opt => opt.MapFrom((entity, dto) => entity.RestartAfterFailInterval?.TotalSeconds))
    //    .Ignore(x => x.LastDurationSeconds)
    //    .AfterMap(MapTaskEntityToDto)
    //    .ReverseMap()
    //    .Ignore(x => x.Actions)
    //    .ForMember(
    //        x => x.Timeout,
    //        opt => opt.MapFrom((dto, entity) => dto.TimeoutSeconds == null ? null : (TimeSpan?)TimeSpan.FromSeconds((int)dto.TimeoutSeconds)))
    //    .ForMember(
    //        x => x.RestartAfterFailInterval,
    //        opt => opt.MapFrom((dto, entity) => dto.RestartAfterFailIntervalSeconds == null
    //            ? null
    //            : (TimeSpan?)TimeSpan.FromSeconds((int)dto.RestartAfterFailIntervalSeconds)))
    //    .ForMember(i => i.Id, opt => opt.Condition(i => i.Id != Guid.Empty));

    CreateMap<TaskEntity, TaskHeaderDto>()
        .ForMember(x => x.TimeoutSeconds, opt => opt.MapFrom((entity, dto) => entity.Timeout?.TotalSeconds))
        .ForMember(x => x.RestartAfterFailIntervalSeconds, opt => opt.MapFrom((entity, dto) => entity.RestartAfterFailInterval?.TotalSeconds))
        .Ignore(x => x.LastDurationSeconds)
        .AfterMap(MapTaskEntityToDto)
        .ReverseMap()
        .Ignore(x => x.Actions)
        .ForMember(
            x => x.Timeout,
            opt => opt.MapFrom((dto, entity) => dto.TimeoutSeconds == null ? null : (TimeSpan?)TimeSpan.FromSeconds((int)dto.TimeoutSeconds)))
        .ForMember(
            x => x.RestartAfterFailInterval,
            opt => opt.MapFrom((dto, entity) => dto.RestartAfterFailIntervalSeconds == null
                ? null
                : (TimeSpan?)TimeSpan.FromSeconds((int)dto.RestartAfterFailIntervalSeconds)))
        .ForMember(i => i.Id, opt => opt.Condition(i => i.Id != Guid.Empty));
  }

  private static void MapTaskEntityToDto<T>(TaskEntity src, T dest) where T : TaskHeaderDto
  {
    //if (src.Executions == null || !src.Executions.Any())
    //{
    //  dest.LastDurationSeconds = null;
    //  return;
    //}
    //var latestExecution = src.Executions
    //    .OrderByDescending(x => x.FinishedAtUtc ?? DateTime.MaxValue)
    //    .ThenByDescending(x => x.StartedAtUtc ?? DateTime.MaxValue)
    //    .FirstOrDefault();
    //if (latestExecution == null || !latestExecution.StartedAtUtc.HasValue)
    //{
    //  dest.LastDurationSeconds = null;
    //  return;
    //}
    //var duration = (latestExecution.FinishedAtUtc ?? DateTime.UtcNow) - latestExecution.StartedAtUtc.Value;
    //dest.LastDurationSeconds = (int)duration.TotalSeconds;
  }
}
