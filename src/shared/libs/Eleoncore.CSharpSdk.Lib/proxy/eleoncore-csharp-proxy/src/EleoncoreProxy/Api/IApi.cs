using System.Net.Http;

namespace EleoncoreProxy.Api
{
  /// <summary>
  /// Any Api client
  /// </summary>
  public interface IApi
  {
    /// <summary>
    /// The HttpClient
    /// </summary>
    HttpClient HttpClient { get; }
  }
}