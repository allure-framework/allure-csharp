using System;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.Xunit.Internal.Functions;
using Xunit;

namespace Allure.Xunit.Internal.Registration;

class AllureXunitContext : ExecutionStateContext, ICorrelationContext
{
    public CorrelationUid CurrentCorrelationUid =>
        TestContext.Current is { TestAssembly.Traits: { } traits }
            && XunitTraits.TryGetCorrelationUid(traits, out var correlationUid)
                ? correlationUid
                : throw new InvalidOperationException(
                    "Cannot get the current correlation UID."
                );

    public override ScopeExecutionStateUid? CurrentScopeUid =>
        TestContext.Current.Test is { UniqueID: var testCaseId }
            ? new(testCaseId)
            : null;

    public override TestExecutionStateUid? CurrentTestUid =>
        TestContext.Current.Test is { UniqueID: var uid }
            ? new(uid)
            : null;

    protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => null;

    protected override StepExecutionStateUid? CurrentFrameworkStepUid => null;
}
