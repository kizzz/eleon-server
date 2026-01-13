using AutoMapper;
using Common.Module.ValueObjects;
using VPortal.Accounting.Module.AuditEntities;
using VPortal.Accounting.Module.Entities;

namespace VPortal.Accounting.Module;

public class AccountingDomainAutoMapperProfile : Profile
{
  public AccountingDomainAutoMapperProfile()
  {
    CreateMap<AccountEntity, AccountAuditEntity>(MemberList.None).ReverseMap();
    CreateMap<AccountEntity, AccountValueObject>(MemberList.None).ReverseMap();
  }
}
