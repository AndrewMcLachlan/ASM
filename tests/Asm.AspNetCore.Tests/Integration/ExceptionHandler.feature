Feature: Exception Handler
    Middleware that handles exceptions and returns ProblemDetails

@Integration
Scenario: Exception handler returns ProblemDetails for unhandled exceptions
    Given I have a test web application
    When I make a GET request to '/error'
    Then the response status code should be 500
    And the response content type should be 'application/problem+json'
    And the response should contain ProblemDetails

@Integration
Scenario: Exception handler includes exception details in development
    Given I have a test web application
    When I make a GET request to '/error'
    Then the response should contain ProblemDetails with type
    And the response should contain ProblemDetails with detail containing 'Test exception'

@Integration
Scenario: Exception handler returns 400 with field errors for validation exceptions
    Given I have a test web application
    When I make a GET request to '/validation-error'
    Then the response status code should be 400
    And the response content type should be 'application/problem+json'
    And the response should contain validation error for 'First' with message 'is required'
    And the response should contain validation error for 'Second' with message 'is required'
