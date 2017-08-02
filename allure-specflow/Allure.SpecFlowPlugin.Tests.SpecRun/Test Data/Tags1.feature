@foo
Feature: Tags1

@testdata @bar
Scenario: Foo and Bar
	Given Step is 'passed'

@testdata 
Scenario: Foo 1
	Given Step is 'passed'

@testdata @foo
Scenario: Foo 2
	Given Step is 'passed'