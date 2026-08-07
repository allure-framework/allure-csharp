using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Functions;

static class TestResults
{
    public static TestResult Create(
        string name,
        AllureTestingPlatformConfiguration config,
        IDictionary environmentVariables
    ) =>
    new()
    {
        Uuid = Ids.NewUuid(),
        Name = name,
        Labels = [
            Label.Language(),
            Label.Host(),
            ..GlobalLabels.FromEnvironmentVariables(environmentVariables),
            ..GlobalLabels.FromConfiguration(config),
        ],
    };

    extension (TestResult testResult)
    {
        public void ApplyTimings(TimingProperty timing)
        {
            // If present, TimingProperty is the source of truth for timing.
            testResult.Start = timing.GlobalTiming.StartTime.ToUnixTimeMilliseconds();
            testResult.Stop = timing.GlobalTiming.EndTime.ToUnixTimeMilliseconds();
        }

        public void ApplyStateAsFallback(
            ImmutableList<string> failExceptions,
            TestNodeStateProperty property
        )
        {
            if (testResult.Status == Status.Unknown)
            {
                testResult.Status = ToStatus(failExceptions, property);
            }

            testResult.StatusDetails ??= ToStatusDetails(property);
        }

        public void ApplyIdentityAsFallback(
            TestMethodIdentifierProperty identifierProperty
        )
        {
            var sb = new StringBuilder();
            List<string> titlePath = [];
            var assembly = identifierProperty.AssemblyFullName;
            if (assembly is not null)
            {
                if (assembly.Contains(','))
                {
                    assembly = new AssemblyName(assembly).Name;
                }
                sb.Append(assembly);
                sb.Append(":");
                titlePath.Add(assembly);
            }

            var @namespace = identifierProperty.Namespace;
            if (@namespace is not null)
            {
                sb.Append(@namespace);
                sb.Append(".");
                titlePath.AddRange(@namespace.Split('.'));
            }

            var typeName = identifierProperty.TypeName;
            if (typeName is not null)
            {
                sb.Append(typeName);
                sb.Append(".");
                titlePath.Add(typeName);
            }

            var methodName = identifierProperty.MethodName;
            if (methodName is not null)
            {
                sb.Append(methodName);
            }

            var parameterTypes = string.Join(",", identifierProperty.ParameterTypeFullNames);
            sb.Append("(");
            sb.Append(parameterTypes);
            sb.Append(")");

            if (parameterTypes.Length > 0)
            {
                titlePath.Add($"{methodName}({parameterTypes})");
            }

            testResult.FullName ??= sb.ToString();

            if (testResult.TitlePath.Count == 0)
            {
                testResult.TitlePath.AddRange(titlePath);
            }

            SuiteLabels.Ensure(testResult, assembly, @namespace, typeName);
        }
    }

    static Status ToStatus(IReadOnlyList<string> failExceptions, TestNodeStateProperty state) =>
    state switch
    {
        FailedTestNodeStateProperty => Status.Failed,
        PassedTestNodeStateProperty => Status.Passed,
        SkippedTestNodeStateProperty => Status.Skipped,
        TimeoutTestNodeStateProperty => Status.Broken,
        ErrorTestNodeStateProperty { Exception: { } exception } =>
            ErrorStatus.Resolve(failExceptions, exception),
        ErrorTestNodeStateProperty => Status.Broken,
        _ => Status.Unknown,
    };

    public static StatusDetails? ToStatusDetails(TestNodeStateProperty state) =>
    state switch
    {
        FailedTestNodeStateProperty { Exception: { } exception } =>
            StatusDetails.FromException(exception),

        ErrorTestNodeStateProperty { Exception: { } exception } =>
            StatusDetails.FromException(exception),

        TimeoutTestNodeStateProperty { Exception: { } exception } =>
            StatusDetails.FromException(exception),

        TimeoutTestNodeStateProperty { Explanation: null } =>
            new(){ Message = "The test has timed out." },

        { Explanation: { Length: > 0 } explanation } =>
            new(){ Message = explanation },

        _ => null,
    };
}