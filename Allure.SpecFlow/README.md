## SpecFlow Adapter

[![Nuget release](https://img.shields.io/nuget/v/Allure.SpecFlow?style=flat)](https://www.nuget.org/packages/Allure.SpecFlow)
[![Nuget downloads](https://img.shields.io/nuget/dt/Allure.SpecFlow?label=downloads&style=flat)](https://www.nuget.org/packages/Allure.SpecFlow)

> An Allure adapter for [SpecFlow](https://specflow.org/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

The adapter works with [SpecFlow](http://specflow.org/) version 3, starting from
3.9.8.

### Quick start

1. Install the [Allure.SpecFlow](https://www.nuget.org/packages/Allure.SpecFlow)
Nuget package according to your SpecFlow version.
2. Add the following entry to your `specflow.json`:
    ```json
    {
      "stepAssemblies": [
        {"assembly": "Allure.SpecFlowPlugin"}
      ]
    }
    ```
3. Run the tests.

### Further readings

Learn more from [the documentation for Allure SpecFlow](https://allurereport.org/docs/specflow/).

### Known issues

#### Selective run issues

Selective run (test plans) might not work under rare circumstances.
Issue [#369] contains some additional details. If you are affected by this, you
may try to switch to the `Debug` configuration as a workaround until we come up
with a solution.

[#369]: https://github.com/allure-framework/allure-csharp/issues/369
