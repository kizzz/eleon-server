using Common.Module.Constants;
using EleonsoftSdk.modules.Messaging.Module.SystemMessages.Notificator.NotificationType;
using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using SharedModule.modules.Helpers.Module;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VPortal.Infrastructure.Module.Domain.DomainServices;
using VPortal.Lifecycle.Feature.Module.Entities;
using VPortal.Lifecycle.Feature.Module.Localization;
using VPortal.Lifecycle.Feature.Module.Repositories.Audits;
using VPortal.Lifecycle.Feature.Module.ValueObjects;

namespace VPortal.Lifecycle.Feature.Module.DomainServices
{
  public class LifecycleManagerDomainService : DomainService
  {
    private readonly ICurrentTenant currentTenant;
    private readonly ICurrentUser currentUser;
    private readonly IVportalLogger<LifecycleManagerDomainService> logger;
    private readonly IStatesGroupAuditsRepository statesGroupAuditsRepository;
    private readonly IdentityUserManager identityUserManager;
    private readonly IStringLocalizer<LifecycleFeatureModuleResource> localizer;
    private readonly ConditionDomainService conditionsDomainService;
    private readonly IdentityRoleManager identityRoleManager;
    private readonly IUnitOfWorkManager unitOfWorkManager;
    private readonly IDistributedEventBus distributedEventBus;
    private readonly StatesGroupAuditDomainService statesGroupAuditDomainService;
    private readonly IPermissionChecker permissionChecker;
    private readonly StatesGroupTemplateDomainService statesGroupTemplateDomainService;

    public LifecycleManagerDomainService(
      ICurrentTenant currentTenant,
      ICurrentUser currentUser,
      IStatesGroupAuditsRepository statesGroupAuditsRepository,
      IVportalLogger<LifecycleManagerDomainService> logger,
      IdentityUserManager identityUserManager,
      IStringLocalizer<LifecycleFeatureModuleResource> localizer,
      ConditionDomainService conditionsDomainService,
      IdentityRoleManager identityRoleManager,
      IUnitOfWorkManager unitOfWorkManager,
      IDistributedEventBus distributedEventBus,
      StatesGroupAuditDomainService statesGroupAuditDomainService,
      IPermissionChecker permissionChecker,
      StatesGroupTemplateDomainService statesGroupTemplateDomainService
    )
    {
      this.currentTenant = currentTenant;
      this.currentUser = currentUser;
      this.logger = logger;
      this.statesGroupAuditsRepository = statesGroupAuditsRepository;
      this.identityUserManager = identityUserManager;
      this.localizer = localizer;
      this.conditionsDomainService = conditionsDomainService;
      this.identityRoleManager = identityRoleManager;
      this.unitOfWorkManager = unitOfWorkManager;
      this.distributedEventBus = distributedEventBus;
      this.statesGroupAuditDomainService = statesGroupAuditDomainService;
      this.permissionChecker = permissionChecker;
      this.statesGroupTemplateDomainService = statesGroupTemplateDomainService;
    }

