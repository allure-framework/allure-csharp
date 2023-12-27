using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.XUnit
{
    public static class AllureXunitFacade
    {
        public static IMessageSink CreateAllureXunitMessageHandler(
            IRunnerLogger logger
        )
        {
            var secondReporter = ResolveSecondReporter();
            var (startupMessage, sink) = ResolveMessageAndSink(
                new AllureMessageSink(logger),
                secondReporter,
                logger
            );
            logger.LogImportantMessage(startupMessage);
            return sink;
        }

        static IRunnerReporter? ResolveSecondReporter() =>
            AllureXunitConfiguration.CurrentConfig.XunitRunnerReporter switch
            {
                "none" => null,
                "auto" => ResolveAutoReporter(),
                string reporterName => ResolveExplicitReporter(reporterName)
            };

        static (string, IMessageSink) ResolveMessageAndSink(
            AllureMessageSink allureSink,
            IRunnerReporter? secondReporter,
            IRunnerLogger logger
        ) =>
            secondReporter is null ? (
                ALLURE_REPORTER_ON_MSG,
                allureSink
            ) : (
                $"{ALLURE_REPORTER_ON_MSG}. " +
                    $"Secondary reporter: {secondReporter.RunnerSwitch}",
                new ComposedMessageSink(
                    allureSink,
                    secondReporter.CreateMessageHandler(logger)
                )
            );

        static IRunnerReporter? ResolveAutoReporter() =>
            (
                from reporter in GetReporters()
                where reporter.IsEnvironmentallyEnabled
                select reporter
            ).FirstOrDefault();

        static IRunnerReporter ResolveExplicitReporter(string reporterName)
        {
            var reporterType = Type.GetType(reporterName);
            var resolvedReporter = TryCreateReporterByType(reporterType);
            resolvedReporter ??= TryCreateReporterByName(reporterName);
            if (resolvedReporter is null)
            {
                throw new InvalidOperationException(
                    $"Can't load the {reporterName} reporter"
                );
            }
            return resolvedReporter;
        }

        static IRunnerReporter? TryCreateReporterByType(Type? reporterType) =>
            reporterType is null
                ? null
                : (Activator.CreateInstance(reporterType) as IRunnerReporter);

        static IRunnerReporter? TryCreateReporterByName(string reporterName) =>
            (
                from reporter in GetReporters()
                where StringComparer.OrdinalIgnoreCase.Equals(
                    reporter.RunnerSwitch,
                    reporterName
                )
                select reporter
            ).FirstOrDefault();

        static IEnumerable<IRunnerReporter> GetReporters() =>
            from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where IsPotentialReporterAssembly(assembly)
            from type in assembly.GetTypes()
            where IsReporterType(type)
            select Activator.CreateInstance(type) as IRunnerReporter;

        /// <summary>
        /// Save some time skipping core assemblies. Allure.* assemblies are
        /// skipped as well, because there is only one reporter there and it
        /// has already been picked at the time this code is run.
        /// </summary>
        static bool IsPotentialReporterAssembly(Assembly assembly) =>
            assembly?.FullName is not null
                &&
            ASSEMBLY_PREFIXES_TO_SKIP.All(
                a => !assembly.FullName.StartsWith(
                    a,
                    StringComparison.OrdinalIgnoreCase
                )
            );

        static bool IsReporterType(Type type) =>
            type.GetInterfaces().Contains(typeof(IRunnerReporter))
                &&
            !type.IsAbstract
                &&
            !type.IsGenericTypeDefinition;

        static readonly string[] ASSEMBLY_PREFIXES_TO_SKIP = new[]
        {
            "System.",
            "Microsoft.",
            "Allure."
        };

        const string ALLURE_REPORTER_ON_MSG = "Allure reporter enabled";
    }
}
