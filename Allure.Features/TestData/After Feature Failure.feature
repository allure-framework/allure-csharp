﻿@afterfeaturefailed
Feature: After Feature Failure

  @passed
  Scenario: After Feature Failure 1
    Given Step is 'passed'

  @failed
  Scenario: After Feature Failure 3
    Given Step is 'failed'



