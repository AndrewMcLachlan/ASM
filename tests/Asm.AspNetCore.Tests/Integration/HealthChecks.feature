Feature: Health Checks
    Health check endpoint with custom response writer

@Integration
Scenario: Health check endpoint returns JSON response
    Given I have a test web application
    When I make a GET request to '/healthz'
    Then the response status code should be 200
    And the response content type should be 'application/json'
    And the health check response should have status 'Healthy'
    And the health check response should have a checks array
    And the health check response should contain check 'test-check'
