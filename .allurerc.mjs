export default {
  name: "Allure C#",
  output: "./out/allure-report",
  plugins: {
    testops: {
      options: {
        launchName: `Allure C# GitHub actions run (${new Date().toISOString()})`,
      },
    },
  },
};
