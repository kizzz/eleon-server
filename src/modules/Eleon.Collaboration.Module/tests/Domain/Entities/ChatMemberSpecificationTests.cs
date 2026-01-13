using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Specifications;
using VPortal.Collaboration.Feature.Module.Entities;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace CollaborationModule.Test.Domain.Entities;

public class ChatMemberSpecificationTests
{
  [Fact]
  public void ToExpression_ShouldMatchUserOrRole()
  {
    var userId = Guid.NewGuid();
    var roles = new List<string> { "Admin", "Editor" };
    var spec = new ChatMemberSpecification(userId, roles);
    var predicate = spec.ToExpression().Compile();

    var userMember = new ChatMemberEntity(Guid.NewGuid(), userId.ToString(), Guid.NewGuid(), ChatMemberRole.Owner)
    {
      Type = ChatMemberType.User
    };
    var roleMember = new ChatMemberEntity(Guid.NewGuid(), "Admin", Guid.NewGuid(), ChatMemberRole.Regular)
    {
      Type = ChatMemberType.Role
    };
    var otherMember = new ChatMemberEntity(Guid.NewGuid(), "Other", Guid.NewGuid(), ChatMemberRole.Regular)
    {
      Type = ChatMemberType.Role
    };

    predicate(userMember).ShouldBeTrue();
    predicate(roleMember).ShouldBeTrue();
    predicate(otherMember).ShouldBeFalse();
  }
}
