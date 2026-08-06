using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Allure.Sdk.Configuration;

public sealed record class TrackedConfiguration<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    public string SourceName { get; }

    public TConfiguration Configuration { get; }

    public ImmutableHashSet<string> AssignedProperties { get; }

    public TrackedConfiguration(
        string sourceName,
        TConfiguration configuration,
        params IEnumerable<string> assignedProperties
    )
    {
        ImmutableHashSet<string> propertySet = [.. assignedProperties];
        var type = typeof(TConfiguration);

        foreach (var propertyName in propertySet)
        {
            var property = type.GetProperty(
                propertyName,
                BindingFlags.Instance | BindingFlags.Public
            );

            if (property is null || !TrackedConfiguration.IsTrackedProperty(property))
            {
                throw new ArgumentException(
                    $"Configuration type {type.Name} "
                        + $"does not define a trackable property named {propertyName}.",
                    nameof(propertyName)
                );
            }
        }

        this.SourceName = sourceName;
        this.Configuration = configuration;
        this.AssignedProperties = propertySet;
    }

    public TrackedConfiguration(
        string sourceName,
        TConfiguration configuration,
        IEnumerable<Expression<Func<TConfiguration, object>>> assignedProperties
    ) :
        this(sourceName, configuration, ExtractPropertyNames(assignedProperties))
    {
    }

    public bool IsPropertySet(string name) => this.AssignedProperties.Contains(name);

    public bool IsPropertySet<TValue>(Expression<Func<TConfiguration, TValue>> expression) =>
        this.AssignedProperties.Contains(
            ExtractPropertyName(expression)
        );

    public TrackedConfiguration<TConfiguration> WithProperty<TValue>(
        Expression<Func<TConfiguration, TValue>> propertySelector,
        TValue value,
        Func<TConfiguration, TValue, TConfiguration> update
    )
    {
        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        var propertyName = ExtractPropertyName(propertySelector);

        var updatedConfiguration = update(this.Configuration, value)
            ?? throw new InvalidOperationException(
                "The configuration transformation returned null."
            );

        return new(
            this.SourceName,
            updatedConfiguration,
            this.AssignedProperties.Add(propertyName)
        );
    }

    public TrackedConfiguration<TConfiguration> WithPropertyIfUnset<TValue>(
        Expression<Func<TConfiguration, TValue>> propertySelector,
        TValue value,
        Func<TConfiguration, TValue, TConfiguration> update
    )
    {
        if (propertySelector is null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        if (update is null)
        {
            throw new ArgumentNullException(nameof(update));
        }

        var propertyName = ExtractPropertyName(propertySelector);
        if (this.IsPropertySet(propertyName))
        {
            return this;
        }

        var updatedConfiguration = update(this.Configuration, value)
            ?? throw new InvalidOperationException(
                "The configuration transformation returned null."
            );

        return new(this.SourceName, updatedConfiguration, this.AssignedProperties.Add(propertyName));
    }

    static IEnumerable<string> ExtractPropertyNames(
        IEnumerable<Expression<Func<TConfiguration, object>>> expressions
    ) =>
        expressions.Select(ExtractPropertyName);

    static string ExtractPropertyName(
        Expression<Func<TConfiguration, object>> expression
    )
    {
        var body = expression.Body;

        // Handle value type properties
        if (body is UnaryExpression
        {
            NodeType: ExpressionType.Convert,
            Operand: { Type.IsValueType: true } operand,
        })
        {
            body = operand;
        }

        return ExtractPropertyName(expression.Parameters[0], body);
    }

    static string ExtractPropertyName<TValue>(
        Expression<Func<TConfiguration, TValue>> expression
    ) =>
        ExtractPropertyName(expression.Parameters[0], expression.Body);

    static string ExtractPropertyName(Expression parameter, Expression body)
    {
        if (body is MemberExpression
        {
            Member: PropertyInfo { Name: var propertyName },
            Expression: Expression targetObjectExpression
        } && ReferenceEquals(targetObjectExpression, parameter))
        {
            return propertyName;
        }
        else
        {
            throw new ArgumentException(
                "The expression is not a valid property access expression.",
                nameof(body)
            );
        }
    }
}

public static class TrackedConfiguration
{
    public static TrackedConfiguration<TConfiguration> WithAllPropertiesSet<TConfiguration>(
        string sourceName,
        TConfiguration configuration
    )
        where TConfiguration : AllureConfiguration
    =>
        new (sourceName, configuration, GetAllProperties<TConfiguration>());

    static IEnumerable<string> GetAllProperties<TConfiguration>() =>
        typeof(TConfiguration)
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsTrackedProperty)
            .Select(static (property) => property.Name);

    internal static bool IsTrackedProperty(PropertyInfo property) =>
        property.GetMethod is not null
            && property.GetIndexParameters().Length == 0;
}
