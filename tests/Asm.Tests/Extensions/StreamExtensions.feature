Feature: StreamExtensions
    As a developer
    I want to test the Stream extension methods
    So that I can ensure they work correctly with long offsets and counts

@Unit
Scenario: Read from stream with long parameters
    Given I have a MemoryStream with data "Hello World"
    And I have a read buffer of size 11
    When I read 11 bytes into buffer offset 0 using long parameters
    Then the read buffer should contain "Hello World"

@Unit
Scenario: Read from stream with buffer offset
    Given I have a MemoryStream with data "Hello"
    And I have a read buffer of size 10
    When I read 5 bytes into buffer offset 2 using long parameters
    Then the read buffer at offset 2 with length 5 should be "Hello"

@Unit
Scenario: Write to stream from buffer with long parameters
    Given I have an empty MemoryStream
    And I have a write buffer with data "Hello World"
    When I write 5 bytes from buffer offset 0 using long parameters
    Then the stream should contain "Hello"

@Unit
Scenario: Write to stream from buffer offset
    Given I have an empty MemoryStream
    And I have a write buffer with data "Hello World"
    When I write 5 bytes from buffer offset 6 using long parameters
    Then the stream should contain "World"

@Unit
Scenario: Read throws when stream is null
    Given I have a null stream
    When I try to read from the null stream
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: Write throws when stream is null
    Given I have a null stream
    When I try to write to the null stream
    Then an exception of type 'System.ArgumentNullException' should be thrown
