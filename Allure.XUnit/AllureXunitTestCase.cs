using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.XUnit
{
    internal class AllureXunitTestCase : XunitTestCase, ITestResultAccessor
    {
        public TestResultContainer TestResultContainer { get; set; }

        public TestResult TestResult { get; set; }

#pragma warning disable CS0618
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AllureXunitTestCase()
#pragma warning restore
        {
        }

        public AllureXunitTestCase(IMessageSink diagnosticMessageSink, TestMethodDisplay testMethodDisplay,
            TestMethodDisplayOptions defaultMethodDisplayOptions,
            ITestMethod testMethod, object[] testMethodArguments = null)
            : base(diagnosticMessageSink, testMethodDisplay, defaultMethodDisplayOptions, testMethod,
                testMethodArguments)
        {
        }

        public override async Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink,
            IMessageBus messageBus,
            object[] constructorArguments,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            Steps.TestResultAccessor = this;
            messageBus = new AllureMessageBus(messageBus);
            var summary = await base.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator,
                cancellationTokenSource);
            return summary;
        }

        protected override string GetUniqueID()
        {
            //GetUniqueID may throw an exception if TestMethodArguments cannot be serialized
            try
            {
                return base.GetUniqueID();
            }
            catch (ArgumentException)
            {
                //almost all code from base.GetUniqueID()
                using var stream = new MemoryStream();
                var assemblyName = TestMethod.TestClass.TestCollection.TestAssembly.Assembly.Name;
                if (TestMethod.TestClass.TestCollection.TestAssembly.Assembly is IReflectionAssemblyInfo assembly)
                {
                    assemblyName = assembly.Assembly.GetName().Name;
                }

                Write(stream, assemblyName);
                Write(stream, TestMethod.TestClass.Class.Name);
                Write(stream, TestMethod.Method.Name);

                //only this part is different
                if (TestMethodArguments != null)
                {
                    for (var i = 0; i < TestMethodArguments.Length; i++)
                    {
                        if (TestMethodArguments[i] != null)
                        {
                            Write(stream, $"{i}{TestMethodArguments[i].GetHashCode().ToString()}");
                        }
                    }
                }

                var genericTypes = MethodGenericTypes;
                if (genericTypes != null)
                {
                    foreach (var genericType in genericTypes)
                    {
                        Write(stream, TypeUtility.ConvertToSimpleTypeName(genericType));
                    }
                }

                stream.Position = 0;

                var hash = new byte[20];
                var data = stream.ToArray();

                var hasher = new Sha1Digest();
                hasher.BlockUpdate(data, 0, data.Length);
                hasher.DoFinal(hash, 0);
                return BytesToHexString(hash);
            }
        }

        static string BytesToHexString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length * 2];
            int i = 0;
            foreach (byte b in bytes)
            {
                chars[i++] = NibbleToHexChar(b >> 4);
                chars[i++] = NibbleToHexChar(b & 0xF);
            }

            return new string(chars);
        }

        static char NibbleToHexChar(int b)
        {
            return (char) (b < 10 ? b + '0' : (b - 10) + 'a');
        }

        static void Write(Stream stream, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            stream.Write(bytes, 0, bytes.Length);
            stream.WriteByte(0);
        }
    }
}