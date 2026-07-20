using System;
using Allure.Runtime;
using Allure.Abstractions;

namespace Allure;

public static partial class AllureApi
{
    public static void SetUp(string name, Action body) =>
        AllureFrontend.Client.Operations.Sync.SetUp(name, [], body);

    public static void SetUp(string name, Action<IAllureFixtureContext> body) =>
        AllureFrontend.Client.Operations.Sync.SetUp(name, [], body);

    public static void TearDown(string name, Action body) =>
        AllureFrontend.Client.Operations.Sync.TearDown(name, [], body);

    public static void TearDown(string name, Action<IAllureFixtureContext> body) =>
        AllureFrontend.Client.Operations.Sync.TearDown(name, [], body);
}
