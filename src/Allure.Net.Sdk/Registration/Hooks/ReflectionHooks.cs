using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Registration.Hooks;

/// <summary>
/// Resolves registration hooks from configuration and environment variables.
/// </summary>
public static class ReflectionHooks
{
    /// <summary>
    /// Creates the hook type named by the runtime configuration, if configured.
    /// </summary>
    public static THook? FromConfiguration<TConfiguration, THook>(TConfiguration configuration)
        where TConfiguration : AllureConfiguration
    =>
        configuration.RuntimeRegistrationHook is { } hookTypeName
            ? Resolve<THook>(hookTypeName)
            : default;

    /// <summary>
    /// Creates the hook type named by an environment variable, if configured.
    /// </summary>
    public static THook? FromEnvironmentVariable<THook>(string variableName) =>
        Environment.GetEnvironmentVariable(variableName) is { Length: > 0 } hookTypeName
            ? Resolve<THook>(hookTypeName)
            : default;

    internal static THook Resolve<THook>(string assemblyQualifiedTypeName)
    {
        var type = Type.GetType(
            assemblyQualifiedTypeName,
            ResolveAssembly,
            typeResolver: null,
            throwOnError: true,
            ignoreCase: false
        );

        if (!type.GetInterfaces().Any(static (iFace) => iFace == typeof(THook)))
        {
            throw new InvalidOperationException(
                $"Type '{type}' must implement '{typeof(THook)}' to be an "
                    + "Allure registration hook."
            );
        }

        if (type.GetConstructor([]) is null)
        {
            throw new InvalidOperationException(
                $"Allure registration hook type '{type}' must have a public "
                    + "parameterless constructor."
            );
        }

        return (THook)Activator.CreateInstance(type);
    }

    static Assembly? ResolveAssembly(AssemblyName requestedName)
    {
        var candidates = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(assembly => AssemblyNamesMatch(assembly.GetName(), requestedName))
            .ToArray();

        return candidates.Length switch
        {
            0 => TryLoad(requestedName),

            1 => candidates[0],

            _ => throw new InvalidOperationException(
                $"Multiple loaded assemblies match '{requestedName}'. " +
                    "The registration hook assembly cannot be selected unambiguously."
            )
        };
    }

    static bool AssemblyNamesMatch(AssemblyName actual, AssemblyName requested)
    {
        if (!string.Equals(
            actual.Name,
            requested.Name,
            StringComparison.OrdinalIgnoreCase
        ))
        {
            return false;
        }

        // Honor additional identity components when the user supplies them.

        if (requested.Version is not null && actual.Version != requested.Version)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(requested.CultureName) &&
            !string.Equals(
                actual.CultureName,
                requested.CultureName,
                StringComparison.OrdinalIgnoreCase
            ))
        {
            return false;
        }

        var requestedToken = requested.GetPublicKeyToken();

        return requestedToken is not { Length: > 0 } ||
            requestedToken.SequenceEqual(actual.GetPublicKeyToken() ?? []);
    }

    static Assembly? TryLoad(AssemblyName name)
    {
        try
        {
            return Assembly.Load(name);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }
}
