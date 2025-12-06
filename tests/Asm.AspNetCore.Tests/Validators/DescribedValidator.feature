Feature: DescribedValidator

@Unit
Scenario: Valid described object passes validation
    Given I have a described object with name 'Test Item' and description 'A test description'
    When I validate the described object
    Then the validation should pass

@Unit
Scenario: Empty name fails validation
    Given I have a described object with name '' and description 'A test description'
    When I validate the described object
    Then the validation should fail
    And the validation error should be for property 'Name'

@Unit
Scenario: Null name fails validation
    Given I have a described object with null name and description 'A test description'
    When I validate the described object
    Then the validation should fail
    And the validation error should be for property 'Name'

@Unit
Scenario: Name exceeding default max length fails validation
    Given I have a described object with name exceeding 50 characters and description 'A test description'
    When I validate the described object
    Then the validation should fail
    And the validation error should be for property 'Name'

@Unit
Scenario: Description exceeding default max length fails validation
    Given I have a described object with name 'Test' and description exceeding 255 characters
    When I validate the described object
    Then the validation should fail
    And the validation error should be for property 'Description'

@Unit
Scenario: Null description passes validation
    Given I have a described object with name 'Test Item' and null description
    When I validate the described object
    Then the validation should pass

@Unit
Scenario: Custom length validator allows longer name
    Given I have a described object with name exceeding 50 characters and description 'A test description'
    And I use a custom validator with name length 100 and description length 500
    When I validate the described object with custom validator
    Then the validation should pass
