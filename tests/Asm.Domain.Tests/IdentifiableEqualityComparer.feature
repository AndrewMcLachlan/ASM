Feature: Identifiable Equality Comparer
Tests for IIdentifiableEqualityComparer

@Unit
Scenario Outline: Test equality with IIdentifiableEqualityComparer
    Given I have an identifiable entity with ID <First ID>
    And I have a second identifiable entity with ID <Second ID>
    When I check equality with IIdentifiableEqualityComparer
    Then the boolean value <Result> is returned

Examples:
    | First ID | Second ID | Result |
    | 1        | 1         | true   |
    | 1        | 2         | false  |
    | 2        | 1         | false  |
    | 1        | <NULL>    | false  |
    | <NULL>   | 1         | false  |
    | <NULL>   | <NULL>    | false  |

@Unit
Scenario: GetHashCode returns consistent value
    Given I have an identifiable entity with ID 42
    When I get the hash code using IIdentifiableEqualityComparer
    Then the hash code should equal the ID hash code

@Unit
Scenario: GetHashCode with null throws ArgumentNullException
    When I get the hash code of null using IIdentifiableEqualityComparer
    Then an exception of type 'System.ArgumentNullException' should be thrown
