Feature: Route Parameter Authorisation
    Authorisation handlers that check route parameters

@Unit
Scenario: RouteParamAuthorisationHandler succeeds when route parameter exists and is authorised
    Given I have an HttpContext with route parameter 'id' value '123'
    And I have an authorised route param handler
    When the route param handler handles the requirement
    Then the authorisation should succeed

@Unit
Scenario: RouteParamAuthorisationHandler fails when route parameter is not authorised
    Given I have an HttpContext with route parameter 'id' value '999'
    And I have an unauthorised route param handler
    When the route param handler handles the requirement
    Then the authorisation should fail

@Unit
Scenario: RouteParamAuthorisationHandler fails when route parameter is missing
    Given I have an HttpContext with no route parameters
    And I have an authorised route param handler
    When the route param handler handles the requirement
    Then the authorisation should fail
