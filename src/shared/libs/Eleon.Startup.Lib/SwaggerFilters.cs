using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eleon.Startup.Lib;
public class EnumSchemaFilter : ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (context.Type.IsEnum)
    {
      var array = new OpenApiArray();
      array.AddRange(System.Enum.GetNames(context.Type).Select(n => new OpenApiString(n)));
      // NSwag
      schema.Extensions.Add("x-enumNames", array);
      // Openapi-generator
      schema.Extensions.Add("x-enum-varnames", array);
    }
  }
}

public class IncludeClassesForSdkSwaggerFilter : IDocumentFilter
{
  public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
  {
    var allTypes = new List<System.Type>()
    {

    };

    // allTypes.AddRange(Messaging.Module.MessagingConsts.SystemEventTypes);

    RegisterTypes(context, allTypes);
  }

  private void RegisterTypes(DocumentFilterContext context, IEnumerable<System.Type> types)
  {
    foreach (var type in types)
    {
      var schemaId = type.Name;

      if (!context.SchemaRepository.Schemas.ContainsKey(schemaId))
      {
        context.SchemaGenerator.GenerateSchema(type, context.SchemaRepository);
      }
    }
  }
}

public class PropertySchemaFilter : ISchemaFilter
{
  public void Apply(OpenApiSchema schema, SchemaFilterContext context)
  {
    if (schema.Properties == null || !schema.Properties.Any())
      return;

    // Iterate through all properties in the schema
    foreach (var property in schema.Properties)
    {
      var propertyKey = property.Key;
      var propertySchema = property.Value;

      // Check if the property was originally marked as required
      bool isRequired = schema.Required.Contains(propertyKey);

      // Remove from the required list to mark it as not required
      schema.Required.Remove(propertyKey);

      // Add a custom property to indicate its original required state
      propertySchema.Extensions.Add("EleoncoreRequired", new OpenApiBoolean(isRequired));
    }
  }
}
