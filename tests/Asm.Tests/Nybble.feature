Feature: Nybble
  Scenario: Create a Nybble with a valid value
    Given I have a value 10
    When I create a Nybble
    Then the Nybble value should be 10

  Scenario: Create a Nybble with an invalid value
    Given I have a value 20
    When I create a Nybble
    Then an exception of type "System.OverflowException" should be thrown

  Scenario: Add two Nybbles
    Given I have a Nybble with value 5
    And I have another Nybble with value 3
    When I add the two Nybbles
    Then the byte result should be 83

  Scenario: Convert Nybble array to uint
    Given I have a Nybble array with values 1, 2, 3, 4
    When I convert the array to uint
    Then the uint result should be 0x1234

  Scenario: Check equality of two Nybbles
    Given I have a Nybble with value 7
    And I have another Nybble with value 7
    When I check equality
    Then the boolean result should be true
