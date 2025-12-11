Feature: EndpointGroupBase
    As a developer
    I want to create endpoint groups with common configurations
    So that I can reduce boilerplate code for endpoint mapping

@Unit
Scenario: Endpoint group base defines name and path
    Given I have a simple endpoint group
    Then the group name should be 'SimpleGroup'
    And the group path should be '/api/simple'

@Unit
Scenario: Endpoint group base has default empty tags
    Given I have a minimal endpoint group
    Then the group tags should be empty

@Unit
Scenario: Endpoint group base can have custom tags
    Given I have a simple endpoint group
    Then the group tags should be 'simple,test'

@Unit
Scenario: Endpoint group base has default empty authorization policy
    Given I have a minimal endpoint group
    Then the authorization policy should be empty

@Unit
Scenario: Endpoint group base can have a specific authorization policy
    Given I have a simple endpoint group
    Then the authorization policy should be 'RequireAdmin'

@Unit
Scenario: Endpoint group base maps to a route group builder
    Given I have a simple endpoint group
    And I have an endpoint route builder
    When I map the endpoint group
    Then the route group builder should be returned

@Unit
Scenario: Endpoint group base applies authorization when policy is specified
    Given I have a simple endpoint group with policy 'TestPolicy'
    And I have an endpoint route builder
    When I map the endpoint group
    Then the route group builder should have the authorization policy applied

@Unit
Scenario: Endpoint group base requires any authorization when policy is empty
    Given I have a minimal endpoint group
    And I have an endpoint route builder
    When I map the endpoint group
    Then the route group builder should be returned

@Unit
Scenario: Endpoint group base cannot be instantiated directly
    Then EndpointGroupBase cannot be instantiated directly
