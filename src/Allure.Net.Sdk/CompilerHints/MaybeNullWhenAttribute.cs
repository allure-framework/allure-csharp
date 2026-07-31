namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates that the parameter may be <see langword="null"/> when the method
/// returns the specified value.
/// This attribute is not included in .NET Standard 2.0, so it is defined locally.
/// </summary>
/// <param name="returnValue">
/// The return value for which the parameter may be <see langword="null"/>.
/// </param>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute {
    public bool ReturnValue { get; } = returnValue;
}
