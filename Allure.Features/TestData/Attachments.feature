@data
Feature: Attachments

@attachment @beforescenario @afterscenario
Scenario: With attachments
	Given Step with attachment
	Given Step is 'passed'

@failed @attachment @beforescenario
Scenario: Failed BeforeScenario with attachment
	Given Step with attachment
	Given Step is 'passed'

@failed @attachment @afterscenario
Scenario: Failed AfterScenario with attachment
	Given Step with attachment
	Given Step is 'passed'

@failed @attachment @beforestep
Scenario: Failed BeforeStep with attachment
	Given Step with attachment
	Given Step is 'passed'

@failed @attachment @afterstep
Scenario: Failed AfterStep with attachment
	Given Step with attachment
	Given Step is 'passed'
