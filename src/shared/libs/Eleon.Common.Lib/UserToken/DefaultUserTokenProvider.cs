using Microsoft.AspNetCore.Http;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Eleon.Common.Lib.UserToken;

public class DefaultUserTokenProvider : IUserTokenProvider
{
  private string? _token;
  private static readonly AsyncLocal<string?> _ambientToken = new AsyncLocal<string?>();

  public string? Token
  {
    get => string.IsNullOrWhiteSpace(_token) ? _ambientToken.Value : _token;
    set
    {
      _token = value;
      _ambientToken.Value = value;
    }
  }
}
