using Common.Module.Constants;
using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;
using VPortal.Accounting.Module.Constants;

namespace VPortal.Accounting.Module.Accounts
{
  public class AccountListRequestDto : PagedAndSortedResultRequestDto
  {
    public string SearchQuery { get; set; }
    public AccountListRequestType RequestType { get; set; }
    public DateTime? CreationDateFilterStart { get; set; }
    public DateTime? CreationDateFilterEnd { get; set; }
    public string InitiatorNameFilter { get; set; }
    public IList<AccountStatus> AccountStatusFilter { get; set; }
    public LifecycleActorTypes? ActorTypeFilter { get; set; }
    public string ActorRefIdFilter { get; set; }
    public bool? ApprovalNeededFilter { get; set; }
    public IList<Guid> OrganizationUnitFilter { get; set; }
  }
}
