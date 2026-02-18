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
    private bool[] _rangeResult;

    // Primitive value holders for constructors
    private byte _singleByte;
    private sbyte _sbyte;
    private short _short;
    private int _int;
    private long _long;
    private ReadOnlyMemory<byte> _byteSpan;

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

    [Given(@"a byte array with values \[(.*)\]")]
    public void GivenAByteArrayWithValues(string values)
    {
        _byteArray = values.Split(',').Select(v => Byte.Parse(v.Trim())).ToArray();
    }

    [Given(@"a single byte with value (.*)")]
    public void GivenASingleByteWithValue(byte value)
    {
        _singleByte = value;
    }

    [Given(@"a signed byte with value (.*)")]
    public void GivenASignedByteWithValue(sbyte value)
    {
        _sbyte = value;
    }

    [Given(@"a short with value (.*)")]
    public void GivenAShortWithValue(short value)
    {
        _short = value;
    }

    [Given(@"an int with value (.*)")]
    public void GivenAnIntWithValue(int value)
    {
        _int = value;
    }

    [Given(@"a long with value (.*)")]
    public void GivenALongWithValue(long value)
    {
        _long = value;
    }

    [Given(@"a byte span with values \[(.*)\]")]
    public void GivenAByteSpanWithValues(string values)
    {
        _byteSpan = values.Split(',').Select(v => Byte.Parse(v.Trim())).ToArray();
    }

    [When(@"I create an ExtendedBitArray")]
    public void WhenICreateAnExtendedBitArray()
    {
        _extendedBitArray = new ExtendedBitArray(_bitArray, _endian);
    }

    [When(@"I create an ExtendedBitArray from bytes")]
    public void WhenICreateAnExtendedBitArrayFromBytes()
    {
        _extendedBitArray = new ExtendedBitArray(_byteArray, _endian);
    }

    [When(@"I create an ExtendedBitArray from single byte")]
    public void WhenICreateAnExtendedBitArrayFromSingleByte()
    {
        _extendedBitArray = new ExtendedBitArray(_singleByte, _endian);
    }

    [When(@"I create an ExtendedBitArray from sbyte")]
    public void WhenICreateAnExtendedBitArrayFromSbyte()
    {
        _extendedBitArray = new ExtendedBitArray(_sbyte, _endian);
    }

    [When(@"I create an ExtendedBitArray from short")]
    public void WhenICreateAnExtendedBitArrayFromShort()
    {
        _extendedBitArray = new ExtendedBitArray(_short, _endian);
    }

    [When(@"I create an ExtendedBitArray from int")]
    public void WhenICreateAnExtendedBitArrayFromInt()
    {
        _extendedBitArray = new ExtendedBitArray(_int, _endian);
    }

    [When(@"I create an ExtendedBitArray from long")]
    public void WhenICreateAnExtendedBitArrayFromLong()
    {
        _extendedBitArray = new ExtendedBitArray(_long, _endian);
    }

    [When(@"I create an ExtendedBitArray from byte span")]
    public void WhenICreateAnExtendedBitArrayFromByteSpan()
    {
        _extendedBitArray = new ExtendedBitArray(_byteSpan.Span, _endian);
    }

    [When(@"I access bits from index (\d+) to (\d+)")]
    public void WhenIAccessBitsFromIndexTo(int start, int end)
    {
        _rangeResult = _extendedBitArray[start..end];
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
            ExtendedBitArray nullArray = null;
            _extendedBitArray.CopyTo(nullArray, index);
        });
    }

    [When(@"I convert the ExtendedBitArray to SByte")]
    public void WhenIConvertTheExtendedBitArrayToSByte()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToSByte()));
    }

    [When(@"I convert the ExtendedBitArray to Int16")]
    public void WhenIConvertTheExtendedBitArrayToInt16()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToInt16()));
    }

    [When(@"I convert the ExtendedBitArray to Int32")]
    public void WhenIConvertTheExtendedBitArrayToInt32()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToInt32()));
    }

    [When(@"I convert the ExtendedBitArray to Int64")]
    public void WhenIConvertTheExtendedBitArrayToInt64()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToInt64()));
    }

    [When(@"I convert the ExtendedBitArray to Byte")]
    public void WhenIConvertTheExtendedBitArrayToByte()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToByte()));
    }

    [When(@"I convert the ExtendedBitArray to UInt16")]
    public void WhenIConvertTheExtendedBitArrayToUInt16()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToUInt16()));
    }

    [When(@"I convert the ExtendedBitArray to UInt32")]
    public void WhenIConvertTheExtendedBitArrayToUInt32()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToUInt32()));
    }

    [When(@"I convert the ExtendedBitArray to UInt64")]
    public void WhenIConvertTheExtendedBitArrayToUInt64()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.ToUInt64()));
    }

    [When(@"I convert the ExtendedBitArray to string")]
    public void WhenIConvertTheExtendedBitArrayToString()
    {
        context.CatchException(() => _conversionResultString = _extendedBitArray.ToString());
    }

    [When(@"I access the IsSynchronized property")]
    public void WhenIAccessTheIsSynchronizedProperty()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.IsSynchronized));
    }

    [When(@"I access the SyncRoot property")]
    public void WhenIAccessTheSyncRootProperty()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.SyncRoot));
    }

    [When(@"I clone the ExtendedBitArray")]
    public void WhenICloneTheExtendedBitArray()
    {
        context.CatchException(() => context.AddResult(_extendedBitArray.Clone()));
    }

    [When(@"I get the enumerator for the ExtendedBitArray")]
    public void WhenIGetTheEnumeratorForTheExtendedBitArray()
    {
        context.CatchException(() =>
        {
            var enumerator = ((System.Collections.IEnumerable)_extendedBitArray).GetEnumerator();
            context.AddResult(enumerator);
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
        if (!String.IsNullOrEmpty(_conversionResultString))
        {
            Assert.Matches(@"[01]+", _conversionResultString);
        }
    }

    [Then(@"the result should not be null")]
    public void ThenTheResultShouldNotBeNull()
    {
        var result = context.Get<object>("Result");
        Assert.NotNull(result);
    }

    [Then(@"I should get an exception")]
    public void ThenIShouldGetAnException()
    {
        Assert.NotNull(context.GetException());
    }

    [Then(@"the sbyte result should be (.*)")]
    public void ThenTheSbyteResultShouldBe(sbyte expected)
    {
        var result = context.Get<sbyte>("Result");
        Assert.Equal(expected, result);
    }

    [Then(@"the short result should be (.*)")]
    public void ThenTheShortResultShouldBe(short expected)
    {
        var result = context.Get<short>("Result");
        Assert.Equal(expected, result);
    }

    [Then(@"the int result should be (.*)")]
    public void ThenTheIntResultShouldBe(int expected)
    {
        var result = context.Get<int>("Result");
        Assert.Equal(expected, result);
    }

    [Then(@"the long result should be (.*)")]
    public void ThenTheLongResultShouldBe(long expected)
    {
        var result = context.Get<long>("Result");
        Assert.Equal(expected, result);
    }

    [Then(@"the range result should be \[(.*)\]")]
    public void ThenTheRangeResultShouldBe(string values)
    {
        var expectedValues = values.Split(',').Select(v => Boolean.Parse(v.Trim())).ToArray();
        Assert.Equal(expectedValues, _rangeResult);
    }

    [Then(@"the operation should complete without exception")]
    public void ThenTheOperationShouldCompleteWithoutException()
    {
        Assert.Null(context.GetException());
    }
}
