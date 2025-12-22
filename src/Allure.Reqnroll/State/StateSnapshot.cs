using Allure.Net.Commons;
using Reqnroll.Bindings.Reflection;

namespace Allure.ReqnrollPlugin.State;

internal record class StateSnapshot(
    AllureContext State,
    IBindingMethod Origin
);
