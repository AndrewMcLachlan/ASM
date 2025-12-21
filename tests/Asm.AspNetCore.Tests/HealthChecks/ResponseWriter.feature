Feature: ResponseWriter
    Health check response writer that outputs JSON with status, version, and check details

@Unit
Scenario: Write healthy response
    Given I have a health report with status 'Healthy'
    And the report has no entries
    When I write the health response
    Then the response content type should contain 'application/json'
    And the response status should be 'Healthy'

@Unit
Scenario: Write unhealthy response
    Given I have a health report with status 'Unhealthy'
    And the report has no entries
    When I write the health response
    Then the response status should be 'Unhealthy'

@Unit
Scenario: Write degraded response
    Given I have a health report with status 'Degraded'
    And the report has no entries
    When I write the health response
    Then the response status should be 'Degraded'

@Unit
Scenario: Write response with health check entries
    Given I have a health report with status 'Healthy'
    And the report has an entry 'Database' with status 'Healthy' and description 'Connected'
    And the report has an entry 'Cache' with status 'Degraded' and description 'Slow response'
    When I write the health response
    Then the response should contain 2 checks
    And the response should contain a check named 'Database' with status 'Healthy'
    And the response should contain a check named 'Cache' with status 'Degraded'

@Unit
Scenario: Write response with entry data
    Given I have a health report with status 'Healthy'
    And the report has an entry 'API' with status 'Healthy' and data key 'endpoint' value 'https://api.example.com'
    When I write the health response
    Then the response should contain a check with data

@Unit
Scenario: Response includes total duration
    Given I have a health report with status 'Healthy' and duration 150 milliseconds
    And the report has no entries
    When I write the health response
    Then the response should include total duration
