namespace Allure.Build.SourceGenerators.Assertions;

public static class Methods
{
    public static string ScalarPropertyExists(string methodName, PropertyMetadata property) =>
        $$"""
                public {{Types.NarrowToJsonPropertyAssertion("TObject", property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    return new ("{{property.JsonName}}", ctx);
                }
        """;

    public static string ScalarPropertyEquals(string methodName, PropertyMetadata property, string equatableType) =>
        $$"""
                public {{Types.JsonPropertyEquatableAssertion("TObject", property, equatableType)}} {{methodName}}(
                    {{equatableType}} expectedValue,
                    {{Attributes.CallerArgumentExpressionFor("expectedValue")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new ("{{property.JsonName}}", ctx, expectedValue);
                }
        """;

    public static string ScalarPropertyEqualsByComparer(string methodName, PropertyMetadata property) =>
        $$"""
                public {{Types.JsonPropertyComparerAssertion("TObject", property)}} {{methodName}}(
                    {{property.ValueType}} expected{{property.Name}},
                    {{Types.IEqualityComparer(property.ValueType)}} comparer,
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.Name}")}} string? expected{{property.Name}}Expression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expected{{property.Name}}Expression ?? "..."}, {comparerExpression ?? "..."})");

                    return new ("{{property.JsonName}}", ctx, expected{{property.Name}}, comparer);
                }
        """;

    public static string ScalarPropertyConstrained(string methodName, PropertyMetadata property) =>
        $$"""
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.Constraint(property.ValueType)}} constraints,
                    {{Attributes.CallerArgumentExpressionFor("constraints")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new ("{{property.JsonName}}", ctx, constraints);
                }
        """;

    public static string CollectionPropertyExists(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    return new ("{{property.JsonName}}", ctx);
                }
        """;

    public static string SingleScalar(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionAssertion(property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, "{{property.ItemName}}");
                }
        """;

    public static string OneScalarByCriteria(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}} criteria,
                    {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, criteria, "{{property.ItemName}}");
                }
        """;

    public static string OneScalarByName(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                    string name,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, e => e.HasName(name), "{{property.ItemName}}");
                }
        """;

    public static string OneScalarByNameWithComparer(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionByCriteriaAssertion(property)}} {{methodName}}(
                    string name,
                    {{Types.IEqualityComparer("string")}} comparer,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? nameExpression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, e => e.HasName(name, comparer), "{{property.ItemName}}");
                }
        """;

    public static string OneScalarByIndex(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionByIndexAssertion(property)}} {{methodName}}(
                    int index,
                    {{Attributes.CallerArgumentExpressionFor("index")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, index, "{{property.ItemName}}");
                }
        """;

    public static string ConstrainedItems(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                public {{Types.CollectionItemConstraintsAssertion(property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}}[] constraints,
                    {{Attributes.CallerArgumentExpressionFor("constraints")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, constraints, "{{property.ItemName}}");
                }
        """;

    public static string SingleCollection(string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionToCollectionAssertion(property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, "{{property.ItemName}}");
                }
        """;

    public static string OneCollectionByCriteria(string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionToCollectionByCriteriaAssertion(property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}} criteria,
                    {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, criteria, "{{property.ItemName}}");
                }
        """;

    public static string OneCollectionByIndex(string methodName, CollectionOfCollectionsPropertyMetadata property) =>
        $$"""
                public {{Types.NarrowCollectionToCollectionByIndexAssertion(property)}} {{methodName}}(
                    int index,
                    {{Attributes.CallerArgumentExpressionFor("index")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    var propertyAssertion =
                        new {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}}(
                            "{{property.JsonName}}",
                            source.Context);

                    var narrowedContext = {{Types.AssertionAccessors(property.ValueType)}}.GetContext(
                        propertyAssertion.And
                    );

                    narrowedContext.ExpressionBuilder.Length -= 4;

                    return new(narrowedContext, index, "{{property.ItemName}}");
                }
        """;
}