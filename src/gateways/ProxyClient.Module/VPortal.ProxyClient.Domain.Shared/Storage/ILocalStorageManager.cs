using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.Domain.Shared.Storage
{
    public interface ILocalStorageManager
    {
        Task<string> ReadFile(string filename);
        Task<bool> WriteFile(string filename, string content);
    }
}
