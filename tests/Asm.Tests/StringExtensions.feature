Feature: String Extensions

@Unit
Scenario Outline: Append
    Given I have a string '<string>'
    And I have a separator ','
    When I Append '<append>' to the string
    Then the string value '<value>' is returned

Examples:
    | string | append | value       |
    |        | Hello  | Hello       |
    | Hello  | World  | Hello,World |

@Unit
Scenario Outline: Prepend
    Given I have a string '<string>'
    And I have a separator ','
    When I Prepend '<prepend>' to the string
    Then the string value '<value>' is returned

Examples:
    | string | prepend | value       |
    |        | Hello   | Hello       |
    | Hello  | World   | World,Hello |

@Unit
Scenario: Squish
    Given I have a string 'Hello World'
    When I Squish the string by 2 characters
    Then the string value 'llo Wor' is returned

@Unit
Scenario: Squish with negative fromStart throws ArgumentOutOfRangeException
    Given I have a string 'Hello World'
    When I Squish the string with fromStart -1 and fromEnd 0
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Squish with fromStart exceeding length throws ArgumentOutOfRangeException
    Given I have a string 'Hello'
    When I Squish the string with fromStart 10 and fromEnd 0
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Squish with negative fromEnd throws ArgumentOutOfRangeException
    Given I have a string 'Hello World'
    When I Squish the string with fromStart 0 and fromEnd -1
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Squish with fromEnd exceeding length throws ArgumentOutOfRangeException
    Given I have a string 'Hello'
    When I Squish the string with fromStart 0 and fromEnd 10
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Squish with sum exceeding length throws InvalidOperationException
    Given I have a string 'Hello'
    When I Squish the string with fromStart 3 and fromEnd 3
    Then an exception of type 'System.InvalidOperationException' should be thrown

@Unit
Scenario: Convert string to title case
    Given I have a string 'hello world'
    When I convert the string to title case
    Then the string value 'Hello World' is returned

@Unit
Scenario Outline: ToMachine
    Given I have a string '<string>'
    When I convert the string to machine format
    Then the string value '<value>' is returned

Examples:
    | string                                   | value       |
    | Hello, World                             | hello-world |
    | Hello & World                            | hello-world |
    | Hello- !"£$%^&*()[]{}<>,./?;:'@#~\\World | hello-world |