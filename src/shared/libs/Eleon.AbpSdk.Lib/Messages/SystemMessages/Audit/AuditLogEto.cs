using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.Auditing;

namespace EleonsoftSdk.modules.Messaging.Module.SystemMessages.Audit;

public class AuditLogEto
{
  public Guid? UserId { get; set; }
  public string UserName { get; set; }
  public Guid? TenantId { get; set; }
  public Guid? ImpersonatorUserId { get; set; }
  public Guid? ImpersonatorTenantId { get; set; }
  public DateTime ExecutionTime { get; set; }
  public int ExecutionDuration { get; set; }
  public string ClientIpAddress { get; set; }
  public string ClientName { get; set; }
  public string BrowserInfo { get; set; }
  public string HttpMethod { get; set; }
  public int? HttpStatusCode { get; set; }
  public string Url { get; set; }
  public List<AuditLogActionEto> Actions { get; set; } = new();
  public List<ExceptionEto> Exceptions { get; set; } = new();
  public Dictionary<string, object> ExtraProperties { get; set; } = new();
  public List<EntityChangeEto> EntityChanges { get; set; } = new();
  public List<string> Comments { get; set; } = new();
    public string? ApplicationName { get; set; }

    public static AuditLogEto FromAuditInfo(AuditLogInfo auditLogInfo)
  {
    if (auditLogInfo == null)
    {
      return null;
    }

    return new AuditLogEto
    {
        ApplicationName = auditLogInfo.ApplicationName,
      UserId = auditLogInfo.UserId,
      UserName = auditLogInfo.UserName,
      TenantId = auditLogInfo.TenantId,
      ImpersonatorUserId = auditLogInfo.ImpersonatorUserId,
      ImpersonatorTenantId = auditLogInfo.ImpersonatorTenantId,
      ExecutionTime = auditLogInfo.ExecutionTime,
      ExecutionDuration = auditLogInfo.ExecutionDuration,
      ClientIpAddress = auditLogInfo.ClientIpAddress,
      ClientName = auditLogInfo.ClientName,
      BrowserInfo = auditLogInfo.BrowserInfo,
      HttpMethod = auditLogInfo.HttpMethod,
      HttpStatusCode = auditLogInfo.HttpStatusCode,
      Url = auditLogInfo.Url,
      Actions = auditLogInfo.Actions?.Select(AuditLogActionEto.FromActionInfo).Where(a => a != null).ToList() ?? new List<AuditLogActionEto>(),
      Exceptions = auditLogInfo.Exceptions?.Select(ExceptionEto.FromException).Where(e => e != null).ToList() ?? new List<ExceptionEto>(),
      ExtraProperties = auditLogInfo.ExtraProperties?.ToDictionary(k => k.Key, v => v.Value) ?? new Dictionary<string, object>(),
      EntityChanges = auditLogInfo.EntityChanges?.Select(EntityChangeEto.FromEntityChangeInfo).Where(c => c != null).ToList() ?? new List<EntityChangeEto>(),
      Comments = auditLogInfo.Comments?.ToList() ?? new List<string>(),
    };
  }

  public AuditLogInfo ToAuditInfo()
  {
    var info = new AuditLogInfo
    {
        ApplicationName = ApplicationName,
        UserId = UserId,
      UserName = UserName,
      TenantId = TenantId,
      ImpersonatorUserId = ImpersonatorUserId,
      ImpersonatorTenantId = ImpersonatorTenantId,
      ExecutionTime = ExecutionTime,
      ExecutionDuration = ExecutionDuration,
      ClientIpAddress = ClientIpAddress,
      ClientName = ClientName,
      BrowserInfo = BrowserInfo,
      HttpMethod = HttpMethod,
      HttpStatusCode = HttpStatusCode,
      Url = Url,
      Comments = Comments?.ToList() ?? new List<string>()
    };

    if (Actions != null)
    {
      foreach (var action in Actions.Select(a => a?.ToActionInfo()).Where(a => a != null))
      {
        info.Actions.Add(action);
      }
    }

    if (Exceptions != null)
    {
      foreach (var exception in Exceptions.Select(e => e?.ToException()).Where(e => e != null))
      {
        info.Exceptions.Add(exception);
      }
    }

    if (ExtraProperties != null)
    {
      foreach (var kv in ExtraProperties)
      {
        info.ExtraProperties[kv.Key] = kv.Value;
      }
    }

    if (EntityChanges != null)
    {
      foreach (var change in EntityChanges.Select(c => c?.ToEntityChangeInfo()).Where(c => c != null))
      {
        info.EntityChanges.Add(change);
      }
    }

    return info;
  }
}

public class AuditLogActionEto
{
  public string ServiceName { get; set; }
  public string MethodName { get; set; }
  public string Parameters { get; set; }
  public DateTime ExecutionTime { get; set; }
  public int ExecutionDuration { get; set; }
  public Dictionary<string, string> ExtraProperties { get; set; } = new();

  public static AuditLogActionEto FromActionInfo(AuditLogActionInfo action)
  {
    if (action == null)
    {
      return null;
    }

    return new AuditLogActionEto
    {
      ServiceName = action.ServiceName,
      MethodName = action.MethodName,
      Parameters = action.Parameters,
      ExecutionTime = action.ExecutionTime,
      ExecutionDuration = action.ExecutionDuration,
      ExtraProperties = action.ExtraProperties?.ToDictionary(k => k.Key, v => v.Value?.ToString()) ?? new Dictionary<string, string>()
    };
  }

