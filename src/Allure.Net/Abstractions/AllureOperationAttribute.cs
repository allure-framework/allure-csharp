using System;

namespace Allure.Abstractions;

/// <summary>
/// Provides a base class for attributes that wrap methods with Allure operations
/// like steps or fixtures.
/// </summary>
/// <param name="name">A name of the operation. Supports parameter interpolation</param>
public abstract class AllureOperationAttribute(string? name) :
    Attribute,
    IAllureNameSource
{
    /// <summary>
    /// Gets the operation name format, or <see langword="null"/> if no format was provided.
    /// </summary>
    public string? Name => name;
}
