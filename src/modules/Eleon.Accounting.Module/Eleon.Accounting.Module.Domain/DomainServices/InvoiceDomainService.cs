using Common.Module.Helpers;
using Logging.Module;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using VPortal.Accounting.Module.Repositories;

namespace VPortal.Accounting.Module.DomainServices
{

  public class InvoiceDomainService : DomainService
  {
    private readonly IVportalLogger<InvoiceDomainService> logger;
    private readonly IDistributedEventBus massTransitPublisher;
    private readonly XmlSerializerHelper _xmlSerializerHelper;
    private readonly IObjectMapper objectMapper;
    private readonly IAccountRepository accountRepository;
    private readonly IdentityUserManager userManager;

    public InvoiceDomainService(IVportalLogger<InvoiceDomainService> logger,
        IObjectMapper objectMapper,
        XmlSerializerHelper _xmlSerializerHelper,
        IDistributedEventBus massTransitPublisher,
        IAccountRepository accountRepository,
        IdentityUserManager userManager)
    {
      this.logger = logger;
      this.objectMapper = objectMapper;
      this._xmlSerializerHelper = _xmlSerializerHelper;
      this.massTransitPublisher = massTransitPublisher;
      this.accountRepository = accountRepository;
      this.userManager = userManager;
    }
  }
}