  public AuditLogActionInfo ToActionInfo()
  {
    var actionInfo = new AuditLogActionInfo
    {
      ServiceName = ServiceName,
      MethodName = MethodName,
      Parameters = Parameters,
      ExecutionTime = ExecutionTime,
      ExecutionDuration = ExecutionDuration
    };

    if (ExtraProperties != null)
    {
      foreach (var kv in ExtraProperties)
      {
        actionInfo.ExtraProperties[kv.Key] = kv.Value;
      }
    }

    return actionInfo;
  }
}

public class EntityChangeEto
{
  public DateTime ChangeTime { get; set; }
  public EntityChangeType ChangeType { get; set; }
  public string EntityId { get; set; }
  public string EntityTypeFullName { get; set; }
  public List<EntityPropertyChangeEto> PropertyChanges { get; set; } = new();
  public Dictionary<string, string> ExtraProperties { get; set; } = new();

  public static EntityChangeEto FromEntityChangeInfo(EntityChangeInfo change)
  {
    if (change == null)
    {
      return null;
    }

    return new EntityChangeEto
    {
      ChangeTime = change.ChangeTime,
      ChangeType = change.ChangeType,
      EntityId = change.EntityId,
      EntityTypeFullName = change.EntityTypeFullName,
      PropertyChanges = change.PropertyChanges?.Select(EntityPropertyChangeEto.FromPropertyChange).Where(p => p != null).ToList() ?? new List<EntityPropertyChangeEto>(),
      ExtraProperties = change.ExtraProperties?.ToDictionary(k => k.Key, v => v.Value?.ToString()) ?? new Dictionary<string, string>()
    };
  }

  public EntityChangeInfo ToEntityChangeInfo()
  {
    var changeInfo = new EntityChangeInfo
    {
      ChangeTime = ChangeTime,
      ChangeType = ChangeType,
      EntityId = EntityId,
      EntityTypeFullName = EntityTypeFullName,
      PropertyChanges = PropertyChanges?.Select(pc => pc?.ToPropertyChangeInfo()).Where(pc => pc != null).ToList() ?? new List<EntityPropertyChangeInfo>(),
      EntityEntry = null // intentionally excluded to keep payload serializable
    };

    if (ExtraProperties != null)
    {
      foreach (var kv in ExtraProperties)
      {
        changeInfo.ExtraProperties[kv.Key] = kv.Value;
      }
    }

    return changeInfo;
  }
}

public class EntityPropertyChangeEto
{
  public string NewValue { get; set; }
  public string OriginalValue { get; set; }
  public string PropertyName { get; set; }
  public string PropertyTypeFullName { get; set; }

  public static EntityPropertyChangeEto FromPropertyChange(EntityPropertyChangeInfo propertyChange)
  {
    if (propertyChange == null)
    {
      return null;
    }

    return new EntityPropertyChangeEto
    {
      NewValue = propertyChange.NewValue,
      OriginalValue = propertyChange.OriginalValue,
      PropertyName = propertyChange.PropertyName,
      PropertyTypeFullName = propertyChange.PropertyTypeFullName
    };
  }

  public EntityPropertyChangeInfo ToPropertyChangeInfo()
  {
    return new EntityPropertyChangeInfo
    {
      NewValue = NewValue,
      OriginalValue = OriginalValue,
      PropertyName = PropertyName,
      PropertyTypeFullName = PropertyTypeFullName
    };
  }
}

public class ExceptionEto
{
  public string Message { get; set; }
  public string Type { get; set; }
  public string StackTrace { get; set; }
  public string HelpLink { get; set; }
  public string Source { get; set; }
  public int HResult { get; set; }
  public Dictionary<string, string> Data { get; set; } = new();
  public ExceptionEto Inner { get; set; }

  public static ExceptionEto FromException(Exception exception)
  {
    if (exception == null)
    {
      return null;
    }

    var eto = new ExceptionEto
    {
      Message = exception.Message,
      Type = exception.GetType().FullName,
      StackTrace = exception.StackTrace,
      HelpLink = exception.HelpLink,
      Source = exception.Source,
      HResult = exception.HResult,
      Inner = FromException(exception.InnerException),
      Data = new Dictionary<string, string>()
    };

    foreach (var key in exception.Data.Keys)
    {
      eto.Data[key?.ToString() ?? string.Empty] = exception.Data[key]?.ToString();
    }

    return eto;
  }

  public Exception ToException()
  {
    var inner = Inner?.ToException();
    var exception = new Exception(Message, inner)
    {
      HelpLink = HelpLink,
      Source = Source,
      HResult = HResult
    };

    if (!string.IsNullOrEmpty(StackTrace))
    {
      typeof(Exception)
          .GetField("_stackTraceString", BindingFlags.Instance | BindingFlags.NonPublic)?
          .SetValue(exception, StackTrace);
    }

    if (Data != null)
    {
      foreach (var kv in Data)
      {
        exception.Data[kv.Key] = kv.Value;
      }
    }

    return exception;
  }
}

