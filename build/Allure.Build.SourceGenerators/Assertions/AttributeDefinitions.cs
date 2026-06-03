namespace Allure.Build.SourceGenerators.Assertions;

public static class AttributeDefinitions
{
    public const string GenerateAllureAssertionsAttribute =
        """
        namespace Allure.Testing.Assertions
        {
            /// <summary>
            /// Marks an interface as representing a JSON property of an Allure model object that implements it.
            /// For such objects, a set of extension methods will be added to <see cref="global::Allure.Testing.AllureAssertionExtensions" />
            /// for asserting the property.
            /// </summary>
            [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
            [global::System.AttributeUsage(global::System.AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
            internal class GenerateAllureAssertionsAttribute: global::System.Attribute
            {
                /// <summary>
                /// A property name. Defines the generated extension method names, the expected JSON property key, and,
                /// for collections, the item descriptions used for error messages.
                /// </summary>
                /// <remarks>
                /// The default value is extracted from the interface name by omitting `IAllure` on the left
                /// and `Property` on the right.
                /// </remarks>
                public string PropertyName { get; init; }

                /// <summary>
                /// An expected JSON property key.
                /// </summary>
                /// <remarks>
                /// The default value is camel-cased <see cref="PropertyName"/>.
                /// </remarks>
                public string JsonName { get; init; }

                /// <summary>
                /// A name of the generated extension methods. For methods that come after
                /// `.With`, the name is used as is.
                /// For methods that extend `IAssertionSource<T>`, "Has" is prepended.
                /// </summary>
                /// <remarks>
                /// The default value is <see cref="PropertyName" />.
                /// </remarks>
                public string MethodName { get; init; }

                /// <summary>
                /// A value that defines the generated collection-specific extension method names.
                /// Not used unless the interface extends
                /// <see cref="global::Allure.Testing.Assertions.Model.Properties.IAllureArrayProperty{TElement, TFactory, TSelf}" />.
                /// </summary>
                /// <remarks>
                /// The default value is produced from <see cref="MethodName" /> by removing the final 's' or
                /// appending "Item".
                /// </remarks>
                public string ItemMethodName { get; init; }

                /// <summary>
                /// A description of a collection item to show in assertion failure messages.
                /// Not used unless the interface extends
                /// <see cref="global::Allure.Testing.Assertions.Model.Properties.IAllureArrayProperty{TElement, TFactory, TSelf}" />.
                /// </summary>
                /// <remarks>
                /// The default value is produced from <see cref="ItemMethodName" /> by converting from pascal case to white-space
                /// separated words.
                /// </remarks>
                public string ItemName { get; init; }
            }
        }
        """;
}
