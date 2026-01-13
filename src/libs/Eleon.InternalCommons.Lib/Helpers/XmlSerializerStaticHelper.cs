using System.Xml.Serialization;

namespace Common.Module.Helpers;

public static class XmlSerializerStaticHelper
{
  public static string SerializeToXml<T>(this T toSerialize)
  {
    var xmlSerializer = new XmlSerializer(toSerialize.GetType());
    using var textWriter = new StringWriter();
    xmlSerializer.Serialize(textWriter, toSerialize);
    return textWriter.ToString();
  }

  public static T DeserializeFromXml<T>(this string toDeserialize, string[] ignored = null)
  {
    var xmlSerializer = XmlSerializerHelper.GetSerializer(typeof(T), ignored);
    using var reader = new StringReader(toDeserialize);
    return (T)xmlSerializer.Deserialize(reader);
  }
}
