Feature: Modules
    Module registration and endpoint mapping

@Unit
Scenario: RegisterModules with assembly registers modules
    Given I have a WebApplicationBuilder
    And I have an assembly with an IModule implementation
    When I call RegisterModules with the assembly
    Then the builder should be returned
    And the module services should be registered

@Unit
Scenario: RegisterModules discovers modules without being given them
    Given I have a WebApplicationBuilder
    When I call RegisterModules with no arguments
    Then the builder should be returned
    And the module services should be registered
    And the discovered modules should include TestModule and SecondTestModule

@Unit
Scenario: Discovery searches assemblies the AppDomain has not loaded
    Given an assembly is deployed alongside the application but not loaded
    When I get the candidate assemblies for discovery
    Then the candidates should include every loaded application assembly
    And the candidates should include the assembly that was not loaded
    And the candidates should exclude framework assemblies

@Unit
Scenario: RegisterModules with a marker type registers modules from that assembly
    Given I have a WebApplicationBuilder
    When I call RegisterModules with a marker type
    Then the builder should be returned
    And the module services should be registered

@Unit
Scenario: RegisterModules with pattern registers matching modules
    Given I have a WebApplicationBuilder
    When I call RegisterModules with pattern 'Asm.AspNetCore.Tests'
    Then the builder should be returned

@Unit
Scenario: RegisterModules with func registers provided modules
    Given I have a WebApplicationBuilder
    When I call RegisterModules with a module factory
    Then the builder should be returned
    And the module services should be registered

@Unit
Scenario: MapModuleEndpoints maps registered module endpoints
    Given I have a WebApplicationBuilder
    And I have registered modules
    When I build the application and map module endpoints
    Then the module endpoints should be mapped

@Unit
Scenario: AddModule registers a module in dependency injection
    Given I have a module service collection
    When I add a TestModule to the collection
    Then the service provider should resolve 1 module(s)
    And the module services should be registered in the collection

@Unit
Scenario: Module registration is additive
    Given I have a module service collection
    When I add a TestModule to the collection
    And I add a SecondTestModule to the collection
    Then the service provider should resolve 2 module(s)

@Unit
Scenario: MapModuleEndpoints maps endpoints for every DI-registered module
    Given I have a WebApplicationBuilder
    And I have registered two modules
    When I build the application and map module endpoints
    Then both module endpoints should be mapped
