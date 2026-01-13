using Common.Module.Constants;
using Logging.Module;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;
using VPortal.JobScheduler.Module.Entities;
using VPortal.JobScheduler.Module.Repositories;

namespace VPortal.JobScheduler.Module.DomainServices
{
    
    public class ActionDomainService : DomainService
    {
        private readonly IVportalLogger<ActionDomainService> logger;
        private readonly IActionRepository repository;
        private readonly ITaskRepository _taskRepository;

        public ActionDomainService(
            IVportalLogger<ActionDomainService> logger, IActionRepository repository, ITaskRepository taskRepository)
        {
            this.logger = logger;
            this.repository = repository;
            _taskRepository = taskRepository;
        }

        public async Task<ActionEntity> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await repository.GetAsync(id, true);
                return entity;
            }
            catch (Exception e)
            {
                logger.Capture(e);
                throw;
            }
            finally
            {
            }
        }

        public async Task<KeyValuePair<int, List<ActionEntity>>> GetListAsync(
            Guid? taskId = null,
            string nameFilter = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0)
        {
            try
            {
                return await repository.GetListAsync(taskId, nameFilter, sorting, maxResultCount, skipCount);
            }
            catch (Exception e)
            {
                logger.Capture(e);
                throw;
            }
            finally
            {
            }
        }

        public async Task<ActionEntity> AddAsync(Guid taskId, ActionEntity action)
        {
            try
            {
                action.ActionExtraParams ??= string.Empty;
                action.OnFailureRecepients ??= string.Empty;
                var task = await _taskRepository.GetAsync(taskId, true);

                if (task.Status == JobSchedulerTaskStatus.Running)
                {
                    throw new UserFriendlyException("Cannot add action while task is running.", JobSchedulerErrorCodes.NowAllowedWhenTaskRunning);
                }

                task.Actions ??= new List<ActionEntity>();
                task.Actions.Add(action);

                ValidateActionParams(action);
                EnsureCorrectActionOrder(task);

                var createdAction = await repository.InsertAsync(action, true);
                return createdAction;
            }
            catch (Exception e)
            {
                logger.Capture(e);
                throw;
            }
            finally
            {
            }
        }

        public async Task<ActionEntity> UpdateAsync(ActionEntity action)
        {
            try
            {
                var trackedEntity = await repository.GetAsync(action.Id, true);
                var task = await _taskRepository.GetAsync(trackedEntity.TaskId, true);

                if (task.Status == JobSchedulerTaskStatus.Running)
                {
                    throw new UserFriendlyException("Cannot update action while task is running.", JobSchedulerErrorCodes.NowAllowedWhenTaskRunning);
                }

                ValidateActionParams(action);

                trackedEntity.DisplayName = action.DisplayName;
                trackedEntity.EventName = action.EventName;
                trackedEntity.ActionParams = action.ActionParams;
                trackedEntity.ActionExtraParams = action.ActionExtraParams ?? string.Empty;
                trackedEntity.RetryInterval = action.RetryInterval;
                trackedEntity.MaxRetryAttempts = action.MaxRetryAttempts;
                trackedEntity.TimeoutInMinutes = action.TimeoutInMinutes;
                trackedEntity.OnFailureRecepients = action.OnFailureRecepients ?? string.Empty;
                trackedEntity.ParamsFormat = action.ParamsFormat;

                trackedEntity.ParentActions.Clear();
                trackedEntity.ParentActions.AddRange(action.ParentActions.Select(x => new ActionParentEntity(x.ChildActionId, x.ParentActionId)));

                EnsureCorrectActionOrder(task);

                var result = await repository.UpdateAsync(trackedEntity, true);
                return result;
            }
            catch (Exception e)
            {
                logger.Capture(e);
                throw;
            }
            finally
            {
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var dbContext = await repository.GetDbContextAsync();
                var trackedEntity = await repository.GetAsync(id, true);
                var task = await _taskRepository.GetAsync(trackedEntity.TaskId, true);

                if (task.Status == JobSchedulerTaskStatus.Running)
                {
                    throw new UserFriendlyException("Cannot delete action while task is running.", JobSchedulerErrorCodes.NowAllowedWhenTaskRunning);
                }

                // removing action from parent actions
                await dbContext.Set<ActionParentEntity>().Where(x => x.ParentActionId == trackedEntity.Id).ExecuteDeleteAsync();
                await repository.DeleteAsync(trackedEntity);

                if (!task.Actions.Where(x => x.Id != trackedEntity.Id).Any())
                {
                    task.IsActive = false;
                    task.Status = JobSchedulerTaskStatus.Inactive;
                    await _taskRepository.UpdateAsync(task, true);
                }

                await dbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                logger.Capture(e);
                return false;
            }
            finally
            {
            }
        }

    private void ValidateActionParams(ActionEntity action)
    {
      if (action.ParamsFormat == TextFormat.Json)
      {
        try
        {
          JsonDocument.Parse(action.ActionParams);
        }
        catch (JsonException)
        {
          throw new UserFriendlyException("Non-json action params", JobSchedulerErrorCodes.NonValidActionParameters);
        }
      }
    }

        private void EnsureCorrectActionOrder(TaskEntity task)
        {
            // Build adjacency list for actions - handle duplicates by taking the first occurrence
            var actionDict = task.Actions
                .GroupBy(a => a.Id)
                .ToDictionary(g => g.Key, g => g.First());
            var graph = new Dictionary<Guid, List<Guid>>();
            foreach (var action in task.Actions)
            {
                if (!graph.ContainsKey(action.Id))
                    graph[action.Id] = new List<Guid>();
                foreach (var parent in action.ParentActions)
                {
                    // parent.ParentActionId -> parent.ChildActionId (current action)
                    if (actionDict.ContainsKey(parent.ParentActionId))
                    {
                        if (!graph.ContainsKey(parent.ParentActionId))
                            graph[parent.ParentActionId] = new List<Guid>();
                        graph[parent.ParentActionId].Add(parent.ChildActionId);
                    }
                }
            }

            // Detect cycles using DFS
            var visited = new HashSet<Guid>();
            var recStack = new HashSet<Guid>();

            bool HasCycle(Guid node)
            {
                if (!visited.Contains(node))
                {
                    visited.Add(node);
                    recStack.Add(node);

                    if (graph.TryGetValue(node, out var neighbors))
                    {
                        foreach (var neighbor in neighbors)
                        {
                            if (!visited.Contains(neighbor) && HasCycle(neighbor))
                                return true;
                            else if (recStack.Contains(neighbor))
                                return true;
                        }
                    }
                }
                recStack.Remove(node);
                return false;
            }

            foreach (var actionId in graph.Keys)
            {
                if (HasCycle(actionId))
                    throw new UserFriendlyException("Cyclic action reference detected in task actions.", JobSchedulerErrorCodes.CyclicActionDependency);
            }
        }
    }
}
