using System;
using System.Collections.Generic;

namespace VPortal.Collaboration.Feature.Module.ChatMembers
{
  public class AddChatMembersRequestDto
  {
    public Guid ChatId { get; set; }
    public List<Guid> UserIds { get; set; }
  }
}
