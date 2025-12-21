namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// This attribute hints the compiler that the parameter it's applied to
/// is not null if the method returns a specified value.
/// It's not included in netstandard2.0 but can be define it the project's code.
/// </summary>
/// <param name="returnValue">
/// A return value that indicated the argument is not null
/// </param>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute {
    public bool ReturnValue { get; } = returnValue;
}