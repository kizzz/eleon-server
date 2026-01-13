using Microsoft.Exchange.WebServices.Data;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace EleonsoftSdk.modules.Azure;


// Azure Mail Service using EWS with OAuth2 (MSAL)
public class AzureMailService
{
  private static ExchangeService _exchangeService;
  private readonly AzureEwsOptions _options;

  public AzureMailService(AzureEwsOptions options)
  {
    ArgumentNullException.ThrowIfNull(options, nameof(options));

    _options = options;
  }

  private static async Task<ExchangeService> CreateExchangeService(AzureEwsOptions options, CancellationToken ct = default)
  {
    ArgumentNullException.ThrowIfNull(options);

    // Enforce TLS 1.2 (older frameworks sometimes need this explicitly)
    // System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

    // Build MSAL confidential client
    var authority = string.Format(
        CultureInfo.InvariantCulture,
        "https://login.microsoftonline.com/{0}",
        options.TenantId);

    var _cca = ConfidentialClientApplicationBuilder
        .Create(options.ClientId)
        .WithClientSecret(options.ClientSecret)
        .WithAuthority(new Uri(authority))
        .Build();


    // Acquire token (app-only)
    var authResult = await _cca
        .AcquireTokenForClient(options.EwsScopes)
        .ExecuteAsync(ct)
        .ConfigureAwait(false);

    // Configure EWS
    var service = new ExchangeService
    {
      Url = new Uri(options.ExchangeUrl),
      Credentials = new OAuthCredentials(authResult.AccessToken),
      TraceEnabled = options.TraceEnabled
    };

    // Optional impersonation
    if (!string.IsNullOrWhiteSpace(options.ImpersonatedSmtpAddress))
    {
      service.ImpersonatedUserId =
          new ImpersonatedUserId(ConnectingIdType.SmtpAddress, options.ImpersonatedSmtpAddress);
    }

    // X-AnchorMailbox helps routing in EXO
    var anchor = string.IsNullOrWhiteSpace(options.AnchorMailbox)
        ? options.ImpersonatedSmtpAddress
        : options.AnchorMailbox;

    if (!string.IsNullOrWhiteSpace(anchor))
    {
      service.HttpHeaders["X-AnchorMailbox"] = anchor!;
    }

    // (Optional) Ignore SSL errors — strongly discouraged outside dev/test
    if (options.IgnoreServerCertificateErrors)
    {
#pragma warning disable SYSLIB0014
      System.Net.ServicePointManager.ServerCertificateValidationCallback =
          (_, __, ___, ____) => true;
#pragma warning restore SYSLIB0014
    }

    return service;
  }

  private static async Task<ExchangeService> GetActiveConnection(AzureEwsOptions options, CancellationToken ct = default)
  {
    if (_exchangeService == null || !await IsExchangeActiveAsync(_exchangeService))
    {
      CloseConnection();
      _exchangeService = await CreateExchangeService(options, ct);
    }

    return _exchangeService;
  }

  public static void CloseConnection()
  {
    _exchangeService = null;
  }

  public static async Task<bool> IsExchangeActiveAsync(ExchangeService service)
  {
    try
    {
      // Use a lightweight call, e.g., to the Inbox folder
      var folder = await Task.Run(() => Folder.Bind(service, WellKnownFolderName.Inbox));
      return true;
    }
    catch (ServiceRequestException)
    {
      // Typically network or authentication problem
      return false;
    }
    catch (ServiceResponseException)
    {
      // Service responded but with an error
      return false;
    }
    catch (Exception)
    {
      // Other errors (auth, timeout, etc.)
      return false;
    }
  }

  public void Close()
  {
    CloseConnection();
  }

  public async Task SendEmailAsync(List<string> toEmails, string subject, string body, bool isHtmp = true, Dictionary<string, string> attachments = null)
  {
    ArgumentNullException.ThrowIfNull(toEmails, nameof(toEmails));

    var exchangeService = await GetActiveConnection(_options);

    var email = new EmailMessage(exchangeService)
    {
      Subject = subject,
      Body = new MessageBody(isHtmp ? BodyType.HTML : BodyType.Text, body)
    };

    foreach (var toEmail in toEmails)
    {
      email.ToRecipients.Add(toEmail);
    }

    if (attachments != null)
    {
      foreach (var attachment in attachments)
      {
        var decodedValue = Convert.FromBase64String(attachment.Value);
        email.Attachments.AddFileAttachment(attachment.Key, decodedValue);
      }
    }

    email.SendAndSaveCopy();
  }

