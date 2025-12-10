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

Scenario: Copy throws when start index is out of range
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 10 to length 1
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

Scenario: Copy throws when length exceeds array size
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 0 to length 10
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

Scenario: CopyTo throws when index is negative
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    And I have another ExtendedBitArray with values [false, false, false, false]
    When I copy to the other array with index -1
    Then an exception of type 'System.ArgumentException' should be thrown

Scenario: CopyTo throws when index exceeds array length
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    And I have another ExtendedBitArray with values [false, false, false, false]
    When I copy to the other array with index 10
    Then an exception of type 'System.ArgumentException' should be thrown

Scenario: CopyTo throws when array is null
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy to null array with index 0
    Then an exception of type 'System.ArgumentNullException' should be thrown
