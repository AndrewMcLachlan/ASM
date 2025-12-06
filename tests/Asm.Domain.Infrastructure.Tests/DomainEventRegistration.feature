Feature: Domain Event Registration

@Unit
Scenario: AddDomainEvents registers valid handler as transient
    Given I have a service collection
    And I have an assembly with a valid domain event handler
    When I call AddDomainEvents with the assembly
    Then the handler should be registered as transient for the interface type

@Unit
Scenario: AddDomainEvents skips abstract classes
    Given I have a service collection
    And I have an assembly with an abstract domain event handler
    When I call AddDomainEvents with the assembly
    Then no handler should be registered

@Unit
Scenario: AddDomainEvents skips non-handler classes
    Given I have a service collection
    And I have an assembly with a non-handler class
    When I call AddDomainEvents with the assembly
    Then no handler should be registered

@Unit
Scenario: AddDomainEvents registers all interfaces for multi-handler
    Given I have a service collection
    And I have an assembly with a multi-interface handler
    When I call AddDomainEvents with the assembly
    Then both handler interfaces should be registered

@Unit
Scenario: AddDomainEvents always registers IPublisher
    Given I have a service collection
    And I have an empty assembly
    When I call AddDomainEvents with the assembly
    Then IPublisher should be registered as transient

@Unit
Scenario: AddDomainEvents returns service collection for chaining
    Given I have a service collection
    And I have an empty assembly
    When I call AddDomainEvents with the assembly
    Then the same service collection instance should be returned

@Unit
Scenario: AddDomainEvents with null assembly throws exception
    Given I have a service collection
    And I have a null assembly
    When I call AddDomainEvents with the null assembly
    Then an exception of type 'System.NullReferenceException' is thrown

@Unit
Scenario: AddDomainEvents does not replace existing IPublisher
    Given I have a service collection with IPublisher already registered as singleton
    And I have an empty assembly
    When I call AddDomainEvents with the assembly
    Then only one IPublisher registration should exist
    And the IPublisher should be singleton

@Unit
Scenario: AddDomainEvents with mixed types only registers valid handlers
    Given I have a service collection
    And I have an assembly with mixed valid and invalid types
    When I call AddDomainEvents with the assembly
    Then only the valid handler should be registered

@Unit
Scenario: AddDomainEvents called multiple times registers all handlers
    Given I have a service collection
    And I have two assemblies with different handlers
    When I call AddDomainEvents with both assemblies
    Then both handlers should be registered

@Unit
Scenario: AddDomainEvent registers handler as transient
    Given I have a service collection
    When I call AddDomainEvent for a specific handler
    Then the handler should be registered as transient for the interface type

@Unit
Scenario: AddDomainEvent registers IPublisher
    Given I have a service collection
    When I call AddDomainEvent for a specific handler
    Then IPublisher should be registered with Publisher implementation

@Unit
Scenario: AddDomainEvent returns same service collection
    Given I have a service collection
    When I call AddDomainEvent for a specific handler
    Then the same service collection instance should be returned

@Unit
Scenario: AddDomainEvent with null services throws exception
    Given I have a null service collection
    When I call AddDomainEvent on the null collection
    Then an exception is thrown

@Unit
Scenario: AddDomainEvent called multiple times registers multiple handlers
    Given I have a service collection
    When I call AddDomainEvent twice for the same handler
    Then two handler registrations should exist

@Unit
Scenario: AddDomainEvent registers IPublisher only once
    Given I have a service collection
    When I call AddDomainEvent for two different handlers
    Then only one IPublisher registration should exist

@Unit
Scenario: AddDomainEvent registers LazyCache
    Given I have a service collection
    When I call AddDomainEvent for a specific handler
    Then IAppCache should be registered

@Unit
Scenario: Registered handler can be resolved
    Given I have a service collection
    And I call AddDomainEvent for a specific handler
    When I build the service provider
    Then the handler can be resolved
