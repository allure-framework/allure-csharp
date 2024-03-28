@foo
Feature: Tags

  @passed @bar
  Scenario: Foo and Bar
    Given Step is 'passed'

  @passed
  Scenario: Foo 1
    Given Step is 'passed'
    And Step is 'passed'
    When Step is 'passed'
    And Step is 'passed'
    Then Step is 'passed'
    And Step is 'passed'

  @passed @foo
  Scenario: Foo 2
    Given Step is 'passed'