namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// This attribute hints the compiler that the parameter it's applied to
/// has the nullability described by it type, but it may additionally be null
/// when the method returns a specified value.
/// It's not included in netstandard2.0 but can be define it the project's code.
/// </summary>
/// <param name="returnValue">
/// A return value that indicated the argument is not null
/// </param>
[AttributeUsage(AttributeTargets.Parameter)]
internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute {
    public bool ReturnValue { get; } = returnValue;
}