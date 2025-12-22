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

@Unit
Scenario: Map a paged query
    Given I have an IEndpointRouteBuilder
    When I map a paged query with pattern "/test-paged"
    Then the route should be mapped correctly

@Unit
Scenario: Map a DELETE request with response
    Given I have an IEndpointRouteBuilder
    When I map a DELETE command with response with pattern "/test-delete-response"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST command with response
    Given I have an IEndpointRouteBuilder
    When I map a POST command with response with pattern "/test-command"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST command without response
    Given I have an IEndpointRouteBuilder
    When I map a POST command without response with pattern "/test-command-empty"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PUT create command
    Given I have an IEndpointRouteBuilder
    When I map a PUT create command with pattern "/test-put-create" and route name "testPutRoute"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PATCH command
    Given I have an IEndpointRouteBuilder
    When I map a PATCH command with pattern "/test-patch"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PUT command with response
    Given I have an IEndpointRouteBuilder
    When I map a PUT command with response with pattern "/test-put"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PUT command without response
    Given I have an IEndpointRouteBuilder
    When I map a PUT command without response with pattern "/test-put-empty"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST create command with binding
    Given I have an IEndpointRouteBuilder
    When I map a POST create command with pattern "/test-create-binding", route name "testRoute" and binding "Body"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PUT create command with binding
    Given I have an IEndpointRouteBuilder
    When I map a PUT create command with pattern "/test-put-create-binding", route name "testPutRoute" and binding "Parameters"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST command with response and binding
    Given I have an IEndpointRouteBuilder
    When I map a POST command with response, pattern "/test-command-binding" and binding "Body"
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST command without response with custom status code
    Given I have an IEndpointRouteBuilder
    When I map a POST command without response with pattern "/test-command-status" and status code 202
    Then the route should be mapped correctly

@Unit
Scenario: Map a POST command without response with status code and binding
    Given I have an IEndpointRouteBuilder
    When I map a POST command without response with pattern "/test-command-status-binding", status code 202 and binding "Body"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PATCH command with binding
    Given I have an IEndpointRouteBuilder
    When I map a PATCH command with pattern "/test-patch-binding" and binding "Parameters"
    Then the route should be mapped correctly

@Unit
Scenario: Map a PUT command with binding
    Given I have an IEndpointRouteBuilder
    When I map a PUT command with pattern "/test-put-binding" and binding "Body"
    Then the route should be mapped correctly
