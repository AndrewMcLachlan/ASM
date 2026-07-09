Feature: Validator Filter
    Endpoint filter that validates request parameters using FluentValidation

@Unit
Scenario: ValidatorFilter calls next when validation passes
    Given I have a valid test request
    When the validator filter is invoked
    Then the next delegate should be called
    And the result should be the expected response

@Unit
Scenario: ValidatorFilter throws when validation fails
    Given I have an invalid test request
    When the validator filter is invoked
    Then a validation exception should be thrown

@Unit
Scenario: ValidatorFilter returns 400 when the argument is missing
    Given I have a null test request
    When the validator filter is invoked
    Then the next delegate should not be called
    And a 400 result should be returned
