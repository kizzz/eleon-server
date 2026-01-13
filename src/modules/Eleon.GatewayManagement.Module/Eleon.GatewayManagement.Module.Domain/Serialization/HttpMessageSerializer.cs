using Common.Module.Serialization;
using Volo.Abp.DependencyInjection;

namespace Common.Module.Helpers
{
  public class HttpMessageSerializer : IHttpMessageSerializer, ITransientDependency
  {
    public async Task<string> HttpMessageToBase64(HttpRequestMessage request)
    {
      var content = new HttpMessageContent(request);
      return await HttpContentToBase64(content);
    }

    public async Task<string> HttpMessageToBase64(HttpResponseMessage response)
    {
      var content = new HttpMessageContent(response);
      return await HttpContentToBase64(content);
    }

    public async Task<HttpRequestMessage> CreateHttpRequestMessageFromBase64(string base64)
    {
      var bytes = Convert.FromBase64String(base64);
      var wrapperRequest = new HttpRequestMessage()
      {
        Content = new ByteArrayContent(bytes),
      };

      wrapperRequest.Content.Headers.Add("Content-Type", $"application/http;msgtype=request");

      var unwrapped = await wrapperRequest.Content.ReadAsHttpRequestMessageAsync();
      return unwrapped;
    }

    public async Task<HttpResponseMessage> CreateHttpResponseMessageFromBase64(string base64)
    {
      var bytes = Convert.FromBase64String(base64);
      var wrapperRequest = new HttpResponseMessage()
      {
        Content = new ByteArrayContent(bytes),
      };

      wrapperRequest.Content.Headers.Add("Content-Type", $"application/http;msgtype=response");

      var unwrapped = await wrapperRequest.Content.ReadAsHttpResponseMessageAsync();
      return unwrapped;
    }

    private static async Task<string> HttpContentToBase64(HttpMessageContent content)
    {
      var bytes = await content.ReadAsByteArrayAsync();
      return Convert.ToBase64String(bytes);
    }
  }
}
