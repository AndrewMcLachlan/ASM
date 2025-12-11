Feature: IEndpointGroup
    As a developer
    I want to group related endpoints together
    So that I can organize and reuse common endpoint configurations

@Unit
Scenario: Endpoint group can map to a route group builder
    Given I have a test endpoint group
    And I have an endpoint route builder
    When I map the endpoint group
    Then the route group builder should be returned
    And the endpoint group should confirm it was mapped

@Unit
Scenario: Endpoint group implements the IEndpointGroup interface
    Given I have a test endpoint group
    Then the endpoint group should implement IEndpointGroup
