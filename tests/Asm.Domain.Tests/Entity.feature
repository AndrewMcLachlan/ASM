Feature: Entity tests
Verify that Entity base class functionality works correctly

@Unit
Scenario: New entity has empty events collection
    When I create a new test entity
    Then the entity should have 0 events

@Unit
Scenario: Add domain event to entity
    Given I have a test entity
    When I add a domain event to the entity
    Then the entity should have 1 events

@Unit
Scenario: Add multiple domain events to entity
    Given I have a test entity
    When I add 3 domain events to the entity
    Then the entity should have 3 events

@Unit
Scenario: Clear domain events from entity
    Given I have a test entity with 2 domain events
    When I clear the domain events
    Then the entity should have 0 events
