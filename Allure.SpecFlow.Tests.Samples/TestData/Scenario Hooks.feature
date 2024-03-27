Feature: Scenario and Step Bindings

  @broken @beforescenario @beforestep
  Scenario: Should handle BeforeScenario and BeforeStep failure
    Given Step is 'passed'
    Given Step is 'passed'

  @broken @beforescenario
  Scenario: Should handle BeforeScenario failure
    Given Step is 'passed'
    Given Step is 'passed'

  @broken @beforestep
  Scenario: Should handle BeforeStep failure
    Given Step is 'passed'
    Given Step is 'passed'
    Given Step is 'passed'

  @broken @afterstep
  Scenario: Should handle AfterStep failure
    Given Step is 'passed'
    Given Step is 'passed'

  @broken @afterscenario
  Scenario: Should handle AfterScenario failure
    Given Step is 'passed'
    Given Step is 'passed'