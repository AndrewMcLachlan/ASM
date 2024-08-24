Feature: DateOnlyExtensions
  In order to use DateOnly extension methods
  As a developer
  I want to have unit tests for the DateOnly extension methods

@Unit
Scenario: Get today's date
    When I get today's date
    Then the result should be today's date

@Unit
Scenario: Convert DateOnly to DateTime at start of day
    Given I have a date "2023-10-01"
    When I convert the date to DateTime at the start of the day
    Then the DateTime result should be "2023-10-01T00:00:00"

@Unit
Scenario: Convert DateOnly to DateTime at end of day
    Given I have a date "2023-10-01"
    When I convert the date to DateTime at the end of the day
    Then the DateTime result should be "2023-10-01T23:59:59.9999999"

@Unit
Scenario: Calculate difference in months between two dates
    Given I have a date "<Date>"
    And I have another date "<Other Date>"
    When I calculate the difference in months between the dates
    Then the integer result should be <Result>

Examples:
    | Date       | Other Date | Result |
    | 2023-01-01 | 2023-01-01 | 0      |
    | 2023-01-01 | 2023-02-01 | 1      |
    | 2023-02-01 | 2023-01-01 | 1      |
    | 2023-01-01 | 2022-12-01 | 1      |
    | 2023-01-01 | 2022-01-01 | 12     |
    | 2023-01-01 | 2023-01-02 | 0      |
    | 2023-10-01 | 2022-08-15 | 13     |
    | 2022-08-15 | 2023-10-01 | 13     |
    | 2023-08-01 | 2022-10-15 | 9      |
    | 2022-10-15 | 2023-08-01 | 9      |