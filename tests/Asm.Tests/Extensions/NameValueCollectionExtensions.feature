Feature: NameValueCollectionExtensions

@Unit
Scenario: GetValue returns value when key exists
    Given I have a NameValueCollection with key 'count' and value '42'
    When I call GetValue for 'count' with default 0
    Then the integer result should be 42

@Unit
Scenario: GetValue returns default when key does not exist
    Given I have an empty NameValueCollection
    When I call GetValue for 'missing' with default 99
    Then the integer result should be 99

@Unit
Scenario: GetValue returns default when value is null
    Given I have a NameValueCollection with key 'nullkey' and null value
    When I call GetValue for 'nullkey' with default 'fallback'
    Then the string result should be 'fallback'

@Unit
Scenario: GetValue returns default on format exception
    Given I have a NameValueCollection with key 'invalid' and value 'not-a-number'
    When I call GetValue for 'invalid' with default 0
    Then the integer result should be 0

@Unit
Scenario: GetValue throws on null collection
    Given I have a null NameValueCollection
    When I call GetValue and expect an exception
    Then an exception of type 'System.ArgumentNullException' should be thrown

@Unit
Scenario: GetValue converts string to boolean
    Given I have a NameValueCollection with key 'enabled' and value 'true'
    When I call GetValue for 'enabled' with default false as boolean
    Then the boolean result should be true

@Unit
Scenario: GetValue converts string to decimal
    Given I have a NameValueCollection with key 'price' and value '19.99'
    When I call GetValue for 'price' with default 0.0 as decimal
    Then the decimal result should be 19.99
