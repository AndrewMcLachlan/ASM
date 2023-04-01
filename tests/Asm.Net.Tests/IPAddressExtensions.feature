@IPAddressExtensions
Feature: IPAddress Extensions
    In order to get IP addresses in different notations
    I want to be able to convert from a subnet mask to CIDR
    So that I can see IP addresses with CIDR notation

@Unit
Scenario Outline: Get IP address in CIDR notation
    Given I have an IP Address '<IP Address>'
    And I have a subnet mask '<Mask>'
    When I call ToCidrString
    Then the string value '<Value>' will be returned

    Examples:
    | IP Address  | Mask            | Value          |
    | 192.168.1.1 | 0.0.0.0         | 0.0.0.0/0      |
    | 192.168.1.1 | 128.0.0.0       | 128.0.0.0/1    |
    | 192.168.1.1 | 192.0.0.0       | 192.0.0.0/2    |
    | 192.168.1.1 | 224.0.0.0       | 192.0.0.0/3    |
    | 192.168.1.1 | 240.0.0.0       | 192.0.0.0/4    |
    | 192.168.1.1 | 248.0.0.0       | 192.0.0.0/5    |
    | 192.168.1.1 | 252.0.0.0       | 192.0.0.0/6    |
    | 192.168.1.1 | 254.0.0.0       | 192.0.0.0/7    |
    | 192.168.1.1 | 255.0.0.0       | 192.0.0.0/8    |
    | 192.168.1.1 | 255.128.0.0     | 192.128.0.0/9  |
    | 192.168.1.1 | 255.192.0.0     | 192.128.0.0/10 |
    | 192.168.1.1 | 255.224.0.0     | 192.160.0.0/11 |
    | 192.168.1.1 | 255.240.0.0     | 192.160.0.0/12 |
    | 192.168.1.1 | 255.248.0.0     | 192.168.0.0/13 |
    | 192.168.1.1 | 255.252.0.0     | 192.168.0.0/14 |
    | 192.168.1.1 | 255.254.0.0     | 192.168.0.0/15 |
    | 192.168.1.1 | 255.255.0.0     | 192.168.0.0/16 |
    | 192.168.1.1 | 255.255.128.0   | 192.168.0.0/17 |
    | 192.168.1.1 | 255.255.192.0   | 192.168.0.0/18 |
    | 192.168.1.1 | 255.255.224.0   | 192.168.0.0/19 |
    | 192.168.1.1 | 255.255.240.0   | 192.168.0.0/20 |
    | 192.168.1.1 | 255.255.248.0   | 192.168.0.0/21 |
    | 192.168.1.1 | 255.255.252.0   | 192.168.0.0/22 |
    | 192.168.1.1 | 255.255.254.0   | 192.168.0.0/23 |
    | 192.168.1.1 | 255.255.255.0   | 192.168.1.0/24 |
    | 192.168.1.1 | 255.255.255.128 | 192.168.1.0/25 |
    | 192.168.1.1 | 255.255.255.192 | 192.168.1.0/26 |
    | 192.168.1.1 | 255.255.255.224 | 192.168.1.0/27 |
    | 192.168.1.1 | 255.255.255.240 | 192.168.1.0/28 |
    | 192.168.1.1 | 255.255.255.248 | 192.168.1.0/29 |
    | 192.168.1.1 | 255.255.255.252 | 192.168.1.0/30 |
    | 192.168.1.1 | 255.255.255.254 | 192.168.1.0/31 |
    | 192.168.1.1 | 255.255.255.255 | 192.168.1.1/32 |

@Unit
Scenario Outline: Get IP address in CIDR notation with invalid mask
    Given I have an IP Address '<IP Address>'
    And I have a subnet mask '<Mask>'
    When I call ToCidrString expecting an exception
    Then an exception of type '<Exception Type>' is thrown
    And the exception message is '<Message>'

    Examples:
    | IP Address  | Mask            | Exception Type         | Message       |
    | 192.168.1.1 | 255.255.255.253 | System.FormatException | Invalid mask  |

@Unit
Scenario Outline: Get IP address in CIDR notation with invalid input
    Given I have an IP Address '<IP Address>'
    And I have a subnet mask '<Mask>'
    When I call ToCidrString expecting an exception
    Then an exception of type 'System.ArgumentException' is thrown
    And the exception message is 'Not an IPv4 address (Parameter '<Parameter>')'
    And the exception parameter name is '<Parameter>'

    Examples:
    | IP Address               | Mask                     | Parameter |
    | fe80::200:f8ff:fe21:67cf | 255.255.255.255          | ipAddress |
    | 192.168.0.1              | fe80::200:f8ff:fe21:67cf | mask      |

@Unit
Scenario Outline: Get IP address as an unsigned 32 bit integer
    Given I have an IP Address '<IP Address>'
    When I call ToUInt32
    Then the unsigned 32 bit integer value <Value> will be returned

    Examples:
    | IP Address      | Value      |
    | 255.255.255.255 | 4294967295 |
    | 204.204.204.204 | 3435973836 |

@Unit
Scenario: Create an IP address from an unsigned 32 bit integer
    Given I have an unsigned 32 bit integer 3435973836
    When I call FromUInt32
    Then the IP Address 204.204.204.204 is returned

@Unit
Scenario: Get IP address as an unsigned 32 bit integer with invalid input
    Given I have an IP Address 'fe80::200:f8ff:fe21:67cf'
    When I call ToUInt32 expecting an exception
    Then an exception of type 'System.ArgumentException' is thrown
    And the exception message is 'Not an IPv4 address (Parameter 'ipAddress')'
    And the exception parameter name is 'ipAddress'