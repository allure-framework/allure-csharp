using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Registration;

public record class AllureLifecycleFactoryContext(
    AllureConfiguration Config,
    IAllureResultsWriter Writer,
    Dictionary<Type, ITypeFormatter> TypeFormatters
);
