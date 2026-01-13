using System;
using Volo.Abp.Domain.Entities.Auditing;

namespace VPortal.Otp.Module.Entities
{
  public class OtpEntity : FullAuditedAggregateRoot<Guid>
  {
    /// <summary>
    /// The unique identifier of the OTP.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// To whom the OTP was sent.
    /// </summary>
    public string Recipient { get; set; }

    /// <summary>
    /// Text value of the OTP.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Detemines how many seconds after creation is the OTP valid.
    /// </summary>
    public int DurationS { get; set; }

    /// <summary>
    /// The zero-based order number of the retry attempt for which this OTP was generated, in a seria of retries.
    /// </summary>
    public int RetryAttempt { get; set; }

    /// <summary>
    /// Determines whether this OTP has been used.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>
    /// The number of failed validation attempts.
    /// </summary>
    public int FailedValidationAttempts { get; set; }

    /// <summary>
    /// Determines whether this OTP is ignored when calculating next OTP validity.
    /// </summary>
    public bool IsIgnored { get; set; }

    public OtpEntity()
    {
    }

    public OtpEntity(Guid id)
    {
      Id = id;
    }

    public bool RetryPossible(int maxAttempts)
        => maxAttempts <= 0 || RetryAttempt + 1 < maxAttempts;

    public bool IsExpired()
        => CreationTime.AddSeconds(DurationS) < DateTime.UtcNow;

    public bool IsUsable()
        => !IsExpired() && !IsUsed;

    public void MarkAsUsed()
    {
      IsUsed = true;
    }
  }
}
