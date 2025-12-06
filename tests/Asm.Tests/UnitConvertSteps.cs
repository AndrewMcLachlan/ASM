namespace Asm.Tests;

[Binding]
public class UnitConvertSteps
{
    private double _result;

    [When(@"I convert (.*) inches to metres")]
    public void WhenIConvertInchesToMetres(double inches)
    {
        _result = UnitConvert.InchesToM(inches);
    }

    [When(@"I convert (.*) feet and (.*) inches to metres")]
    public void WhenIConvertFeetAndInchesToMetres(double feet, double inches)
    {
        _result = UnitConvert.FeetToM(feet, inches);
    }

    [When(@"I convert (.*) pounds to kilograms")]
    public void WhenIConvertPoundsToKilograms(double pounds)
    {
        _result = UnitConvert.PoundsToKg(pounds);
    }

    [When(@"I convert (.*) stones and (.*) pounds to kilograms")]
    public void WhenIConvertStonesAndPoundsToKilograms(double stones, double pounds)
    {
        _result = UnitConvert.StonesToKg(stones, pounds);
    }

    [Then(@"the result should be approximately (.*) metres")]
    public void ThenTheResultShouldBeApproximatelyMetres(double expected)
    {
        Assert.Equal(expected, _result, 3);
    }

    [Then(@"the result should be approximately (.*) kilograms")]
    public void ThenTheResultShouldBeApproximatelyKilograms(double expected)
    {
        Assert.Equal(expected, _result, 3);
    }
}
