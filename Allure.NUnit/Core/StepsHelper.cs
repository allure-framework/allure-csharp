using System;
using System.ComponentModel;
using Allure.Net.Commons.Steps;

namespace NUnit.Allure.Core
{
    [Obsolete("Members of this class are now a part of the end user API represented by the Allure facade. " +
        "Please, use the Allure.Net.Commons.Allure class instead.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class StepsHelper : CoreStepsHelper
    {
    }
}