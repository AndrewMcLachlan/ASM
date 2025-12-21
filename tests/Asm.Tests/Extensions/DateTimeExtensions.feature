Feature: DateTime Extensions

@Unit
Scenario: Get First Day of Week
    This scenario depends on the locale in which the test is run
    Given I have a date '2019-04-04'
    When I call FirstDayOfWeek
    Then the date '2019-03-31' is returned

@Unit
Scenario Outline: Get First Day of Week for a specific locale
    Given I have a date '2019-04-04'
    And my locale is '<Locale>'
    When I call FirstDayOfWeek
    Then the date '<Result>' is returned

Examples:
    | Locale | Result     |
    | en-US  | 2019-03-31 |
    | en-GB  | 2019-04-01 |

@Unit
Scenario: Get Last Day of Week
    This scenario depends on the locale in which the test is run
    Given I have a date '2019-04-04'
    When I call LastDayOfWeek
    Then the date '2019-04-06' is returned

@Unit
Scenario Outline: Get Last Day of Week for a specific locale
    Given I have a date '2019-04-04'
    And my locale is '<Locale>'
    When I call LastDayOfWeek
    Then the date '<Result>' is returned

Examples:
    | Locale | Result     |
    | en-US  | 2019-04-06 |
    | en-GB  | 2019-04-07 |

@Unit
Scenario Outline: Get difference in months
    Given I have a date '<Date>'
    And I have another date '<Other Date>'
    When I call DifferenceInMonths
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
Scenario: Convert DateTime to DateOnly
    Given I have a datetime '2023-06-15 14:30:00'
    When I call ToDateOnly
    Then the DateOnly result should be '2023-06-15'

@Unit
Scenario: Convert DateTime to TimeOnly
    Given I have a datetime '2023-06-15 14:30:45'
    When I call ToTimeOnly
    Then the TimeOnly result should be '14:30:45'

@Unit
Scenario: Access DateOnly extension property
    Given I have a datetime '2023-12-25 10:00:00'
    When I access the DateOnly extension property
    Then the DateOnly result should be '2023-12-25'

@Unit
Scenario: Access TimeOnly extension property
    Given I have a datetime '2023-12-25 18:45:30'
    When I access the TimeOnly extension property
    Then the TimeOnly result should be '18:45:30'