@data @failed
Feature: Invalid Steps


Scenario: All steps are invalid
	Given I don't have such step
	And I don't have such step too


Scenario: Some steps are invalid
	Given Step is 'passed'
	Given I don't have such step
	Given Step is 'passed'
	And I don't have such step too

Scenario: Failed step followed by invalid step
	Given Step is 'failed'
	Given I don't have such step
	Given Step is 'passed'
	And I don't have such step too
