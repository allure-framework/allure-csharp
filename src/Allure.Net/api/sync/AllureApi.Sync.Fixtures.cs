using System;
using Allure.Runtime;
using Allure.Abstractions;

namespace Allure;

public static partial class AllureApi
{
    public static void SetUp(string name, Action body) =>
        AllureFrontend.Client.TestApi.Sync.SetUp(name, [], body);

    public static void SetUp(string name, Action<IAllureFixtureContext> body) =>
        AllureFrontend.Client.TestApi.Sync.SetUp(name, [], body);

    public static void TearDown(string name, Action body) =>
        AllureFrontend.Client.TestApi.Sync.TearDown(name, [], body);

    public static void TearDown(string name, Action<IAllureFixtureContext> body) =>
        AllureFrontend.Client.TestApi.Sync.TearDown(name, [], body);
}
