using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.XUnit
{
    public class AllureRunnerReporter : IRunnerReporter
    {
        public string Description { get; } = "allure-xunit";

        public bool IsEnvironmentallyEnabled { get; } = true;

        public string RunnerSwitch { get; } = "allure";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            AllureXunitPatcher.PatchXunit(logger);
            var sink = new AllureMessageSink(logger);
            CurrentSink ??= sink;
            return TryWrapSecondReporter(logger, sink);
        }

        internal static AllureMessageSink? CurrentSink { get; private set; }

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
            AllureXunitConfiguration.CurrentConfig.XunitReporter switch
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
                : ((IRunnerReporter)Activator.CreateInstance(reporterType));

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
            select (IRunnerReporter)Activator.CreateInstance(type);

        static bool IsPotentialReporterAssembly(Assembly assembly) =>
            !assembly.FullName.StartsWith("System.")
                &&
            !assembly.FullName.StartsWith("Microsoft.");

        static bool IsReporterType(Type type) =>
            type.GetInterfaces().Contains(typeof(IRunnerReporter))
                &&
            !type.IsAbstract
                &&
            !type.IsGenericTypeDefinition
                &&
            type != typeof(AllureRunnerReporter);
    }
}