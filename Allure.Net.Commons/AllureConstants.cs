namespace Allure.Net.Commons
{
    public sealed class AllureConstants
    {
        public const string ALLURE_CONFIG_ENV_VARIABLE = "ALLURE_CONFIG";
        public const string CONFIG_FILENAME = "allureConfig.json";
        public const string DEFAULT_RESULTS_FOLDER = "allure-results";

        public const string TEST_RESULT_FILE_SUFFIX = "-result.json";
        public const string TEST_RESULT_CONTAINER_FILE_SUFFIX = "-container.json";
        public static string TEST_RUN_FILE_SUFFIX = "-testrun.json";
        public const string ATTACHMENT_FILE_SUFFIX = "-attachment";

        public const string OLD_ALLURE_ID_LABEL_NAME = "AS_ID";
        public const string NEW_ALLURE_ID_LABEL_NAME = "ALLURE_ID";

        public const string OLD_ALLURE_TESTPLAN_ENV_NAME = "AS_TESTPLAN_PATH";
        public const string NEW_ALLURE_TESTPLAN_ENV_NAME = "ALLURE_TESTPLAN_PATH";
    }
}