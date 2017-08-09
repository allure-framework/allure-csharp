@data
Feature: Scenarios and Steps

Scenario: Empty scenario
 
Scenario: Passed scenario
	Given Step is 'passed'
 
Scenario: Failed scenario
	Given Step is 'passed'
	Given Step is 'failed'
	Given Step is 'passed'
