namespace Common.Module.Serialization
{
  public interface IHttpMessageSerializer
  {
    Task<HttpRequestMessage> CreateHttpRequestMessageFromBase64(string base64);
    Task<HttpResponseMessage> CreateHttpResponseMessageFromBase64(string base64);
    Task<string> HttpMessageToBase64(HttpRequestMessage request);
    Task<string> HttpMessageToBase64(HttpResponseMessage response);
  }
}
