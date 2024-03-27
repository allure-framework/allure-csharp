using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Allure.Net.Commons;

namespace NUnit.Allure
{
    static class DeprecationWarnings
    {
        internal const string OLD_NAMESPACE =
            "The namespace NUnit.Allure is deprecated. Use Allure.NUnit instead";
        internal const string OLD_ATTR_NAMESPACE =
            "The namespace NUnit.Allure.Attributes is deprecated. Use Allure.NUnit.Attributes instead";
        internal const string OLD_ALLURE_ATTRIBUTE =
            "Use Allure.NUnit.AllureNUnitAttribute instead";
        internal const string OLD_ABSTRACT_ATTRIBUTE =
            "This attribute has no effect. Use Allure.NUnit.Attributes.AllureTestCaseAttribute instead";
    }

    namespace Attributes
    {
        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureAfterAttribute : global::Allure.NUnit.Attributes.AllureAfterAttribute
        {
            public AllureAfterAttribute(string name = null) : base(name)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureBeforeAttribute : global::Allure.NUnit.Attributes.AllureBeforeAttribute
        {
            public AllureBeforeAttribute(string name = null) : base(name)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureDescriptionAttribute : global::Allure.NUnit.Attributes.AllureDescriptionAttribute
        {
            public AllureDescriptionAttribute(string description, bool html = false)
                : base(description, html)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureDisplayIgnoredAttribute : global::Allure.NUnit.Attributes.AllureDisplayIgnoredAttribute
        {
            public AllureDisplayIgnoredAttribute() { }

            [Obsolete("Allure attributes are now supported for ignored tests. Use them instead")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public AllureDisplayIgnoredAttribute(string suiteNameForIgnoredTests = "Ignored") : base(suiteNameForIgnoredTests)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureEpicAttribute : global::Allure.NUnit.Attributes.AllureEpicAttribute
        {
            public AllureEpicAttribute(string epic) : base(epic)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureFeatureAttribute : global::Allure.NUnit.Attributes.AllureFeatureAttribute
        {
            public AllureFeatureAttribute(params string[] feature) : base(feature)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureIdAttribute : global::Allure.NUnit.Attributes.AllureIdAttribute
        {
            public AllureIdAttribute(int id) : base(id)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureIssueAttribute : global::Allure.NUnit.Attributes.AllureIssueAttribute
        {
            public AllureIssueAttribute(string name) : base(name)
            {
            }

            public AllureIssueAttribute(string name, string url) : base(name, url)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureLabelAttribute : global::Allure.NUnit.Attributes.AllureLabelAttribute
        {
            public AllureLabelAttribute(string name, string value) : base(name, value)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureLinkAttribute : global::Allure.NUnit.Attributes.AllureLinkAttribute
        {
            public AllureLinkAttribute(string url) : base(url)
            {
            }

            public AllureLinkAttribute(string name, string url) : base(name, url)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureNameAttribute : global::Allure.NUnit.Attributes.AllureNameAttribute
        {
            public AllureNameAttribute(string name) : base(name)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureOwnerAttribute : global::Allure.NUnit.Attributes.AllureOwnerAttribute
        {
            public AllureOwnerAttribute(string owner) : base(owner)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureParentSuiteAttribute : global::Allure.NUnit.Attributes.AllureParentSuiteAttribute
        {
            public AllureParentSuiteAttribute(string parentSuite) : base(parentSuite)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureSeverityAttribute : global::Allure.NUnit.Attributes.AllureSeverityAttribute
        {
            public AllureSeverityAttribute(SeverityLevel severity = SeverityLevel.normal) : base(severity)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureStepAttribute : global::Allure.NUnit.Attributes.AllureStepAttribute
        {
            public AllureStepAttribute(string name = null) : base(name)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureStoryAttribute : global::Allure.NUnit.Attributes.AllureStoryAttribute
        {
            public AllureStoryAttribute(params string[] story) : base(story)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureSubSuiteAttribute : global::Allure.NUnit.Attributes.AllureSubSuiteAttribute
        {
            public AllureSubSuiteAttribute(string subSuite) : base(subSuite)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureSuiteAttribute : global::Allure.NUnit.Attributes.AllureSuiteAttribute
        {
            public AllureSuiteAttribute(string suite) : base(suite)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureTagAttribute : global::Allure.NUnit.Attributes.AllureTagAttribute
        {
            public AllureTagAttribute(params string[] tag) : base(tag)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ABSTRACT_ATTRIBUTE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public abstract class AllureTestCaseAttribute : global::Allure.NUnit.Attributes.AllureTestCaseAttribute
        {
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureTmsAttribute : global::Allure.NUnit.Attributes.AllureTmsAttribute
        {
            public AllureTmsAttribute(string name) : base(name)
            {
            }

            public AllureTmsAttribute(string name, string url) : base(name, url)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class NameAttribute : global::Allure.NUnit.Attributes.NameAttribute
        {
            public NameAttribute(string name) : base(name)
            {
            }
        }

        [Obsolete(DeprecationWarnings.OLD_ATTR_NAMESPACE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class SkipAttribute : global::Allure.NUnit.Attributes.SkipAttribute
        {
        }
    }

    namespace Core
    {
        public static class AllureExtensions
        {
            [Obsolete("This method does nothing and can be safely replaced with the direct call of the delegate")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static void WrapSetUpTearDownParams(
                Action action,
                string customName = "",
                [CallerMemberName] string callerName = ""
            )
            {
            }


            [Obsolete("Use [AllureStep] or AllureApi.Step instead")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static void WrapInStep(
                this AllureLifecycle lifecycle,
                Action action,
                string stepName = "",
                [CallerMemberName] string callerName = ""
            ) =>
                AllureApi.Step(
                    string.IsNullOrEmpty(stepName) ? callerName : stepName,
                    action
                );

            [Obsolete("Use [AllureStep] or AllureApi.Step instead")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static T WrapInStep<T>(
                this AllureLifecycle lifecycle,
                Func<T> func,
                string stepName = "",
                [CallerMemberName] string callerName = ""
            ) =>
                AllureApi.Step(
                    string.IsNullOrEmpty(stepName) ? callerName : stepName,
                    func
                );

            [Obsolete("Use [AllureStep] or AllureApi.Step instead")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static async Task WrapInStepAsync(
                this AllureLifecycle lifecycle,
                Func<Task> action,
                string stepName = "",
                [CallerMemberName] string callerName = ""
            ) =>
                await AllureApi.Step(
                    string.IsNullOrEmpty(stepName) ? callerName : stepName,
                    action
                );

            [Obsolete("Use [AllureStep] or AllureApi.Step instead")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            public static async Task<T> WrapInStepAsync<T>(
                this AllureLifecycle lifecycle,
                Func<Task<T>> func,
                string stepName = "",
                [CallerMemberName] string callerName = ""
            ) =>
                await AllureApi.Step(
                    string.IsNullOrEmpty(stepName) ? callerName : stepName,
                    func
                );
        }

        [Obsolete(DeprecationWarnings.OLD_ALLURE_ATTRIBUTE)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public class AllureNUnitAttribute : global::Allure.NUnit.AllureNUnitAttribute
        {
            public AllureNUnitAttribute()
            {
            }
        }
    }

    [Obsolete(DeprecationWarnings.OLD_NAMESPACE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class Attachments
    {
        public static void Text(string name, string content) =>
            global::Allure.NUnit.Attachments.Text(name, content);
        public static void Bytes(string name, byte[] content, string extension = "") =>
            global::Allure.NUnit.Attachments.Bytes(name, content, extension);
        public static void File(string name, string path) =>
            global::Allure.NUnit.Attachments.File(name, path);
        public static void File(string fileName) =>
            global::Allure.NUnit.Attachments.File(fileName);
    }
}