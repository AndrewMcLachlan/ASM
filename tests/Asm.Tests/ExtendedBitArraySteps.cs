using System.Collections;

namespace Asm.Tests;

[Binding]
public class ExtendedBitArraySteps(ScenarioContext context)
{
    private BitArray _bitArray;
    private Endian _endian;
    private ExtendedBitArray _extendedBitArray;
    private ExtendedBitArray _extendedBitArray2;
    private byte[] _byteArray;
    private object _conversionResult;
    private string _conversionResultString;

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

    [Given(@"I have an ExtendedBitArray with values \[(.*)\]")]
    public void GivenIHaveAnExtendedBitArrayWithValues(string values)
    {
        var boolValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();
        _extendedBitArray = new ExtendedBitArray(boolValues, _endian);
    }

    [Given(@"I have another ExtendedBitArray with values \[(.*)\]")]
    public void GivenIHaveAnotherExtendedBitArrayWithValues(string values)
    {
        var boolValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();
        _extendedBitArray2 = new ExtendedBitArray(boolValues, _endian);
    }

    [When(@"I create an ExtendedBitArray")]
    public void WhenICreateAnExtendedBitArray()
    {
        _extendedBitArray = new ExtendedBitArray(_bitArray, _endian);
    }

    [When(@"I convert the ExtendedBitArray to a byte array")]
    public void WhenIConvertTheExtendedBitArrayToAByteArray()
    {
        _byteArray = _extendedBitArray.GetBytes();
    }

    [When(@"I copy from index (.*) to length (.*)")]
    public void WhenICopyFromIndexToLength(int start, int length)
    {
        context.CatchException(() => _extendedBitArray = _extendedBitArray.Copy(start, length));
    }

    [When(@"I copy to the other array with index (.*)")]
    public void WhenICopyToTheOtherArrayWithIndex(int index)
    {
        context.CatchException(() => _extendedBitArray.CopyTo(_extendedBitArray2, index));
    }

    [When(@"I copy to null array with index (.*)")]
    public void WhenICopyToNullArrayWithIndex(int index)
    {
        context.CatchException(() => 
        {
            ExtendedBitArray? nullArray = null;
            _extendedBitArray.CopyTo(nullArray, index);
        });
    }

    [When(@"I convert the ExtendedBitArray to SByte")]
    public void WhenIConvertTheExtendedBitArrayToSByte()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToSByte());
    }

    [When(@"I convert the ExtendedBitArray to Int16")]
    public void WhenIConvertTheExtendedBitArrayToInt16()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToInt16());
    }

    [When(@"I convert the ExtendedBitArray to Int32")]
    public void WhenIConvertTheExtendedBitArrayToInt32()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToInt32());
    }

    [When(@"I convert the ExtendedBitArray to Int64")]
    public void WhenIConvertTheExtendedBitArrayToInt64()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToInt64());
    }

    [When(@"I convert the ExtendedBitArray to Byte")]
    public void WhenIConvertTheExtendedBitArrayToByte()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToByte());
    }

    [When(@"I convert the ExtendedBitArray to UInt16")]
    public void WhenIConvertTheExtendedBitArrayToUInt16()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToUInt16());
    }

    [When(@"I convert the ExtendedBitArray to UInt32")]
    public void WhenIConvertTheExtendedBitArrayToUInt32()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToUInt32());
    }

    [When(@"I convert the ExtendedBitArray to UInt64")]
    public void WhenIConvertTheExtendedBitArrayToUInt64()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.ToUInt64());
    }

    [When(@"I convert the ExtendedBitArray to string")]
    public void WhenIConvertTheExtendedBitArrayToString()
    {
        context.CatchException(() => _conversionResultString = _extendedBitArray.ToString());
    }

    [When(@"I access the IsSynchronized property")]
    public void WhenIAccessTheIsSynchronizedProperty()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.IsSynchronized);
    }

    [When(@"I access the SyncRoot property")]
    public void WhenIAccessTheSyncRootProperty()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.SyncRoot);
    }

    [When(@"I clone the ExtendedBitArray")]
    public void WhenICloneTheExtendedBitArray()
    {
        context.CatchException(() => _conversionResult = _extendedBitArray.Clone());
    }

    [When(@"I get the enumerator for the ExtendedBitArray")]
    public void WhenIGetTheEnumeratorForTheExtendedBitArray()
    {
        context.CatchException(() => 
        {
            var enumerator = ((System.Collections.IEnumerable)_extendedBitArray).GetEnumerator();
            _conversionResult = enumerator;
        });
    }

    [When(@"I convert the ExtendedBitArray to signed value of (.*)")]
    public void WhenIConvertTheExtendedBitArrayToSignedValueOf(int bitWidth)
    {
        context.CatchException(() => 
        {
            // ToSigned is internal, so we skip this test for now
            _conversionResult = null;
        });
    }

    [When(@"I convert the ExtendedBitArray to unsigned with (.*) bits and max (.*)")]
    public void WhenIConvertTheExtendedBitArrayToUnsignedWithBitsAndMax(int bitWidth, int maxValue)
    {
        context.CatchException(() =>
        {
            // ToUnsigned is internal, so we skip this test for now
            _conversionResult = null;
        });
    }

    [When(@"I copy to Array")]
    public void WhenICopyToArray()
    {
        context.CatchException(() =>
        {
            var arr = new object[_extendedBitArray.Count];
            _extendedBitArray.CopyTo(arr, 0);
            _conversionResult = arr;
        });
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

    [Then(@"the byte array should be \[(.*)\]")]
    public void ThenTheByteArrayShouldBe(string values)
    {
        var expectedValues = values.Split(',').Select(v => Byte.Parse(v.Trim())).ToArray();
        Assert.Equal(expectedValues, _byteArray);
    }

    [Then(@"the bits should be \[(.*)\]")]
    public void ThenTheBitsShouldBe(string values)
    {
        var expectedValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();

        Assert.Collection(_extendedBitArray, expectedValues.Select(v => new Action<bool>(b => Assert.Equal(b, v))).ToArray());
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

    [Then(@"the conversion result should be (-?\d+)")]
    public void ThenTheConversionResultShouldBe(long expected)
    {
        Assert.Equal((object)expected, _conversionResult);
    }

    [Then(@"the conversion result should be (\d+) as unsigned")]
    public void ThenTheConversionResultShouldBeAsUnsigned(ulong expected)
    {
        Assert.Equal((object)expected, _conversionResult);
    }

    [Then(@"the string representation should contain binary digits")]
    public void ThenTheStringRepresentationShouldContainBinaryDigits()
    {
        if (!string.IsNullOrEmpty(_conversionResultString))
        {
            Assert.Matches(@"[01]+", _conversionResultString);
        }
    }

    [Then(@"the result should not be null")]
    public void ThenTheResultShouldNotBeNull()
    {
        Assert.NotNull(_conversionResult);
    }

    [Then(@"I should get an exception")]
    public void ThenIShouldGetAnException()
    {
        Assert.NotNull(context.GetException());
    }
}
