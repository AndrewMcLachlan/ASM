Feature: HttpContextPrincipalProvider

@Unit
Scenario: Returns principal from HttpContext when available
    Given I have an HttpContextAccessor with a user principal
    When I get the principal from HttpContextPrincipalProvider
    Then the principal should not be null
    And the principal should have the expected identity

@Unit
Scenario: Returns null when HttpContext is null
    Given I have an HttpContextAccessor with no HttpContext
    When I get the principal from HttpContextPrincipalProvider
    Then the principal should be null

@Unit
Scenario: Returns null when HttpContext User is null
    Given I have an HttpContextAccessor with HttpContext but no user
    When I get the principal from HttpContextPrincipalProvider
    Then the principal should be null
