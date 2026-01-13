using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;
using VPortal.Collaboration.Feature.Module.Chats;

namespace VPortal.Collaboration.Feature.Module.Entities
{
  [DisableAuditing]
  public class ChatRoomEntity : FullAuditedAggregateRoot<Guid>, IMultiTenant
  {
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string RefId { get; set; }
    public ChatRoomType Type { get; set; }
    public ChatRoomStatus Status { get; set; } = ChatRoomStatus.Opened;
    public string Description { get; set; }
    public string Tags { get; set; }
    public bool IsPublic { get; set; } = false;
    public ChatMemberRole DefaultRole { get; set; } = ChatMemberRole.Regular;
    public List<ViewChatPermissionEntity> ViewChatPermissions { get; set; } = new List<ViewChatPermissionEntity>();

    public List<ChatMessageEntity> Messages { get; set; } = new List<ChatMessageEntity>();

    public ChatRoomEntity(Guid id, ChatRoomType type)
    {
      Id = id;
      Type = type;
    }

    [NotMapped]
    public int MembersAmount { get; set; }

    [NotMapped]
    public List<ChatMemberInfo> ChatMembersPreview { get; set; }

    [NotMapped]
    public List<Guid> ChatMemberIdsPreview { get; set; }

    public List<string> GetTags()
    {
      if (string.IsNullOrEmpty(Tags))
      {
        return new List<string>();
      }

      return Tags.Split(';').Where(x => !string.IsNullOrEmpty(x)).ToList();
    }

    public string SetTags(IEnumerable<string> tags)
    {
      Tags = string.Join(';', tags.Where(x => !string.IsNullOrEmpty(x)));
      return Tags;
    }
  }
}