  public async Task<IEnumerable<Item>> LoadPropertiesAsync(IEnumerable<Item> items, PropertySet propertySet)
  {
    var exchangeService = await GetActiveConnection(_options);

    exchangeService.LoadPropertiesForItems(
        items,
        propertySet);

    return items;
  }

  /// <summary>
  /// Reads an emails from azure ews
  /// </summary>
  /// <param name="wellKnownFolderName">Folder name to read</param>
  /// <param name="skip">skip count</param>
  /// <param name="take">take count</param>
  /// <param name="propertySet">Properties for requst info. Configures which information will be requested from email server. Read more in the end of class.</param>
  /// <returns></returns>
  public async Task<IReadOnlyList<Item>> ReadItemsAsync(WellKnownFolderName wellKnownFolderName, int skip = 0, int take = 100, PropertySet propertySet = null)
  {
    var exchangeService = await GetActiveConnection(_options);

    propertySet ??= new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived)
    {
      RequestedBodyType = BodyType.Text
    };

    var view = new ItemView(take, skip)
    {
      PropertySet = propertySet
    };
    var findResults = exchangeService.FindItems(wellKnownFolderName, view);
    return findResults.Items;
  }


  /// <summary>
  /// Reads an emails from azure ews
  /// </summary>
  /// <param name="folderName">Folder name to read</param>
  /// <param name="skip">skip count</param>
  /// <param name="take">take count</param>
  /// <param name="propertySet">Properties for requst info. Configures which information will be requested from email server. Read more in the end of class.</param>
  /// <returns></returns>
  public async Task<IReadOnlyList<Item>> ReadItemsAsync(string folderName, int skip = 0, int take = 100, PropertySet propertySet = null)
  {
    var exchangeService = await GetActiveConnection(_options);

    propertySet ??= new PropertySet(BasePropertySet.IdOnly, ItemSchema.Subject, ItemSchema.DateTimeReceived)
    {
      RequestedBodyType = BodyType.Text
    };

    var view = new ItemView(take, skip);
    var findResults = exchangeService.FindItems(folderName, view);
    return findResults.Items;
  }

  public async Task<IReadOnlyList<Folder>> ReadFoldersAsync(WellKnownFolderName wellKnownFolderName, int skip = 0, int take = 100)
  {
    var exchangeService = await GetActiveConnection(_options);

    var rootfolder = Folder.Bind(exchangeService, wellKnownFolderName);
    var view = new FolderView(take, skip);
    var findResults = rootfolder.FindFolders(view);
    return findResults.ToList();
  }

  public async Task MoveItemAsync(ItemId itemId, FolderId destinationFolder)
  {
    var exchangeService = await GetActiveConnection(_options);

    try
    {
      var item = await Task.Run(() => Item.Bind(exchangeService, itemId));
      var movedItem = item.Move(destinationFolder);
    }
    catch (ServiceResponseException ex)
    {
      throw new InvalidOperationException($"EWS service error while moving item: {ex.Message}", ex);
    }
    catch (ServiceRequestException ex)
    {
      throw new InvalidOperationException($"EWS request failed while moving item: {ex.Message}", ex);
    }
    catch (Exception ex)
    {
      throw new InvalidOperationException($"Unexpected error while moving item: {ex.Message}", ex);
    }
  }

  public async Task TestConnectionAsync()
  {
    var exchangeService = await GetActiveConnection(_options);
  }

  /*

What you can configure inside PropertySet

You can add many schemas depending on what data you need:

📄 Message contents

ItemSchema.Body → full message body (subject to RequestedBodyType).

ItemSchema.Mentions → mentions (only Exchange 2015+).

ItemSchema.Importance → Low, Normal, High.

👤 People info

EmailMessageSchema.From → sender.

EmailMessageSchema.Sender → sending account.

EmailMessageSchema.ToRecipients, CcRecipients, BccRecipients → recipients.

📎 Attachments

ItemSchema.Attachments → metadata of attachments (for full data, you must call Load later).

📅 Time-related

ItemSchema.DateTimeSent

ItemSchema.DateTimeCreated

ItemSchema.LastModifiedTime

📌 Flags / categories

ItemSchema.Categories → category labels.

EmailMessageSchema.IsRead → read/unread flag.

EmailMessageSchema.IsDraft → draft flag.

Example with more properties
var propertySet = new PropertySet(BasePropertySet.IdOnly,
  ItemSchema.Subject,
  ItemSchema.DateTimeReceived,
  ItemSchema.Body,
  EmailMessageSchema.From,
  EmailMessageSchema.ToRecipients,
  ItemSchema.Attachments)
{
  RequestedBodyType = BodyType.Text,
  Size = int.MaxValue, // optional: max size of properties retrieved
  RequestedAdditionalProperties = null // you can also extend later
};

   */
}
