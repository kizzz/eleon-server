namespace Common.Module.Constants
{
  public enum FeaturePack
  {
    None = 0,
    Masav = 1,
    PurchaseRequest = 2,
    APInvoice = 3,
    PickAndPack = 4,
    Production = 5,
    SalesOrder = 6,
    GoodReceipt = 7,
    ExpenseReport = 8,
    TravelRequest = 9,
    Inventory = 10,
    Account = 11,
    Reseller = 12,
    LanguageManagement = 13,
    Delivery = 14,
    HR = 15,
    ImmuClients = 16,
    BusinessPartner = 17,
    Items = 18,
    ImmuDeals = 19,
    TenantManagement = 20,
    PurchaseOrder = 21,
    RequestForProposal = 22,
    Company = 23,
    Currency = 24,
    UoM = 25,
  }

  public static class FeaturePackHelper
  {
    public static List<FeaturePack> GetFeaturePacks()
        => Enum.GetValues<FeaturePack>()
            .Where(x => x != FeaturePack.None)
            .ToList();
  }
}
