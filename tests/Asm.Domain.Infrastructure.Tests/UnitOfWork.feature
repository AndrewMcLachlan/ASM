Feature: Unit of Work Registration

@Unit
Scenario: AddUnitOfWork with null services throws ArgumentNullException
    Given I have a null service collection for UnitOfWork
    When I call AddUnitOfWork on the null collection
    Then an exception of type 'System.ArgumentNullException' is thrown

@Unit
Scenario: AddUnitOfWork returns same service collection
    Given I have a service collection for UnitOfWork
    When I call AddUnitOfWork
    Then the same UnitOfWork service collection should be returned

@Unit
Scenario: AddUnitOfWork registers scoped service for IUnitOfWork
    Given I have a service collection for UnitOfWork
    When I call AddUnitOfWork
    Then IUnitOfWork should be registered with scoped lifetime

@Unit
Scenario: AddUnitOfWork with concrete type registered resolves correctly
    Given I have a service collection for UnitOfWork
    And the concrete UnitOfWork type is registered
    When I call AddUnitOfWork
    And I build the UnitOfWork service provider
    Then IUnitOfWork can be resolved as the concrete type

@Unit
Scenario: AddUnitOfWork without concrete type throws on resolve
    Given I have a service collection for UnitOfWork
    When I call AddUnitOfWork
    And I build the UnitOfWork service provider
    Then resolving IUnitOfWork throws InvalidOperationException

@Unit
Scenario: AddUnitOfWork called multiple times adds multiple descriptors
    Given I have a service collection for UnitOfWork
    When I call AddUnitOfWork twice
    Then two IUnitOfWork registrations should exist
