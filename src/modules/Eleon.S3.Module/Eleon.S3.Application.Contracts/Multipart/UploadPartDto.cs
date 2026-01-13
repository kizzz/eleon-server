namespace EleonS3.Application.Contracts.Multipart;

public class UploadPartDto
{
    public string BucketName { get; set; } = default!;
    public string Key { get; set; } = default!;
    public string UploadId { get; set; } = default!;
    public int PartNumber { get; set; }
    public Stream Stream { get; set; } = default!;
    public long Size { get; set; }
    public string ETag { get; set; } = default!;
}
