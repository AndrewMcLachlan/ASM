Feature: ICollection Extensions

@Unit
Scenario: AddRange adds all items to collection
    Given I have a collection with values [1, 2, 3]
    And I have items to add [4, 5, 6]
    When I call AddRange on the collection
    Then the collection should contain [1, 2, 3, 4, 5, 6]

@Unit
Scenario: AddRange to empty collection
    Given I have an empty collection
    And I have items to add [1, 2, 3]
    When I call AddRange on the collection
    Then the collection should contain [1, 2, 3]

@Unit
Scenario: AddRange with empty items does nothing
    Given I have a collection with values [1, 2, 3]
    And I have empty items to add
    When I call AddRange on the collection
    Then the collection should contain [1, 2, 3]

@Unit
Scenario: AddRange with null items does nothing
    Given I have a collection with values [1, 2, 3]
    And I have null items to add
    When I call AddRange on the collection
    Then the collection should contain [1, 2, 3]

@Unit
Scenario: AddRange on null collection throws ArgumentNullException
    Given I have a null collection
    And I have items to add [1, 2, 3]
    When I call AddRange on the collection
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: AddRange on readonly collection throws InvalidOperationException
    Given I have a readonly collection with values [1, 2, 3]
    And I have items to add [4, 5, 6]
    When I call AddRange on the collection
    Then an exception of type 'System.InvalidOperationException' should be thrown
    And the exception message is 'Collection is readonly'
