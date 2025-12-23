Feature: RouteGroupBuilderExtensions
    Extensions for mapping endpoint groups

@Unit
Scenario: MapGroups with assembly maps endpoint groups
    Given I have a RouteGroupBuilder
    And I have an assembly with an IEndpointGroup implementation
    When I call MapGroups with the assembly
    Then the endpoint group should be mapped
    And the route group builder should be returned

@Unit
Scenario: MapGroups with no endpoint groups returns builder
    Given I have a RouteGroupBuilder
    And I have an assembly with no IEndpointGroup implementations
    When I call MapGroups with the assembly
    Then the route group builder should be returned
