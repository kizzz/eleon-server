using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class GetIdentitySettingsForRegistrationGotMsg : VportalEvent
  {
    public bool AllowChangeEmail { get; set; }

    public bool AllowChangeUserName { get; set; }

    public bool EnablePassword { get; set; }

    public bool EnableTwoAuth { get; set; }

    public string TwoAuthOption { get; set; }

    public string SmsProviderOption { get; set; }

    public bool EnableSelfRegistration { get; set; }

    public bool RequireConfirmedEmail { get; set; }

    public bool RequireConfirmedPhone { get; set; }

    public int PasswordRequiredLength { get; set; }

    public int PasswordRequiredUniqueChars { get; set; }

    public bool PasswordRequireNonAlphanumeric { get; set; }

    public bool PasswordRequireLowercase { get; set; }

    public bool PasswordRequireUppercase { get; set; }

    public bool PasswordRequireDigit { get; set; }

    public GetIdentitySettingsForRegistrationGotMsg()
    {
    }
  }
}
