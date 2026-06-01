namespace Allure.Build.SourceGenerators.Assertions;

public static class Methods
{
    public static string NoProperty(string methodName, PropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if the object doesn't include the \"{{property.JsonName}}\" property.
                /// </summary>
                public {{Types.NoJsonPropertyAssertion("TObject")}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    return new ("{{property.JsonName}}", ctx);
                }
        """;

    public static string ScalarPropertyExists(string methodName, PropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if the \"{{property.JsonName}}\" property exists in the object and narrows the
                /// assertion chain to its value.
                /// </summary>
                public {{Types.NarrowToJsonPropertyAssertion("TObject", property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    return new ("{{property.JsonName}}", ctx);
                }
        """;

    public static string PropertyEquals(string methodName, PropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if the value of "{{property.JsonName}}" is equal to the expected value.
                /// </summary>
                public {{Types.JsonPropertyEquatableAssertion("TObject", property)}} {{methodName}}(
                    {{Types.IEquatable(property.ValueType)}} expectedValue,
                    {{Attributes.CallerArgumentExpressionFor("expectedValue")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new ("{{property.JsonName}}", ctx, expectedValue);
                }
        """;

    public static string PropertyEqualsByComparer(string methodName, PropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if the value of "{{property.JsonName}}" is equal to the expected value.
                /// </summary>
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
                /// <summary>
                /// Checks if the value of "{{property.JsonName}}" satisfies the provided constraints.
                /// </summary>
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
                /// <summary>
                /// Checks if the "{{property.JsonName}}" property exists in the object and narrows the
                /// assertion chain to its value.
                /// </summary>
                public {{Types.NarrowToJsonCollectionPropertyAssertion("TObject", property)}} {{methodName}}()
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}()");

                    return new ("{{property.JsonName}}", ctx);
                }
        """;

    public static string OneComparableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{property.ItemType}} expected{{property.ItemMethodName}},
                    {{Types.IEqualityComparer(property.ItemType)}} comparer,
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expected{{property.ItemMethodName}}Expression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({expected{{property.ItemMethodName}}Expression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasOneComparableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            comparer,
                            "{{property.ItemName}}"));
                }
        """;

    public static string OneEquatableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.IEquatable(property.ItemType)}}? expected{{property.ItemMethodName}},
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasOneEquatableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            "{{property.ItemName}}"));
                }
        """;

    public static string OneItemByCriteria(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// matches the provided criteria.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}} criteria,
                    {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasOneItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            criteria,
                            "{{property.ItemName}}"));
                }
        """;

    public static string OneItemByName(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasOneItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name),
                            "{{property.ItemName}}"));
                }
        """;

    public static string OneItemByNameComparator(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Types.IEqualityComparer("string")}} comparer,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? nameExpression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasOneItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name, comparer),
                            "{{property.ItemName}}"));
                }
        """;

    public static string ComparableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains at least one {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{property.ItemType}} expected{{property.ItemMethodName}},
                    {{Types.IEqualityComparer(property.ItemType)}} comparer,
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expected{{property.ItemMethodName}}Expression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({expected{{property.ItemMethodName}}Expression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasComparableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            comparer,
                            "{{property.ItemName}}"));
                }
        """;

    public static string EquatableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains at least one {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.IEquatable(property.ItemType)}}? expected{{property.ItemMethodName}},
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasEquatableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            "{{property.ItemName}}"));
                }
        """;

    public static string ItemByCriteria(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains at least one {{property.ItemName}} that
                /// matches the provided criteria.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}} criteria,
                    {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            criteria,
                            "{{property.ItemName}}"));
                }
        """;

    public static string ItemByName(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name),
                            "{{property.ItemName}}"));
                }
        """;

    public static string ItemByNameComparator(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Types.IEqualityComparer("string")}} comparer,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? nameExpression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name, comparer),
                            "{{property.ItemName}}"));
                }
        """;

    public static string NoComparableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if "{{property.JsonName}}" does not contain {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{property.ItemType}} expected{{property.ItemMethodName}},
                    {{Types.IEqualityComparer(property.ItemType)}} comparer,
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expected{{property.ItemMethodName}}Expression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({expected{{property.ItemMethodName}}Expression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasNoComparableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            comparer,
                            "{{property.ItemName}}"));
                }
        """;

    public static string NoEquatableItem(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if "{{property.JsonName}}" does not contain {{property.ItemName}} that
                /// is equal to the provided one.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.IEquatable(property.ItemType)}}? expected{{property.ItemMethodName}},
                    {{Attributes.CallerArgumentExpressionFor($"expected{property.ItemMethodName}")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasNoEquatableItemAssertion(property)}}(
                            coll.Context,
                            expected{{property.ItemMethodName}},
                            "{{property.ItemName}}"));
                }
        """;

    public static string NoItemByCriteria(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if "{{property.JsonName}}" does not contain {{property.ItemName}} that
                /// matches the provided criteria.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    {{Types.Constraint(property.ItemType)}} criteria,
                    {{Attributes.CallerArgumentExpressionFor("criteria")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasNoItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            criteria,
                            "{{property.ItemName}}"));
                }
        """;

    public static string NoItemByName(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if "{{property.JsonName}}" does not contain {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? expression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append($".{nameof({{methodName}})}({expression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasNoItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name),
                            "{{property.ItemName}}"));
                }
        """;

    public static string NoItemByNameComparator(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Passes if "{{property.JsonName}}" does not contain {{property.ItemName}} that
                /// has the provided name.
                /// </summary>
                public {{Types.JsonPropertyCriteriaAssertion("TObject", property)}} {{methodName}}(
                    string name,
                    {{Types.IEqualityComparer("string")}} comparer,
                    {{Attributes.CallerArgumentExpressionFor("name")}} string? nameExpression = null,
                    {{Attributes.CallerArgumentExpressionFor("comparer")}} string? comparerExpression = null
                )
                {
                    var ctx = source.Context;
                    ctx.ExpressionBuilder.Append(
                        $".{nameof({{methodName}})}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

                    return new(
                        "{{property.JsonName}}",
                        source.Context,
                        coll => new {{Types.HasNoItemByCriteriaAssertion(property)}}(
                            coll.Context,
                            item => item.HasName(name, comparer),
                            "{{property.ItemName}}"));
                }
        """;

    public static string SingleScalar(string methodName, CollectionPropertyMetadata property) =>
        $$"""
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} and
                /// narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// matches the provided criteria and narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name and narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// has the provided name and narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains enough items and narrows the assertion
                /// chain to the {{property.ItemName}} at the specified index.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains the exact number of items and
                /// each {{property.ItemName}} satisfies the corresponding constraint.
                /// </summary>
                /// <remarks>
                /// Pass <c>null</c> or a function returning <c>null</c> for a noop constraint.
                /// </remarks>
                public {{Types.CollectionItemConstraintsAssertion(property)}} {{methodName}}(
                    {{Types.OptionalConstraint(property.ItemType)}}[] constraints,
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} and
                /// narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains exactly one {{property.ItemName}} that
                /// matches the provided criteria and narrows the assertion chain to that {{property.ItemName}}.
                /// </summary>
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
                /// <summary>
                /// Checks if "{{property.JsonName}}" contains enough items and narrows the assertion
                /// chain to the {{property.ItemName}} at the specified index.
                /// </summary>
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