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
            AllureXunitPatcher.PatchXunit(logger);
            var sink = new AllureMessageSink(logger);
            return TryWrapSecondReporter(logger, sink);
        }

        static IMessageSink TryWrapSecondReporter(
            IRunnerLogger logger,
            AllureMessageSink allureSink
        ) =>
            TryWrapResolvedReporter(
                logger,
                allureSink,
                ResolveReporter()
            );

        static IMessageSink TryWrapResolvedReporter(
            IRunnerLogger logger,
            AllureMessageSink allureSink,
            IRunnerReporter? secondReporter
        ) =>
            secondReporter is null ? allureSink : new ComposedMessageSink(
                allureSink,
                secondReporter.CreateMessageHandler(logger)
            );

        static IRunnerReporter? ResolveReporter() =>
            AllureXunitConfiguration.CurrentConfig.XunitRunnerReporter switch
            {
                "none" => null,
                "auto" => ResolveAutoReporter(),
                string reporterName => ResolveExplicitReporter(reporterName)
            };

        static IRunnerReporter? ResolveAutoReporter() =>
            (
                from reporter in GetReporters()
                where reporter.IsEnvironmentallyEnabled
                select reporter
            ).FirstOrDefault();

        static IRunnerReporter ResolveExplicitReporter(string reporterName) =>
            TryCreateReporterByType(
                Type.GetType(reporterName)
            ) ?? TryCreateReporterByName(reporterName)
                ?? throw new InvalidOperationException(
                    $"Can't load the {reporterName} reporter"
                );

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
    }
}
