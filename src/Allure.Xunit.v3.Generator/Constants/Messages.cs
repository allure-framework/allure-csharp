namespace Allure.Xunit.Generator.Constants;

static class Messages
{
    public const string SelfRegistrationNotFound =
        $"[Allure.Xunit.v3]: Could not find the {TypeNames.SelfRegisteredExtensions} "
            + $"class with the {MemberNames.AddSelfRegisteredExtensions} method in "
            + "the root namespace of the project. Make sure the Microsoft.Testing.Platform "
            + "extension self-registration is enabled and try again.";
}
