Feature: ReadOnly DbContext Registration

@Unit
Scenario: AddReadOnlyDbContext returns same service collection
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with default parameters
    Then the same service collection should be returned

@Unit
Scenario: AddReadOnlyDbContext with null optionsAction returns service collection
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with null optionsAction
    Then the same service collection should be returned

@Unit
Scenario: AddReadOnlyDbContext with optionsAction returns service collection
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with an optionsAction
    Then the same service collection should be returned

@Unit
Scenario: AddReadOnlyDbContext registers IReadOnlyDbContext service
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with an in-memory database
    And I build the DbContext service provider
    Then IReadOnlyDbContext can be resolved

@Unit
Scenario: AddReadOnlyDbContext with scoped lifetime can resolve within scope
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with scoped lifetime
    And I build the DbContext service provider
    Then IReadOnlyDbContext can be resolved within a scope

@Unit
Scenario: AddReadOnlyDbContext invokes optionsAction on resolve
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with a tracking optionsAction
    And I build the DbContext service provider
    And I resolve IReadOnlyDbContext
    Then the optionsAction should have been invoked

@Unit
Scenario: AddReadOnlyDbContext with singleton lifetime returns same instance
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with singleton lifetime
    And I build the DbContext service provider
    Then resolving IReadOnlyDbContext twice returns the same instance

@Unit
Scenario: AddReadOnlyDbContext with transient lifetime returns different instances
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with transient lifetime
    And I build the DbContext service provider
    Then resolving IReadOnlyDbContext twice returns different instances

@Unit
Scenario Outline: AddReadOnlyDbContext with various lifetimes returns service collection
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with context lifetime '<contextLifetime>' and options lifetime '<optionsLifetime>'
    Then the same service collection should be returned

    Examples:
        | contextLifetime | optionsLifetime |
        | Scoped          | Scoped          |
        | Singleton       | Singleton       |
        | Transient       | Transient       |
        | Scoped          | Singleton       |
        | Singleton       | Scoped          |

@Unit
Scenario: AddReadOnlyDbContext registers concrete DbContext type
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with singleton lifetime
    Then the concrete DbContext type should be registered

@Unit
Scenario: AddReadOnlyDbContext called multiple times does not throw
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext twice
    Then no exception is thrown

@Unit
Scenario: AddReadOnlyDbContext with IServiceProvider optionsAction receives valid provider
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with IServiceProvider optionsAction
    And I build the DbContext service provider
    And I resolve IReadOnlyDbContext
    Then the IServiceProvider should have been captured

@Unit
Scenario Outline: AddReadOnlyDbContext registers with correct context lifetime
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with only context lifetime '<lifetime>'
    Then the IReadOnlyDbContext should be registered with '<lifetime>' lifetime

    Examples:
        | lifetime  |
        | Scoped    |
        | Singleton |
        | Transient |

@Unit
Scenario: AddReadOnlyDbContext with service interface registers correctly
    Given I have a service collection for DbContext
    When I call AddReadOnlyDbContext with TContextService and TContextImplementation
    And I build the DbContext service provider
    Then IReadOnlyDbContext can be resolved
