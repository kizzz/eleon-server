using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleonsoftAbp.Messages.ApiKey;
public class GenerateTokenRequestMsg
{
  public string ApiKey { get; set; }
  public DateTime Timestamp { get; set; }
  public string Nonce { get; set; }
  public string Signature { get; set; }

  public string ClientId { get; set; }
  public string ClientSecret { get; set; }
}
