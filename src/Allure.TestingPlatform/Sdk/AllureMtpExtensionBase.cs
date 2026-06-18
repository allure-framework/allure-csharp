using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Testing.Platform.Extensions;

namespace Allure.TestingPlatform.Sdk;

public abstract class AllureMtpExtensionBase(
    string uid,
    string displayName,
    string description
) : IExtension
{
    public string Uid => uid;

    public string Version => GetCurrentPackageVersion();

    public string DisplayName => displayName;

    public string Description => description;

    public virtual Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public static string? CurrentPackageVersion { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion
            ?.Split('+')
            .First();

    public static string GetCurrentPackageVersion(string fallback) =>
        CurrentPackageVersion ?? fallback;

    public static string GetCurrentPackageVersion() =>
        CurrentPackageVersion
            ?? throw new InvalidOperationException("Couldn't read the package version");
}