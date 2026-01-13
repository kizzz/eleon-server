using Messaging.Module;
using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.EventBus.Lib.Full.EventBus.Common.EventBus.Module;

public static class BusHelpers
{
  public static bool IsPreventedType(Type eventType)
  {
    if (eventType is null) throw new ArgumentNullException(nameof(eventType));

    // Normalize: if someone passes a constructed generic, use its definition for open-generic matching.
    // But keep the original for assignability checks.
    var type = eventType;

    foreach (var ignored in MessagingConsts.IgnoredMessageTypes)
    {
      // 1) Exact / assignable match for non-generic ignored types
      if (!ignored.IsGenericTypeDefinition)
      {
        if (ignored.IsAssignableFrom(type))
          return true;

        continue;
      }

      // 2) Open-generic match:
      // If ignored is EntityCreatedEto<>, block EntityCreatedEto<T> for any T
      // Also match if the open generic appears anywhere in base types / interfaces.
      if (IsOrImplementsOpenGeneric(type, ignored))
        return true;
    }

    return false;
  }

  private static bool IsOrImplementsOpenGeneric(Type type, Type openGeneric)
  {
    if (type == null) return false;
    if (openGeneric == null) return false;
    if (!openGeneric.IsGenericTypeDefinition) return false;

    // Check the type itself and its base types
    for (var current = type; current != null && current != typeof(object); current = current.BaseType!)
    {
      if (current.IsGenericType && current.GetGenericTypeDefinition() == openGeneric)
        return true;
    }

    // Check interfaces (important if event types are interfaces or implement generic interfaces)
    foreach (var it in type.GetInterfaces())
    {
      if (it.IsGenericType && it.GetGenericTypeDefinition() == openGeneric)
        return true;
    }

    return false;
  }
}
