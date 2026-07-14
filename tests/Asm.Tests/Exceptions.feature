Feature: Exceptions

@Unit
Scenario: Create AsmException with default constructor
    When I create an AsmException with the default constructor
    Then no exception is thrown
    And the AsmException has a unique Id
    And the AsmException ErrorId is 0

@Unit
Scenario: Create AsmException with message
    When I create an AsmException with message 'Test error message'
    Then no exception is thrown
    And the AsmException has a unique Id
    And the AsmException message is 'Test error message'

@Unit
Scenario: Create AsmException with message and inner exception
    Given I have an inner exception with message 'Inner error'
    When I create an AsmException with message 'Outer error' and the inner exception
    Then no exception is thrown
    And the AsmException message is 'Outer error'
    And the AsmException has an inner exception with message 'Inner error'

@Unit
Scenario: Create AsmException with error id
    When I create an AsmException with error id 42
    Then no exception is thrown
    And the AsmException ErrorId is 42

@Unit
Scenario: Create AsmException with message and error id
    When I create an AsmException with message 'Test error' and error id 99
    Then no exception is thrown
    And the AsmException message is 'Test error'
    And the AsmException ErrorId is 99

@Unit
Scenario: Create AsmException with message, error id and inner exception
    Given I have an inner exception with message 'Inner error'
    When I create an AsmException with message 'Outer error', error id 100 and the inner exception
    Then no exception is thrown
    And the AsmException message is 'Outer error'
    And the AsmException ErrorId is 100
    And the AsmException has an inner exception with message 'Inner error'

@Unit
Scenario: AsmException is abstract
    Then AsmException is abstract

@Unit
Scenario: An Asm library exception type resolves by its full name
    When I catch an Asm NotFoundException
    Then an exception of type 'Asm.NotFoundException' is thrown

@Unit
Scenario: CatchException can be called more than once in a scenario
    When I catch an exception then catch another
    Then an exception of type 'System.InvalidOperationException' is thrown
