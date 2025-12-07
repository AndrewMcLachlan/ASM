Feature: Domain Events

@Integration
Scenario: Domain Event is Handled
    Given An entity defines a domain event
    When I call SaveChanges
    Then The domain event is handled

@Integration
Scenario: Domain Event is Handled Multiple Times
    Given An entity defines a multi domain event
    When I call SaveChanges
    Then The domain event is handled multiple times
