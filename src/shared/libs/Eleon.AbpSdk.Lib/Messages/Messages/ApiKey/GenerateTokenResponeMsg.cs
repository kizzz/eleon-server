using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.ApiKey;
public class GenerateTokenResponeMsg
{
  public bool IsSuccess { get; set; }
  public string AccessToken { get; set; }
  public int ExpiresIn { get; set; }
  public string Error { get; set; }
  public string ErrorDescription { get; set; }
}
