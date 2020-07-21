using NUnit.Framework;
using System;
using System.IO;

namespace Allure.SpecFlowPlugin.Tests
{
  [SetUpFixture]
  public class TestSetup
  {
    [OneTimeSetUp]
    public void Setup()
    {
      // setup current folder for nUnit engine
      Environment.CurrentDirectory = Path.GetDirectoryName(typeof(TestSetup).Assembly.Location);
    }
  }
}