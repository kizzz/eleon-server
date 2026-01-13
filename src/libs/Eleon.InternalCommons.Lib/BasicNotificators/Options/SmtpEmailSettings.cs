namespace VPortal.Notificator.Module.Emailing
{
  public class SmtpEmailSettings
  {
    public SmtpSettings Smtp { get; set; }
    public bool AllowFallbackForTenants { get; set; }
    public SenderSettings Sender { get; set; }

    public class SmtpSettings
    {
      public string Server { get; set; }
      public int Port { get; set; }
      public bool UseSsl { get; set; }
      public SmtpCredentials Credentials { get; set; }
    }

    public class SmtpCredentials
    {
      public string UserName { get; set; }
      public string Password { get; set; }
    }

    public class SenderSettings
    {
      public string Address { get; set; }
      public string DisplayName { get; set; }
    }

    public override string ToString()
    {
      return
          $"Smtp:\n" +
          $"  Server: {Smtp?.Server}\n" +
          $"  Port: {Smtp?.Port}\n" +
          $"  UseSsl: {Smtp?.UseSsl}\n" +
          $"  Credentials:\n" +
          $"    UserName: {Smtp?.Credentials?.UserName}\n" +
          $"    Password: {(string.IsNullOrEmpty(Smtp?.Credentials?.Password) ? "<empty>" : "<hidden>")}\n" +
          $"AllowFallbackForTenants: {AllowFallbackForTenants}\n" +
          $"Sender:\n" +
          $"  Address: {Sender?.Address}\n" +
          $"  DisplayName: {Sender?.DisplayName}";
    }
  }
}
