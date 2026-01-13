using System.Threading.Tasks;

namespace ExternalLogin.Module
{
  public interface IExternalLoginSecurityLogManager
  {
    Task WriteSecurityLog(string action, string authScheme);
  }
}
