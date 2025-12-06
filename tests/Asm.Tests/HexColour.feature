Feature: HexColour

@Unit
Scenario: Create a HexColor from a string
    Given I have a string '<Hex>'
    When I create a new HexColour from the string
    Then the result should be a HexColour with value '<HexValue>'
Examples:
    | Hex     | HexValue |
    | #FF5733 | #FF5733  |
    | FF5733  | #FF5733  |
    | #ff5733 | #FF5733  |
    | ff5733  | #FF5733  |
    | #FFF    | #FFFFFF  |
    | FFF     | #FFFFFF  |
    | #fff    | #FFFFFF  |
    | fff     | #FFFFFF  |
    | #123    | #112233  |
    | 123     | #112233  |
    | #000    | #000000  |
    | 000     | #000000  |
    | #FFFFFF | #FFFFFF  |
    | FFFFFF  | #FFFFFF  |
    | #ffffff | #FFFFFF  |
    | ffffff  | #FFFFFF  |
    | #000000 | #000000  |
    | 000000  | #000000  |

@Unit
Scenario: Create a HexColor from invalid strings should throw FormatException
    Given I have a string '<Hex>'
    When I create a new HexColour from the string
    Then an exception of type 'System.FormatException' should be thrown

Examples:
    | Hex      |
    | #FF573   |
    | FF573    |
    | #FF57333 |
    | FF57333  |
    | #ZZZ     |
    | ZZZ      |
    | #1234    |
    | 1234     |

@Unit
Scenario: Create a HexColour from a null or empty string should throw ArgumentException
    Given I have a string '<Hex>'
    When I create a new HexColour from the string
    Then an exception of type 'System.ArgumentException' should be thrown

Examples:
    | Hex    |
    |        |
    | <NULL> |

@Unit
Scenario: Create a HexColour from a whitespace string should throw ArgumentException
    Given I have a string '   '
    When I create a new HexColour from the string
    Then an exception of type 'System.ArgumentException' should be thrown

@Unit
Scenario: Parse a HexColor from a string
    Given I have a string '<Hex>'
    When I parse a new HexColour from the string
    Then the result should be a HexColour with value '<HexValue>'
Examples:
    | Hex     | HexValue |
    | #FF5733 | #FF5733  |
    | FF5733  | #FF5733  |
    | #ff5733 | #FF5733  |
    | ff5733  | #FF5733  |
    | #FFF    | #FFFFFF  |
    | FFF     | #FFFFFF  |
    | #fff    | #FFFFFF  |
    | fff     | #FFFFFF  |
    | #123    | #112233  |
    | 123     | #112233  |
    | #000    | #000000  |
    | 000     | #000000  |
    | #FFFFFF | #FFFFFF  |
    | FFFFFF  | #FFFFFF  |
    | #ffffff | #FFFFFF  |
    | ffffff  | #FFFFFF  |
    | #000000 | #000000  |
    | 000000  | #000000  |

@Unit
Scenario: Parse a HexColor from invalid strings should throw FormatException
    Given I have a string '<Hex>'
    When I parse a new HexColour from the string
    Then an exception of type 'System.FormatException' should be thrown

Examples:
    | Hex      |
    | #FF573   |
    | FF573    |
    | #FF57333 |
    | FF57333  |
    | #ZZZ     |
    | ZZZ      |
    | #1234    |
    | 1234     |

@Unit
Scenario: Parse a HexColour from a null or empty string should throw ArgumentException
    Given I have a string '<Hex>'
    When I parse a new HexColour from the string
    Then an exception of type 'System.ArgumentException' should be thrown

Examples:
    | Hex    |
    |        |
    | <NULL> |

@Unit
Scenario: Parse a HexColour from a whitespace string should throw ArgumentException
    Given I have a string '   '
    When I parse a new HexColour from the string
    Then an exception of type 'System.ArgumentException' should be thrown

@Unit
Scenario: Try Parse a HexColor from a string
    Given I have a string '<Hex>'
    When I try parse a new HexColour from the string
    Then the boolean value true is returned
    And the result should be a HexColour with value '<HexValue>'
Examples:
    | Hex     | HexValue |
    | #FF5733 | #FF5733  |
    | FF5733  | #FF5733  |
    | #ff5733 | #FF5733  |
    | ff5733  | #FF5733  |
    | #FFF    | #FFFFFF  |
    | FFF     | #FFFFFF  |
    | #fff    | #FFFFFF  |
    | fff     | #FFFFFF  |
    | #123    | #112233  |
    | 123     | #112233  |
    | #000    | #000000  |
    | 000     | #000000  |
    | #FFFFFF | #FFFFFF  |
    | FFFFFF  | #FFFFFF  |
    | #ffffff | #FFFFFF  |
    | ffffff  | #FFFFFF  |
    | #000000 | #000000  |
    | 000000  | #000000  |

@Unit
Scenario: Try Parse a HexColor from invalid strings should throw FormatException
    Given I have a string '<Hex>'
    When I try parse a new HexColour from the string
    Then the boolean value false is returned

Examples:
    | Hex      |
    | #FF573   |
    | FF573    |
    | #FF57333 |
    | FF57333  |
    | #ZZZ     |
    | ZZZ      |
    | #1234    |
    | 1234     |

@Unit
Scenario: Try Parse a HexColour from a null or empty string should throw ArgumentException
    Given I have a string '<Hex>'
    When I try parse a new HexColour from the string
    Then the boolean value false is returned

Examples:
    | Hex    |
    |        |
    | <NULL> |

@Unit
Scenario: Try Parse a HexColour from a whitespace string should throw ArgumentException
    Given I have a string '   '
    When I try parse a new HexColour from the string
    Then the boolean value false is returned


@Unit
Scenario: Create a HexColor from an unsigned integer
    Given I have a uint <Hex>
    When I create a new HexColour from the uint
    Then the result should be a HexColour with value '<HexValue>'
Examples:
    | Hex      | HexValue |
    | 0        | #000000  |
    | 16777215 | #FFFFFF  |
    | 16711680 | #FF0000  |
    | 65280    | #00FF00  |
    | 255      | #0000FF  |
    | 1193046  | #123456  |

@Unit
Scenario: Create a HexColor from an invalid unsigned integer
    Given I have a uint <Hex>
    When I create a new HexColour from the uint
    Then an exception of type 'System.ArgumentOutOfRangeException' should be thrown
Examples:
    | Hex      |
    | 16777216 |

@Unit
Scenario: Serialize HexColour to JSON
    Given I have a HexColour with value '#FF5733'
    When I serialize the HexColour to JSON
    Then the JSON should be '"#FF5733"'

@Unit
Scenario: Deserialize HexColour from JSON
    Given I have a JSON string '"#00FF00"'
    When I deserialize the JSON to HexColour
    Then the result should be a HexColour with value '#00FF00'

@Unit
Scenario: Deserialize empty string returns default HexColour
    Given I have a JSON string '""'
    When I deserialize the JSON to HexColour
    Then the result should be a HexColour with value '#000000'