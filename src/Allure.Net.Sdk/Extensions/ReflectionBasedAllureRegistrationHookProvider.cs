using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Allure.Sdk.Configuration;

namespace Allure.Sdk.Extensions;

public class ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook>(
    string? assemblyQualifiedTypeName
) :
    IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>

    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    readonly Type? hookType = assemblyQualifiedTypeName is not null
        ? ResolveType(assemblyQualifiedTypeName)
        : null;

    public bool HasHook => this.hookType is not null;

    public THook GetHook()
    {
        if (this.hookType is null)
        {
            throw new InvalidOperationException("Cannot resolve the hook class.");
        }

        return (THook)Activator.CreateInstance(this.hookType);
    }

    public static ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook> FromConfiguration(
        TConfiguration configuration
    ) =>
        new(configuration.RegistrationHook);

    public static ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook> FromEnvironmentVariable(
        string environmentVariableName
    ) =>
        new(Environment.GetEnvironmentVariable(environmentVariableName));

    public static ReflectionBasedAllureRegistrationHookProvider<TConfiguration, THook> FromEnvironmentVariable() =>
        FromEnvironmentVariable("ALLURE_REGISTRATION_HOOK");

    static Type ResolveType(string assemblyQualifiedTypeName)
    {
        var type = Type.GetType(
            assemblyQualifiedTypeName,
            ResolveAssembly,
            typeResolver: null,
            throwOnError: false,
            ignoreCase: false
        );

        if (!type.GetInterfaces().Any(static (iFace) => iFace == typeof(THook)))
        {
            throw new InvalidOperationException(
                $"An Allure runtime registration hook must implement {typeof(THook)}"
            );
        }

        if (type.GetConstructor([]) is null)
        {
            throw new InvalidOperationException(
                $"An Allure runtime registration hook must have a public parameterless constructor"
            );
        }

        return type;
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
