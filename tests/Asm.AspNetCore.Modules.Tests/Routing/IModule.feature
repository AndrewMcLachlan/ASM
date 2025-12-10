Feature: IModule
    As a developer
    I want to use modules to register services and map endpoints
    So that I can organize my application with modular components

@Unit
Scenario: Module can add services to the service collection
    Given I have a test module
    And I have a service collection
    When I add services from the module
    Then the services should be added to the collection
    And the module should confirm services were added

@Unit
Scenario: Module can map endpoints to the endpoint route builder
    Given I have a test module
    And I have an endpoint route builder
    When I map endpoints from the module
    Then the endpoints should be mapped
    And the module should confirm endpoints were mapped

@Unit
Scenario: Module methods return the builders for chaining
    Given I have a test module
    And I have a service collection
    And I have an endpoint route builder
    When I add services from the module
    Then the service collection should be returned
    When I map endpoints from the module
    Then the endpoint route builder should be returned
