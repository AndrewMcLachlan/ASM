Feature: Exception Handler
    Middleware that handles exceptions and returns ProblemDetails

@Integration
Scenario: Exception handler returns ProblemDetails for unhandled exceptions
    Given I have a test web application
    When I make a GET request to '/error'
    Then the response status code should be 500
    And the response content type should be 'application/json'
    And the response should contain ProblemDetails

@Integration
Scenario: Exception handler includes exception details in development
    Given I have a test web application
    When I make a GET request to '/error'
    Then the response should contain ProblemDetails with type
    And the response should contain ProblemDetails with detail containing 'Test exception'
