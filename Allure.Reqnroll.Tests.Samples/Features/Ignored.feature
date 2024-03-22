Feature: Ignored
  Treat exceptions thrown by IUnitTestRuntimeProvider.TestIgnore as skipped
  (works in NUnit and xUnit.net)

  @beforescenario @runtimeignore @skipped
  Scenario: Should be ignored at runtime
