using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;

public interface IAllureInProcessTestApi : IAllureTestApi<IAllureInProcessStepContext, IAllureInProcessFixtureContext>
{
    void UpdateTestResult(Action<TestResult> update);

    bool TryReadTestResult<TResult>(
        Func<TestResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );

    void UpdateFixtureResult(Action<FixtureResult> update);

    bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );

    void UpdateStepResult(Action<StepResult> update);

    bool TryReadStepResult<TResult>(
        Func<StepResult, TResult> read,
        [MaybeNullWhen(false)] out TResult result
    );
}
