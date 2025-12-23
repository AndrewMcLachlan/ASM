Feature: SerilogEnrichWithUserMiddleware
    Middleware that enriches Serilog log context with the current user

@Unit
Scenario: Middleware invokes next delegate
    Given I have a SerilogEnrichWithUserMiddleware
    And I have an HttpContext with user 'testuser'
    When I invoke the middleware
    Then the next delegate should have been called

@Unit
Scenario: Middleware handles anonymous user
    Given I have a SerilogEnrichWithUserMiddleware
    And I have an HttpContext without a user for the middleware
    When I invoke the middleware
    Then the next delegate should have been called
