using Common.Module.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messaging.Module.ETO
{
  public class UserOtpSettingsEto
  {
    public UserOtpType UserOtpType { get; set; }
    public string OtpEmail { get; set; }
    public string OtpPhoneNumber { get; set; }
    public Guid UserId { get; set; }
  }
}
