## SpecFlow Adapter

[![Nuget](https://img.shields.io/nuget/v/Allure.SpecFlow?style=flat)](https://www.nuget.org/packages/Allure.SpecFlow)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.SpecFlow?style=flat)](https://www.nuget.org/packages/Allure.SpecFlow)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.specflow?label=downloads&style=flat)

> An Allure adapter for [SpecFlow](https://specflow.org/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

The plugin currently supports [SpecFlow](http://specflow.org/) v2.1 - 3.9.*

### Quick start

1. Install the [Allure.SpecFlow](https://www.nuget.org/packages/Allure.SpecFlow)
nuget package according to your Specflow version.
2. Add the following entry to your `specflow.json`:
    ```json
    {
      "stepAssemblies": [
        {"assembly": "Allure.SpecFlowPlugin"}
      ]
    }
    ```
3. Run the tests.

#### For users of Mac with Apple silicon

If you're developing on a Mac machine with Apple silicon, make sure you have
Rosetta installed. Follow this article for the instructions:
https://support.apple.com/en-us/HT211861

You may also install Rosetta via the CLI:

```shell
/usr/sbin/softwareupdate --install-rosetta --agree-to-license
```

### Further readings

Learn more from [the documentation for Allure SpecFlow](https://allurereport.org/docs/specflow/).

### Known issues

#### Selective run issues

Selective run doesn't work in .NET 8.0 until the support is added to
Monomod.Core.

Additionally, it might not work under other rare circumstances.
Issue [#369] contains some additional details. If you are affected by this, you
may try to switch to the `Debug` configuration as a workaround until we come up
with a solution.

[#369]: https://github.com/allure-framework/allure-csharp/issues/369
