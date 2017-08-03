Feature: Scenario Hooks

 @BeforeScenario @fail
Scenario: Should handle BeforeScenario failure
	Given Step is 'passed'
	Given Step is 'passed'

 @BeforeStep @fail
Scenario: Should handle BeforeStep failure
	Given Step is 'passed'
	Given Step is 'passed'
	Given Step is 'passed'

 @AfterStep @fail
Scenario: Should handle AfterStep failure
	Given Step is 'passed'
	Given Step is 'passed'

 @AfterScenario @fail
Scenario: Should handle AfterScenario failure
	Given Step is 'passed'
	Given Step is 'passed'