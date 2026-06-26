namespace Allure.TestingPlatform.Sdk;

public interface IAllureContextUid
{
    string Value { get; }
}

public readonly record struct CorrelationUid(string Value);

public readonly record struct TestContextUid(string Value) : IAllureContextUid;

public readonly record struct ScopeContextUid(string Value) : IAllureContextUid;

public readonly record struct FixtureContextUid(string Value) : IAllureContextUid;

public readonly record struct StepContextUid(string Value) : IAllureContextUid;
