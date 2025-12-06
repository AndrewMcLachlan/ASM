Feature: IList Extensions

@Unit
Scenario: Shuffle list returns same elements in different order
    Given I have a list with values [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    When I call Shuffle on the list
    Then the list should contain all original elements
    And the list should have 10 elements

@Unit
Scenario: Shuffle null list throws ArgumentNullException
    Given I have a null list
    When I call Shuffle on the list
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: Shuffle readonly list throws InvalidOperationException
    Given I have a readonly list with values [1, 2, 3]
    When I call Shuffle on the list
    Then an exception of type 'System.InvalidOperationException' should be thrown
    And the exception message is 'List is readonly'

@Unit
Scenario: IsNullOrEmpty returns true for null list
    Given I have a null list
    When I call IsNullOrEmpty on the list
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns true for empty list
    Given I have an empty list
    When I call IsNullOrEmpty on the list
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns false for non-empty list
    Given I have a list with values [1, 2, 3]
    When I call IsNullOrEmpty on the list
    Then the boolean value false is returned

@Unit
Scenario: Empty returns true for empty list
    Given I have an empty list
    When I call Empty on the list
    Then the boolean value true is returned

@Unit
Scenario: Empty returns false for non-empty list
    Given I have a list with values [1, 2, 3]
    When I call Empty on the list
    Then the boolean value false is returned
