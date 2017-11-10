using Allure.Commons;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Allure.SpecFlowPlugin
{
    public class PluginConfiguration
    {
        public bool ConvertToParameters { get; }
        public Regex ParamNameRegex { get; } = new Regex(".*");
        public Regex ParamValueRegex { get; } = new Regex(".*");

        public PluginConfiguration(IConfiguration configuration)
        {
            try
            {
                bool.TryParse(configuration["specflow:stepArguments:convertToParameters"], out bool convertToParameters);
                ConvertToParameters = convertToParameters;

                ParamNameRegex = new Regex(
                    Regex.Unescape(configuration["specflow:stepArguments:paramNameRegex"]),
                    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
                    );
                ParamValueRegex = new Regex(
                    Regex.Unescape(configuration["specflow:stepArguments:paramValueRegex"]),
                    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase
                    );
            }
            catch
            {
            }

        }
    }
}
