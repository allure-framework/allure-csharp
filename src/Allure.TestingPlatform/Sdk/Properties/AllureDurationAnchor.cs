namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Defines which timestamp remains fixed when applying
/// <see cref="AllureDurationProperty{TModel}"/>.
/// </summary>
public enum AllureDurationAnchor
{
    /// <summary>
    /// Keeps the start time fixed and updates the stop time.
    /// </summary>
    Start,

    /// <summary>
    /// Keeps the stop time fixed and updates the start time.
    /// </summary>
    Stop,
}
