using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Sdk.Properties;

namespace Allure.TestingPlatform.Internal.Functions;

static class TestResults
{
    static readonly ConditionalWeakTable<TestResult, TestResultMetadata> metadataByTestResult = new();

    public static TestResult Create(
        string name,
        AllureTestingPlatformConfiguration configuration,
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
            ..GlobalLabels.FromConfiguration(configuration),
        ],
    };

    extension (TestResult testResult)
    {
        TestResultMetadata GetMetadata() =>
            metadataByTestResult.GetValue(testResult, static (_) => new TestResultMetadata());

        public void RememberDefaultSuites(string? parentSuite, string? suite, string? subSuite)
        {
            var metadata = testResult.GetMetadata();
            metadata.DefaultSuites = (parentSuite, suite, subSuite);
        }

        public void ApplyTimings(TimingProperty timing)
        {
            // If present, TimingProperty is the source of truth for timing.
            testResult.Start = timing.GlobalTiming.StartTime.ToUnixTimeMilliseconds();
            testResult.Stop = timing.GlobalTiming.EndTime.ToUnixTimeMilliseconds();
        }

        public void ApplyStateAsFallback(
            IEnumerable<string> failExceptions,
            TestNodeStateProperty property
        )
        {
            if (testResult.Status == Status.Unknown)
            {
                testResult.Status = TestNodeStates.ToStatus(failExceptions, property);
            }

            testResult.StatusDetails ??= TestNodeStates.ToStatusDetails(property);
        }

        public void ApplyIdentityAsFallback(
            TestMethodIdentifierProperty identifierProperty
        )
        {
            var assemblyName = TestMethodIdentifiers.AssemblyName(identifierProperty);
            testResult.FullName ??= TestMethodIdentifiers.FullName(identifierProperty);

            if (testResult.TitlePath.Count == 0)
            {
                testResult.TitlePath.AddRange(
                    TestMethodIdentifiers.TitlePath(identifierProperty)
                );
            }

            var metadata = testResult.GetMetadata();
            metadata.DefaultSuites ??= (
                assemblyName,
                identifierProperty.Namespace,
                identifierProperty.TypeName
            );
        }

        public bool IsCancelled =>
            testResult.Labels.Any(IsCancellationMarker);

        public void ApplyDefaultSuites()
        {
            var metadata = testResult.GetMetadata();
            var defaults = metadata.DefaultSuites;
            if (defaults is null)
            {
                return;
            }

            SuiteLabels.Ensure(
                testResult,
                defaults.Value.ParentSuite,
                defaults.Value.Suite,
                defaults.Value.SubSuite
            );
        }
    }

    static bool IsCancellationMarker(Label label) =>
        label.Name == AllureCancelProperty.CANCEL_LABEL_NAME && label.Value == "true";

    sealed class TestResultMetadata
    {
        public (string? ParentSuite, string? Suite, string? SubSuite)? DefaultSuites { get; set; }
    }
}
