namespace VPortal.Accounting.Module;

public static class AccountingErrorCodes
{
  public const string CanNotCreateWithWrongStatusCode = "Accounting:010000";
  public const string CanNotOverrideNonDraft = "Accounting:010001";
  public const string CanNotAddActualDataToNonNew = "Accounting:010002";
  public const string CanNotCancelFinished = "Accounting:010003";
  public const string CanNotRemoveNonDraft = "Accounting:010004";
  public const string DocumentVersionIsOutdated = "Accounting:010005";
}
