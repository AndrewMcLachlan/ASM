Feature: ByteArray

@Unit
Scenario: Copy method should return correct sub-array
    Given a ByteArray with values 1, 2, 3, 4, 5 and big endian
    When I copy from index 1 with length 3
    Then the result should be a ByteArray with values 2, 3, 4

@Unit
Scenario: Copy method throws when start index is out of range
    Given a ByteArray with values 1, 2, 3, 4, 5 and big endian
    When I copy from index 10 with length 1
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: Copy method throws when length exceeds array size
    Given a ByteArray with values 1, 2, 3, 4, 5 and big endian
    When I copy from index 0 with length 10
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown

@Unit
Scenario: ToCharArray method should convert to char array
    Given a ByteArray with values <Values> and <Endian> endian
    When I convert to char array
    Then the result should be a char array with values <Expected>
Examples:
    | Endian | Values   | Expected |
    | big    | 65,66,67 | A,B,C    |
    | little | 65,66,67 | C,B,A    |

@Unit
Scenario: ToUInt16BE method should convert to UInt16
    Given a ByteArray with values 1, 2 and big endian
    When I convert to UInt16
    Then the ushort result should be 258

@Unit
Scenario: ToUInt16LE method should convert to UInt16
    Given a ByteArray with values 1, 2 and little endian
    When I convert to UInt16
    Then the ushort result should be 513

@Unit
Scenario: ToUInt16 throws OverflowException when array is too large
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I convert to UInt16
    Then an exception of type 'System.OverflowException' should be thrown

@Unit
Scenario: ToUInt32BE method should convert to UInt32
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I convert to UInt32
    Then the uint result should be 16909060

@Unit
Scenario: ToUInt32LE method should convert to UInt32
    Given a ByteArray with values 1, 2, 3, 4 and little endian
    When I convert to UInt32
    Then the uint result should be 67305985

@Unit
Scenario: ToUInt32 throws OverflowException when array is too large
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and big endian
    When I convert to UInt32
    Then an exception of type 'System.OverflowException' should be thrown

@Unit
Scenario: ToUInt64BE method should convert to UInt64
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and big endian
    When I convert to UInt64
    Then the ulong result should be 72623859790382856

@Unit
Scenario: ToUInt64LE method should convert to UInt64
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and little endian
    When I convert to UInt64
    Then the ulong result should be 578437695752307201

@Unit
Scenario: ToUInt64 throws OverflowException when array is too large
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 and big endian
    When I convert to UInt64
    Then an exception of type 'System.OverflowException' should be thrown

@Unit
Scenario: ToGuid method should convert to Guid
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 and big endian
    When I convert to Guid
    Then the GUID result should be "04030201-0605-0807-090a-0b0c0d0e0f10"

@Unit
Scenario: ToGuid throws when array is too large
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 and big endian
    When I convert to Guid
    Then an exception of type 'System.InvalidOperationException' should be thrown

@Unit
Scenario: ToGuid throws when array is too small
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 and big endian
    When I convert to Guid
    Then an exception of type 'System.InvalidOperationException' should be thrown

@Unit
Scenario: Check equality
    Given a ByteArray with values <Values 1> and <Endian 1> endian
    And another ByteArray with values <Values 2> and <Endian 2> endian
    When I check for equality
    Then the boolean result should be <Result>

Examples:
    | Values 1 | Endian 1 | Values 2 | Endian 2 | Result |
    | 1,2,3,4  | big      | 1,2,3,4  | big      | true   |
    | 1,2,3,4  | little   | 1,2,3,4  | little   | true   |
    | 1,2,3,4  | big      | 1,2,3,4  | little   | false  |
    | 1,2,3,4  | little   | 1,2,3,4  | big      | false  |
    | 1,2,3,4  | big      | 1,2,3,5  | big      | false  |

@Unit
Scenario: Check equality with null object returns false
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I check equality with null
    Then the boolean result should be false

@Unit
Scenario: Check equality with incompatible object returns false
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I check equality with incompatible object
    Then the boolean result should be false

@Unit
Scenario: ToInt16 method should convert to Int16
    Given a ByteArray with values 1, 2 and little endian
    When I convert to Int16
    Then the short result should be 513

@Unit
Scenario: ToInt32 method should convert to Int32
    Given a ByteArray with values 1, 2, 3, 4 and little endian
    When I convert to Int32
    Then the int result should be 67305985

@Unit
Scenario: ToInt64 method should convert to Int64
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and little endian
    When I convert to Int64
    Then the long result should be 578437695752307201

@Unit
Scenario: ToString method should convert to string
    Given a ByteArray with values 65, 66, 67 and big endian
    When I convert to string
    Then the string result should be "ABC"

@Unit
Scenario: Check inequality returns true for different arrays
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    And another ByteArray with values 1, 2, 3, 5 and big endian
    When I check for inequality
    Then the boolean result should be true

@Unit
Scenario: Check inequality returns false for same arrays
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    And another ByteArray with values 1, 2, 3, 4 and big endian
    When I check for inequality
    Then the boolean result should be false

@Unit
Scenario: Indexer get returns correct byte
    Given a ByteArray with values 10, 20, 30 and big endian
    When I access index 1
    Then the byte result should be 20

@Unit
Scenario: GetHashCode returns a value
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I get the hash code
    Then the result should not be null

@Unit
Scenario: Implicit conversion from byte array
    Given a raw byte array with values 1, 2, 3
    When I implicitly convert to ByteArray
    Then the ByteArray should have 3 bytes

@Unit
Scenario: Implicit conversion to byte array
    Given a ByteArray with values 1, 2, 3 and big endian
    When I implicitly convert to byte array
    Then the byte array should have 3 bytes
