Feature: Handlers

@Unit
Scenario: HandleQuery dispatches query and returns Ok result
    Given I have a query dispatcher that returns 'test result'
    When I invoke HandleQuery with the query
    Then the result should be Ok with value 'test result'

@Unit
Scenario: HandlePagedQuery dispatches query and returns results with header
    Given I have a query dispatcher that returns a paged result with 100 total items
    And I have an HttpContext
    When I invoke HandlePagedQuery with the query
    Then the result should be Ok
    And the X-Total-Count header should be '100'

@Unit
Scenario: HandleDelete dispatches command and returns NoContent
    Given I have a command dispatcher
    When I invoke HandleDelete with a delete command
    Then the result should be NoContent

@Unit
Scenario: HandleDelete with response dispatches command and returns Ok with result
    Given I have a command dispatcher that returns 'deleted item'
    When I invoke HandleDelete with response with the command
    Then the result should be Ok with value 'deleted item'

@Unit
Scenario: HandleCommand dispatches command and returns result
    Given I have a command dispatcher that returns 42
    When I invoke HandleCommand with the command
    Then the command result should be 42

@Unit
Scenario: HandleCommand without response dispatches command
    Given I have a command dispatcher
    When I invoke HandleCommand without response
    Then the command should be dispatched

@Unit
Scenario: CreateCreateHandler returns CreatedAtRoute result
    Given I have a command dispatcher that returns a created item with id 123
    When I invoke CreateCreateHandler with route name 'GetItem' and the command
    Then the result should be CreatedAtRoute

@Unit
Scenario: CreateCommandHandler returns specified status code
    Given I have a command dispatcher
    When I invoke CreateCommandHandler with status code 202
    Then the result should have status code 202

@Unit
Scenario: CreateCommandHandler with response returns specified status code
    Given I have a command dispatcher that returns 'processed'
    When I invoke CreateCommandHandler with response and status code 200
    Then the result should have status code 200
