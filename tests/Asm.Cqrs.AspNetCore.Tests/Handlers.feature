Feature: Handlers

@Unit
Scenario: HandleQuery dispatches query and returns Ok result
    Given I have a query dispatcher that returns 'test result'
    When I invoke the query handler
    Then the result should be Ok with value 'test result'

@Unit
Scenario: HandlePagedQuery dispatches query and returns results with header
    Given I have a query dispatcher that returns a paged result with 100 total items
    And I have an HttpContext
    When I invoke the paged query handler
    Then the result should be Ok
    And the X-Total-Count header should be '100'

@Unit
Scenario: Delete command handler returns NoContent
    Given I have a command dispatcher
    When I invoke the delete handler
    Then the result should be NoContent

@Unit
Scenario: Delete command handler with response returns Ok with result
    Given I have a command dispatcher that returns 'deleted item'
    When I invoke the delete handler with response
    Then the result should be Ok with value 'deleted item'

@Unit
Scenario: Command handler with response returns Ok with result
    Given I have a command dispatcher that returns 42
    When I invoke the command handler with response
    Then the result should be Ok with int value 42

@Unit
Scenario: Command handler with response honours a custom status code
    Given I have a command dispatcher that returns 42
    When I invoke the command handler with response and status code 202
    Then the result should have status code 202

@Unit
Scenario: Void command handler executes the command
    Given I have a command dispatcher
    When I invoke the void command handler
    Then the command should be executed

@Unit
Scenario: CreateCreateHandler returns CreatedAtRoute result
    Given I have a command dispatcher that returns a created item with id 123
    When I invoke the create handler with route name 'GetItem'
    Then the result should be CreatedAtRoute

@Unit
Scenario Outline: Void command handler respects binding options
    Given I have a command dispatcher
    When I invoke the void command handler with binding '<Binding>'
    Then the command should be executed
    And the handler should use '<Binding>' binding

Examples:
    | Binding    |
    | None       |
    | Body       |
    | Parameters |

@Unit
Scenario Outline: Command handler with response respects binding options
    Given I have a command dispatcher that returns 42
    When I invoke the command handler with response and binding '<Binding>'
    Then the result should be Ok with int value 42
    And the handler should use '<Binding>' binding

Examples:
    | Binding    |
    | None       |
    | Body       |
    | Parameters |

@Unit
Scenario Outline: CreateCreateHandler respects binding options
    Given I have a command dispatcher that returns a created item with id 456
    When I invoke the create handler with route name 'GetItem' and binding '<Binding>'
    Then the result should be CreatedAtRoute
    And the handler should use '<Binding>' binding

Examples:
    | Binding    |
    | None       |
    | Body       |
    | Parameters |
