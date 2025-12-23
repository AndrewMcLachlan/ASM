Feature: ExtendedBitArray
  As a developer
  I want to test the ExtendedBitArray class
  So that I can ensure it behaves correctly

@Unit
Scenario: Initialize ExtendedBitArray with BitArray
    Given I have a BitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I create an ExtendedBitArray
    Then the ExtendedBitArray should have 4 bits
    And the bits should be [true, false, true, false]

@Unit
Scenario: Convert ExtendedBitArray to byte array
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I convert the ExtendedBitArray to a byte array
    Then the byte array should be [5]

@Unit
Scenario: Copy part of ExtendedBitArray
    Given I have an ExtendedBitArray with values [true, false, true, false, true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 2 to length 4
    Then the new ExtendedBitArray should have 4 bits
    And the bits should be [true, false, true, false]

@Unit
Scenario: Copy throws when start index is out of range
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 10 to length 1
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Copy throws when length exceeds array size
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy from index 0 to length 10
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: CopyTo throws when index is negative
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    And I have another ExtendedBitArray with values [false, false, false, false]
    When I copy to the other array with index -1
    Then an exception of type 'System.ArgumentException' should be thrown

@Unit
Scenario: CopyTo throws when index exceeds array length
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    And I have another ExtendedBitArray with values [false, false, false, false]
    When I copy to the other array with index 10
    Then an exception of type 'System.ArgumentException' should be thrown

@Unit
Scenario: CopyTo throws when array is null
    Given I have an ExtendedBitArray with values [true, false, true, false]
    And the endianness is LittleEndian
    When I copy to null array with index 0
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: Initialize ExtendedBitArray with byte array
    Given a byte array with values [5]
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from bytes
    Then the ExtendedBitArray should have 8 bits

@Unit
Scenario: Initialize ExtendedBitArray with single byte
    Given a single byte with value 170
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from single byte
    Then the ExtendedBitArray should have 8 bits

@Unit
Scenario: Initialize ExtendedBitArray with sbyte
    Given a signed byte with value 127
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from sbyte
    Then the ExtendedBitArray should have 8 bits

@Unit
Scenario: Initialize ExtendedBitArray with short
    Given a short with value 256
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from short
    Then the ExtendedBitArray should have 16 bits

@Unit
Scenario: Initialize ExtendedBitArray with int
    Given an int with value 65536
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from int
    Then the ExtendedBitArray should have 32 bits

@Unit
Scenario: Initialize ExtendedBitArray with long
    Given a long with value 4294967296
    And the endianness is LittleEndian
    When I create an ExtendedBitArray from long
    Then the ExtendedBitArray should have 64 bits

@Unit
Scenario: Initialize ExtendedBitArray with ReadOnlySpan of bytes
    Given a byte span with values [1, 2]
    And the endianness is BigEndian
    When I create an ExtendedBitArray from byte span
    Then the ExtendedBitArray should have 16 bits

@Unit
Scenario: Convert ExtendedBitArray to SByte
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, true, true, true, true, true, true, true]
    When I convert the ExtendedBitArray to SByte
    Then the sbyte result should be -1

@Unit
Scenario: Convert ExtendedBitArray to Int16 little endian
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to Int16
    Then the short result should be 1

@Unit
Scenario: Convert ExtendedBitArray to Int32 little endian
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to Int32
    Then the int result should be 1

@Unit
Scenario: Convert ExtendedBitArray to Int64 little endian
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to Int64
    Then the long result should be 1

@Unit
Scenario: Convert ExtendedBitArray to Byte
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false, false, false, false, false]
    When I convert the ExtendedBitArray to Byte
    Then the byte result should be 5

@Unit
Scenario: Convert ExtendedBitArray to UInt16
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to UInt16
    Then the ushort result should be 257

@Unit
Scenario: Convert ExtendedBitArray to UInt32
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to UInt32
    Then the uint result should be 65537

@Unit
Scenario: Convert ExtendedBitArray to UInt64
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false]
    When I convert the ExtendedBitArray to UInt64
    Then the ulong result should be 4294967297

@Unit
Scenario: Convert ExtendedBitArray to string
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false]
    When I convert the ExtendedBitArray to string
    Then the string representation should contain binary digits

@Unit
Scenario: Access IsSynchronized property
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false]
    When I access the IsSynchronized property
    Then the result should not be null

@Unit
Scenario: Access SyncRoot property
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false]
    When I access the SyncRoot property
    Then the result should not be null

@Unit
Scenario: Clone ExtendedBitArray
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false]
    When I clone the ExtendedBitArray
    Then the result should not be null

@Unit
Scenario: Get IEnumerable enumerator
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false]
    When I get the enumerator for the ExtendedBitArray
    Then the result should not be null

@Unit
Scenario: Access bits using range indexer
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, false, true, false, true, true, false, false]
    When I access bits from index 2 to 5
    Then the range result should be [true, false, true]

@Unit
Scenario: CopyTo ExtendedBitArray successfully
    Given the endianness is LittleEndian
    And I have an ExtendedBitArray with values [true, true]
    And I have another ExtendedBitArray with values [false, false, false, false]
    When I copy to the other array with index 1
    Then the operation should complete without exception
