Feature: Domain Events

@integration
Scenario: Domain Event is Handled
    Given An entity defines a domain event
    When I call SaveChanges
    Then The domain event is handled