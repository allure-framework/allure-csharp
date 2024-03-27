using System;

namespace Allure.Xunit.Attributes
{
    /// <summary>
    /// Internal AllureId of test case, used to permanent mapping result to existing automatic test case. NB: don't confuse with "test case id" aka id for external TMS system.
    /// https://docs.qameta.io/allure-testops/ecosystem/intellij-plugin/#link-tests-in-ide-to-tests-cases-in-allure-testops-via-allureid
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class AllureIdAttribute : Attribute, IAllureInfo
    {
        public AllureIdAttribute(string allureId)
        {
            AllureId = allureId;
        }

        public string AllureId { get; }
    }
}