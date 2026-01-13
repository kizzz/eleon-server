using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.ProxyClient.UI.Windows.Exceptions
{
    public interface IErrorProvider
    {
        string LastError { get; }
    }
}
