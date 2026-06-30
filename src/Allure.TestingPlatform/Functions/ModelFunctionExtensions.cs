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
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Functions;

static class ModelFunctionExtensions
{
    extension (ModelFunctions)
    {
        public static TestResult CreateTestResult(AllureConfiguration config) =>
        new()
        {
            uuid = IdFunctions.CreateUUID(),
            labels = [
                Label.Language(),
                Label.Host(),
                ..ModelFunctions.EnumerateEnvironmentLabels(),
                ..ModelFunctions.EnumerateGlobalLabels(config),
            ],
        };

        public static void ApplyTimings(TestResult testResult, TimingProperty timing)
        {
            // If present, TimingProperty is the source of truth for timing.
            testResult.start = timing.GlobalTiming.StartTime.ToUnixTimeMilliseconds();
            testResult.stop = timing.GlobalTiming.EndTime.ToUnixTimeMilliseconds();
        }

        public static void ApplyStateAsFallback(
            IReadOnlyList<string> failExceptions,
            TestResult testResult,
            TestNodeStateProperty property
        )
        {
            if (testResult.status == Status.none)
            {
                testResult.status = ToStatus(failExceptions, property);
            }

            testResult.statusDetails ??= ToStatusDetails(property);
        }

        static Status ToStatus(IReadOnlyList<string> failExceptions, TestNodeStateProperty state) =>
            state switch
            {
                FailedTestNodeStateProperty => Status.failed,
                PassedTestNodeStateProperty => Status.passed,
                SkippedTestNodeStateProperty => Status.skipped,
                TimeoutTestNodeStateProperty => Status.broken,
                ErrorTestNodeStateProperty { Exception: { } exception } =>
                    ModelFunctions.ResolveErrorStatus(failExceptions, exception),
                ErrorTestNodeStateProperty => Status.broken,
                _ => Status.none,
            };

        public static StatusDetails? ToStatusDetails(TestNodeStateProperty state) =>
            state switch
            {
                FailedTestNodeStateProperty { Exception: { } exception } =>
                    ModelFunctions.ToStatusDetails(exception),

                ErrorTestNodeStateProperty { Exception: { } exception } =>
                    ModelFunctions.ToStatusDetails(exception),

                TimeoutTestNodeStateProperty { Exception: { } exception } =>
                    ModelFunctions.ToStatusDetails(exception),

                TimeoutTestNodeStateProperty { Explanation: null } =>
                    new(){ message = "The test has timed out." },

                _ => new () { message = state.Explanation },
            };

        public static void ApplyIdentityAsFallback(
            TestResult testResult,
            TestMethodIdentifierProperty identifierProperty
        )
        {
            var sb = new StringBuilder();
            List<string> titlePath = [];
            var assembly = identifierProperty.AssemblyFullName;
            if (assembly is not null)
            {
                if (assembly.Contains(','))
                {
                    assembly = new AssemblyName(assembly).Name;
                }
                sb.Append(assembly);
                sb.Append(":");
                titlePath.Add(assembly);
            }

            var @namespace = identifierProperty.Namespace;
            if (@namespace is not null)
            {
                sb.Append(@namespace);
                sb.Append(".");
                titlePath.AddRange(@namespace.Split('.'));
            }

            var typeName = identifierProperty.TypeName;
            if (typeName is not null)
            {
                sb.Append(typeName);
                sb.Append(".");
                titlePath.Add(typeName);
            }

            var methodName = identifierProperty.MethodName;
            if (methodName is not null)
            {
                sb.Append(methodName);
            }

            var parameterTypes = string.Join(",", identifierProperty.ParameterTypeFullNames);
            sb.Append("(");
            sb.Append(parameterTypes);
            sb.Append(")");

            if (parameterTypes.Length > 0)
            {
                titlePath.Add($"{methodName}({parameterTypes})");
            }

            testResult.fullName ??= sb.ToString();

            if (testResult.titlePath.Count == 0)
            {
                testResult.titlePath = titlePath;
            }

            ModelFunctions.EnsureSuites(testResult, assembly, @namespace, typeName);
        }

        public static void AddTxtAttachment(
            IAllureResultsWriter writer,
            TestResult testResult,
            string name,
            string content
        )
        {
            var outputFileName = ModelFunctions.GetAttachmentSourceName(".txt");
            var contentBytes = Encoding.UTF8.GetBytes(content);

            testResult.attachments.Add(new()
            {
                name = name,
                type = "text/plain",
                source = outputFileName,
            });
            writer.Write(outputFileName, contentBytes);
        }

        public static void AddFileAttachment(
            IAllureResultsWriter writer,
            TestResult testResult,
            string? name,
            FileInfo file
        )
        {
            var inputPath = file.FullName;
            var outputFileName = ModelFunctions.GetAttachmentSourceName(file.Extension);

            var attachment = new Attachment
            {
                name = name ?? file.Name,
                source = outputFileName,
            };

            writer.Write(outputFileName, inputPath);
            testResult.attachments.Add(attachment);
        }

        public static void AddGlobalAttachmentFile(IAllureResultsWriter writer, string? name, FileInfo file)
        {
            var inputPath = file.FullName;
            var outputFileName = ModelFunctions.GetAttachmentSourceName(file.Extension);
            var timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            writer.Write(outputFileName, inputPath);
            writer.Write(
                new Globals
                {
                    attachments =
                    [
                        new GlobalAttachment
                        {
                            name = name ?? file.Name,
                            source = outputFileName,
                            timestamp = timestamp
                        }
                    ]
                }
            );
        }
    }
}
