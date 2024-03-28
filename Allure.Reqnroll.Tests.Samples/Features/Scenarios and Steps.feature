Feature: Scenarios and Steps

  @passed
  Scenario: Empty scenario

  @passed
  Scenario: Passed scenario
    Given Step is 'passed'

  @failed
  Scenario: Failed scenario
    Given Step is 'passed'
    Given Step is 'failed'
    Given Step is 'passed'

  @passed @examples
  Scenario Outline: Scenario with examples
    Given Step with table
      | id   | <id>   |
      | name | <name> |
    
    Examples: 
      | id | name |
      | 1  | John |
      | 2  | Alex |