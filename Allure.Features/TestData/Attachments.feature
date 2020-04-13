Feature: Attachments

  @passed @attachment @beforescenario @afterscenario
  Scenario: With attachments
    Given Step with attachment
    Given Step is 'passed'

  @broken @attachment @beforescenario
  Scenario: Failed BeforeScenario with attachment
    Given Step with attachment
    Given Step is 'passed'

  @broken @attachment @afterscenario
  Scenario: Failed AfterScenario with attachment
    Given Step with attachment
    Given Step is 'passed'

  @broken @attachment @beforestep
  Scenario: Failed BeforeStep with attachment
    Given Step with attachment
    Given Step is 'passed'

  @broken @attachment @afterstep
  Scenario: Failed AfterStep with attachment
    Given Step with attachment
    Given Step is 'passed'
