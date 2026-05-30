using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Assertions;

public class NarrowToJsonCollectionPropertyAssertion<TObject, TProperty, TValue, TItem>(
    AssertionContext<TObject> context
) :
    CollectionAssertionBase<TValue, TItem>(context.Map(Mapper))

    where TObject : IAllureModelObject<TObject>, TProperty
    where TProperty : IAllureProperty<TValue, TObject>
    where TValue : IReadOnlyList<TItem>
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TValue> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    protected override string GetExpectation() =>
        $"\"{TProperty.PropertyName}\"";

    public static Func<TObject?, TValue?> Mapper { get;} =
        item =>
            TProperty.GetValue(item) switch
            {
                { IsPassed: true, Value: var value } => value,

                { Message: var error } => throw new InvalidOperationException(error),
            };
}