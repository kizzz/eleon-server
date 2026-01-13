namespace EleonS3.Application.Contracts.Objects;

public class GetObjectOutputDto
{
    public string ContentType { get; set; } = "application/octet-stream";
    public long ContentLength { get; set; }
    public Stream Stream { get; set; } = default!;
}
