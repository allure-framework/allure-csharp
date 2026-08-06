using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Allure.Sdk.Configuration;

/// <summary>
/// Identifies the configuration properties assigned by a custom JSON converter.
/// </summary>
/// <remarks>
/// Implement this interface on a custom converter for <typeparamref name="TConfiguration"/>
/// when its JSON contract is not an object contract. The returned names must identify
/// readable, non-indexed public properties of <typeparamref name="TConfiguration"/>.
/// </remarks>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
public interface IJsonConfigurationAssignmentTracker<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets the names of the configuration properties assigned from a JSON object.
    /// </summary>
    /// <param name="configurationObject">The JSON object passed to the converter.</param>
    /// <param name="options">The serializer options used to deserialize the object.</param>
    /// <returns>The CLR property names assigned by the converter.</returns>
    IEnumerable<string> GetAssignedPropertyNames(
        JsonObject configurationObject,
        JsonSerializerOptions options
    );
}
