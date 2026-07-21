using System.Reflection;

namespace Allure.Net.Tests.ApiContract;

public class OverloadConsistencyTests
{
    [Test]
    public async Task EveryAsyncFacadeMethodHasCancellationTokenOverload()
    {
        var methods = PublicMethods();
        var missing = methods
            .Where(method => method.Name.EndsWith("Async", StringComparison.Ordinal))
            .Where(method => method.GetParameters().LastOrDefault()?.ParameterType != typeof(CancellationToken))
            .Where(method => !methods.Any(candidate => IsCancellationOverload(method, candidate)))
            .Select(method => method.ToString()!)
            .ToArray();

        await Assert.That(missing).IsEmpty();
    }

    [Test]
    public async Task EveryNonDelegateSyncFacadeMethodHasAsyncCounterpart()
    {
        var methods = PublicMethods();
        var missing = methods
            .Where(method => method.ReturnType == typeof(void))
            .Where(method => method.GetParameters().All(parameter => !typeof(Delegate).IsAssignableFrom(parameter.ParameterType)))
            .Where(method => !methods.Any(candidate => IsAsyncCounterpart(method, candidate)))
            .Select(method => method.ToString()!)
            .ToArray();

        await Assert.That(missing).IsEmpty();
    }

    static MethodInfo[] PublicMethods() =>
        typeof(AllureApi).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => !method.IsSpecialName)
            .ToArray();

    static bool IsCancellationOverload(MethodInfo method, MethodInfo candidate)
    {
        var parameters = method.GetParameters();
        var candidateParameters = candidate.GetParameters();
        return candidate.Name == method.Name
            && candidate.GetGenericArguments().Length == method.GetGenericArguments().Length
            && candidateParameters.Length == parameters.Length + 1
            && ParametersMatch(parameters, candidateParameters[..^1])
            && candidateParameters[^1].ParameterType == typeof(CancellationToken);
    }

    static bool IsAsyncCounterpart(MethodInfo method, MethodInfo candidate)
    {
        var parameters = method.GetParameters();
        var candidateParameters = candidate.GetParameters();
        return candidate.Name == $"{method.Name}Async"
            && candidate.GetGenericArguments().Length == method.GetGenericArguments().Length
            && (
                ParametersMatch(parameters, candidateParameters)
                || (
                    candidateParameters.Length == parameters.Length + 1
                    && ParametersMatch(parameters, candidateParameters[..^1])
                    && candidateParameters[^1].ParameterType == typeof(CancellationToken)
                )
            );
    }

    static bool ParametersMatch(ParameterInfo[] left, ParameterInfo[] right) =>
        left.Length == right.Length
        && left.Zip(right).All(pair =>
            TypesMatch(pair.First.ParameterType, pair.Second.ParameterType)
            || ParamsCollectionsMatch(pair.First, pair.Second)
        );

    static bool TypesMatch(Type left, Type right)
    {
        if (left.IsGenericParameter && right.IsGenericParameter)
        {
            return left.GenericParameterPosition == right.GenericParameterPosition;
        }
        if (left.IsArray && right.IsArray)
        {
            return TypesMatch(left.GetElementType()!, right.GetElementType()!);
        }
        if (!left.IsGenericType || !right.IsGenericType)
        {
            return left == right;
        }
        return left.GetGenericTypeDefinition() == right.GetGenericTypeDefinition()
            && left.GetGenericArguments().Zip(right.GetGenericArguments())
                .All(pair => TypesMatch(pair.First, pair.Second));
    }

    static bool ParamsCollectionsMatch(ParameterInfo left, ParameterInfo right) =>
        IsParams(left)
        && IsParams(right)
        && TypesMatch(ParamsElementType(left.ParameterType), ParamsElementType(right.ParameterType));

    static bool IsParams(ParameterInfo parameter) =>
        parameter.GetCustomAttributesData().Any(attribute =>
            attribute.AttributeType.FullName is
                "System.ParamArrayAttribute"
                or "System.Runtime.CompilerServices.ParamCollectionAttribute"
        );

    static Type ParamsElementType(Type type) =>
        type.IsArray ? type.GetElementType()! : type.GetGenericArguments().Single();
}
