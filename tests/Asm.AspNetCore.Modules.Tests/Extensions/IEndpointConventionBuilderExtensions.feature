Feature: IEndpointConventionBuilderExtensions
    As a developer
    I want to add names to endpoints with a simplified API
    So that I can set both the display name and operation name in one call

@Unit
Scenario: WithNames sets display name and converted name
    Given I have an endpoint
    When I set the names to 'Test Endpoint'
    Then the endpoint should have the names set

@Unit
Scenario: WithNames returns the builder for method chaining
    Given I have an endpoint
    When I set the names to 'Test Endpoint'
    And I call WithOpenApi
    Then the builder should be returned for chaining

@Unit
Scenario: WithNames works with simple display names
    Given I have an endpoint
    When I set the names to 'Hello'
    Then the endpoint should have the names set

@Unit
Scenario: WithNames works with multiple word display names
    Given I have an endpoint
    When I set the names to 'Update User Endpoint'
    Then the endpoint should have the names set

@Unit
Scenario Outline: WithNames works with different HTTP methods
    Given I have a '<Method>' endpoint at '<Route>'
    When I set the names to '<DisplayName>'
    Then the endpoint should have the names set

Examples:
    | Method | Route | DisplayName |
    | GET | /api/items | Get All Items |
    | POST | /api/items | Create New Item |
    | PUT | /api/items/{id} | Update Existing Item |
    | DELETE | /api/items/{id} | Delete Item |

@Unit
Scenario: WithNames works with route group builder
    Given I have a route group at '/api'
    When I set the names to 'API Group'
    Then the route group builder should have the names set
