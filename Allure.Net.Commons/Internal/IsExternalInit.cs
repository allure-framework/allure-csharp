using System.ComponentModel;

namespace System.Runtime.CompilerServices;

/// <summary>
/// This class serves as an init-only setter modreq to make a library that
/// uses init only setters compile against pre-net5.0 TFMs (including .NET
/// Standard). See
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
