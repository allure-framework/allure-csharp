using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk.Properties;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Functions;

static class ModelFunctionExtensions
{
    extension (ModelFunctions)
    {
        public static bool IsCancelled(TestResult result) =>
            result.labels.Any(IsCancellationMarker);

        public static bool IsCancellationMarker(Label label) =>
            label.name == AllureCancelProperty.CANCEL_LABEL_NAME && label.value == "true";
    }
}
