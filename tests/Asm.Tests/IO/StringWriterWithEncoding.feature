Feature: StringWriterWithEncoding

@Unit
Scenario: StringWriterWithEncoding uses specified encoding
    Given I create a StringWriterWithEncoding with UTF8 encoding
    Then the Encoding property should be UTF8

@Unit
Scenario: StringWriterWithEncoding uses ASCII encoding
    Given I create a StringWriterWithEncoding with ASCII encoding
    Then the Encoding property should be ASCII

@Unit
Scenario: StringWriterWithEncoding writes text correctly
    Given I create a StringWriterWithEncoding with UTF8 encoding
    When I write 'Hello World' to the writer
    Then the written content should be 'Hello World'
