using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Allure.Net.Commons;
using Allure.Net.Commons.Storage;
using Allure.XUnit;
using Allure.Xunit.Attributes;
using Allure.XUnit.Attributes;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.Xunit
{
    public static class AllureXunitHelper
    {
        internal static List<Type> ExceptionTypes = new List<Type> { typeof(XunitException) };
        static AllureXunitHelper()
        {
            const string allureConfigEnvVariable = "ALLURE_CONFIG";
            const string allureConfigName = "allureConfig.json";

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(allureConfigEnvVariable)))
            {
                return;
            }

            var allureConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, allureConfigName);

            Environment.SetEnvironmentVariable(allureConfigEnvVariable, allureConfigPath);
        }

        internal static void AddDistinct(this List<Label> labels, Label labelToInsert, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelToInsert.name);
            }

            labels.Add(labelToInsert);
        }

        internal static void AddDistinct(this List<Label> labels, string labelName, string[] values, bool overwrite)
        {
            if (overwrite)
            {
                labels.RemoveAll(label => label.name == labelName);
            }

            foreach (var value in values)
            {
                labels.Add(new Label {name = labelName, value = value});
            }
        }
    }
}