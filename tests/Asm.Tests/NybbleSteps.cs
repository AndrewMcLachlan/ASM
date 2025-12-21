using System.Collections;

namespace Asm.Tests;

[Binding]
public class NybbleSteps(ScenarioContext context)
{
    private byte _byteValue;
    private uint _uintValue;
    private int _intValue;
    private byte[] _bytes;
    private Nybble _nybble1;
    private Nybble _nybble2;
    private Nybble[] _nybbleArray;
    private int _hashCode;

    [Given(@"I have a value (.*)")]
    public void GivenIHaveAValue(int value)
    {
        _byteValue = (byte)value;
    }

    [When(@"I create a Nybble")]
    public void WhenICreateANybble()
    {
        context.CatchException(() => _nybble1 = new Nybble(_byteValue));
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

    [Given(@"I have a byte with value (.*)")]
    public void GivenIHaveAByteWithValue(byte value)
    {
        _byteValue = value;
    }

    [Given(@"I have an int with value (.*)")]
    public void GivenIHaveAnIntWithValue(int value)
    {
        _intValue = value;
    }

    [Given(@"I have a uint value (.*)")]
    public void GivenIHaveAUintValue(uint value)
    {
        _uintValue = value;
    }

    [Given(@"I have a byte value (.*)")]
    public void GivenIHaveAByteValue(byte value)
    {
        _byteValue = value;
    }

    [When(@"I convert the byte to Nybbles")]
    public void WhenIConvertTheByteToNybbles()
    {
        _nybbleArray = Nybble.ToNybbles(_byteValue);
    }

    [When(@"I convert the int to Nybbles")]
    public void WhenIConvertTheIntToNybbles()
    {
        _nybbleArray = Nybble.ToNybbles(_intValue);
    }

    [Then(@"the Nybble array should be ([^,].*), ([^,]*)")]
    public void ThenTheNybbleArrayShouldBe(byte byte1, byte byte2)
    {
        Assert.Equal(byte1, _nybbleArray[0].ByteValue);
        Assert.Equal(byte2, _nybbleArray[1].ByteValue);
    }

    [Given(@"I have a byte array with values ([^,]*), ([^,]*)")]
    public void GivenIHaveAByteArrayWithValues(byte value1, byte value2)
    {
        _bytes = [value1, value2];
    }

    [When(@"I convert the byte array to Nybbles")]
    public void WhenIConvertTheByteArrayToNybbles()
    {
        _nybbleArray = Nybble.ToNybbles(_bytes);
    }

    [Then(@"the Nybble array should be ([^,]*), ([^,]*), ([^,]*), (.*)")]
    public void ThenTheNybbleArrayShouldBe(byte byte1, byte byte2, byte byte3, byte byte4)
    {
        Assert.Equal(byte1, _nybbleArray[0].ByteValue);
        Assert.Equal(byte2, _nybbleArray[1].ByteValue);
        Assert.Equal(byte3, _nybbleArray[2].ByteValue);
        Assert.Equal(byte4, _nybbleArray[3].ByteValue);
    }

    [When(@"I add the uint and Nybble")]
    public void WhenIAddTheUintAndNybble()
    {
        context.AddResult(_uintValue + _nybble1);
    }

    [When(@"I add the byte and Nybble")]
    public void WhenIAddTheByteAndNybble()
    {
        context.AddResult(_byteValue + _nybble1);
    }

    [When(@"I check inequality")]
    public void WhenICheckInequality()
    {
        context.AddResult(_nybble1 != _nybble2);
    }

    [When(@"I get the Nybble hash code")]
    public void WhenIGetTheNybbleHashCode()
    {
        _hashCode = _nybble1.GetHashCode();
    }

    [Then(@"the hash code should match the byte value hash code")]
    public void ThenTheHashCodeShouldMatchTheByteValueHashCode()
    {
        Assert.Equal(_nybble1.ByteValue.GetHashCode(), _hashCode);
    }

    [When(@"I check equality with null object")]
    public void WhenICheckEqualityWithNullObject()
    {
        context.AddResult(_nybble1.Equals(null));
    }

    [When(@"I check equality with a string object")]
    public void WhenICheckEqualityWithAStringObject()
    {
        context.AddResult(_nybble1.Equals("not a nybble"));
    }
}
