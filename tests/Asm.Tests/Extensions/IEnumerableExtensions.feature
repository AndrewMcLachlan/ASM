Feature: IEnumerable Extensions

@Unit
Scenario Outline: Page enumerable returns correct items
    Given I have an enumerable with values [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    When I call Page with page size <PageSize> and page number <PageNumber>
    Then the result should be [<Expected>]

Examples:
    | PageSize | PageNumber | Expected       |
    | 3        | 1          | 1, 2, 3        |
    | 3        | 2          | 4, 5, 6        |
    | 3        | 3          | 7, 8, 9        |
    | 3        | 4          | 10             |
    | 5        | 1          | 1, 2, 3, 4, 5  |
    | 5        | 2          | 6, 7, 8, 9, 10 |
    | 10       | 1          | 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 |
    | 10       | 2          |                |

@Unit
Scenario: Page enumerable with page beyond range returns empty
    Given I have an enumerable with values [1, 2, 3]
    When I call Page with page size 10 and page number 5
    Then the result should be empty

@Unit
Scenario: Shuffle enumerable returns same elements
    Given I have an enumerable with values [1, 2, 3, 4, 5]
    When I call Shuffle on the enumerable
    Then the result should contain all original elements
    And the result should have 5 elements

@Unit
Scenario: IsNullOrEmpty returns true for null enumerable
    Given I have a null enumerable
    When I call IsNullOrEmpty on the enumerable
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns true for empty enumerable
    Given I have an empty enumerable
    When I call IsNullOrEmpty on the enumerable
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns false for non-empty enumerable
    Given I have an enumerable with values [1, 2, 3]
    When I call IsNullOrEmpty on the enumerable
    Then the boolean value false is returned

@Unit
Scenario: Empty returns true for empty enumerable
    Given I have an empty enumerable
    When I call Empty on the enumerable
    Then the boolean value true is returned

@Unit
Scenario: Empty returns false for non-empty enumerable
    Given I have an enumerable with values [1, 2, 3]
    When I call Empty on the enumerable
    Then the boolean value false is returned
