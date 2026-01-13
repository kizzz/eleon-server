using Logging.Module.ErrorHandling.Constants;
using Logging.Module.ErrorHandling.Extensions;
using ProxyRouter.Minimal.HttpApi.ErrorHandling.Exceptions;
using ProxyRouter.Minimal.HttpApi.Helpers;
using ProxyRouter.Minimal.HttpApi.Models.Messages;

namespace VPortal.ProxyRouter
{
  public class HtmlHelper
  {
    // TODO: remove hardcoded HTML template from here (Vladi said to hardcode it, I am not guilty!!!!!)
    public const string Template = @"
<!doctype html>
<html lang=""en"" data-critters-container>

<head>
  <meta charset=""utf-8"">
  <title>{title}</title>
  <base href=""{appPath}"">
  {pwaLink}
  <script>
    {script}
  </script>
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <link rel=""shortcut icon"" href=""/resources/media/logos/favicon-logo-tree-gray.ico"">
  <link rel=""icon"" type=""image/x-icon"" href=""favicon.ico"">
  <style>
    body {
      margin: 0;
      padding: 0;
    }

    #splash-screen {
      position: absolute;
      z-index: 1000;
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      background-color: #f2f3f8;
    }
    #retry {
      position: absolute;
      z-index: 1000;
      width: 100%;
      height: 100%;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      background-color: rgba(0,0,0,0.5);
    }

    #splash-screen img {
      margin-left: calc(100vw - 100%);
      margin-bottom: 30px;
    }

    #splash-screen.hidden {
      opacity: 0;
      visibility: hidden;
    }
    #retry.hidden {
      opacity: 0;
      visibility: hidden;
    }

    .splash-spinner {
      animation: rotate 2s linear infinite;
      margin-left: calc(100vw - 100%);
      width: 50px;
      height: 50px;
    }

    .splash-spinner .path {
      stroke: #5d78ff;
      stroke-linecap: round;
      animation: dash 1.5s ease-in-out infinite;
    }

    @keyframes rotate {
      100% {
        transform: rotate(360deg);
      }
    }

    @keyframes dash {
      0% {
        stroke-dasharray: 1, 150;
        stroke-dashoffset: 0;
      }

      50% {
        stroke-dasharray: 90, 150;
        stroke-dashoffset: -35;
      }

      100% {
        stroke-dasharray: 90, 150;
        stroke-dashoffset: -124;
      }
    }


    #root {
      opacity: 1;
      transition: opacity 1s ease-in-out;
    }
  </style>
</head>

<body dir=""ltr"">" +
@"<div id=""splash-screen"" class=""splash-screen"">
    <svg class=""splash-spinner"" viewBox=""0 0 50 50"">
      <circle class=""path"" cx=""25"" cy=""25"" r=""20"" fill=""none"" stroke-width=""5""/>
    </svg>
  </div>" +
@"<div id=""retry"" class=""retry-screen hidden"">
    <div style=""background-color: white; border-radius: 10px; padding: 1rem;"">
      <p>Auth server is offline, retry later</p>
      <button style=""background-color: #007BFF; color: white; padding: 10px 20px; border: none; border-radius: 5px; font-size: 16px; cursor: pointer;""
        >
        Retry
        </button>
    </div>

  </div>
  <app-root></app-root>
  <noscript>Please enable JavaScript to continue using this application.</noscript>
  <script src=""{appPath}polyfills.js"" type=""module""></script>
  <script src=""{appPath}main.js"" type=""module""></script>
</body>
</html>

        ";


    public static string GenerateHtml(string title, string path, string pwaLink)
    {
      try
      {
        string template = Template
            .Replace("{title}", title)
            .Replace("{pwaLink}", pwaLink)
            .Replace("{script}", "")
            .Replace("{appPath}", path.EnsureEndsWith('/'))
            ;
        return template;
      }
      catch (Exception ex)
      {
        if (ex is not ProxyException prEx)
        {
          prEx = new ProxyException("An error has occured while generating html response");
        }

        throw prEx
            .WithStatusCode(EleonsoftStatusCodes.Proxy.GeneratingProxySourcesError);
      }
    }
  }
}
