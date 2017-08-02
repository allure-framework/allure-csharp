Feature: Scenario Hooks

@testdata @BeforeScenario @fail
Scenario: Should handle BeforeScenario failure
	Given Step is 'passed'
	Given Step is 'passed'

@testdata @BeforeStep @fail
Scenario: Should handle BeforeStep failure
	Given Step is 'passed'
	Given Step is 'passed'
	Given Step is 'passed'

@testdata @AfterStep @fail
Scenario: Should handle AfterStep failure
	Given Step is 'passed'
	Given Step is 'passed'

@testdata @AfterScenario @fail
Scenario: Should handle AfterScenario failure
	Given Step is 'passed'
	Given Step is 'passed'