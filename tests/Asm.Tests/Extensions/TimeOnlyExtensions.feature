Feature: TimeOnlyExtensions
  In order to use TimeOnly extension methods
  As a developer
  I want to have unit tests for the TimeOnly extension methods

@Unit
Scenario: Get current time as Now
    When I get TimeOnly Now
    Then the result should be approximately the current time

@Unit
Scenario: Get current UTC time
    When I get TimeOnly UtcNow
    Then the result should be approximately the current UTC time

