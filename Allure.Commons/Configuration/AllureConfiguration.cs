namespace Allure.Commons.Configuration
{
    public partial class AllureConfiguration
    {
        public virtual string Directory { get; set; } = "allure-results";

        protected virtual void Validate()
        {

        }
    }
}
