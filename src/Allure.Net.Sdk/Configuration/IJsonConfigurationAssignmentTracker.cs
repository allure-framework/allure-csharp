using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Allure.Sdk.Configuration;

public interface IJsonConfigurationAssignmentTracker<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    IEnumerable<string> GetAssignedPropertyNames(
        JsonObject configurationObject,
        JsonSerializerOptions options
    );
}
