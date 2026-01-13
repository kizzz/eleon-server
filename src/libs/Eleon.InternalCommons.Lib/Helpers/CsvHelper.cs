using System.Text;

namespace Common.Module.Helpers
{
  public class CsvUser
  {
    public string UserName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string[] OrganizationUnitNames { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string Status { get; set; }
    public string Message { get; set; }

    public CsvUser(
                string userName,
                string email)
    {
      UserName = userName;
      Email = email;
    }
  }
  public static class CsvHelper
  {
    public static List<CsvUser> GetCsvUsers(string csvContent)
    {
      List<CsvUser> result = new List<CsvUser>();
      StringReader reader = new StringReader(csvContent);
      //reader.ReadLine();
      string line;
      while ((line = reader.ReadLine()) != null)
      {
        string[] fields = line.Split(';');

        if (fields.Length < 6)
        {
          // Skip invalid rows
          continue;
        }

        if (fields.All(string.IsNullOrWhiteSpace))
        {
          continue;
        }

        string userName = fields[0];
        string email = fields[1];
        string phoneNumber = fields[2];
        string[] organizationUnitNames = fields[3].Split(','); // Assuming organization units are separated by ';'
        string fullName = fields[4];
        string password = fields[5];

        var user = new CsvUser(
            userName,
            email
        )
        {
          PhoneNumber = phoneNumber,
          OrganizationUnitNames = organizationUnitNames,
          FullName = fullName,
          Password = password,
        };

        if (user.UserName == nameof(user.UserName) &&
            user.Email == nameof(user.Email) &&
            user.PhoneNumber == nameof(user.PhoneNumber) &&
            user.Password == nameof(user.Password))
        {
          continue;
        }

        result.Add(user);
      }

      reader.Dispose();

      return result;
    }
    public static string ConvertToCsvString(List<CsvUser> users)
    {
      StringBuilder csvContent = new StringBuilder();

      // Optional: Write header line
      csvContent.AppendLine("UserName;Email;PhoneNumber;OrganizationUnitNames;FullName;Password;Status;Message");

      foreach (var user in users)
      {
        string organizationUnits = "\"" + string.Join(",", user.OrganizationUnitNames) + "\""; // Handle commas within quotes for CSV format

        // Create CSV line for each user
        csvContent.AppendLine(
            $"{EscapeCsvValue(user.UserName)};" +
            $"{EscapeCsvValue(user.Email)};" +
            $"{EscapeCsvValue(user.PhoneNumber)};" +
            $"{organizationUnits};" +
            $"{EscapeCsvValue(user.FullName)};" +
            $"{EscapeCsvValue(user.Password)};" +
            $"{EscapeCsvValue(user.Status)};" +
            $"{EscapeCsvValue(user.Message)}"
        );
      }

      return csvContent.ToString();
    }

    private static string EscapeCsvValue(string value)
    {
      if (string.IsNullOrEmpty(value))
      {
        return "";
      }
      // Enclose in quotes if the value contains a semicolon, quote, or newline
      if (value.Contains(';') || value.Contains('"') || value.Contains("\n"))
      {
        return $"\"{value.Replace("\"", "\"\"")}\""; // Double quotes for CSV format
      }
      return value;
    }
  }
}
