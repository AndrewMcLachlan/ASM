Feature: Domain Events

@integration
Scenario: Domain Event is Handled
    Given An entity defines a domain event
    When I call SaveChanges
    Then The domain event is handled

@integration
Scenario: Domain Event is Handled Multiple Times
    Given An entity defines a multi domain event
    When I call SaveChanges
    Then The domain event is handled multiple times