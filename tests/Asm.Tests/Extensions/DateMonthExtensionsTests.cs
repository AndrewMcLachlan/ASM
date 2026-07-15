using System;

namespace Asm.Tests.Extensions;

public class DateMonthExtensionsTests
{
    [Fact]
    [Trait("Category", "Unit")]
    public void DateOnlyToStartOfMonthReturnsFirstDay()
    {
        Assert.Equal(new DateOnly(2026, 7, 1), new DateOnly(2026, 7, 15).ToStartOfMonth());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DateOnlyToEndOfMonthReturnsLastDay()
    {
        Assert.Equal(new DateOnly(2026, 7, 31), new DateOnly(2026, 7, 15).ToEndOfMonth());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DateOnlyToEndOfMonthHandlesLeapFebruary()
    {
        Assert.Equal(new DateOnly(2024, 2, 29), new DateOnly(2024, 2, 10).ToEndOfMonth());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DateTimeToStartOfMonthReturnsFirstDayAtMidnight()
    {
        Assert.Equal(new DateTime(2026, 7, 1, 0, 0, 0), new DateTime(2026, 7, 15, 13, 45, 30).ToStartOfMonth());
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void DateTimeToEndOfMonthReturnsLastDayAtMidnight()
    {
        Assert.Equal(new DateTime(2026, 2, 28, 0, 0, 0), new DateTime(2026, 2, 10, 8, 0, 0).ToEndOfMonth());
    }
}
