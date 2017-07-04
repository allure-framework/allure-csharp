using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Allure.Commons.Writer
{
    interface IAllureResultsWriter
    {
        string Output { get; }
        void Write(TestResult testResult);
        void Write(TestResultContainer testResult);
        void Write(string source, byte[] attachment);
    }
}
