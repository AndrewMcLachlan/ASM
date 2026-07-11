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
    Then the byte result should be 8

@Unit
Scenario: Adding two Nybbles wraps modulo 16 on overflow
    Given I have a Nybble with value 15
    And I have another Nybble with value 15
    When I add the two Nybbles
    Then the byte result should be 14

@Unit
Scenario: Combine two Nybbles into a byte
    Given I have a Nybble with value 5
    And I have another Nybble with value 3
    When I combine the two Nybbles
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
    Given I have an int with value 0x12345678
    When I convert the int to Nybbles
    Then the full Nybble array should be 1, 2, 3, 4, 5, 6, 7, 8

@Unit
Scenario: Add uint and Nybble
    Given I have a uint value 5
    And I have a Nybble with value 3
    When I add the uint and Nybble
    Then the uint result should be 8

@Unit
Scenario: Append a Nybble to a uint
    Given I have a uint value 5
    And I have a Nybble with value 3
    When I append the Nybble to the uint
    Then the ulong result should be 83

@Unit
Scenario: Add byte and Nybble
    Given I have a byte value 5
    And I have a Nybble with value 3
    When I add the byte and Nybble
    Then the integer result should be 8

@Unit
Scenario: Append a Nybble to a byte
    Given I have a byte value 5
    And I have a Nybble with value 3
    When I append the Nybble to the byte
    Then the integer result should be 83

@Unit
Scenario: Check inequality of two Nybbles
    Given I have a Nybble with value 5
    And I have another Nybble with value 3
    When I check inequality
    Then the boolean result should be true

@Unit
Scenario: Check inequality returns false for equal Nybbles
    Given I have a Nybble with value 5
    And I have another Nybble with value 5
    When I check inequality
    Then the boolean result should be false

@Unit
Scenario: Get Nybble hash code
    Given I have a Nybble with value 10
    When I get the Nybble hash code
    Then the hash code should match the byte value hash code

@Unit
Scenario: Check Nybble equality with null object
    Given I have a Nybble with value 5
    When I check equality with null object
    Then the boolean result should be false

@Unit
Scenario: Check Nybble equality with non-Nybble object
    Given I have a Nybble with value 5
    When I check equality with a string object
    Then the boolean result should be false