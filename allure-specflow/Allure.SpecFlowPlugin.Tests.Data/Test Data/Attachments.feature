Feature: Attachments

 @attachment @BeforeScenario @AfterScenario
Scenario: With attachments
	Given Step with attachment
	Given Step is 'passed'

 @fail @attachment @BeforeScenario
Scenario: Failed BeforeScenario with attachment
	Given Step with attachment
	Given Step is 'passed'

 @fail @attachment @AfterScenario
Scenario: Failed AfterScenario with attachment
	Given Step with attachment
	Given Step is 'passed'

 @fail @attachment @BeforeStep
Scenario: Failed BeforeStep with attachment
	Given Step with attachment
	Given Step is 'passed'

 @fail @attachment @AfterStep
Scenario: Failed AfterStep with attachment
	Given Step with attachment
	Given Step is 'passed'
