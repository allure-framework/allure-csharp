@afterfeaturefailed
Feature: After Feature Failure

  @passed
  Scenario: After Feature Failure 1
    Given Step is 'passed'

  @broken
  Scenario: After Feature Failure 2
    Given Step is 'failed'



