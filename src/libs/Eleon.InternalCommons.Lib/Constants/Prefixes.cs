namespace Common.Module.Constants
{
  public class Prefixes
  {
    public static readonly Dictionary<string, string> ObjectTypePrefixes = new()
        {
            { "UserFieldSet", "UFS" },
            { "UserField", "UFF" },
            { "Audit", "AU" },
            { "BackgroundJob", "BJ" },
            { "HRCandidate", "CD" },
            { "HRJobPosition", "JP" },
            { "Chat", string.Empty },
            { "SupportTicket", string.Empty },
            { "Account", "AT" },
            { "AccountModule", "ACM" },
            { "AccountPackage", "ACP" },
            { "Company", "CP" },
            { "HRCandidateDraft", "JPD" },
            { "AccountNew", "AT" },
            { "AccountTemp", "ATT" },
            { "AccountDraft", "ATD" },
        };
  }
}
