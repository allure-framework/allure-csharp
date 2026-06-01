namespace Allure.Build.SourceGenerators.Assertions;

public static class AttributeDefinitions
{
    public const string GenerateAllureAssertionsAttribute =
        """
        namespace Allure.Testing.Assertions
        {
            [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
            internal class GenerateAllureAssertionsAttribute: global::System.Attribute
            {
                public string PropertyName { get; init; }

                public string JsonName { get; init; }

                public string MethodName { get; init; }

                public string ItemMethodName { get; init; }

                public string ItemName { get; init; }
            }
        }
        """;
}
