using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commons.Module.Messages.Google;
public sealed class GoogleDriveUploadRequestMsg
{
  public string Name { get; set; } = default!;
  public byte[] Content { get; set; } = default!;
  public string ContentType { get; set; } = "application/octet-stream";
  public string MimeType { get; set; } = "application/octet-stream";
}

public sealed class GoogleDriveUploadResponseMsg
{
  public string? FileId { get; set; }
  public string? Error { get; set; }
}

// Delete
public sealed class GoogleDriveDeleteRequestMsg
{
  public string FileId { get; set; } = default!;
}

public sealed class GoogleDriveDeleteResponseMsg
{
  public bool Success { get; set; }
  public string? Error { get; set; }
}

// Download
public sealed class GoogleDriveDownloadRequestMsg
{
  public string FileId { get; set; } = default!;
  /// <summary>Google export MIME, e.g. "application/pdf" for Google Docs export.</summary>
  public string ContentType { get; set; } = "application/pdf";
  /// <summary>Optional filename hint for receivers.</summary>
  public string? FileName { get; set; }
}

public sealed class GoogleDriveDownloadResponseMsg
{
  public string? FileName { get; set; }
  public string? ContentType { get; set; }
  public byte[]? Content { get; set; }
  public string? Error { get; set; }
}

// Create Link
public sealed class GoogleDriveCreateLinkRequestMsg
{
  public string FileId { get; set; } = default!;
  /// <summary>"reader" | "commenter" | "writer"</summary>
  public string PermissionRole { get; set; } = "reader";
}

public sealed class GoogleDriveCreateLinkResponseMsg
{
  public string? WebViewLink { get; set; }
  public string? Error { get; set; }
}