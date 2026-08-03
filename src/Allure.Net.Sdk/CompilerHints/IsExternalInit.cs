using System.ComponentModel;

#pragma warning disable IDE0130

namespace System.Runtime.CompilerServices;

/// <summary>
/// Provides the compiler-required modifier type for init-only setters when
/// targeting frameworks earlier than .NET 5, including .NET Standard. See
/// <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/init#metadata-encoding">
///   this article
/// </see>
/// and
/// <see href="https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809#TPIN-N1249582">
///   this answer
/// </see>
/// for more details.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
internal static class IsExternalInit { }
