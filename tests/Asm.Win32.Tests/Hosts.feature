Feature: Hosts File Parsing

@Unit
Scenario: Parse hosts file from stream with various entry types
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then the hosts file should have 8 entries
    And entry 0 should be a Comment
    And entry 1 should be a Blank
    And entry 2 should be a Comment
    And entry 3 should be a Blank
    And entry 4 should be an IPv4Entry
    And entry 5 should be an IPv4Entry
    And entry 6 should be an IPv4Entry
    And entry 7 should be an IPv4EntryCommented

@Unit
Scenario: Parse comment entry correctly
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then entry 0 should have comment "##Comment###"
    And entry 2 should have comment " Comment with space"

@Unit
Scenario: Parse localhost entry correctly
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then entry 4 should have address "127.0.0.1"
    And entry 4 should have alias "localhost"

@Unit
Scenario: Parse entry with comment correctly
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then entry 5 should have address "127.0.0.1"
    And entry 5 should have alias "dave"
    And entry 5 should have comment "With comment"

@Unit
Scenario: Parse entry with hash in comment correctly
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then entry 6 should have address "127.0.0.1"
    And entry 6 should have alias "commented"
    And entry 6 should have comment "#With comment"

@Unit
Scenario: Parse commented entry correctly
    Given I have a hosts file stream with standard entries
    When I create a Hosts instance from the stream
    Then entry 7 should have address "127.0.0.1"
    And entry 7 should have alias "commented"
    And entry 7 should have comment "#With comment"
    And entry 7 should be commented

@Unit
Scenario: Invalid entry throws FormatException
    Given I have a hosts file stream with an invalid entry
    When I create a Hosts instance from the invalid stream
    Then an exception of type 'System.FormatException' is thrown

@Integration
Scenario: Load hosts file from file path
    Given I have a mock hosts file on disk
    When I create a Hosts instance from the file path
    Then the hosts file should have 2 entries
    And entry 0 should be an IPv4Entry
    And entry 0 should have address "127.0.0.1"
    And entry 0 should have alias "localhost"

@Integration
Scenario: Static Current property loads system hosts file
    Given I have a mock hosts file configured as the system hosts file
    When I access the Current hosts instance
    Then the hosts file should have 2 entries
    And entry 0 should have alias "localhost"

@Integration
Scenario: Refresh reloads hosts file from disk
    Given I have a mock hosts file on disk
    And I create a Hosts instance from the file path
    When I update the mock hosts file with new content
    And I call Refresh
    Then the hosts file should have 3 entries

@Unit
Scenario: WriteHosts writes entries to stream
    Given I have a hosts file stream with standard entries
    And I create a Hosts instance from the stream
    When I write the hosts to an output stream
    Then the output stream should contain the hosts content

@Integration
Scenario: Non-existent file throws ArgumentException
    Given I have a path to a non-existent hosts file
    When I create a Hosts instance from the non-existent file
    Then an exception of type 'System.ArgumentException' is thrown

@Unit
Scenario: Address-only line loads without error
    Given I have a hosts file stream with an address-only entry
    When I create a Hosts instance from the stream
    Then the hosts file should have 2 entries
    And entry 0 should have address "127.0.0.1"
    And entry 1 should have address "127.0.0.2"
    And entry 1 should have alias "host2"

@Integration
Scenario: Writing fewer entries truncates the hosts file
    Given I have a mock hosts file configured as the system hosts file
    And I create a Hosts instance from the file path
    When I remove the last entry
    And I write the hosts file
    And I call Refresh
    Then the hosts file should have 1 entries

@Integration
Scenario: Parameterless write targets the instance's own file
    Given I have a mock hosts file configured as the system hosts file
    And I have a second mock hosts file on disk
    And I create a Hosts instance from the second file path
    When I remove the last entry
    And I write the hosts file
    Then the second mock hosts file should have 1 entries on disk
    And the system hosts file should be unchanged

@Integration
Scenario: Setting SystemHostsFile resets the Current instance
    Given I have a mock hosts file configured as the system hosts file
    And I access the Current hosts instance
    When I configure a second mock hosts file as the system hosts file
    And I access the Current hosts instance
    Then the hosts file should have 3 entries
