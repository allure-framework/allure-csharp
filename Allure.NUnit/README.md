# Allure NUnit adapter
NUnit adapter for Allure Framework 

[![build](https://github.com/unickq/allure-nunit/actions/workflows/dotnet.yml/badge.svg?branch=refactoring)](https://github.com/unickq/allure-nunit/actions/workflows/dotnet.yml)

[![Nuget](https://img.shields.io/nuget/v/nunit.allure?label=nuget&style=flat)](https://www.nuget.org/packages/NUnit.Allure)
[![Nuget pre](https://img.shields.io/nuget/vpre/nunit.allure?label=nuget%20pre&style=flat)](https://www.nuget.org/packages/NUnit.Allure)

![Nuget downloads](https://img.shields.io/nuget/dt/nunit.allure?label=downloads&style=flat)



![Allure report](https://raw.githubusercontent.com/unickq/allure-nunit/master/AllureScreen.png)


### [Code examples](https://github.com/unickq/allure-nunit/tree/master/src/allure-nunit-tests):

```cs
[TestFixture(Author = "unickq", Description = "Examples")]
[AllureNUnit]
[AllureLink("https://github.com/unickq/allure-nunit")]
public class Tests
{
    [OneTimeSetUp]
    public void ClearResultsDir()
    {
        AllureLifecycle.Instance.CleanupResultDirectory();
    }

    //Allure.Steps required
    [AllureStep("This method is just saying hello")]
    private void SayHello()
    {
        Console.WriteLine("Hello!");
    }

    [Test]
    [AllureTag("NUnit", "Debug")]
    [AllureIssue("GitHub#1", "https://github.com/unickq/allure-nunit")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureFeature("Core")]
    public void EvenTest([Range(0, 5)] int value)
    {
        SayHello();
            
        //Wrapping Step
        AllureLifecycle.Instance.WrapInStep(
            () => { Assert.IsTrue(value % 2 == 0, $"Oh no :( {value} % 2 = {value % 2}"); },
            "Validate calculations");
    }
}
```

### Installation and Usage
- Download from Nuget with all dependencies
- Configure allureConfig.json
- Set `[AllureNUnit]` attribute under test fixture
- Use other [attributes](https://github.com/unickq/allure-nunit/wiki/Attributes) if needed