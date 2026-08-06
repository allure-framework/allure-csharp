using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Allure.Sdk.Configuration;

/// <summary>
/// Associates a configuration with its source and the properties explicitly assigned
/// by that source.
/// </summary>
/// <typeparam name="TConfiguration">The configuration type.</typeparam>
public sealed record class TrackedConfiguration<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets the human-readable name of the configuration source.
    /// </summary>
    public string SourceName { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public TConfiguration Configuration { get; }

    /// <summary>
    /// Gets the CLR names of the properties explicitly assigned by the source or by a
    /// subsequent transformation.
    /// </summary>
    public ImmutableHashSet<string> AssignedProperties { get; }

    /// <summary>
    /// Creates tracked configuration from a sequence of CLR property names.
    /// </summary>
    /// <param name="sourceName">The human-readable name of the configuration source.</param>
    /// <param name="configuration">The configuration to track.</param>
    /// <param name="assignedProperties">The CLR names of the assigned properties.</param>
    /// <exception cref="ArgumentException">
    /// An assigned property name does not identify a readable, non-indexed public property
    /// of <typeparamref name="TConfiguration"/>.
    /// </exception>
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

    /// <summary>
    /// Creates tracked configuration from a sequence of property selectors.
    /// </summary>
    /// <param name="sourceName">The human-readable name of the configuration source.</param>
    /// <param name="configuration">The configuration to track.</param>
    /// <param name="assignedProperties">Expressions that select the assigned properties.</param>
    /// <exception cref="ArgumentException">
    /// An expression does not directly select a property of the configuration.
    /// </exception>
    public TrackedConfiguration(
        string sourceName,
        TConfiguration configuration,
        IEnumerable<Expression<Func<TConfiguration, object>>> assignedProperties
    ) :
        this(sourceName, configuration, ExtractPropertyNames(assignedProperties))
    {
    }

    /// <summary>
    /// Determines whether the property with the specified CLR name has been assigned.
    /// </summary>
    /// <param name="name">The CLR property name.</param>
    /// <returns><see langword="true"/> if the property has been assigned; otherwise, <see langword="false"/>.</returns>
    public bool IsPropertySet(string name) => this.AssignedProperties.Contains(name);

    /// <summary>
    /// Determines whether the selected property has been assigned.
    /// </summary>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="expression">An expression that selects a property.</param>
    /// <returns><see langword="true"/> if the property has been assigned; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="expression"/> does not directly select a property of the configuration.
    /// </exception>
    public bool IsPropertySet<TValue>(Expression<Func<TConfiguration, TValue>> expression) =>
        this.AssignedProperties.Contains(
            ExtractPropertyName(expression)
        );

    /// <summary>
    /// Returns tracked configuration with the selected property updated and marked as assigned.
    /// </summary>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="propertySelector">An expression that selects the property to update.</param>
    /// <param name="value">The value passed to <paramref name="update"/>.</param>
    /// <param name="update">A function that returns a configuration containing the new value.</param>
    /// <returns>A new tracked configuration containing the update.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertySelector"/> or <paramref name="update"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="propertySelector"/> does not directly select a property of the configuration.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="update"/> returns <see langword="null"/>.
    /// </exception>
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

    /// <summary>
    /// Returns tracked configuration with the selected property updated only if it has not
    /// already been assigned.
    /// </summary>
    /// <typeparam name="TValue">The property value type.</typeparam>
    /// <param name="propertySelector">An expression that selects the property to update.</param>
    /// <param name="value">The value passed to <paramref name="update"/>.</param>
    /// <param name="update">A function that returns a configuration containing the new value.</param>
    /// <returns>
    /// This instance if the property is already assigned; otherwise, a new tracked
    /// configuration containing the update.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="propertySelector"/> or <paramref name="update"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="propertySelector"/> does not directly select a property of the configuration.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// <paramref name="update"/> returns <see langword="null"/>.
    /// </exception>
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

/// <summary>
/// Creates tracked configuration values.
/// </summary>
public static class TrackedConfiguration
{
    /// <summary>
    /// Creates tracked configuration in which every readable, non-indexed public property
    /// is marked as assigned.
    /// </summary>
    /// <typeparam name="TConfiguration">The configuration type.</typeparam>
    /// <param name="sourceName">The human-readable name of the configuration source.</param>
    /// <param name="configuration">The configuration to track.</param>
    /// <returns>The tracked configuration.</returns>
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
