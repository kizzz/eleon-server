using System;
using System.Collections.Generic;
using System.Text;

namespace Eleon.Common.Lib.UserToken;

public interface IUserTokenProvider
{
  public string? Token { get; set; }
}
