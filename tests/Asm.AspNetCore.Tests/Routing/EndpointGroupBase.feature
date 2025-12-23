Feature: EndpointGroupBase
    Base class for endpoint groups

@Unit
Scenario: MapGroup creates a route group with authorization
    Given I have an EndpointGroupBase implementation
    And the endpoint group has no authorization policy
    When I call MapGroup
    Then a route group should be created
    And the route group should have default authorization

@Unit
Scenario: MapGroup with custom authorization policy
    Given I have an EndpointGroupBase implementation
    And the endpoint group has authorization policy 'CustomPolicy'
    When I call MapGroup
    Then a route group should be created
    And the route group should have custom authorization
