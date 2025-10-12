# Asm.Testing

The `Asm.Testing` library provides utilities and abstractions to simplify Behaviour-Driven-Development (BDD) testing with `Reqnroll` and `xUnit` in .NET applications. It includes features for mocking, assertions, and test setup to enhance the testing experience.


## Features

- **Test Utilities**: Common utilities to streamline unit testing
- **Integration with xUnit**: Preconfigured support for xUnit-based test projects
- **Reqnroll Step Definitions**: Reusable step definitions for common testing scenarios
- **Exception Handling Steps**: Built-in steps for testing exception scenarios
- **Simple Assertion Steps**: Common assertion steps for BDD scenarios
- **Scenario Context Extensions**: Helper methods for working with Reqnroll's `ScenarioContext`
- **Step Argument Transformations**: Custom transformations for common test data types
- **Code Coverage Exclusion**: Automatically excludes test-related code from coverage reports

## Installation

To install the `Asm.Testing` library, use the .NET CLI:

`dotnet add package Asm.Testing`

Or via the NuGet Package Manager:

`Install-Package Asm.Testing`

## Usage

### Setting Up a Test Project

Create a test project and add the required packages:

```bash
dotnet new xunit -n MyApp.Tests
cd MyApp.Tests
dotnet add package Asm.Testing
dotnet add package Reqnroll
dotnet add package Reqnroll.xUnit
```
### Using Built-In Exception Steps

The library provides built-in steps for testing exceptions:

```gherkin
Feature: HexColour Validation

@Unit
Scenario: Create a HexColour from invalid string
  Given I have a string 'INVALID'
  When I create a new HexColour from the string
  Then an exception of type 'System.FormatException' should be thrown
  And the exception message is 'Invalid hex colour format: INVALID'
```

These exception steps are automatically available through `ExceptionSteps` class.

### Using Scenario Context Extensions

The library provides helpful extensions for working with `ScenarioContext`:

```csharp
using Asm.Testing;

// Store and retrieve exceptions
context.SetException(new InvalidOperationException("Error"));
var exception = context.GetException();
```

### Using Step Argument Transformations

The library includes transformations for common test data:

```csharp
// Automatically transforms special tokens in feature files:
// - <NULL> ? null
// - \n ? newline
// - \t ? tab
// - Other whitespace escape sequences
```

Example in a feature file:

```gherkin
Scenario: Handle null values
  Given I have a string '<NULL>'
  When I process the string
  Then an exception should be thrown
```

### Using Simple Assertion Steps

Built-in assertion steps for common scenarios:

```gherkin
Scenario: Boolean assertions
  When I call the IsValid method
  Then the boolean value true is returned

Scenario: String assertions
  When I call the GetName method
  Then the string value 'John Doe' is returned

Scenario: Integer assertions  
  When I call the GetCount method
  Then the integer value 42 is returned
```

## Common Test Patterns

### Testing Exceptions

```gherkin
Scenario: Test argument validation
  Given I have an invalid input
  When I call the method
  Then an exception of type 'System.ArgumentException' should be thrown
  And the exception parameter name is 'input'
  And the exception message is 'Input cannot be null or empty'
```

### Testing No Exception

```gherkin
Scenario: Test valid operation
  Given I have valid input
  When I call the method
  Then no exception is thrown
```

## Dependencies

The `Asm.Testing` library depends on the following packages:

- `Reqnroll`
- `xUnit`
- `Microsoft.Extensions.DependencyModel`

## Contributing

Contributions are welcome! If you encounter any issues or have suggestions for improvements, feel free to open an issue or submit a pull request on GitHub.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
