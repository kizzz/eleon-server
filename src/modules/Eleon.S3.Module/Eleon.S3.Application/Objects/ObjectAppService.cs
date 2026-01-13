
using EleonCore.Modules.S3.Permissions;
using EleonS3.Application.Contracts.Objects;
using EleonS3.Domain;
using EleonS3.Domain.Shared.Objects;
using Logging.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Security;
using System.Text;
using System.Text.Json;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace EleonS3.Application.Objects;

public class ObjectAppService : ApplicationService, IObjectAppService
{
    private readonly ObjectDomainService _domainService;

    public ObjectAppService(ObjectDomainService domainService)
    {
        _domainService = domainService;
    }

    public async Task SaveAsync(string bucket, string key, byte[] bytes)
    {

        await _domainService.SaveAsync(bucket, key, bytes);
    }

    public async Task<S3ObjectDto> GetAsync(string bucket, string key, string range)
    {
        S3ObjectDto response = null;
        response = new S3ObjectDto();
        var s3Object = await _domainService.GetAsync(bucket, key, range);
        response.Content = s3Object.Content;
        response.End = s3Object.End;
        response.Start = s3Object.Start;
        response.Slice = s3Object.Slice;
        response.IsFullObject = s3Object.IsFullObject;
        response.Etag = s3Object.ETag;
        return response;
    }

    public async Task<ObjectMetadataDto> GetHeadAsync(string bucket, string key)
    {
        ObjectMetadataDto response = null;
        var s3Object = await _domainService.GetHeadAsync(bucket, key);

        response = ObjectMapper.Map<S3Object, ObjectMetadataDto>(s3Object);

        return response;
    }

    public async Task<bool> DeleteAsync(string bucket, string key)
    {
        bool response = false;
        response = await _domainService.DeleteAsync(bucket, key);

        return response;
    }
    public async Task<ObjectListDto> GetListFileInfoAsync(string bucket, string? prefix)
    {

        ObjectListDto response = new ObjectListDto();
        var listXml = await _domainService.ListFileInfosAsync(bucket, prefix);

        response.Xml = listXml;

        return response;
    }
}
