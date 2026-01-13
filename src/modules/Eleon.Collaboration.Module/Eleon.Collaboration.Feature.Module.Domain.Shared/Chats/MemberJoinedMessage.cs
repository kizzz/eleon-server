using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPortal.Collaboration.Feature.Module.Chats;

namespace ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Chats;
public class MemberJoinedMessage
{
  public ChatMemberInfo JoinedUser { get; set; }
}
