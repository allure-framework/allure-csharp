Feature: Scenarios and Steps

@testdata 
Scenario: Empty scenario

@testdata 
Scenario: Passed scenario
	Given Step is 'passed'

@testdata 
Scenario: Failed scenario
	Given Step is 'passed'
	Given Step is 'failed'
	Given Step is 'passed'

@testdata 
Scenario: Broken scenario
	Given Step is 'passed'
	Given Step is 'broken'
	Given Step is 'passed'