Feature: Queryable Registration

@Unit
Scenario: AddQueryable returns same service collection
    Given I have a service collection for Queryable
    When I call AddQueryable for an entity type
    Then the same Queryable service collection should be returned

@Unit
Scenario: AddQueryable registers transient IQueryable service
    Given I have a service collection for Queryable
    When I call AddQueryable for an entity type
    Then IQueryable of the entity should be registered as transient

@Unit
Scenario: AddQueryable for multiple entity types registers all
    Given I have a service collection for Queryable
    When I call AddQueryable for two different entity types
    Then both IQueryable services should be registered

@Unit
Scenario: AddQueryable on empty collection adds one descriptor
    Given I have an empty service collection for Queryable
    When I call AddQueryable for an entity type
    Then exactly one service descriptor should be added

@Unit
Scenario: AddQueryable allows duplicate registrations
    Given I have a service collection for Queryable
    When I call AddQueryable twice for the same entity type
    Then two IQueryable registrations should exist for the entity
