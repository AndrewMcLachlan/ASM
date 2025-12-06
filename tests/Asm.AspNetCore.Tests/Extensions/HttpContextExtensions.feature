Feature: HttpContext Extensions

@Unit
Scenario: GetUserName returns user name and preferred username when available
    Given I have an HttpContext with user claims
        | Type               | Value           |
        | name               | John Doe        |
        | preferred_username | john.doe@test.com |
    When I call GetUserName
    Then the string value 'John Doe (john.doe@test.com)' is returned

@Unit
Scenario: GetUserName returns dash when context is null
    Given I have a null HttpContext
    When I call GetUserName
    Then the string value '-' is returned

@Unit
Scenario: GetUserName returns dash when user is null
    Given I have an HttpContext with no user
    When I call GetUserName
    Then the string value '-' is returned

@Unit
Scenario: GetUserName returns dash when identity has no name claim
    Given I have an HttpContext with user claims
        | Type               | Value             |
        | preferred_username | john.doe@test.com |
    When I call GetUserName
    Then the string value '-' is returned

@Unit
Scenario: GetUserName returns dash when name claim is null
    Given I have an HttpContext with empty name claim
    When I call GetUserName
    Then the string value '-' is returned
