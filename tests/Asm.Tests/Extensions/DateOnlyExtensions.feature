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
Scenario Outline: Get difference in months
    Given I have a DateOnly '<Date>'
    And I have another DateOnly '<Other Date>'
    When I call DifferenceInMonths on DateOnly
    Then the integer value <Difference> is returned

Examples:
    | Date       | Other Date | Difference |
    | 2023-01-01 | 2023-01-01 | 0          |
    | 2023-01-01 | 2023-02-01 | 1          |
    | 2023-02-01 | 2023-01-01 | 1          |
    | 2023-01-01 | 2022-12-01 | 1          |
    | 2023-01-01 | 2022-01-01 | 12         |
    | 2023-01-01 | 2023-01-02 | 0          |
    | 2023-10-01 | 2022-08-15 | 13         |
    | 2022-08-15 | 2023-10-01 | 13         |
    | 2023-08-01 | 2022-10-15 | 9          |
    | 2022-10-15 | 2023-08-01 | 9          |
    | 2023-01-31 | 2023-03-01 | 1          |
    | 2023-01-15 | 2023-03-15 | 2          |
    | 2023-01-15 | 2023-02-01 | 0          |
    | 2023-01-31 | 2023-02-01 | 0          |

@Unit
Scenario: Get today
    When I get today as DateOnly
    Then the result should be today