using Asm.AspNetCore.Validators;
using FluentValidation.Results;

namespace Asm.AspNetCore.Tests.Validators;

[Binding]
public class DescribedValidatorSteps
{
    private TestDescribedObject _describedObject;
    private DescribedValidator<TestDescribedObject> _validator;
    private DescribedValidator<TestDescribedObject> _customValidator;
    private ValidationResult _validationResult;

    public DescribedValidatorSteps()
    {
        _validator = new DescribedValidator<TestDescribedObject>();
    }

    [Given(@"I have a described object with name '(.*)' and description '(.*)'")]
    public void GivenIHaveADescribedObjectWithNameAndDescription(string name, string description)
    {
        _describedObject = new TestDescribedObject { Name = name, Description = description };
    }

    [Given(@"I have a described object with null name and description '(.*)'")]
    public void GivenIHaveADescribedObjectWithNullNameAndDescription(string description)
    {
        _describedObject = new TestDescribedObject { Name = null, Description = description };
    }

    [Given(@"I have a described object with name exceeding (.*) characters and description '(.*)'")]
    public void GivenIHaveADescribedObjectWithNameExceedingCharactersAndDescription(int length, string description)
    {
        _describedObject = new TestDescribedObject
        {
            Name = new string('A', length + 1),
            Description = description
        };
    }

    [Given(@"I have a described object with name '(.*)' and description exceeding (.*) characters")]
    public void GivenIHaveADescribedObjectWithNameAndDescriptionExceedingCharacters(string name, int length)
    {
        _describedObject = new TestDescribedObject
        {
            Name = name,
            Description = new string('A', length + 1)
        };
    }

    [Given(@"I have a described object with name '(.*)' and null description")]
    public void GivenIHaveADescribedObjectWithNameAndNullDescription(string name)
    {
        _describedObject = new TestDescribedObject { Name = name, Description = null };
    }

    [Given(@"I use a custom validator with name length (.*) and description length (.*)")]
    public void GivenIUseACustomValidatorWithNameLengthAndDescriptionLength(int nameLength, int descriptionLength)
    {
        _customValidator = new DescribedValidator<TestDescribedObject>(nameLength, descriptionLength);
    }

    [When(@"I validate the described object")]
    public void WhenIValidateTheDescribedObject()
    {
        _validationResult = _validator.Validate(_describedObject);
    }

    [When(@"I validate the described object with custom validator")]
    public void WhenIValidateTheDescribedObjectWithCustomValidator()
    {
        _validationResult = _customValidator.Validate(_describedObject);
    }

    [Then(@"the validation should pass")]
    public void ThenTheValidationShouldPass()
    {
        Assert.True(_validationResult.IsValid);
    }

    [Then(@"the validation should fail")]
    public void ThenTheValidationShouldFail()
    {
        Assert.False(_validationResult.IsValid);
    }

    [Then(@"the validation error should be for property '(.*)'")]
    public void ThenTheValidationErrorShouldBeForProperty(string propertyName)
    {
        Assert.Contains(_validationResult.Errors, e => e.PropertyName == propertyName);
    }

    private class TestDescribedObject : IDescribed, INamed
    {
        public string Name { get; init; }
        public string Description { get; init; }
    }
}
