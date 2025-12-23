Feature: RouteHandlerBuilderExtensions
    Extension methods for RouteHandlerBuilder

@Integration
Scenario: WithValidation allows valid data through
    Given I have a test web application with validation
    When I make a POST request to '/validate' with valid data
    Then the validation response status code should be 200
    And the response should contain the request data
