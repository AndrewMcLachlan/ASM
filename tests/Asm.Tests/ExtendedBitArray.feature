Feature: ExtendedBitArray
  As a developer
  I want to test the ExtendedBitArray class
  So that I can ensure it behaves correctly

Scenario: Initialize ExtendedBitArray with BitArray
    Given I have a BitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I create an ExtendedBitArray
    Then the ExtendedBitArray should have 4 bits
    And the bits should be [true, false, true, false]

Scenario: Convert ExtendedBitArray to byte array
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I convert the ExtendedBitArray to a byte array
    Then the byte array should be [5]

Scenario: Copy part of ExtendedBitArray
    Given I have an ExtendedBitArray with values [true, false, true, false, true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 2 to length 4
    Then the new ExtendedBitArray should have 4 bits
    And the bits should be [true, false, true, false]
