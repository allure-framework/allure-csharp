using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.Xunit
{
    internal static class AllureXunitHelper
    {
        internal const string NS_OBSOLETE_MSG =
            "The Allure.XUnit namespace is deprecated. Please, use Allure.Xunit instead";
    }
}