Feature: Scenario and Step Bindings

@beforescenario @beforestep @failed
Scenario: Should handle BeforeScenario and BeforeStep failure
	Given Step is 'passed'
	Given Step is 'passed'

@beforescenario @failed
Scenario: Should handle BeforeScenario failure
	Given Step is 'passed'
	Given Step is 'passed'

@beforestep @failed
Scenario: Should handle BeforeStep failure
	Given Step is 'passed'
	Given Step is 'passed'
	Given Step is 'passed'

@afterstep @failed
Scenario: Should handle AfterStep failure
	Given Step is 'passed'
	Given Step is 'passed'

@afterscenario @failed
Scenario: Should handle AfterScenario failure
	Given Step is 'passed'
	Given Step is 'passed'