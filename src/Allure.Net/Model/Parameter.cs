namespace Allure.Model;

/// <summary>
/// Describes a test, fixture, or step parameter.
/// </summary>
public sealed class Parameter
{
    /// <summary>
    /// Gets or sets the parameter name.
    /// </summary>
    required public string Name { get; set; }

    /// <summary>
    /// Gets or sets the serialized parameter value.
    /// </summary>
    required public string Value { get; set; }

    /// <summary>
    /// Gets or sets how the parameter value is displayed.
    /// </summary>
    public ParameterMode? Mode { get; set; }

    /// <summary>
    /// Gets or sets whether the parameter is excluded from history comparison.
    /// </summary>
    /// <remarks>
    /// Has no effect on fixture and step parameters.
    /// </remarks>
    public bool Excluded { get; set; }
}
