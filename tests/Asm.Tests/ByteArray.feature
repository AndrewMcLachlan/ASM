Feature: ByteArray
Scenario: Copy method should return correct sub-array
    Given a ByteArray with values 1, 2, 3, 4, 5 and big endian
    When I copy from index 1 with length 3
    Then the result should be a ByteArray with values 2, 3, 4

Scenario: ToCharArray method should convert to char array
    Given a ByteArray with values <Values> and <Endian> endian
    When I convert to char array
    Then the result should be a char array with values <Expected>
Examples:
    | Endian | Values   | Expected |
    | big    | 65,66,67 | A,B,C    |
    | little | 65,66,67 | C,B,A    |

Scenario: ToUInt16BE method should convert to UInt16
    Given a ByteArray with values 1, 2 and big endian
    When I convert to UInt16
    Then the ushort result should be 258

Scenario: ToUInt16LE method should convert to UInt16
    Given a ByteArray with values 1, 2 and little endian
    When I convert to UInt16
    Then the ushort result should be 513

Scenario: ToUInt32BE method should convert to UInt32
    Given a ByteArray with values 1, 2, 3, 4 and big endian
    When I convert to UInt32
    Then the uint result should be 16909060

Scenario: ToUInt32LE method should convert to UInt32
    Given a ByteArray with values 1, 2, 3, 4 and little endian
    When I convert to UInt32
    Then the uint result should be 67305985

Scenario: ToUInt64BE method should convert to UInt64
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and big endian
    When I convert to UInt64
    Then the ulong result should be 72623859790382856

Scenario: ToUInt64LE method should convert to UInt64
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8 and little endian
    When I convert to UInt64
    Then the ulong result should be 578437695752307201

Scenario: ToGuid method should convert to Guid
    Given a ByteArray with values 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 and big endian
    When I convert to Guid
    Then the GUID result should be "04030201-0605-0807-090a-0b0c0d0e0f10"

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
