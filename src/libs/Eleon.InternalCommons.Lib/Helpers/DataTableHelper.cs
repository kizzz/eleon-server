using System.Data;
using System.Text;

namespace Common.Module.Helpers
{
  public static class DataTableHelper
  {
    public static DataTable CreateDataTable<T>(string tableName, IEnumerable<T> rows, params (string title, Func<T, string> valueSelector)[] columns)
    {
      var table = new DataTable();
      table.TableName = tableName;

      foreach (var col in columns)
      {
        var dtCol = new DataColumn()
        {
          DataType = typeof(string),
          ColumnName = col.title,
        };

        table.Columns.Add(dtCol);
      }

      foreach (var row in rows)
      {
        var dtRow = table.NewRow();
        foreach (var col in columns)
        {
          dtRow[col.title] = col.valueSelector(row);
        }

        table.Rows.Add(dtRow);
      }

      return table;
    }

    public static string ToXml(DataTable table)
    {
      using var sw = new StringWriter();
      table.WriteXml(sw, XmlWriteMode.WriteSchema);
      return sw.ToString();
    }

    public static DataTable FromXml(string xml)
    {
      using var ms = new MemoryStream(Encoding.Unicode.GetBytes(xml));
      var table = new DataTable();
      table.ReadXml(ms);
      return table;
    }
  }
}
