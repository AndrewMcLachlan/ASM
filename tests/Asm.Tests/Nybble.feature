Feature: Nybble

@Unit
Scenario: Create a Nybble with a valid value
    Given I have a value 10
    When I create a Nybble
    Then the Nybble value should be 10

@Unit
Scenario: Create a Nybble with an invalid value
    Given I have a value 20
    When I create a Nybble
    Then an exception of type "System.OverflowException" should be thrown

@Unit
Scenario: Add two Nybbles
    Given I have a Nybble with value 5
    And I have another Nybble with value 3
    When I add the two Nybbles
    Then the byte result should be 83

@Unit
Scenario: Convert Nybble array to uint
    Given I have a Nybble array with values 1, 2, 3, 4
    When I convert the array to uint
    Then the uint result should be 0x1234

@Unit
Scenario: Check equality of two Nybbles
    Given I have a Nybble with value <Nybble>
    And I have another Nybble with value <Other Nybble>
    When I check equality
    Then the boolean result should be <Result>
Examples:
    | Nybble | Other Nybble | Result |
    | 5      | 5            | true   |
    | 5      | 3            | false  |
    | 3      | 5            | false  |

@Unit
Scenario: Convert byte to Nybbles
    Given I have a byte with value 0x12
    When I convert the byte to Nybbles
    Then the Nybble array should be 1, 2

@Unit
Scenario: Convert bytes to Nybbles
    Given I have a byte array with values 0x12, 0x34
    When I convert the byte array to Nybbles
    Then the Nybble array should be 1, 2, 3, 4

Scenario: Convert int to Nybbles
    Given I have an int with value 0x12
    When I convert the int to Nybbles
    Then the Nybble array should be 2, 1, 0, 0