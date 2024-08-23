namespace Asm.Tests;

[Binding]
public class NybbleSteps(ScenarioContext context)
{
    private byte _value;
    private Nybble _nybble1;
    private Nybble _nybble2;

    [Given(@"I have a value (.*)")]
    public void GivenIHaveAValue(int value)
    {
        _value = (byte)value;
    }

    [When(@"I create a Nybble")]
    public void WhenICreateANybble()
    {
        context.CatchException(() => _nybble1 = new Nybble(_value));
    }

    [Then(@"the Nybble value should be (.*)")]
    public void ThenTheNybbleValueShouldBe(int expectedValue)
    {
        Assert.Equal((byte)expectedValue, _nybble1.ByteValue);
    }

    [Given(@"I have a Nybble with value (.*)")]
    public void GivenIHaveANybbleWithValue(int value)
    {
        _nybble1 = new Nybble((byte)value);
    }

    [Given(@"I have another Nybble with value (.*)")]
    public void GivenIHaveAnotherNybbleWithValue(int value)
    {
        _nybble2 = new Nybble((byte)value);
    }

    [When(@"I add the two Nybbles")]
    public void WhenIAddTheTwoNybbles()
    {
        context.AddResult(_nybble1 + _nybble2);
    }

    [Given(@"I have a Nybble array with values (.*)")]
    public void GivenIHaveANybbleArrayWithValues(string values)
    {
        var valueArray = values.Split(',');
        var nybbleArray = new Nybble[valueArray.Length];
        for (int i = 0; i < valueArray.Length; i++)
        {
            nybbleArray[i] = new Nybble(Byte.Parse(valueArray[i].Trim()));
        }
        _nybble1 = nybbleArray[0];
        _nybble2 = nybbleArray[1];
        context.AddResult(Nybble.ToUInt32(nybbleArray));
    }

    [When(@"I convert the array to uint")]
    public void WhenIConvertTheArrayToUint()
    {
        // Conversion already done in Given step
    }

    [When(@"I check equality")]
    public void WhenICheckEquality()
    {
        context.AddResult(_nybble1 == _nybble2);
    }
}
