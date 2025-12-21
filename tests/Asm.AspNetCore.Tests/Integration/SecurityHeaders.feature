Feature: Security Headers
    Middleware that adds security headers to responses

@Integration
Scenario: Security headers are added to responses
    Given I have a test web application
    When I make a GET request to '/'
    Then the response should have header 'Referrer-Policy' with value 'no-referrer'
    And the response should have header 'Cross-Origin-Opener-Policy' with value 'same-origin-allow-popups'
    And the response should have header 'Cross-Origin-Embedder-Policy' with value 'require-corp'
    And the response should have header 'Cross-Origin-Resource-Policy' with value 'same-origin'
    And the response should have header 'X-Content-Type-Options' with value 'nosniff'
    And the response should have header 'X-Frame-Options' with value 'SAMEORIGIN'
    And the response should have header 'X-Permitted-Cross-Domain-Policies' with value 'none'
