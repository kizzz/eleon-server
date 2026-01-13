using System;
using System.Collections.Generic;

namespace VPortal.Collaboration.Feature.Module.ChatMembers
{
  public class KickChatMembersRequestDto
  {
    public Guid ChatId { get; set; }
    public List<Guid> UserIds { get; set; }
  }
}
