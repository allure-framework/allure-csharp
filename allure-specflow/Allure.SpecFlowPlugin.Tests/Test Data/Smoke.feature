Feature: Scenarios and Steps

Scenario: Empty scenario

Scenario: Passed scenario
	Given Step is 'passed'
	Given Step is 'passed'

Scenario: Failed scenario
	Given Step is 'passed'
	Given Step is 'failed'
	Given Step is 'passed'

Scenario: Broken scenario
	Given Step is 'passed'
	Given Step is 'broken'

Scenario: Broken scenario (multisteps)
	Given Step is 'passed'
	Given Step is 'broken'
	Given Step is 'passed'