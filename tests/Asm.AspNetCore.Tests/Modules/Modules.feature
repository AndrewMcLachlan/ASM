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
