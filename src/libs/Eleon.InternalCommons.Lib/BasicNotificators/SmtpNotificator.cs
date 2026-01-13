using EleonsoftModuleCollector.Notificator.Module.Notificator.Module.Domain.Notificators;
using Logging.Module;
using MailKit.Security;
using MimeKit;
using Volo.Abp.DependencyInjection;
using VPortal.Notificator.Module.Emailing;

namespace VPortal.Notificator.Module.Notificators.Implementations
{
  public class SmtpNotificator
  {
    private readonly IVportalLogger<SmtpNotificator> logger;
    private readonly NotificatorBaseHelperService _notificatorHelperService;

    public SmtpNotificator(
        IVportalLogger<SmtpNotificator> logger,
        NotificatorBaseHelperService recipientResolver
        )
    {
      this.logger = logger;
      _notificatorHelperService = recipientResolver;
    }

    public async Task SendEmailAsync(SmtpEmailSettings settings, string subject, string body, List<string> targetEmails, string sourceAddress = null, bool isHtml = true, Dictionary<string, string> attachments = null)
    {
      ArgumentNullException.ThrowIfNull(settings);

      targetEmails = _notificatorHelperService.FilterEmails(targetEmails);

      if (targetEmails.Count == 0)
      {
        return;
      }

      var email = new MimeMessage
      {
        Subject = _notificatorHelperService.GetValidatedSubjectOrDefault(subject)
      };

      // Set the sender's address
      email.From.Add(new MailboxAddress(settings.Sender.DisplayName, settings.Sender.Address));
      if (!string.IsNullOrWhiteSpace(sourceAddress))
      {
        email.From.Clear();
        email.From.Add(MailboxAddress.Parse(sourceAddress));
      }

      // Set the main recipient
      foreach (var targetEmail in targetEmails)
      {
        email.To.Add(MailboxAddress.Parse(targetEmail));
      }

      // Set the email body
      email.Body = new TextPart(isHtml ? "html" : "plain") { Text = body };

      if (attachments != null && attachments.Count > 0)
      {
        var multipart = new Multipart("mixed") { email.Body };
        foreach (var attachment in attachments)
        {
          var attachmentPart = new MimePart()
          {
            Content = new MimeContent(new MemoryStream(Convert.FromBase64String(attachment.Value))),
            ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
            ContentTransferEncoding = ContentEncoding.Base64,
            FileName = attachment.Key
          };
          multipart.Add(attachmentPart);
        }
        email.Body = multipart;
      }

      // Create and configure the SMTP client
      using var smtpClient = new MailKit.Net.Smtp.SmtpClient();

      // Connect securely
      smtpClient.Connect(settings.Smtp.Server, settings.Smtp.Port, GetSecureSocketOption(settings.Smtp.Port, settings.Smtp.UseSsl));

      // Authenticate only if credentials are provided
      if (!string.IsNullOrWhiteSpace(settings.Smtp.Credentials?.UserName) && !string.IsNullOrWhiteSpace(settings.Smtp.Credentials?.Password))
      {
        smtpClient.Authenticate(settings.Smtp.Credentials.UserName, settings.Smtp.Credentials.Password);
      }

      // Send the email
      smtpClient.Send(email);

      // Disconnect
      smtpClient.Disconnect(true);
    }

    private static SecureSocketOptions GetSecureSocketOption(int port, bool enableSsl)
    {
      if (!enableSsl)
        return SecureSocketOptions.None;

      return port switch
      {
        465 => SecureSocketOptions.SslOnConnect, // Implicit SSL
        587 => SecureSocketOptions.StartTls,     // Explicit SSL (STARTTLS)
        _ => SecureSocketOptions.Auto            // Let MailKit decide
      };
    }
  }
}
