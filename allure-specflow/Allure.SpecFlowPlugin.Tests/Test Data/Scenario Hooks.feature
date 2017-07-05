Feature: Scenario Hooks

@BeforeScenarioFailed @hooks
Scenario: Should handle BeforeScenario failure
	Given Step is 'passed'
	Given Step is 'passed'

@BeforeStepFailed @hooks
Scenario: Should handle BeforeStep failure
	Given Step is 'passed'
	Given Step is 'passed'
	Given Step is 'passed'

@AfterStepFailed @hooks
Scenario: Should handle AfterStep failure
	Given Step is 'passed'
	Given Step is 'passed'

@Attachment @hooks
Scenario: Should handle AfterScenario failure
with attachments

	Given Step is 'passed'
	Given Step is 'passed'