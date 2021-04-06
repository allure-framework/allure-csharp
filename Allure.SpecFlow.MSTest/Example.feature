@foo
Feature: Example

  @passed @bar
  Scenario: Foo and Bar
    Given Step is 'passed'

  @passed
  Scenario: Foo 1
    Given Step is 'passed'

  @passed @foo
  Scenario: Foo 2
    Given Step is 'passed'