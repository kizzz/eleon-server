using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using VPortal.Collaboration.Feature.Module.Chats;

namespace VPortal.Collaboration.Feature.Module.ChatRooms
{
  public class ChatRoomDto
  {
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string RefId { get; set; }
    public ChatRoomType Type { get; set; }
    public DateTime CreationTime { get; set; }
    public ChatRoomStatus Status { get; set; }
    public List<ChatMemberInfo> ChatMembersPreview { get; set; }
    public int MembersAmount { get; set; }

    public string Description { get; set; }
    public List<string> Tags { get; set; }
    public bool IsPublic { get; set; } = false;
    public ChatMemberRole DefaultRole { get; set; }
  }
}
