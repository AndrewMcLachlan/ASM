namespace Asm.Tests;

[Binding]
public class ByteArraySteps(ScenarioContext context)
{
    private ByteArray _byteArray;
    private ByteArray _byteArray2;
    private ByteArray _resultByteArray;
    private char[] _resultCharArray;
    private byte[] _rawByteArray;
    private byte[] _resultRawByteArray;

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

    [When(@"I convert to Int16")]
    public void WhenIConvertToInt16()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToInt16()));
    }

    [When(@"I convert to Int32")]
    public void WhenIConvertToInt32()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToInt32()));
    }

    [When(@"I convert to Int64")]
    public void WhenIConvertToInt64()
    {
        context.CatchException(() => context.AddResult(_byteArray.ToInt64()));
    }

    [When(@"I convert to string")]
    public void WhenIConvertToString()
    {
        context.AddResult(_byteArray.ToString());
    }

    [When(@"I check for inequality")]
    public void WhenICheckForInequality()
    {
        context.AddResult(_byteArray != _byteArray2);
    }

    [When(@"I access index (.*)")]
    public void WhenIAccessIndex(int index)
    {
        context.AddResult(_byteArray[index]);
    }

    [When(@"I get the hash code")]
    public void WhenIGetTheHashCode()
    {
        context.AddResult(_byteArray.GetHashCode());
    }

    [Given(@"a raw byte array with values (.*)")]
    public void GivenARawByteArrayWithValues(string values)
    {
        _rawByteArray = Array.ConvertAll(values.Split(','), Byte.Parse);
    }

    [When(@"I implicitly convert to ByteArray")]
    public void WhenIImplicitlyConvertToByteArray()
    {
        _byteArray = _rawByteArray;
    }

    [When(@"I implicitly convert to byte array")]
    public void WhenIImplicitlyConvertToRawByteArray()
    {
        _resultRawByteArray = _byteArray;
    }

    [Then(@"the ByteArray should have (.*) bytes")]
    public void ThenTheByteArrayShouldHaveBytes(int count)
    {
        Assert.Equal(count, _byteArray.GetBytes().Length);
    }

    [Then(@"the byte array should have (.*) bytes")]
    public void ThenTheRawByteArrayShouldHaveBytes(int count)
    {
        Assert.Equal(count, _resultRawByteArray.Length);
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
