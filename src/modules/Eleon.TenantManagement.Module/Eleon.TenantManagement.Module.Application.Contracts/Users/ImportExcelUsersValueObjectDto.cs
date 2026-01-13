using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPortal.TenantManagement.Module.Users
{
  public class ImportExcelUsersValueObjectDto
  {
    public bool Error { get; set; }
    public List<string> ErrorMessages { get; set; } = new List<string>();
    public string CsvUser { get; set; }
  }
}
