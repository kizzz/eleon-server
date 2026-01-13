using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.BlobStoring.FileSystem;

namespace Eleon.Storage.Lib.CustomBlobProviders.FileSystem
{
  public class EleonFileSystemBlobProvider : FileSystemBlobProvider
  {

    public EleonFileSystemBlobProvider(IBlobFilePathCalculator filePathCalculator)
      : base(filePathCalculator)
    {
    }
  }
}
