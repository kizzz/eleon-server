using Microsoft.Extensions.Logging;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
using Volo.Abp.DependencyInjection;

namespace Common.Module.Helpers
{
  public class XmlSerializerHelper
  {
    private static readonly object SerializersLock = new();
    private static readonly Dictionary<string, XmlSerializer> SerializersCache = new();

    private readonly ILogger<XmlSerializerHelper> _logger;

    public XmlSerializerHelper(
        ILogger<XmlSerializerHelper> logger)
    {
      _logger = logger;
    }


    // TODO: Make this method private
    public static XmlSerializer GetSerializer(Type type, string[] ignored)
    {
      var cacheKey = GetCacheKey(type, ignored ?? Array.Empty<string>());
      lock (SerializersLock)
      {
        var cachedSerializer = SerializersCache.GetValueOrDefault(cacheKey);
        if (cachedSerializer != null)
        {
          return cachedSerializer;
        }
      }

      var serializerToCache = CreateSerializer(type, ignored ?? Array.Empty<string>());
      lock (SerializersLock)
      {
        var cachedSerializer = SerializersCache.GetValueOrDefault(cacheKey);
        if (cachedSerializer != null)
        {
          return cachedSerializer;
        }
        else
        {
          SerializersCache[cacheKey] = serializerToCache;
          return serializerToCache;
        }
      }
    }

    private static XmlSerializer CreateSerializer(Type type, string[] userIgnored)
    {
      var xOver = new XmlAttributeOverrides();
      var attrs = new XmlAttributes();
      attrs.XmlIgnore = true;
      var typeHierarchy = GetInheritanceHierarchy(type);
      var ignored = userIgnored?.Distinct() ?? Array.Empty<string>();
      foreach (var prop in ignored)
      {
        foreach (var typeInHierarchy in typeHierarchy)
        {
          xOver.Add(typeInHierarchy, prop, attrs);
        }
      }

      return new XmlSerializer(type, xOver);
    }

    private static IEnumerable<string> GetDefaultIgnoredProperties(Type type)
    {
      var props = type.GetProperties();
      foreach (var prop in props)
      {
        if (prop.PropertyType.IsInterface
            || typeof(IDictionary).IsAssignableFrom(prop.PropertyType))
        {
          yield return prop.Name;
        }
      }
    }

    private static string GetCacheKey(Type type, string[] ignored)
        => string.Join(';', type.GetHashCode(), string.Join(';', ignored));

    private static IEnumerable<Type> GetInheritanceHierarchy(Type type)
    {
      for (var current = type; current != null; current = current.BaseType)
      {
        yield return current;
      }
    }

    public string AddExtraProperties(string path, string name, string entityXml, Dictionary<string, string> extraProperties)
    {
      var xmlDocument = new XmlDocument();
      try
      {
        xmlDocument.LoadXml(entityXml);

        var xmlNode = xmlDocument.SelectSingleNode(path);
        if (xmlNode == null)
        {
          return entityXml;
        }

        var extraPropertiesNode = xmlDocument.CreateNode(XmlNodeType.Element, name, "");
        foreach (var pair in extraProperties)
        {
          var extraPropertyNode = xmlDocument.CreateNode(XmlNodeType.Element, pair.Key.Replace(":", "-"), "");
          extraPropertyNode.InnerText = pair.Value;
          extraPropertiesNode.AppendChild(extraPropertyNode);
        }

        xmlNode.AppendChild(extraPropertiesNode);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while adding extra properties");
      }
      finally
      {
        _logger.LogDebug("Finished adding extra properties");
      }

      return xmlDocument.OuterXml;
    }


    #region XmlExtensions Methods

    /// <summary>
    /// An extension method for serializing an arbitrary object to XML.
    /// </summary>
    /// <typeparam name="T">The type of serialized object.</typeparam>
    /// <param name="toSerialize">An object to serialize.</param>
    /// <returns>A string containing the XML representation of the object.</returns>
    public string SerializeToXml<T>(T toSerialize)
    {
      var result = string.Empty;
      try
      {
        var x = toSerialize.GetType();
        var xmlSerializer = new XmlSerializer(toSerialize.GetType());
        using var textWriter = new StringWriter();
        xmlSerializer.Serialize(textWriter, toSerialize);
        result = textWriter.ToString();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while serializing object to XML");
      }

      return result;
    }

