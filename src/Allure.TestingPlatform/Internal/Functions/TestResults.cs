using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Allure.Model;
using Allure.Sdk.Functions;
using Allure.TestingPlatform.Configuration;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Sdk.Properties;

namespace Allure.TestingPlatform.Internal.Functions;

static class TestResults
{
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

            SuiteLabels.Ensure(
                testResult,
                assemblyName,
                identifierProperty.Namespace,
                identifierProperty.TypeName
            );
        }

        public bool IsCancelled =>
            testResult.Labels.Any(IsCancellationMarker);

    }

    static bool IsCancellationMarker(Label label) =>
        label.Name == AllureCancelProperty.CANCEL_LABEL_NAME && label.Value == "true";
}
