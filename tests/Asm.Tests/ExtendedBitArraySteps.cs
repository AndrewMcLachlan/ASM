using System.Collections;

namespace Asm.Tests;

[Binding]
public class ExtendedBitArraySteps
{
    private BitArray _bitArray;
    private Endian _endian;
    private ExtendedBitArray _extendedBitArray;
    private byte[] _byteArray;

    [Given(@"I have a BitArray with values \[(.*)\]")]
    public void GivenIHaveABitArrayWithValues(string values)
    {
        var boolValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();
        _bitArray = new BitArray(boolValues);
    }

    [Given(@"the endianness is (.*)")]
    public void GivenTheEndiannessIs(Endian endian)
    {
        _endian = endian;
    }

    [When(@"I create an ExtendedBitArray")]
    public void WhenICreateAnExtendedBitArray()
    {
        _extendedBitArray = new ExtendedBitArray(_bitArray, _endian);
    }

    [Then(@"the ExtendedBitArray should have (.*) bits")]
    public void ThenTheExtendedBitArrayShouldHaveBits(int count)
    {
        Assert.Equal(count, _extendedBitArray.Count);
    }

    [Then(@"the first bit should be (.*)")]
    public void ThenTheFirstBitShouldBe(bool value)
    {
        Assert.Equal(value, _extendedBitArray[0]);
    }

    [When(@"I convert the ExtendedBitArray to a byte array")]
    public void WhenIConvertTheExtendedBitArrayToAByteArray()
    {
        _byteArray = _extendedBitArray.GetBytes();
    }

    [Then(@"the byte array should be \[(.*)\]")]
    public void ThenTheByteArrayShouldBe(string values)
    {
        var expectedValues = values.Split(',').Select(v => Byte.Parse(v.Trim())).ToArray();
        Assert.Equal(expectedValues, _byteArray);
    }

    [Given(@"I have an ExtendedBitArray with values \[(.*)\]")]
    public void GivenIHaveAnExtendedBitArrayWithValues(string values)
    {
        var boolValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();
        _extendedBitArray = new ExtendedBitArray(boolValues, _endian);
    }
    [Then(@"the bits should be \[(.*)\]")]
    public void ThenTheBitsShouldBe(string values)
    {
        var expectedValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();

        Assert.Collection(_extendedBitArray, expectedValues.Select(v => new Action<bool>(b => Assert.Equal(b, v))).ToArray());
    }


    [When(@"I copy from index (.*) to length (.*)")]
    public void WhenICopyFromIndexToLength(int start, int length)
    {
        _extendedBitArray = _extendedBitArray.Copy(start, length);
    }

    [Then(@"the new ExtendedBitArray should have (.*) bits")]
    public void ThenTheNewExtendedBitArrayShouldHaveBits(int count)
    {
        Assert.Equal(count, _extendedBitArray.Count);
    }

    [Then(@"the second bit should be (.*)")]
    public void ThenTheSecondBitShouldBe(bool value)
    {
        Assert.Equal(value, _extendedBitArray[1]);
    }
}
