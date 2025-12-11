namespace Asm.Tests;

[Binding]
public class ByteArraySteps(ScenarioContext context)
{
    private ByteArray _byteArray;
    private ByteArray _byteArray2;
    private ByteArray _resultByteArray;
    private char[] _resultCharArray;

    [Given(@"a ByteArray with values (.*) and (.*) endian")]
    public void GivenAByteArrayWithValues(string values, string endian)
    {
        var byteValues = Array.ConvertAll(values.Split(','), Byte.Parse);
        _byteArray = new ByteArray(byteValues, endian == "big" ? Endian.BigEndian : endian == "little" ? Endian.LittleEndian : throw new InvalidOperationException("Invalid endian"));
    }

    [Given(@"another ByteArray with values (.*) and (.*) endian")]
    public void GivenAnotherByteArrayWithValues(string values, string endian)
    {
        var byteValues = Array.ConvertAll(values.Split(','), Byte.Parse);
        _byteArray2 = new ByteArray(byteValues, endian == "big" ? Endian.BigEndian : endian == "little" ? Endian.LittleEndian : throw new InvalidOperationException("Invalid endian"));
    }

    [When(@"I copy from index (.*) with length (.*)")]
    public void WhenICopyFromIndexWithLength(int index, int length)
    {
        context.CatchException(() => _resultByteArray = _byteArray.Copy(index, length));
    }

    [When(@"I convert to char array")]
    public void WhenIConvertToCharArray()
    {
        _resultCharArray = _byteArray.ToCharArray();
    }

    [When(@"I convert to UInt16")]
    public void WhenIConvertToUInt16()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToUInt16()));
    }

    [When(@"I convert to UInt32")]
    public void WhenIConvertToUInt32()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToUInt32()));
    }

    [When(@"I convert to UInt64")]
    public void WhenIConvertToUInt64()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToUInt64()));
    }

    [When(@"I convert to Guid")]
    public void WhenIConvertToGuid()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToGuid()));
    }

    [When(@"I check for equality")]
    public void WhenICheckForEquality()
    {
        context.AddResult(_byteArray == _byteArray2);
    }

    [When(@"I check equality with null")]
    public void WhenICheckEqualityWithNull()
    {
        context.AddResult(_byteArray.Equals(null));
    }

    [When(@"I check equality with incompatible object")]
    public void WhenICheckEqualityWithIncompatibleObject()
    {
        context.AddResult(_byteArray.Equals("not a byte array"));
    }

    [Then(@"the result should be a ByteArray with values (.*)")]
    public void ThenTheResultShouldBeAByteArrayWithValues(string values)
    {
        var expectedValues = Array.ConvertAll(values.Split(','), Byte.Parse);
        Assert.Equal(expectedValues, _resultByteArray.GetBytes());
    }

    [Then(@"the result should be a char array with values (.*)")]
    public void ThenTheResultShouldBeACharArrayWithValues(string values)
    {
        var expectedValues = Array.ConvertAll(values.Replace("'", String.Empty).Split(','), s => Char.Parse(s.Trim()));
        Assert.Equal(expectedValues, _resultCharArray);
    }
}
