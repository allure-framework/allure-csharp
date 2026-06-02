using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Assertions;

public class NarrowToJsonCollectionPropertyAssertion<TObject, TProperty, TValue, TItem>(
    string propertyName,
    AssertionContext<TObject> context
) :
    CollectionAssertionBase<TValue, TItem>(context.Map(CreateMapper(propertyName)))

    where TObject : IAllureModelObject<TObject>, TProperty
    where TProperty : IAllureProperty<TValue, TObject>
    where TValue : IEnumerable<TItem>
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TValue> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    protected override string GetExpectation() =>
        $"\"{propertyName}\"";

    public static Func<TObject?, TValue?> CreateMapper(string propertyName) =>
        item =>
            TProperty.GetValue(item, propertyName) switch
            {
                { IsPassed: true, Value: var value } => value,

                { Message: var error } => throw new InvalidOperationException(error),
            };
}