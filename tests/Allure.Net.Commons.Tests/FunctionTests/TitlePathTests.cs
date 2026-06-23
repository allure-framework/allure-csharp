using System;
using System.Reflection;
using Allure.Net.Commons.Attributes;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests;

class TitlePathTests
{
    [TestCase(
        typeof(TitlePathTests),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests",
        TestName = "Outmost type with namespave"
    )]
    [TestCase(
        typeof(ClassWithoutNamespace),
        "Allure.Net.Commons.Tests",
        "ClassWithoutNamespace",
        TestName = "Outmost type no namespace"
    )]
    [TestCase(
        typeof(MyClass),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass",
        TestName = "Nested class"
    )]
    [TestCase(
        typeof(MyClass<>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass`1[T]",
        TestName = "Nested generic class definition"
    )]
    [TestCase(
        typeof(MyClass<string>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass`1[System.String]",
        TestName = "Nested constructed generic class - system type alias"
    )]
    [TestCase(
        typeof(MyClass<DateTime>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass`1[System.DateTime]",
        TestName = "Nested constructed generic class - system type"
    )]
    [TestCase(
        typeof(MyClass<ClassWithoutNamespace>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass`1[Allure.Net.Commons.Tests:ClassWithoutNamespace]",
        TestName = "Nested constructed generic class - custom type"
    )]
    [TestCase(
        typeof(MyClass<MyClass<string, int>, MyClass<MyClass>>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass`2[" +
            "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.TitlePathTests+MyClass`2[System.String,System.Int32]," +
            "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.TitlePathTests+MyClass`1[" +
                "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.TitlePathTests+MyClass]]",
        TestName = "Nested constructed generic class - complex"
    )]
    [TestCase(
        typeof(ClassWithAllureName<int, string>),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "Foo",
        TestName = "Class with [AllureName]"
    )]
    public void TestTitlePathByClass(Type targetClass, params string[] expectedTitlePath)
    {
        Assert.That(
            IdFunctions.CreateTitlePath(targetClass),
            Is.EqualTo(expectedTitlePath)
        );
    }

    [TestCase(
        typeof(MyClass),
        nameof(MyClass.ParameterlessMethod),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass",
        TestName = "Parameterless method"
    )]
    [TestCase(
        typeof(MyClass),
        nameof(MyClass.ParameterizedMethod),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass",
        "ParameterizedMethod(System.Int32)",
        TestName = "Parameterized method - int"
    )]
    [TestCase(
        typeof(MyClass),
        nameof(MyClass.ParameterizedMethodWithAllureName),
        "Allure.Net.Commons.Tests",
        "Allure",
        "Net",
        "Commons",
        "Tests",
        "FunctionTests",
        "TitlePathTests+MyClass",
        "Foo",
        TestName = "Method with [AllureName]"
    )]
    public void TestTitlePathOfParameterizedMethod(
        Type targetClass,
        string targetMethodName,
        params string[] expectedTitlePath
    )
    {
        var method = targetClass.GetMethod(
            targetMethodName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        var actualTitlePath = IdFunctions.CreateTitlePath(method);

        Assert.That(actualTitlePath, Is.EqualTo(expectedTitlePath));
    }

    class MyClass
    {
        internal void ParameterlessMethod() { }

        internal void ParameterizedMethod(int _) { }

        [AllureName("Foo")]
        internal void ParameterizedMethodWithAllureName(int _) { }
    }

    class MyClass<T> { }

    class MyClass<T1, T2> { }

    [AllureName("Foo")]
    class ClassWithAllureName<T1, T2> { }
}