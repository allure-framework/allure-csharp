using System;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Defines which test result fields are updated from a reflected test method.
/// </summary>
[Flags]
public enum AllureTestMethodUpdateTargets
{
    /// <summary>
    /// Updates the test result full name.
    /// </summary>
    FullName = 0x01 << 0,

    /// <summary>
    /// Updates the test result title path.
    /// </summary>
    TitlePath = 0x01 << 1,

    /// <summary>
    /// Adds class, method, and package labels.
    /// </summary>
    Labels = 0x01 << 2,

    /// <summary>
    /// Adds parameters from method arguments.
    /// </summary>
    Parameters = 0x01 << 3,

    /// <summary>
    /// Applies Allure attributes from the test class and method.
    /// </summary>
    ApiAttributes = 0x01 << 4,

    /// <summary>
    /// Updates all supported fields.
    /// </summary>
    All = FullName | TitlePath | Labels | Parameters | ApiAttributes,
}
