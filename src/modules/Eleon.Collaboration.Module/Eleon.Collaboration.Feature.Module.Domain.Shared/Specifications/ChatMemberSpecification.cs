using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Volo.Abp.Specifications;
using VPortal.Collaboration.Feature.Module.Entities;

namespace VPortal.Collaboration.Feature.Module.Specifications
{
  public class ChatMemberSpecification : Specification<ChatMemberEntity>
  {
    public readonly string UserId;
    public readonly List<string> Roles;

    public ChatMemberSpecification(Guid userId, List<string> roles)
    {
      this.UserId = userId.ToString();
      this.Roles = roles;
    }

    public override Expression<Func<ChatMemberEntity, bool>> ToExpression()
    {
      return (m) =>
              (m.Type == ChatMemberType.User && m.RefId == UserId)
              || (m.Type == ChatMemberType.Role && Roles.Contains(m.RefId));
    }
  }
}
