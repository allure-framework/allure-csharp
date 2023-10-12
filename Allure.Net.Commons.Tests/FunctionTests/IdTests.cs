﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons.Functions;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.FunctionTests;

class IdTests
{
    [Test]
    public void TestUUIDGeneration()
    {
        string generatedUuid = IdFunctions.CreateUUID();

        Assert.That(
            () => Guid.ParseExact(generatedUuid, "d"),
            Throws.Nothing
        );
    }

    [TestCase(typeof(IdTests), "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests")]
    [TestCase(typeof(ClassWithoutNamespace), "Allure.Net.Commons.Tests:ClassWithoutNamespace")]
    [TestCase(typeof(MyClass), "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass")]
    [TestCase(typeof(MyClass<>), "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass`1[T]")]
    public void TestFullNameFromClass(Type targetClass, string expectedFullName)
    {
        Assert.That(
            IdFunctions.CreateFullName(targetClass),
            Is.EqualTo(expectedFullName)
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
        internal void GenericMethodOfGenericClass<V>(List<T> _, List<V> __) { }
    }

    [TestCase(
        nameof(MyClass.ParameterlessMethod),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass.ParameterlessMethod()",
        TestName = "ParameterlessMethod"
    )]
    [TestCase(
        nameof(MyClass.MethodWithParameterOfBuiltInType),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithParameterOfBuiltInType(System.Int32)",
        TestName = "MethodWithParameterOfBuiltInType"
    )]
    [TestCase(
        nameof(MyClass.MethodWithParameterOfUserType),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithParameterOfUserType(" +
                "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ")",
        TestName = "MethodWithParameterOfUserType"
    )]
    [TestCase(
        nameof(MyClass.MethodWithTwoParameters),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithTwoParameters(" +
                "System.Int32," +
                "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ")",
        TestName = "MethodWithTwoParameters"
    )]
    [TestCase(
        nameof(MyClass.MethodWithRefParameter),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithRefParameter(System.Int32&)",
        TestName = "MethodWithRefParameter"
    )]
    [TestCase(
        nameof(MyClass.MethodWithGenericParameter),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithGenericParameter[T]()",
        TestName = "MethodWithGenericParameter"
    )]
    [TestCase(
        nameof(MyClass.MethodWithArgumentOfGenericType),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithArgumentOfGenericType[T](T)",
        TestName = "MethodWithArgumentOfGenericType"
    )]
    [TestCase(
        nameof(MyClass.MethodWithArgumentOfTypeParametrizedByGenericType),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithArgumentOfTypeParametrizedByGenericType[T](" +
                "System.Collections.Generic.Dictionary`2[System.Int32,T]" +
            ")",
        TestName = "MethodWithArgumentOfTypeParametrizedByGenericType"
    )]
    [TestCase(
        nameof(MyClass.MethodWithArgumentOfGenericUserType),
        "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass" +
            ".MethodWithArgumentOfGenericUserType[T](" +
                "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass`1[T]" +
            ")",
        TestName = "MethodWithArgumentOfGenericUserType"
    )]
    public void FullNameFromMethod(string methodName, string expectedFullName)
    {
        var method = typeof(MyClass).GetMethod(
            methodName,
            BindingFlags.Instance | BindingFlags.NonPublic
        );
        
        var actualFullName = IdFunctions.CreateFullName(method);

        Assert.That(actualFullName, Is.EqualTo(expectedFullName));
    }

    [Test]
    public void TestFullNameFromMethodOfNestedGenericClass()
    {
        var method = typeof(MyClass<>).GetMethod(
            nameof(MyClass<int>.GenericMethodOfGenericClass),
            BindingFlags.Instance | BindingFlags.NonPublic
        );

        var actualFullName = IdFunctions.CreateFullName(method);

        Assert.That(actualFullName, Is.EqualTo(
            "Allure.Net.Commons.Tests:Allure.Net.Commons.Tests.FunctionTests.IdTests+MyClass`1[T]" +
                ".GenericMethodOfGenericClass[V](" +
                    "System.Collections.Generic.List`1[T]," +
                    "System.Collections.Generic.List`1[V]" +
                ")"
        ));
    }

    [Test]
    public void TestCaseIdHasFixedLength()
    {
        Assert.That(
            IdFunctions.CreateTestCaseId("full-name"),
            Has.Length.EqualTo(
                IdFunctions.CreateTestCaseId("other full-name").Length
            )
        );
    }

    [Test]
    public void TestCasesFromSameFullNamesAreSame()
    {
        // I.e., no randomness involved

        Assert.That(
            IdFunctions.CreateTestCaseId("full-name"),
            Is.EqualTo(
                IdFunctions.CreateTestCaseId("full-name")
            )
        );
    }

    [Test]
    public void TestCasesFromDifferentFullNamesDiffer()
    {
        // Note: a hash collision is still possible though

        Assert.That(
            IdFunctions.CreateTestCaseId("full-name"),
            Is.Not.EqualTo(
                IdFunctions.CreateTestCaseId("other full-name")
            )
        );
    }

    [Test]
    public void HistoryIdDiffersForDifferentFullNames()
    {
        // Note: a hash collision is still possible though

        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name-1",
                Enumerable.Empty<Parameter>()
            ),
            Is.Not.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name-2",
                    Enumerable.Empty<Parameter>()
                )
            )
        );
    }

    [Test]
    public void HistoryIdHasFixedLength()
    {
        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                Enumerable.Empty<Parameter>()
            ),
            Has.Length.EqualTo(
                IdFunctions.CreateHistoryId(
                    "other full-name",
                    Enumerable.Empty<Parameter>()
                ).Length
            )
        );
    }

    [Test]
    public void HistoryIdDiffersIfParametersCountsDiffer()
    {
        // Note: a hash collision is still possible though

        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                Enumerable.Empty<Parameter>()
            ),
            Is.Not.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(("p", "v"))
                )
            )
        );

        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                CreateParameters(("p1", "v1"))
            ),
            Is.Not.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(
                        ("p1", "v1"),
                        ("p2", "v2")
                    )
                )
            )
        );
    }

    [Test]
    public void HistoryIdDiffersIfParameterValuesAreDifferent()
    {
        // Note: a hash collision is still possible though

        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                CreateParameters(("p1", "v1"))
            ),
            Is.Not.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(
                        ("p1", "v2")
                    )
                )
            )
        );

        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                CreateParameters(
                    ("p1", "v1"),
                    ("p2", "v2")
                )
            ),
            Is.Not.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(
                        ("p1", "v1"),
                        ("p1", "v3")
                    )
                )
            )
        );
    }

    [Test]
    public void HistoryIdNotDependOnParameterOrder()
    {
        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                CreateParameters(
                    ("p1", "v1"),
                    ("p2", "v2")
                )
            ),
            Is.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(
                        ("p2", "v2"),
                        ("p1", "v1")
                    )
                )
            )
        );
    }

    [Test]
    public void HistoryIdNotDependOnParameterNames()
    {
        Assert.That(
            IdFunctions.CreateHistoryId(
                "full-name",
                CreateParameters(
                    ("a", "v1"),
                    ("c", "v2")
                )
            ),
            Is.EqualTo(
                IdFunctions.CreateHistoryId(
                    "full-name",
                    CreateParameters(
                        ("b", "v1"),
                        ("d", "v2")
                    )
                )
            )
        );
    }

    static IEnumerable<Parameter> CreateParameters(
        params (string, string)[] values
    ) =>
        values.Select(v => new Parameter{ name = v.Item1, value = v.Item2 });
}
