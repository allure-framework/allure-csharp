using System;
using System.Collections.Generic;
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
    public void TestTitlePathByClass(Type targetClass, params string[] expectedTitlePath)
    {
        Assert.That(
            IdFunctions.CreateTitlePath(targetClass),
            Is.EqualTo(expectedTitlePath)
        );
    }

    class MyClass
    {
        internal void ParameterlessMethod() { }
        internal void MethodWithParameterOfBuiltInType(int _) { }
        internal void MethodWithParameterOfUserType(MyClass _) { }
        internal void MethodWithTwoParameters(int _, MyClass __) { }
        internal void MethodWithRefParameter(ref int _) { }
        internal void MethodWithGenericParameter<T>() { }
        internal void MethodWithArgumentOfGenericType<T>(T _) { }
        internal void MethodWithArgumentOfTypeParametrizedByGenericType<T>(Dictionary<int, T> _) { }
        internal void MethodWithArgumentOfGenericUserType<T>(MyClass<T> _) { }
    }

    class MyClass<T>
    {
        public class Nested<G> { }
        internal void GenericMethodOfGenericClass<V>(List<T> _, List<V> __) { }
    }

    class MyClass<T1, T2> { }
}