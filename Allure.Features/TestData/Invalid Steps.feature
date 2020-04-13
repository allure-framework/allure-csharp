Feature: Invalid Steps

  @broken
  Scenario: All steps are invalid
    Given I don't have such step
    And I don't have such step too

  @broken
  Scenario: Some steps are invalid
    Given Step is 'passed'
    Given I don't have such step
    Given Step is 'passed'
    And I don't have such step too

  @broken
  Scenario: Failed step followed by invalid step
    Given Step is 'failed'
    Given I don't have such step
    Given Step is 'passed'
    And I don't have such step too
