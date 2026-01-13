using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.GatewayClient.UI.Windows.Exceptions
{
    public interface IErrorProvider
    {
        string LastError { get; }
    }
}
