Feature: ProblemDetailsFactory

@Unit
Scenario: CreateProblemDetails returns basic problem details when no error context
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with no error
    When I create problem details with status 500 and title 'Server Error'
    Then the problem details should have status 500
    And the problem details should have title 'Server Error'

@Unit
Scenario: CreateProblemDetails handles NotFoundException
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with a NotFoundException 'Item not found'
    When I create problem details
    Then the problem details should have status 404
    And the problem details should have title 'Not found'
    And the problem details should have detail 'Item not found'

@Unit
Scenario: CreateProblemDetails handles ExistsException
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with an ExistsException 'Item already exists'
    When I create problem details
    Then the problem details should have status 409
    And the problem details should have title 'Already exists'
    And the problem details should have detail 'Item already exists'

@Unit
Scenario: CreateProblemDetails handles NotAuthorisedException
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with a NotAuthorisedException 'Access denied'
    When I create problem details
    Then the problem details should have status 403
    And the problem details should have title 'Forbidden'
    And the problem details should have detail 'Access denied'

@Unit
Scenario: CreateProblemDetails handles BadHttpRequestException
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with a BadHttpRequestException 'Invalid request'
    When I create problem details
    Then the problem details should have status 400
    And the problem details should have title 'Bad request'

@Unit
Scenario: CreateProblemDetails handles InvalidOperationException
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with an InvalidOperationException 'Invalid operation'
    When I create problem details
    Then the problem details should have status 400
    And the problem details should have title 'Bad request'

@Unit
Scenario: CreateProblemDetails handles AsmException with error code
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with an AsmException 'Custom error' and error id 42
    When I create problem details
    Then the problem details should have status 500
    And the problem details should have title 'Unexpected error occurred'
    And the problem details should have extension 'Code' with value 42

@Unit
Scenario: CreateProblemDetails handles unknown exception in development
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with an unknown exception 'Something went wrong'
    When I create problem details
    Then the problem details should have status 500
    And the problem details should have title 'Unexpected error occurred'
    And the problem details should contain exception details

@Unit
Scenario: CreateProblemDetails handles unknown exception in production
    Given I have a ProblemDetailsFactory with production environment
    And I have an HttpContext with an unknown exception 'Something went wrong'
    When I create problem details
    Then the problem details should have status 500
    And the problem details should have title 'Unexpected error occurred'
    And the problem details should not contain exception details

@Unit
Scenario: CreateValidationProblemDetails returns validation problem details
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with no error
    And I have a ModelStateDictionary with error 'Name' 'Name is required'
    When I create validation problem details
    Then the validation problem details should have status 400
    And the validation problem details should contain error for 'Name'

@Unit
Scenario: CreateValidationProblemDetails with custom title
    Given I have a ProblemDetailsFactory with development environment
    And I have an HttpContext with no error
    And I have a ModelStateDictionary with error 'Name' 'Name is required'
    When I create validation problem details with title 'Custom Validation Error'
    Then the validation problem details should have title 'Custom Validation Error'
