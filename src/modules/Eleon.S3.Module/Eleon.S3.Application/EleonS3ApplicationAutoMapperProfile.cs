using AutoMapper;
using EleonS3.Application.Contracts.Buckets;
using EleonS3.Application.Contracts.Multipart;
using EleonS3.Application.Contracts.Objects;
using EleonS3.Domain.Shared.Objects;
using Volo.Abp.AutoMapper;
namespace VPortal.BusinessPartner.Feature.Module;

public class EleonS3ApplicationAutoMapperProfile : Profile
{
    public EleonS3ApplicationAutoMapperProfile ()
    {
        CreateMap<S3Object, ObjectMetadataDto>();
    }
}
