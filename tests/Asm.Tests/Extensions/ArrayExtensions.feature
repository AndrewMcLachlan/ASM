Feature: Array Extensions

@Unit
Scenario: Shuffle array returns same elements
    Given I have an array with values [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
    When I call Shuffle on the array
    Then the array should contain all original elements
    And the array should have 10 elements

@Unit
Scenario: Shuffle null array throws ArgumentNullException
    Given I have a null array
    When I call Shuffle on the array
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: IsNullOrEmpty returns true for null array
    Given I have a null array
    When I call IsNullOrEmpty on the array
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns true for empty array
    Given I have an empty array
    When I call IsNullOrEmpty on the array
    Then the boolean value true is returned

@Unit
Scenario: IsNullOrEmpty returns false for non-empty array
    Given I have an array with values [1, 2, 3]
    When I call IsNullOrEmpty on the array
    Then the boolean value false is returned

@Unit
Scenario: Empty returns true for empty array
    Given I have an empty array
    When I call Empty on the array
    Then the boolean value true is returned

@Unit
Scenario: Empty returns false for non-empty array
    Given I have an array with values [1, 2, 3]
    When I call Empty on the array
    Then the boolean value false is returned