    public async Task<StatesGroupAuditEntity> GetTrace(string documentObjectType, string documentId)
    {
      StatesGroupAuditEntity result = null;
      try
      {
        var tree = await statesGroupAuditsRepository.GetByDocIdAsync(
          documentObjectType,
          documentId
        );
        if (tree == null)
        {
          return null;
        }

        var currentUserId = currentUser.Id;

        // TODO: check permissions
        //if (currentUserId != tree.CreatorId &&
        //    !await Review(documentObjectType, documentId) &&
        //    !await GetUserLifecycleApprovalPermission(documentObjectType, documentId))
        //{
        //    await CheckPermissionToSeeTrace(documentObjectType);
        //}

        foreach (var state in tree.States)
        {
          foreach (var stateActor in state.Actors)
          {
            await SetActorDisplayName(stateActor);
          }
        }

        if (tree.CurrentState != null && tree.CurrentState.CurrentActor != null)
        {
          await SetActorDisplayName(tree.CurrentState.CurrentActor);
        }

        result = tree;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CanReview(string documentObjectType, string documentId)
    {
      bool result;
      try
      {
        result = await Review(documentObjectType, documentId, false);
      }
      catch (Exception e)
      {
        logger.Capture(e);
        throw;
      }
      finally
      {
      }
      return result;
    }

    public async Task<StatesGroupAuditEntity> StartExistingLifecycle(
      string documentObjectType,
      string documentId
    )
    {
      StatesGroupAuditEntity result = null;
      try
      {
        using (var unitOfWork = unitOfWorkManager.Begin())
        {
          result = await statesGroupAuditsRepository.GetByDocIdAsync(
            documentObjectType,
            documentId
          );

          bool noApprovers =
            result == null
            || result.States == null
            || result.States.Count <= 0
            || !result.States.Any(s => s.Actors.Count > 0)
            || result.States.All(x => !x.IsActive)
            || result
              .States.Where(x =>
                x.IsActive && x.Actors.Count() > 0 && x.Actors.Any(s => s.IsActive)
              )
              .ToList()
              .Count() <= 0;

          if (noApprovers)
          {
            if (result != null)
            {
              result = await statesGroupAuditDomainService.ForceComplete(
                documentObjectType,
                documentId
              );
            }

            await FinishLifecycle(
              documentId,
              "",
              LifecycleFinishedStatus.Approved,
              documentObjectType
            );

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CompleteAsync();

            return result;
          }

          result.Status = LifecycleStatus.Enroute;
          result = await RecognizeCurrentEnrouteState(result);
          await unitOfWork.SaveChangesAsync();
          var notificationGuid = Guid.NewGuid();
          var notificationMessage = new NotificatorRequestedMsg
          {
            Notification = new EleonsoftNotification()
            {
              Id = notificationGuid,
              Message = $"Lifecycle::ApprovalNotification:Enroute",
              Type = new MessageNotificationType
              {
                IsRedirectEnabled = false,
                LanguageKeyParams = new List<string>() { result.DocumentId },
              },
              Recipients = new List<RecipientEto>()
              {
                new RecipientEto()
                {
                  Type = NotificatorRecepientType.User,
                  RefId = result.CreatorId.ToString(),
                },
              },
            },
          };

          await distributedEventBus.PublishAsync(notificationMessage);

          if (result.DocumentObjectType != "PurchaseRequest")
          {
            var chatMessage = new SendDocumentChatMessagesMsg
            {
              Messages =
              [
                new()
                {
                  DocumentId = result.DocumentId,
                  DocumentObjectType = result.DocumentObjectType,
                  LocalizationKey = "Collaboration::Message:ChatDocumentInitialized",
                  LocalizationParams = new string[]
                  {
                    result.DocumentId,
                    result.CurrentState.CurrentActor.ActorName,
                  },
                  MessageSeverity = ChatMessageSeverity.Success,
                },
              ],
            };

            await distributedEventBus.PublishAsync(chatMessage);
          }

          await unitOfWork.SaveChangesAsync();
          await unitOfWork.CompleteAsync();
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<StatesGroupAuditEntity> StartNewLifecycle(
      Guid templateId,
      string documentObjectType,
      string documentId,
      Dictionary<string, object> extras = null,
      bool isSkipFilled = false,
      bool isStartImmediatly = false
    )
    {
      StatesGroupAuditEntity result = null;
      try
      {
        var groupTemplate = await statesGroupTemplateDomainService.GetAsync(templateId);
        if (groupTemplate == null)
        {
          return result;
        }

        using (var unitOfWork = unitOfWorkManager.Begin())
        {
          var checkIfExist = await statesGroupAuditsRepository.GetByDocIdAsync(
            documentObjectType,
            documentId
          );
          if (checkIfExist != null)
          {
            await statesGroupAuditDomainService.DeepCancel(documentObjectType, documentId);
          }

          var groupAudit = await InitializeAuditFromTemplate(groupTemplate, false);
          groupAudit.DocumentId = documentId.ToString();
          groupAudit.DocumentObjectType = documentObjectType;

          if (extras != null)
          {
            foreach (var keyValue in extras)
            {
              groupAudit.SetProperty(keyValue.Key, keyValue.Value);
            }
          }

          await unitOfWork.SaveChangesAsync();

          if (isStartImmediatly || isSkipFilled)
          {
            var lifecycleMessage = new StartLifecycleMsg
            {
              DocumentId = documentId.ToString(),
              DocumentObjectType = documentObjectType,
            };
            await distributedEventBus.PublishAsync(lifecycleMessage);
          }

          if (isSkipFilled)
          {
            result = await statesGroupAuditDomainService.ForceComplete(
              documentObjectType,
              documentId
            );
          }

          await unitOfWork.SaveChangesAsync();
          await unitOfWork.CompleteAsync();

          result = groupAudit;
        }

        if (result.Status == LifecycleStatus.Complete)
        {
          await FinishLifecycle(
            documentId,
            "",
            result.CurrentStatus.Key,
            result.DocumentObjectType
          );
          List<string> data = new List<string>
          {
            result.CurrentStatus.Key.ToString(),
            result.CurrentStatus.Value,
          };
          string url = await UrlManagerDomainService.GetDocumentUrl(
            result.DocumentId,
            result.TenantId
          );
          data.Add(url);
          await NotifyActor(
            new RecipientEto()
            {
              RefId = result.CreatorId.ToString(),
              Type = NotificatorRecepientType.User,
            },
            ModuleTemplateConsts.LifecycleComplete,
            data.JoinAsString(",")
          );
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> GetUserLifecycleApprovalPermission(
      string documentObjectType,
      string documentId
    )
    {
      bool result = false;
      try
      {
        var groupAudit = await statesGroupAuditsRepository.GetByDocIdAsync(
          documentObjectType,
          documentId
        );
        if (groupAudit == null)
        {
          return false;
        }

        if (groupAudit.Status == LifecycleStatus.New)
        {
          return false;
        }

        if (groupAudit.Status == LifecycleStatus.Enroute)
        {
          var state = await GetStateByOrderIndex(
            groupAudit.States,
            groupAudit.CurrentStateOrderIndex
          );
          if (state is null)
          {
            return false;
          }

          if (state.ApprovalType == LifecycleApprovalType.Regular)
          {
            var actor = await GetActorByOrderIndex(state.Actors, state.CurrentActorOrderIndex);
            if (actor is null)
            {
              return false;
            }

            if (await CheckActorForCurrentUser(groupAudit, actor))
            {
              result = true;
            }
          }
          else if (
            state.ApprovalType == LifecycleApprovalType.Parallel
            || state.ApprovalType == LifecycleApprovalType.AtLeast
          )
          {
            var actors = state.Actors.Where(actor => actor.IsEnroute);
            foreach (var actor in actors)
            {
              if (await CheckActorForCurrentUser(groupAudit, actor))
              {
                result = true;
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> GetViewPermission(
      string documentObjectType,
      string documentId,
      bool review = true
    )
    {
      bool result = false;
      try
      {
        result =
          await Review(documentObjectType, documentId, review)
          || await GetUserLifecycleApprovalPermission(documentObjectType, documentId);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> ChangeApprovmentStatus(
      string documentObjectType,
      string documentId,
      string reason,
      bool approved
    )
    {
      bool result = false;
      try
      {
        return await unitOfWorkManager.ExecuteWithConcurrencyHandlingAsync(
          async uow =>
          {
            var status = approved ? LifecycleActorStatus.Approved : LifecycleActorStatus.Rejected;
            var groupAudit = await statesGroupAuditsRepository.GetByDocIdAsync(
              documentObjectType,
              documentId
            );
            if (groupAudit == null || groupAudit.Status == LifecycleStatus.New)
            {
              await uow.CompleteAsync();
              return false;
            }

            StateAuditEntity state = null;
            if (groupAudit.Status == LifecycleStatus.Enroute)
            {
              state = await GetStateByOrderIndex(
                groupAudit.States,
                groupAudit.CurrentStateOrderIndex
              );
              if (state == null)
              {
                await uow.CompleteAsync();
                return false;
              }

              if (state.ApprovalType == LifecycleApprovalType.Regular)
              {
                var actorCheck = await GetActorByOrderIndex(
                  state.Actors,
                  state.CurrentActorOrderIndex
                );
                if (actorCheck == null)
                {
                  await uow.CompleteAsync();
                  return false;
                }
              }
              else if (
                state.ApprovalType == LifecycleApprovalType.Parallel
                || state.ApprovalType == LifecycleApprovalType.AtLeast
              )
              {
                if (!state.Actors.Any(actor => actor.IsEnroute))
                {
                  await uow.CompleteAsync();
                  return false;
                }
              }
            }

            if (!await GetUserLifecycleApprovalPermission(documentObjectType, documentId))
            {
              throw new Exception("You don't  have a permission to change approvment status");
            }

            StateActorAuditEntity currentActor = null;
            if (groupAudit.Status == LifecycleStatus.Enroute)
            {
              if (state.ApprovalType == LifecycleApprovalType.Regular)
              {
                currentActor = await GetActorByOrderIndex(
                  state.Actors,
                  state.CurrentActorOrderIndex
                );
                if (await CheckActorForCurrentUser(groupAudit, currentActor))
                {
                  if (
                    await conditionsDomainService.CheckShouldSkip(
                      groupAudit.DocumentObjectType,
                      groupAudit.DocumentId,
                      LifecycleConditionTargetType.Actor,
                      currentActor.Id
                    )
                  )
                  {
                    currentActor.Status = LifecycleActorStatus.Canceled;
                    await RecognizeCurrentEnrouteState(groupAudit);
                    await uow.CompleteAsync();
                    return true;
                  }

                  // Idempotency: check if already in desired state
                  if (IdempotencyHelpers.IsStatusEquals(currentActor.Status, status))
                  {
                    logger.Log.LogWarning(
                      "Actor {ActorId} for document {DocumentId} already in status {Status}. Treating as idempotent success.",
                      currentActor.Id,
                      documentId,
                      status
                    );
                    await uow.CompleteAsync();
                    return true;
                  }

                  // Only update if not already in a final state
                  if (currentActor.Status != LifecycleActorStatus.Enroute)
                  {
                    logger.Log.LogWarning(
                      "Actor {ActorId} for document {DocumentId} already processed (Status: {Status}). Cannot change to {DesiredStatus}.",
                      currentActor.Id,
                      documentId,
                      currentActor.Status,
                      status
                    );
                    await uow.CompleteAsync();
                    return false;
                  }

                  currentActor.StatusDate = DateTime.Now;
                  currentActor.StatusUserId = (Guid)currentUser.Id;
                  currentActor.StatusUserName = $"{currentUser.Name} {currentUser.SurName}";
                  currentActor.Status = status;
                  currentActor.Reason = reason;
                  state.LastStatusDate = DateTime.Now;
                  await RecognizeCurrentEnrouteState(groupAudit);
                  result = true;
                }
              }
              else if (
                state.ApprovalType == LifecycleApprovalType.Parallel
                || state.ApprovalType == LifecycleApprovalType.AtLeast
              )
              {
                var actors = state.Actors.Where(actor => actor.IsEnroute);
                foreach (var actor in actors)
                {
                  if (await CheckActorForCurrentUser(groupAudit, actor))
                  {
                    // Idempotency: check if already in desired state
                    if (IdempotencyHelpers.IsStatusEquals(actor.Status, status))
                    {
                      logger.Log.LogWarning(
                        "Actor {ActorId} for document {DocumentId} already in status {Status}. Treating as idempotent success.",
                        actor.Id,
                        documentId,
                        status
                      );
                      currentActor = actor;
                      result = true;
                      continue;
                    }

                    // Only update if not already in a final state
                    if (actor.Status != LifecycleActorStatus.Enroute)
                    {
                      logger.Log.LogWarning(
                        "Actor {ActorId} for document {DocumentId} already processed (Status: {Status}). Skipping.",
                        actor.Id,
                        documentId,
                        actor.Status
                      );
                      continue;
                    }

                    actor.StatusDate = DateTime.Now;
                    actor.StatusUserId = (Guid)currentUser.Id;
                    actor.StatusUserName = $"{currentUser.Name} {currentUser.SurName}";
                    actor.Status = status;
                    actor.Reason = reason;
                    await RecognizeCurrentEnrouteState(groupAudit);
                    currentActor = actor;
                    result = true;
                  }
                }
              }
            }

            if (currentActor != null && result)
            {
              var notificationGuid = Guid.NewGuid();
              var notificationMessage = new NotificatorRequestedMsg
              {
                Notification = new EleonsoftNotification()
                {
                  Id = notificationGuid,
                  Type = new MessageNotificationType
                  {
                    IsLocalizedData = true,
                    IsRedirectEnabled = false,
                  },
                  Recipients = new List<RecipientEto>()
                  {
                    new()
                    {
                      Type = NotificatorRecepientType.User,
                      RefId = groupAudit.CreatorId.ToString(),
                    },
                  },
                },
              };
              if (approved)
              {
                notificationMessage.Notification.Message =
                  "Lifecycle::ApprovalNotification:Approved";
                if (notificationMessage.Notification.Type is MessageNotificationType pushType)
                {
                  pushType.LanguageKeyParams = new List<string>()
                  {
                    groupAudit.DocumentId,
                    currentActor.ActorName,
                  };
                }
              }
              else
              {
                notificationMessage.Notification.Message =
                  "Lifecycle::ApprovalNotification:Rejected";
                if (notificationMessage.Notification.Type is MessageNotificationType pushType)
                {
                  pushType.LanguageKeyParams = new List<string>()
                  {
                    groupAudit.DocumentId,
                    currentActor.ActorName,
                  };
                }
              }

              await distributedEventBus.PublishAsync(
                notificationMessage,
                onUnitOfWorkComplete: false
              );

              if (groupAudit.Status == LifecycleStatus.Complete)
              {
                List<string> data = new List<string>();
                data.Add(groupAudit.CurrentStatus.Key.ToString());
                data.Add(groupAudit.CurrentStatus.Value);
                string url = await UrlManagerDomainService.GetDocumentUrl(
                  groupAudit.DocumentId,
                  groupAudit.TenantId
                );
                data.Add(url);
                await NotifyActor(
                  new RecipientEto()
                  {
                    RefId = groupAudit.CreatorId.ToString(),
                    Type = NotificatorRecepientType.User,
                  },
                  ModuleTemplateConsts.LifecycleComplete,
                  data.JoinAsString(",")
                );
                await FinishLifecycle(
                  documentId,
                  reason,
                  groupAudit.CurrentStatus.Key,
                  groupAudit.DocumentObjectType
                );
              }
            }

            await uow.SaveChangesAsync();
            await uow.CompleteAsync();
            return result;
          },
          async () =>
          {
            return false;
          },
          logger.Log,
          "ChangeApprovmentStatus",
          $"{documentObjectType}:{documentId}"
        );
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

    private async Task FinishLifecycle(
      string documentId,
      string reason,
      LifecycleFinishedStatus status,
      string documentObjectType
    )
    {
      var resultMessage = new SyncDocumentWithLifecycleMsg
      {
        DocumentObjectType = documentObjectType,
        DocumentId = documentId.ToString(),
        Status = status,
        StatusDate = DateTime.Now,
        Reason = reason,
        TenantId = currentTenant.Id,
        TenantName = currentTenant.Name,
      };
      await distributedEventBus.PublishAsync(resultMessage);
    }

    public async Task<bool> Review(string documentObjectType, string documentId, bool update = true)
    {
      bool result = false;
      try
      {
        var groupAudit = await statesGroupAuditsRepository.GetByDocIdAsync(
          documentObjectType,
          documentId
        );
        if (groupAudit == null)
        {
          return false;
        }

        if (
          groupAudit.Status == LifecycleStatus.New
          || groupAudit.Status == LifecycleStatus.Canceled
        )
        {
          if (groupAudit.CreatorId == currentUser.Id)
          {
            return true;
          }

          return false;
        }

        if (groupAudit.Status == LifecycleStatus.Complete)
        {
          if (groupAudit.CreatorId == currentUser.Id)
          {
            return true;
          }

          foreach (var state in groupAudit.States)
          {
            foreach (var actor in state.Actors)
            {
              if (
                (
                  actor.Status == LifecycleActorStatus.Approved
                  || actor.Status == LifecycleActorStatus.Rejected
                )
                && actor.StatusUserId == currentUser.Id
              )
              {
                return true;
              }
            }
          }
        }

        if (groupAudit.Status == LifecycleStatus.Enroute)
        {
          var state = await GetStateByOrderIndex(groupAudit.States, null);
          if (state == null)
            return false;
          foreach (
            var actor in state.Actors.Where(actor =>
              actor.OrderIndex == null || actor.OrderIndex == state.CurrentActorOrderIndex
            )
          )
          {
            if (await CheckActorForCurrentUser(groupAudit, actor))
            {
              if (update)
              {
                actor.StatusDate = DateTime.Now;
                actor.StatusUserId = (Guid)currentUser.Id;
                actor.StatusUserName = $"{currentUser.Name} {currentUser.SurName}";
                actor.Status = LifecycleActorStatus.Reviewed;
              }

              result = true;
            }
          }

          if (state.ApprovalType == LifecycleApprovalType.Regular)
          {
            var actor = await GetActorByOrderIndex(state.Actors, state.CurrentActorOrderIndex);
            if (actor == null)
              return false;
            if (await CheckActorForCurrentUser(groupAudit, actor))
            {
              if (update)
              {
                actor.StatusDate = DateTime.Now;
                actor.StatusUserId = (Guid)currentUser.Id;
                actor.StatusUserName = $"{currentUser.Name} {currentUser.SurName}";
                actor.Status = LifecycleActorStatus.Reviewed;
              }

              result = true;
            }
          }
          else if (
            state.ApprovalType == LifecycleApprovalType.Parallel
            || state.ApprovalType == LifecycleApprovalType.AtLeast
          )
          {
            foreach (var actor in state.Actors.Where(actor => actor.IsEnroute))
            {
              if (await CheckActorForCurrentUser(groupAudit, actor))
              {
                if (update)
                {
                  actor.StatusDate = DateTime.Now;
                  actor.StatusUserId = (Guid)currentUser.Id;
                  actor.StatusUserName = $"{currentUser.Name} {currentUser.SurName}";
                  actor.Status = LifecycleActorStatus.Reviewed;
                }

                result = true;
              }
            }
          }
        }
        if (!result)
        {
          foreach (var state in groupAudit.States)
          {
            foreach (var actor in state.Actors)
            {
              if (await CheckActorForCurrentUser(groupAudit, actor))
              {
                result = true;
              }
            }
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> CheckActorForCurrentUser(
      StatesGroupAuditEntity group,
      StateActorAuditEntity actor
    )
    {
      bool result = false;
      try
      {
        if (actor.ActorType == LifecycleActorTypes.Initiator)
        {
          result = group.CreatorId == currentUser.Id;
        }

        if (actor.ActorType == LifecycleActorTypes.User)
        {
          result = actor.RefId == currentUser.Id.ToString();
        }

        if (actor.ActorType == LifecycleActorTypes.Role)
        {
          var user = await identityUserManager.GetByIdAsync((Guid)currentUser.Id);
          var roles = await identityUserManager.GetRolesAsync(user);
          result = roles.Contains(actor.RefId);
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<StateAuditEntity> GetStateByOrderIndex(
      List<StateAuditEntity> states,
      int? OrderIndex
    )
    {
      StateAuditEntity result = null;
      try
      {
        if (OrderIndex == null)
        {
          OrderIndex = states.Min(actor => actor.OrderIndex);
        }

        result = states.Where(state => state.OrderIndex == OrderIndex).FirstOrDefault();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<StateActorAuditEntity> GetActorByOrderIndex(
      List<StateActorAuditEntity> actors,
      int? OrderIndex
    )
    {
      StateActorAuditEntity result = null;
      try
      {
        if (OrderIndex == null)
        {
          OrderIndex = actors.Min(actor => actor.OrderIndex);
        }

        result = actors.Where(actor => actor.OrderIndex == OrderIndex).FirstOrDefault();
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<StatesGroupAuditEntity> InitializeAuditFromTemplate(
      StatesGroupTemplateEntity groupTemplate,
      bool startImmediatly = true
    )
    {
      StatesGroupAuditEntity result = null;
      try
      {
        var groupAudit = new StatesGroupAuditEntity(GuidGenerator.Create());
        groupAudit.Status = startImmediatly ? LifecycleStatus.Enroute : LifecycleStatus.New;
        groupAudit.StatesGroupTemplateId = groupTemplate.Id;
        groupAudit.GroupName = groupTemplate.GroupName;
        groupAudit.DocumentObjectType = groupTemplate.DocumentObjectType;
        await statesGroupAuditsRepository.InsertAsync(groupAudit);
        foreach (var stateTemplate in groupTemplate.States)
        {
          var stateAudit = new StateAuditEntity(GuidGenerator.Create());
          stateAudit.Status = LifecycleStatus.Enroute;
          stateAudit.StatesGroupId = groupAudit.Id;
          stateAudit.StatesTemplateId = stateTemplate.Id;
          stateAudit.OrderIndex = stateTemplate.OrderIndex;
          stateAudit.ApprovalType = stateTemplate.ApprovalType;
          stateAudit.IsMandatory = stateTemplate.IsMandatory;
          stateAudit.IsReadOnly = stateTemplate.IsReadOnly;
          stateAudit.StateName = stateTemplate.StateName;
          stateAudit.IsActive = stateTemplate.IsActive;
          groupAudit.States.Add(stateAudit);
          foreach (var actorTemplate in stateTemplate.Actors)
          {
            var stateActorAudit = new StateActorAuditEntity(GuidGenerator.Create());
            stateActorAudit.Status = LifecycleActorStatus.Enroute;
            stateActorAudit.StatusDate = DateTime.Now;
            stateActorAudit.IsActive = actorTemplate.IsActive;
            stateActorAudit.ActorName = actorTemplate.ActorName;
            stateActorAudit.StateActorTemplateId = actorTemplate.Id;
            stateActorAudit.StateId = stateAudit.Id;
            stateActorAudit.ActorType = actorTemplate.ActorType;
            stateActorAudit.RefId = actorTemplate.RefId;
            stateActorAudit.OrderIndex = actorTemplate.OrderIndex;
            stateActorAudit.IsApprovalAdmin = actorTemplate.IsApprovalAdmin;
            stateActorAudit.IsApprovalManager = actorTemplate.IsApprovalManager;
            stateActorAudit.IsApprovalNeeded = actorTemplate.IsApprovalNeeded;
            stateActorAudit.IsConditional = actorTemplate.IsConditional;
            stateActorAudit.IsFormAdmin = actorTemplate.IsFormAdmin;
            stateActorAudit.RuleId = actorTemplate.RuleId;
            stateAudit.Actors.Add(stateActorAudit);

            //foreach (var taskListTemplate in actorTemplate.TaskLists)
            //{
            //    if (taskListTemplate.TaskListId == Guid.Empty)
            //    {
            //        continue;
            //    }
            //    var taskListAudit = new StateActorTaskListSettingAuditEntity(GuidGenerator.Create());
            //    taskListAudit.Setup(taskListTemplate, stateActorAudit.Id);
            //    stateActorAudit.TaskLists.Add(taskListAudit);
            //}
          }
        }

        await unitOfWorkManager.Current.SaveChangesAsync();
        result = groupAudit;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<StatesGroupAuditEntity> RecognizeCurrentEnrouteState(
      StatesGroupAuditEntity groupAudit
    )
    {
      try
      {
        if (groupAudit.Status != LifecycleStatus.Enroute)
        {
          return groupAudit;
        }

        var activeStates = groupAudit.States.Where(x => x.IsActive).ToList();
        var newStates = activeStates.Where(state => state.Status == LifecycleStatus.Enroute).ToList();
        if (newStates.Count == 0)
        {
          var nextState = activeStates.OrderBy(state => state.OrderIndex).FirstOrDefault();
          if (nextState != null && nextState.Status == LifecycleStatus.New)
          {
            nextState.Status = LifecycleStatus.Enroute;
            newStates.Add(nextState);
          }
        }

        if (newStates.Count == 0)
        {
          groupAudit.Status = LifecycleStatus.Complete;
        }
        else
        {
          groupAudit.CurrentStateOrderIndex = newStates.Min((state) => state.OrderIndex);
          var state = newStates
            .Where(state => state.OrderIndex == groupAudit.CurrentStateOrderIndex)
            .FirstOrDefault();

          if (
            await conditionsDomainService.CheckShouldSkip(
              groupAudit.DocumentObjectType,
              groupAudit.DocumentId,
              LifecycleConditionTargetType.State,
              state.Id
            )
          )
          {
            state.Status = LifecycleStatus.Complete;
            return await RecognizeCurrentEnrouteState(groupAudit);
          }

          state = await RecognizeCurrentEnrouteActors(state, groupAudit);

          if (state.Status == LifecycleStatus.Complete)
          {
            if (state.Actors.Any(actor => actor.Status == LifecycleActorStatus.Rejected))
            {
              foreach (var newState in newStates)
              {
                if (newState.Id != state.Id)
                {
                  newState.Status = LifecycleStatus.Canceled;
                }
              }

              groupAudit.Status = LifecycleStatus.Complete;
            }
            else
            {
              return await RecognizeCurrentEnrouteState(groupAudit);
            }
          }
          else
          {
            var actors = new List<StateActorAuditEntity>();
            if (state.ApprovalType == LifecycleApprovalType.Regular)
            {
              actors.Add(state.CurrentActor);
            }

            if (state.ApprovalType != LifecycleApprovalType.Regular)
            {
              var approvers = state.Actors.Where(actor => actor.IsEnroute);
              actors.AddRange(approvers);
            }

            List<string> data = new List<string>();
            data.Add(state.StateName);
            string url = await UrlManagerDomainService.GetDocumentUrl(
              groupAudit.DocumentId,
              groupAudit.TenantId
            );
            data.Add(url);
            await NotifyActors(
              groupAudit,
              actors,
              ModuleTemplateConsts.LifecycleWaitingForApproval,
              data.JoinAsString(",")
            );
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return groupAudit;
    }

    private async Task<StateAuditEntity> RecognizeCurrentEnrouteActors(
      StateAuditEntity stateAudit,
      StatesGroupAuditEntity groupAudit
    )
    {
      try
      {
        if (stateAudit.Status != LifecycleStatus.Enroute)
        {
          return stateAudit;
        }

        var newActors = stateAudit.Actors.Where(actor => actor.IsEnroute);
        if (newActors.Count() == 0)
        {
          stateAudit.Status = LifecycleStatus.Complete;
          return stateAudit;
        }

        if (stateAudit.Actors.Any(actor => actor.Status == LifecycleActorStatus.Rejected))
        {
          stateAudit.Status = LifecycleStatus.Complete;
          foreach (var actor in newActors)
          {
            actor.Status = LifecycleActorStatus.Canceled;
          }

          return stateAudit;
        }

        if (stateAudit.ApprovalType == LifecycleApprovalType.Regular)
        {
          stateAudit.CurrentActorOrderIndex = newActors.Min(actor => actor.OrderIndex);

          if (
            await conditionsDomainService.CheckShouldSkip(
              groupAudit.DocumentObjectType,
              groupAudit.DocumentId,
              LifecycleConditionTargetType.Actor,
              stateAudit.CurrentActor.Id
            )
          )
          {
            stateAudit.CurrentActor.Status = LifecycleActorStatus.Canceled;
            return await RecognizeCurrentEnrouteActors(stateAudit, groupAudit);
          }

          await PrepareActorsForCurrentApproval(
            new List<StateActorAuditEntity>() { stateAudit.CurrentActor },
            groupAudit
          );
        }

        if (
          stateAudit.ApprovalType is LifecycleApprovalType.AtLeast or LifecycleApprovalType.Parallel
        )
        {
          foreach (var actor in newActors)
          {
            if (
              await conditionsDomainService.CheckShouldSkip(
                groupAudit.DocumentObjectType,
                groupAudit.DocumentId,
                LifecycleConditionTargetType.Actor,
                actor.Id
              )
            )
            {
              actor.Status = LifecycleActorStatus.Canceled;
            }
          }

          newActors = stateAudit.Actors.Where(actor => actor.IsEnroute);

          bool someActorIsCompleted = stateAudit.Actors.Any(x =>
            x.Status == LifecycleActorStatus.Approved || x.Status == LifecycleActorStatus.Rejected
          );

          if (someActorIsCompleted && stateAudit.ApprovalType is LifecycleApprovalType.AtLeast)
          {
            stateAudit.Status = LifecycleStatus.Complete;
          }
          else
          {
            await PrepareActorsForCurrentApproval(newActors.ToList(), groupAudit);
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return stateAudit;
    }

    //public async Task<bool> GetTaskListsStatus(StateActorAuditEntity stateActorAudit, StatesGroupAuditEntity groupAudit)
    //{

    //    bool result = false;
    //    try
    //    {
    //        if (stateActorAudit.TaskLists.Count == 0)
    //        {
    //            return true;
    //        }

    //        foreach (var taskList in stateActorAudit.TaskLists)
    //        {
    //            var message = new CheckIfUserFieldSetsAreFilledMsg
    //            {
    //                DocumentId = groupAudit.DocumentId,
    //                DocumentObjectType = taskList.DocumentObjectType,
    //                RefIdFilter = stateActorAudit.Id
    //            };
    //            var response = await distributedEventBus.RequestAsync<UserFieldSetsFilledCheckedMsg>(message);
    //            result = response.AreFilled;
    //            if (!result)
    //            {
    //                return false;
    //            }
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        logger.Capture(e);
    //    }
    //    finally
    //    {
    //    }

    //    return result;
    //}

    public async Task<bool> PrepareActorsForCurrentApproval(
      List<StateActorAuditEntity> stateActorAuditArray,
      StatesGroupAuditEntity groupAudit
    )
    {
      bool result = false;
      try
      {
        var notificationBulkMessage = new NotificatorRequestedBulkMsg();

        var lifecycleActorForApprovalMessage = new LifecycleActorsForApprovalMsg
        {
          DocumentId = groupAudit.DocumentId,
          DocumentObjectType = groupAudit.DocumentObjectType,
        };

        foreach (var stateActorAudit in stateActorAuditArray)
        {
          if (stateActorAudit == null)
          {
            continue;
          }

          var notificationGuid = Guid.NewGuid();
          var notificationMessage = new NotificatorRequestedMsg
          {
            Notification = new EleonsoftNotification()
            {
              Id = notificationGuid,
              Type = new MessageNotificationType
              {
                IsRedirectEnabled = false,
                IsLocalizedData = false,
              },
              Recipients = new List<RecipientEto>(),
            },
          };

          if (stateActorAudit.ActorType == LifecycleActorTypes.Initiator)
          {
            notificationMessage.Notification.Message =
              $"Lifecycle::ApprovalNotification:ApprovalRequested";
            notificationMessage.Notification.Recipients.Add(
              new RecipientEto()
              {
                Type = NotificatorRecepientType.User,
                RefId = groupAudit.CreatorId.ToString(),
              }
            );

            if (notificationMessage.Notification.Type is MessageNotificationType pushType)
            {
              pushType.LanguageKeyParams = new List<string>()
              {
                groupAudit.GetProperty("DocEntry", string.Empty),
              };
            }
          }

          if (stateActorAudit.ActorType == LifecycleActorTypes.User)
          {
            notificationMessage.Notification.Message =
              $"Lifecycle::ApprovalNotification:ApprovalRequested";
            notificationMessage.Notification.Recipients.Add(
              new RecipientEto()
              {
                Type = NotificatorRecepientType.User,
                RefId = stateActorAudit.RefId,
              }
            );
            if (notificationMessage.Notification.Type is MessageNotificationType pushType)
            {
              pushType.LanguageKeyParams = new List<string>()
              {
                groupAudit.GetProperty("DocEntry", string.Empty),
              };
            }
          }

          if (stateActorAudit.ActorType == LifecycleActorTypes.Role)
          {
            notificationMessage.Notification.Message =
              $"Lifecycle::ApprovalNotification:ApprovalRequestedByRole";
            //IdentityRole role = await identityRoleManager.FindByNameAsync(stateActorAudit.RefId);
            notificationMessage.Notification.Recipients.Add(
              new RecipientEto()
              {
                Type = NotificatorRecepientType.Role,
                RefId = stateActorAudit.RefId,
              }
            );

            if (notificationMessage.Notification.Type is MessageNotificationType pushType)
            {
              pushType.LanguageKeyParams = new List<string>()
              {
                groupAudit.GetProperty("DocEntry", string.Empty),
                stateActorAudit.RefId,
              };
            }
          }

          notificationBulkMessage.Messages.Add(notificationMessage);
        }

        await distributedEventBus.PublishAsync(
          notificationBulkMessage,
          onUnitOfWorkComplete: false
        );
        await distributedEventBus.PublishAsync(
          lifecycleActorForApprovalMessage,
          onUnitOfWorkComplete: false
        );

        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> NotifyActor(RecipientEto actor, string template, string data)
    {
      bool result = false;
      try
      {
        result = await NotifyActors(new List<RecipientEto>() { actor }, template, data);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<bool> NotifyActors(List<RecipientEto> actors, string template, string data)
    {
      bool result = false;
      try
      {
        Guid notificationHandlerId = Guid.NewGuid();
        var notificatioMessage = new NotificatorRequestedMsg
        {
          Notification = new EleonsoftNotification()
          {
            Message = data,
            Id = notificationHandlerId,
            Type = new MessageNotificationType { TemplateName = template },
            Recipients = actors,
          },
        };
        await distributedEventBus.PublishAsync(notificatioMessage, onUnitOfWorkComplete: false);
        result = true;
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<bool> NotifyActors(
      StatesGroupAuditEntity group,
      List<StateActorAuditEntity> actors,
      string template,
      string data
    )
    {
      bool result = false;
      try
      {
        var receipients = actors
          .Select(actor =>
          {
            if (actor.ActorType == LifecycleActorTypes.Initiator)
            {
              return new RecipientEto()
              {
                RefId = group.CreatorId.ToString(),
                Type = NotificatorRecepientType.User,
              };
            }

            if (actor.ActorType == LifecycleActorTypes.Role)
            {
              return new RecipientEto()
              {
                RefId = actor.RefId,
                Type = NotificatorRecepientType.Role,
              };
            }

            return new RecipientEto() { Type = NotificatorRecepientType.User, RefId = actor.RefId };
          })
          .ToList();

        result = await NotifyActors(receipients, template, data);
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    public async Task<LifecycleStatusValueObject> GetLifecycleStatus(
      string documentObjectType,
      string documentId
    )
    {
      LifecycleStatusValueObject result = new LifecycleStatusValueObject();
      ;
      try
      {
        StatesGroupAuditEntity tree = await statesGroupAuditsRepository.GetByDocIdAsync(
          documentObjectType,
          documentId
        );
        if (tree == null)
        {
          return null;
        }

        // TO DO check permissions
        //var currentUserId = currentUser.Id;
        //if (currentUserId != tree.CreatorId &&
        //    !await Review(documentObjectType, documentId) &&
        //    !await GetUserLifecycleApprovalPermission(documentObjectType, documentId))
        //{
        //    await CheckPermissionToSeeTrace(documentObjectType);
        //}

        result.LifecycleStatus = tree.Status;

        if (tree.Status == LifecycleStatus.New || tree.Status == LifecycleStatus.Canceled)
        {
          return result;
        }
        else if (tree.Status == LifecycleStatus.Complete)
        {
          if (tree.CurrentStatus.Key == LifecycleFinishedStatus.Rejected)
          {
            var state = tree.States.FirstOrDefault(x =>
              x.CurrentStatus.Key == LifecycleFinishedStatus.Rejected
            );
            if (state != null)
            {
              result.StatusDate = state.LastStatusDate;
              var actor = state.Actors.FirstOrDefault(actor =>
                actor.Status == LifecycleActorStatus.Rejected
              );
              result.ActorName = actor?.StatusUserName;
              result.ActorStatus = LifecycleActorStatus.Rejected;
              result.RejectedReason = actor.Reason;
            }
          }
          else if (tree.CurrentStatus.Key == LifecycleFinishedStatus.Approved)
          {
            var state = tree.States.FirstOrDefault(x =>
              x.CurrentStatus.Key == LifecycleFinishedStatus.Approved
            );
            if (state != null)
            {
              result.StatusDate = state.LastStatusDate;
              result.ActorStatus = LifecycleActorStatus.Approved;
            }
          }
        }
        else if (tree.Status == LifecycleStatus.Enroute)
        {
          var enrouteState = tree.States.FirstOrDefault(x => x.Status == LifecycleStatus.Enroute);
          if (enrouteState != null)
          {
            result.LifecycleApprovalType = enrouteState.ApprovalType;
            result.StatusDate = enrouteState.LastStatusDate;

            if (enrouteState.ApprovalType == LifecycleApprovalType.Regular)
            {
              result.ActorType = enrouteState.CurrentActor.ActorType;
              result.ActorStatus = enrouteState.CurrentActor.Status;
            }
          }
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }

      return result;
    }

    private async Task<bool> IsPurchaseManager(string docType) =>
      await permissionChecker.IsGrantedAsync($"Permission.{docType}.PurchaseManager");

    private async Task<bool> IsPurchaseVendorManager(string docType) =>
      await permissionChecker.IsGrantedAsync($"Permission.{docType}.PurchaseManager");

    private async Task<bool> IsTravelManager() =>
      await permissionChecker.IsGrantedAsync("Permission.TravelRequest.TravelManager");

    private async Task<bool> IsVendorGeneralPermission() =>
      await permissionChecker.IsGrantedAsync(
        $"Permission.{nameof(FeaturePack.BusinessPartner)}.Vendor.General"
      );

    private async Task<bool> IsCustomerGeneralPermission() =>
      await permissionChecker.IsGrantedAsync(
        $"Permission.{nameof(FeaturePack.BusinessPartner)}.Customer.General"
      );

    private async Task<bool> IsItemsGeneralPermission() =>
      await permissionChecker.IsGrantedAsync($"Permission.Items.General");

    private async Task<bool> IsLocalExpenseManager() =>
      await permissionChecker.IsGrantedAsync(
        $"Permission.{nameof(FeaturePack.ExpenseReport)}.LocalExpenseReport.LocalExpenseManager"
      );

    private async Task<bool> IsTravelExpenseManager() =>
      await permissionChecker.IsGrantedAsync(
        $"Permission.{nameof(FeaturePack.ExpenseReport)}.TravelExpenseReport.TravelExpenseManager"
      );

    private async Task<bool> IsExpenseManager() =>
      await permissionChecker.IsGrantedAsync(
        $"Permission.{nameof(FeaturePack.ExpenseReport)}.ExpenseReport.ExpenseManager"
      );

    private async Task CheckPermissionToSeeTrace(string documentObjectType)
    {
      throw new NotImplementedException("Eleonsoft shouldn't check document permission");
    }

    private async Task SetActorDisplayName(StateActorAuditEntity stateActor)
    {
      try
      {
        if (stateActor.ActorType == LifecycleActorTypes.Initiator)
        {
          stateActor.DisplayName = localizer["Initiator"];
        }

        if (stateActor.ActorType == LifecycleActorTypes.User)
        {
          var user = await identityUserManager.FindByIdAsync(stateActor.RefId);
          if (user != null)
          {
            stateActor.DisplayName = $"{user.Name} {user.Surname}";
          }
        }

        if (stateActor.ActorType == LifecycleActorTypes.Role)
        {
          var role = await identityRoleManager.FindByNameAsync(stateActor.RefId);
          stateActor.DisplayName = role.Name;
        }

        if (stateActor.ActorType == LifecycleActorTypes.Beneficiary)
        {
          stateActor.DisplayName = localizer["Beneficiary"];
        }
      }
      catch (Exception e)
      {
        logger.Capture(e);
      }
      finally
      {
      }
    }
  }
}
