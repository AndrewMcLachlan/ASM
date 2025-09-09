Feature: IEndpointRouteBuilderExtensions
  As a developer
  I want to test the IEndpointRouteBuilderExtensions class
  So that I can ensure it behaves correctly

@Unit
Scenario: Map a request to a query
    Given I have an IEndpointRouteBuilder
    When I map a query with pattern "/test-query"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST request to a command that creates a resource
    Given I have an IEndpointRouteBuilder
    When I map a POST create command with pattern "/test-create" and route name "testRoute"
    Then the route should be mapped correctly

@Unit
Scenario: Map a DELETE request to a command
    Given I have an IEndpointRouteBuilder
    When I map a DELETE command with pattern "/test-delete"
    Then the route should be mapped correctly