    /// <summary>
    /// An extension method for serializing a list objects to list XML.
    /// </summary>
    /// <typeparam name="T">The type of serialized object.</typeparam>
    /// <param name="toSerializeList">An object to serialize.</param>
    /// <returns>A string containing the XML representation of the object.</returns>
    public IEnumerable<string> SerializeManyToXml<T>(List<T> toSerializeList)
    {
      XmlSerializer xmlSerializer = null;
      try
      {
        xmlSerializer = GetSerializer(typeof(T), null);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while serializing many objects to XML");
        throw;
      }

      if (xmlSerializer is null)
      {
        yield break;
      }

      foreach (var toSerialize in toSerializeList)
      {
        using var textWriter = new StringWriter();
        try
        {
          xmlSerializer.Serialize(textWriter, toSerialize);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error occurred while serializing many objects to XML");
        }

        yield return textWriter.ToString();
      }
    }

    ///// <summary>
    ///// An extension method for serializing an IUserFieldsFillable object to XML.
    ///// </summary>
    ///// <typeparam name="T">The type of serialized object.</typeparam>
    ///// <param name="toSerialize">An object to serialize.</param>
    ///// <returns>A string containing the XML representation of the object.</returns>
    //public string SerializeToXml<T>(T toSerialize, bool extraProperties = false)
    //    where T : IExtraProperties
    //{
    //    var xmlString = string.Empty;
    //    try
    //    {
    //        var entityXml = SerializeToXml(toSerialize);
    //        if (!extraProperties)
    //        {
    //            return entityXml;
    //        }

    //        xmlString = AddExtraProperties(
    //            toSerialize.GetType().Name,
    //            toSerialize.ExtraPropertyName,
    //            entityXml,
    //            toSerialize.ExtraProperties);
    //    }
    //    catch (Exception ex)
    //    {
    //        _logger.Capture(ex);
    //    }
    //    finally
    //    {
    //    }

    //    return xmlString;
    //}

    /// <summary>
    /// An extension method for serializing an arbitrary object to XML.
    /// </summary>
    /// <typeparam name="T">The type of serialized object.</typeparam>
    /// <param name="toSerialize">An object to serialize.</param>
    /// <param name="ignored">The list of property names that should be ignored.</param>
    /// <returns>A string containing the XML representation of the object.</returns>
    //public string SerializeToXml<T>(T toSerialize, string[] ignored = null)
    //{
    //    var xmlSerializer = GetSerializer(toSerialize.GetType(), ignored);

    //    using var textWriter = new StringWriter();
    //    xmlSerializer.Serialize(textWriter, toSerialize);
    //    return textWriter.ToString();
    //}

    /// <summary>
    /// An extension method for deserializing an XML string to an object.
    /// </summary>
    /// <typeparam name="T">The type of deserialized object.</typeparam>
    /// <param name="toDeserialize">A string to deserialize.</param>
    /// <returns>An object parsed from the string.</returns>
    public T DeserializeFromXml<T>(string toDeserialize, string[] ignored = null)
    {
      T result = default;
      try
      {
        var xmlSerializer = GetSerializer(typeof(T), ignored);
        using var reader = new StringReader(toDeserialize);
        result = (T)xmlSerializer.Deserialize(reader);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while deserializing object from XML");
      }

      return result;
    }

    /// <summary>
    /// An extension method for deserializing an XML string to an List of objects.
    /// </summary>
    /// <typeparam name="T">The type of deserialized object.</typeparam>
    /// <param name="toDeserialize">A list of strings to deserialize.</param>
    /// <returns>An object list parsed from the string.</returns>
    public IEnumerable<T> DeserializeManyFromXml<T>(List<string> toDeserializeList)
    {
      XmlSerializer xmlSerializer = null;
      try
      {
        xmlSerializer = GetSerializer(typeof(T), null);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error occurred while deserializing many objects from XML");
        throw;
      }

      if (xmlSerializer is null)
      {
        yield break;
      }

      foreach (var toDeserialize in toDeserializeList)
      {
        using TextReader reader = new StringReader(toDeserialize);
        T entity = default;
        try
        {
          entity = (T)xmlSerializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error occurred while deserializing many objects from XML");
        }

        yield return entity;
      }
    }

    #endregion
  }

}
