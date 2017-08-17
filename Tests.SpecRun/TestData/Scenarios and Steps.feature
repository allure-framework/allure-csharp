@data
Feature: Scenarios and Steps

Scenario: Empty scenario
 
Scenario: Passed scenario
	Given Step is 'passed'
 
Scenario: Failed scenario
	Given Step is 'passed'
	Given Step is 'failed'
	Given Step is 'passed'

Scenario: Shared Steps
	Given I execute the steps of 'Scenarios and Steps'.'Passed scenario'
	And Step is 'passed'