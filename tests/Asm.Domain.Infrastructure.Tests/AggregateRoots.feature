Feature: Aggregate Roots Registration

@Unit
Scenario: AddAggregateRoots with null assembly throws exception
    Given I have a service collection for AggregateRoots
    And I have a null entity assembly
    When I call AddAggregateRoots with the null assembly
    Then an exception of type 'System.NullReferenceException' is thrown

@Unit
Scenario: AddAggregateRoots with no aggregate roots does not add services
    Given I have a service collection for AggregateRoots
    And I have an assembly with no aggregate root types
    When I call AddAggregateRoots
    Then no services should be registered

@Unit
Scenario: AddAggregateRoots with empty assembly does not add services
    Given I have a service collection for AggregateRoots
    And I have an empty entity assembly
    When I call AddAggregateRoots
    Then no services should be registered

@Unit
Scenario: AddAggregateRoots with single aggregate root adds one service
    Given I have a service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots
    Then one service descriptor should be added

@Unit
Scenario: AddAggregateRoots with multiple aggregate roots adds all services
    Given I have a service collection for AggregateRoots
    And I have an assembly with two aggregate root types
    When I call AddAggregateRoots
    Then two service descriptors should be added

@Unit
Scenario: AddAggregateRoots only registers types with AggregateRootAttribute
    Given I have a service collection for AggregateRoots
    And I have an assembly with mixed aggregate and non-aggregate types
    When I call AddAggregateRoots
    Then only aggregate root types should be registered

@Unit
Scenario: AddAggregateRoots registers correct IQueryable service type
    Given I have a service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots
    Then the service type should be IQueryable of the aggregate root

@Unit
Scenario: AddAggregateRoots registers with transient lifetime
    Given I have a service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots
    Then the service should have transient lifetime

@Unit
Scenario: AddAggregateRoots returns same service collection
    Given I have a service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots
    Then the same AggregateRoots service collection should be returned

@Unit
Scenario: AddAggregateRoots with no roots returns same service collection
    Given I have a service collection for AggregateRoots
    And I have an empty entity assembly
    When I call AddAggregateRoots
    Then the same AggregateRoots service collection should be returned

@Unit
Scenario: AddAggregateRoots registers factory-based service
    Given I have a service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots
    Then the service should use a factory implementation

@Unit
Scenario: AddAggregateRoots with null services and roots throws exception
    Given I have a null service collection for AggregateRoots
    And I have an assembly with one aggregate root type
    When I call AddAggregateRoots on the null collection
    Then an exception of type 'System.NullReferenceException' is thrown

@Unit
Scenario: AddAggregateRoots with null services and no roots returns null
    Given I have a null service collection for AggregateRoots
    And I have an empty entity assembly
    When I call AddAggregateRoots on the null collection without exception
    Then the AggregateRoots result should be null
