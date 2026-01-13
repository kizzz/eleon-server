using Common.Module.Constants;
using Messaging.Module.ETO;

namespace Messaging.Module.Messages
{
  [Common.Module.Events.DistributedEvent]
  public class OtpValidatedMsg : VportalEvent
  {
    public OtpValidationResultEto Result { get; set; }
  }

  public class OtpValidationResultEto
  {
    public OtpValidationResultEto(bool valid, string errorMessage)
    {
      Valid = valid;
      ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the provided password is valid.
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// A validation error message, if the code is invalid, null otherwise.
    /// </summary>
    public string ErrorMessage { get; set; }
  }
}
