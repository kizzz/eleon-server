using System;
using Eleon.EventBus.Lib.Full.EventBus.Common.EventBus.Module;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Xunit;

namespace Eleon.Common.EventBus.Module.Tests;

public class BusHelpersTests
{
  private class SampleEto { }

  private class NonIgnoredEvent { }

  [Fact]
  public void IsPreventedType_returns_true_for_entity_created_event()
  {
    var eventType = typeof(EntityCreatedEto<SampleEto>);

    var prevented = BusHelpers.IsPreventedType(eventType);

    Assert.True(prevented);
  }

  [Fact]
  public void IsPreventedType_returns_false_for_non_ignored_type()
  {
    var eventType = typeof(NonIgnoredEvent);

    var prevented = BusHelpers.IsPreventedType(eventType);

    Assert.False(prevented);
  }

  [Fact]
  public void IsPreventedType_throws_for_null_type()
  {
    Assert.Throws<ArgumentNullException>(() => BusHelpers.IsPreventedType(null!));
  }
}
