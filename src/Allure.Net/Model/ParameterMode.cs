namespace Allure.Model;

/// <summary>
/// Specifies how a parameter value is displayed in Allure Report.
/// </summary>
public enum ParameterMode
{
    /// <summary>
    /// Displays the parameter value normally.
    /// </summary>
    Default,

    /// <summary>
    /// Replaces the parameter value with a mask.
    /// </summary>
    Masked,

    /// <summary>
    /// Hides the parameter value from the report.
    /// </summary>
    /// <remarks>
    /// The parameter will still affect the history ID of the test
    /// unless <see cref="Parameter.Excluded"/> is set to <see langword="true"/>.
    /// </remarks>
    Hidden,
}
