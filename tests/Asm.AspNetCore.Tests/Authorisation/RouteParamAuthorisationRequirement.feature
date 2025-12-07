Feature: RouteParamAuthorisationRequirement

@Unit
Scenario: Create RouteParamAuthorisationRequirement with name
    When I create a RouteParamAuthorisationRequirement with name 'accountId'
    Then the requirement name should be 'accountId'

@Unit
Scenario: Create RouteParamAuthorisationRequirement with different name
    When I create a RouteParamAuthorisationRequirement with name 'userId'
    Then the requirement name should be 'userId'

@Unit
Scenario: Create RouteParamAuthorisationRequirement with empty name
    When I create a RouteParamAuthorisationRequirement with name ''
    Then the requirement name should be ''
