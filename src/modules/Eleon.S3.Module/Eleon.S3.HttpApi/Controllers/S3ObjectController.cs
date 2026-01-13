using System.Text;
using System.Threading.Tasks;
using EleonS3.Application.Contracts.Objects;
using EleonS3.HttpApi.Helpers;
using Logging.Module;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using Volo.Abp.AspNetCore.Mvc;

namespace EleonS3.HttpApi.Controllers;

[ApiController]
[Route("s3")]
public class S3ObjectController : AbpController
{
    
    private readonly IVportalLogger<S3ObjectController> _logger;

    private readonly IObjectAppService _objectAppService;
    public S3ObjectController(IObjectAppService objectAppService, IVportalLogger<S3ObjectController> logger)
    {
        _objectAppService = objectAppService;
        _logger = logger;
    }

    [HttpPost("{storageProviderId}/{**key}")]
    public async Task<IActionResult> InitiateMultipartUpload(string storageProviderId, string key, [FromQuery] string? uploads)
    {

        _logger.Capture(new Exception("EleonS3 currently not supports multipart upload"));


        return BadRequest();
    }

    [HttpPut("{storageProviderId}/{**key}")]
    public async Task<IActionResult> Put(string storageProviderId, string key,
        [FromQuery] int? partNumber, [FromQuery] string? uploadId)
    {

        if (partNumber != null || uploadId != null)
        {
            _logger.Capture(new Exception("EleonS3 currently not supports multipart upload"));
            return BadRequest();
        }

        if (Request.Body == null)
        {
            return BadRequest(new { Error = "MissingRequestBody" });
        }

        byte[] bytes;
        try
        {
            if (Request.Headers.TryGetValue("x-amz-decoded-content-length", out _))
            {
                // Decode AWS chunked body
                bytes = await AwsSigV4ChunkedDecoder.DecodeAsync(Request.Body);
            }
            else
            {
                using var ms = new MemoryStream();
                await Request.Body.CopyToAsync(ms);
                bytes = ms.ToArray();
            }

            await _objectAppService.SaveAsync(storageProviderId, key, bytes);


            return Ok(new { Bucket = storageProviderId, Key = key, Size = bytes.Length });
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }


    [HttpGet("{bucket}/{**key}")]
    public async Task<IActionResult> Get(string bucket, string key)
    {
        try
        {
            var range = Request.Headers[HeaderNames.Range].ToString();
            var contentType = "application/octet-stream";

            var s3Object = await _objectAppService.GetAsync(bucket, key, range);

            if (!s3Object.IsFullObject)
            {
                Response.StatusCode = 206;
                Response.Headers[HeaderNames.ContentRange] = $"bytes {s3Object.Start}-{s3Object.End}/{s3Object.Content.Length}";
                return File(s3Object.Slice, contentType);
            }

            return File(s3Object.Content, contentType);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpHead("{bucket}/{**key}")]
    public async Task<IActionResult> Head(string bucket, string key)
    {
        try
        {
            var result = await _objectAppService.GetHeadAsync(bucket, key);

            if (result == null)
            {
                return NotFound(new { Error = "NoSuchKey", Message = $"Key '{key}' not found in bucket '{bucket}'" });
            }

            Response.ContentType = "application/octet-stream";
            Response.ContentLength = result.ContentLength;
            Response.Headers.ETag = result.ETag;
            return Ok();
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpDelete("{bucket}/{**key}")]
    public async Task<IActionResult> Delete(string bucket, string key, [FromQuery] string? uploadId)
    {

        try
        {

            if (uploadId != null)
            {
                _logger.Capture(new Exception("EleonS3 currently not supports multipart upload"));
                return BadRequest();
            }

            if (await _objectAppService.DeleteAsync(bucket, key))
            {
                return NoContent();
            }


            return NotFound(new { Error = "NoSuchKey", Message = $"Key '{key}' not found in bucket '{bucket}'" });
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }

    [HttpGet("{bucket}")]
    public async Task<IActionResult> ListObjectsV2(string bucket, [FromQuery(Name = "list-type")] int? listType, [FromQuery] string? prefix)
    {
        try
        {

            if (listType != 2)
            {
                return BadRequest();
            }

            var result = await _objectAppService.GetListFileInfoAsync(bucket, prefix);


            return Content(result.Xml, "application/xml", Encoding.UTF8);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}
