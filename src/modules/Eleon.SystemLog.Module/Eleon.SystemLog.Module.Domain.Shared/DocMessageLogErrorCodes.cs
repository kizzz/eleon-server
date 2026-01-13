namespace VPortal.DocMessageLog.Module;

public static class DocMessageLogErrorCodes
{

  public const string CanNotCreateWithWrongStatusCode = "Candidate:010000";
  public const string CanNotOverrideNonDraft = "Candidate:010001";
  public const string CanNotCancelFinished = "Candidate:010002";
  public const string CanNotRemoveNonDraft = "Candidate:010003";
  public const string DocumentVersionIsOutdated = "Candidate:010004";
}
