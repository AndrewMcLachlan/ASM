Feature: UnitConvert

@Unit
Scenario Outline: Convert inches to metres
    When I convert <Inches> inches to metres
    Then the result should be approximately <Metres> metres

Examples:
    | Inches | Metres |
    | 0      | 0      |
    | 1      | 0.0254 |
    | 12     | 0.3048 |
    | 39.37  | 1.0    |

@Unit
Scenario Outline: Convert feet and inches to metres
    When I convert <Feet> feet and <Inches> inches to metres
    Then the result should be approximately <Metres> metres

Examples:
    | Feet | Inches | Metres |
    | 0    | 0      | 0      |
    | 1    | 0      | 0.3048 |
    | 5    | 10     | 1.778  |
    | 6    | 0      | 1.8288 |

@Unit
Scenario Outline: Convert pounds to kilograms
    When I convert <Pounds> pounds to kilograms
    Then the result should be approximately <Kilograms> kilograms

Examples:
    | Pounds | Kilograms |
    | 0      | 0         |
    | 1      | 0.4539    |
    | 14     | 6.3546    |

@Unit
Scenario Outline: Convert stones and pounds to kilograms
    When I convert <Stones> stones and <Pounds> pounds to kilograms
    Then the result should be approximately <Kilograms> kilograms

Examples:
    | Stones | Pounds | Kilograms |
    | 0      | 0      | 0         |
    | 1      | 0      | 6.3546    |
    | 10     | 7      | 66.7233   |
