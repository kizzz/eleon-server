using Logging.Module;
using Messaging.Module.ETO;
using Messaging.Module.Messages;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.ObjectMapping;
using VPortal.Collaboration.Feature.Module.Entities;
using VPortal.Collaboration.Feature.Module.Repositories;

namespace VPortal.Collaboration.Feature.Module.Push
{
  public class ChatMessagePushManager : ITransientDependency
  {
    private readonly IVportalLogger<ChatMessagePushManager> logger;
    private readonly IObjectMapper objectMapper;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly IUserChatSettingRepository userChatSettingRepository;
    private readonly IChatMemberRepository chatMemberRepository;
    private readonly IChatRoomRepository chatRoomRepository;

    public ChatMessagePushManager(
        IVportalLogger<ChatMessagePushManager> logger,
        IObjectMapper objectMapper,
        IDistributedEventBus massTransitPublisher,
        IUserChatSettingRepository userChatSettingRepository,
        IChatMemberRepository chatMemberRepository,
        IChatRoomRepository chatRoomRepository)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this.massTransitPublisher = massTransitPublisher;
      this.userChatSettingRepository = userChatSettingRepository;
      this.chatMemberRepository = chatMemberRepository;
      this.chatRoomRepository = chatRoomRepository;
    }

    public async Task PushMessageAsync(ChatMessageEntity message)
    {
      try
      {
        var chat = await chatRoomRepository.GetAsync(message.ChatRoomId, true);

        var members = await chatMemberRepository.GetByChat(chat.Id);
        var userIds = members
            .Where(x => x.Type == ChatMemberType.User)
            .Select(x => Guid.Parse(x.RefId))
            .ToList();

        var roles = members
            .Where(x => x.Type == ChatMemberType.Role)
            .Select(x => x.RefId)
            .Concat(chat.ViewChatPermissions
            .Where(x => x.EntityType == PermissionEntityType.Role)
            .Select(x => x.EntityRef))
            .Distinct()
            .ToList();

        var orgUnitIds = chat.ViewChatPermissions
            .Where(x => x.EntityType == PermissionEntityType.OrgUnit)
            .Select(x => x.EntityRef)
            .Where(x => Guid.TryParse(x, out _))
            .Select(x => Guid.Parse(x))
            .ToList();

        var muted = await userChatSettingRepository.GetMutedUsers(chat.Id);

        message.Unread = true;

        var pushMsg = new ChatPushMessageEto()
        {
          ChatRoom = objectMapper.Map<ChatRoomEntity, ChatRoomEto>(chat),
          Message = objectMapper.Map<ChatMessageEntity, ChatMessageEto>(message),
          MutedUsers = muted,
        };

        await userChatSettingRepository.UnarchiveChatAsync(chat.Id, userIds);

        await SendPushNotificationAsync(pushMsg, userIds, roles, orgUnitIds);
      }
      catch (Exception ex)
      {
        logger.Capture(ex);
      }
      finally
      {
      }
    }

    public async Task SendPushNotificationAsync(ChatPushMessageEto message, List<Guid> userIds, List<string> roles, List<Guid> orgUnitIds = null)
    {
      orgUnitIds ??= new List<Guid>();
      await massTransitPublisher.PublishAsync(new PushChatMessageMsg
      {
        AudienceRoles = roles,
        AudienceUserIds = userIds,
        AudienceOrgUnits = orgUnitIds,
        Message = message
      });
    }
  }
}
