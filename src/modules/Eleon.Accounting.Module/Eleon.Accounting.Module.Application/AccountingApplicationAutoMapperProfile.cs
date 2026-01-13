using AutoMapper;
using VPortal.Accounting.Module.AccountPackages;
using VPortal.Accounting.Module.Accounts;
using VPortal.Accounting.Module.AuditEntities;
using VPortal.Accounting.Module.BillingInformations;
using VPortal.Accounting.Module.Entities;
using VPortal.Accounting.Module.Invoices;
using VPortal.Accounting.Module.PackageTemplates;

namespace VPortal.Accounting.Module;

public class AccountingApplicationAutoMapperProfile : Profile
{
  public AccountingApplicationAutoMapperProfile()
  {
    CreateMap<AccountDto, AccountEntity>(MemberList.None).ReverseMap();
    CreateMap<AccountPackageDto, AccountPackageEntity>(MemberList.None).ReverseMap();
    CreateMap<BillingInformationDto, BillingInformationEntity>(MemberList.None).ReverseMap();
    CreateMap<InvoiceDto, InvoiceEntity>(MemberList.None).ReverseMap();
    CreateMap<InvoiceRowDto, InvoiceRowEntity>(MemberList.None).ReverseMap();
    CreateMap<PackageTemplateDto, PackageTemplateEntity>(MemberList.None).ReverseMap();
    CreateMap<PackageTemplateModuleDto, PackageTemplateModuleEntity>(MemberList.None).ReverseMap();
    CreateMap<ReceiptDto, ReceiptEntity>(MemberList.None).ReverseMap();
    CreateMap<AccountEntity, AccountHeaderDto>(MemberList.None);
    CreateMap<AccountAuditEntity, AccountDto>(MemberList.None).ReverseMap();
    CreateMap<MemberEntity, MemberDto>(MemberList.None).ReverseMap();
  }
}
