using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Storage.Lib.Models;

public class SftpOptions
{
  /// <summary>
  /// Corresponds to appsettings key "SFTPServer"
  /// </summary>
  public string Server { get; set; }

  /// <summary>
  /// Corresponds to appsettings key "SFTPPort" (defaults to 22)
  /// </summary>
  public int Port { get; set; } = 22;

  /// <summary>
  /// Corresponds to appsettings key "SFTPUser"
  /// </summary>
  public string User { get; set; }

  /// <summary>
  /// Corresponds to appsettings key "SFTPPassword"
  /// </summary>
  public string Password { get; set; }

  /// <summary>
  /// Corresponds to appsettings key "SFTP_LOAD_FOLDER"
  /// </summary>
  public string LoadFolder { get; set; }

  /// <summary>
  /// Optional local path to private key file (if used by SFTPHelper)
  /// Suggested appsettings key: "SFTPKeyPath"
  /// </summary>
  public string KeyPath { get; set; }

  /// <summary>
  /// Optional key passphrase if the private key is protected.
  /// Suggested appsettings key: "SFTPKeyPassphrase"
  /// </summary>
  public string KeyPassphrase { get; set; }

  /// <summary>
  /// Optional: whether to use key authentication instead of password
  /// Suggested appsettings key: "SFTPUseKeyAuthentication"
  /// </summary>
  public bool UseKeyAuthentication { get; set; } = false;

  /// <summary>
  /// Optional file name filter used when listing SFTP directory (e.g. "*.csv", "BANK_*.*"). Defaults to "*".
  /// Suggested appsettings key: "SFTPFilesFilter"
  /// </summary>
  public string FilesFilter { get; set; } = "*";
}
