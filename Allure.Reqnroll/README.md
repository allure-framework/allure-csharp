## Reqnroll Adapter

[![Nuget](https://img.shields.io/nuget/v/Allure.Reqnroll?style=flat)](https://www.nuget.org/packages/Allure.Reqnroll)
[![Nuget pre](https://img.shields.io/nuget/vpre/Allure.Reqnroll?style=flat)](https://www.nuget.org/packages/Allure.Reqnroll)

![Nuget downloads](https://img.shields.io/nuget/dt/allure.reqnroll?label=downloads&style=flat)

> An Allure adapter for [Reqnroll](https://reqnroll.net/).

[<img src="https://allurereport.org/public/img/allure-report.svg" height="85px" alt="Allure Report logo" align="right" />](https://allurereport.org "Allure Report")

- Learn more about Allure Report at [https://allurereport.org](https://allurereport.org)
- 📚 [Documentation](https://allurereport.org/docs/) – discover official documentation for Allure Report
- ❓ [Questions and Support](https://github.com/orgs/allure-framework/discussions/categories/questions-support) – get help from the team and community
- 📢 [Official announcements](https://github.com/orgs/allure-framework/discussions/categories/announcements) –  stay updated with our latest news and updates
- 💬 [General Discussion](https://github.com/orgs/allure-framework/discussions/categories/general-discussion) – engage in casual conversations, share insights and ideas with the community
- 🖥️ [Live Demo](https://demo.allurereport.org/) — explore a live example of Allure Report in action

---

### Quick start

1. Install the [Allure.Reqnroll](https://www.nuget.org/packages/Allure.Reqnroll) to the project that contains your Reqnroll tests.
2. Add the following entry to your `reqnroll.json`:
    ```json
    {
      "bindingAssemblies": [
        {"assembly": "Allure.ReqnrollPlugin"}
      ]
    }
    ```
3. Run the tests.

### Further readings

Learn more from [the documentation for Allure Reqnroll](https://allurereport.org/docs/reqnroll/).

### Known limitations

Scenarios skipped ar runtime with IUnitTestRuntimeProvider.TestIgnore are
currently reported as broken. Please, prefer using the `@ignore` tag to skip
scenarios.
