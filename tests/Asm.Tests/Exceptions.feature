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
Scenario: Create NotFoundException with default constructor
    When I create a NotFoundException with the default constructor
    Then no exception is thrown

@Unit
Scenario: Create NotFoundException with message
    When I create a NotFoundException with message 'Item not found'
    Then no exception is thrown
    And the NotFoundException message is 'Item not found'

@Unit
Scenario: Create NotFoundException with message and inner exception
    Given I have an inner exception with message 'Database error'
    When I create a NotFoundException with message 'Item not found' and the inner exception
    Then no exception is thrown
    And the NotFoundException message is 'Item not found'
    And the NotFoundException has an inner exception with message 'Database error'

@Unit
Scenario: Create ExistsException with default constructor
    When I create an ExistsException with the default constructor
    Then no exception is thrown

@Unit
Scenario: Create ExistsException with message
    When I create an ExistsException with message 'Item already exists'
    Then no exception is thrown
    And the ExistsException message is 'Item already exists'

@Unit
Scenario: Create ExistsException with message and inner exception
    Given I have an inner exception with message 'Duplicate key'
    When I create an ExistsException with message 'Item already exists' and the inner exception
    Then no exception is thrown
    And the ExistsException message is 'Item already exists'
    And the ExistsException has an inner exception with message 'Duplicate key'

@Unit
Scenario: Create NotAuthorisedException with default constructor
    When I create a NotAuthorisedException with the default constructor
    Then no exception is thrown

@Unit
Scenario: Create NotAuthorisedException with message
    When I create a NotAuthorisedException with message 'Access denied'
    Then no exception is thrown
    And the NotAuthorisedException message is 'Access denied'

@Unit
Scenario: Create NotAuthorisedException with message and inner exception
    Given I have an inner exception with message 'Token expired'
    When I create a NotAuthorisedException with message 'Access denied' and the inner exception
    Then no exception is thrown
    And the NotAuthorisedException message is 'Access denied'
    And the NotAuthorisedException has an inner exception with message 'Token expired'

@Unit
Scenario: Create NotAuthorisedException with message and type
    When I create a NotAuthorisedException with message 'Access denied' and type 'System.String'
    Then no exception is thrown
    And the NotAuthorisedException message is 'Access denied'

@Unit
Scenario: Create NotAuthorisedException with message, type and state
    When I create a NotAuthorisedException with message 'Access denied', type 'System.String' and state 'ReadOnly'
    Then no exception is thrown
    And the NotAuthorisedException message is 'Access denied'

@Unit
Scenario: Throw and catch NotFoundException
    When I throw a NotFoundException with message 'Resource not found'
    Then an exception of type 'Asm.NotFoundException, Asm' should be thrown
    And the exception message is 'Resource not found'

@Unit
Scenario: Throw and catch ExistsException
    When I throw an ExistsException with message 'Resource exists'
    Then an exception of type 'Asm.ExistsException, Asm' should be thrown
    And the exception message is 'Resource exists'

@Unit
Scenario: Throw and catch NotAuthorisedException
    When I throw a NotAuthorisedException with message 'Not authorised'
    Then an exception of type 'Asm.NotAuthorisedException, Asm' should be thrown
    And the exception message is 'Not authorised'
