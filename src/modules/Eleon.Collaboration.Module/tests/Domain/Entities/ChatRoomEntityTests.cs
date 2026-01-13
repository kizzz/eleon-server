using System;
using System.Collections.Generic;
using Shouldly;
using VPortal.Collaboration.Feature.Module.Entities;
using ModuleCollector.CollaborationModule.Collaboration.Feature.Module.Domain.Shared.Constants;

namespace CollaborationModule.Test.Domain.Entities;

public class ChatRoomEntityTests
{
  [Fact]
  public void SetTags_ShouldIgnoreEmptyEntriesAndReturnJoinedString()
  {
    var chat = new ChatRoomEntity(Guid.NewGuid(), ChatRoomType.Group);

    var result = chat.SetTags(new List<string> { "alpha", string.Empty, null, "beta", "  " });

    result.ShouldBe("alpha;beta;  ");
    chat.GetTags().ShouldBe(new List<string> { "alpha", "beta", "  " });
  }
}
