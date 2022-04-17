using System;

namespace FluentValidator.Tests.NumericValidation;

/// <summary>
/// Tests the enumerate examples using the simple <see cref="Check{T}"/> class.
/// Which return a simple <see cref="Result{T}"/> where there is no advanced Error reporting.
/// </summary>
[TestFixture]
internal class IntTests
{
    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheIntAsTrue()
    {
        // Arrange
        var num = 1;
        Func<int, bool> isPositive = (i) => i > 0;
        // Act
        bool result = Check.That(num).Is(isPositive);
        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheIntAsFalse()
    {
        // Arrange
        var num = -1;
        Func<int, bool> isPositive = (i) => i > 0;
        // Act
        bool result = Check.That(num).Is(isPositive);
        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassMultiplePredicatesAndObtainTheResultOfAllOfThem_OneShouldFail()
    {
        // Arrange
        var num = 1;
        Func<int, bool> isPositive = i => i > 0;
        Func<int, bool> isZero = i => i == 0;
        // Act
        bool result = Check.That(num)
            .Is(isPositive)
            .And(isZero);
        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassMultiplePredicatesAndObtainTheResultOfAllOfThem_AllShouldPass()
    {
        // Arrange
        var num = 100;
        Func<int, bool> isPositive = i => i > 0;
        Func<int, bool> isGreaterThanFifty = i => i > 50;
        // Act
        bool result = Check.That(num)
            .Is(isPositive)
            .And(isGreaterThanFifty);
        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void WhenGivenAnArrayOfChecks_ShouldCheckAllOfThem()
    {
        // Arrange
        var num = 100;
        Func<int, bool> isPositive = i => i > 0;
        Func<int, bool> IsDivisibleByTen = i => i % 10 == 0;
        Func<int, bool> isEven = i => i % 2 == 0;
        Func<int, bool> isLessThanAThousand = i => i < 1000;

        Func<int, bool>[] checksArray = new[] { isPositive, IsDivisibleByTen, isEven, isLessThanAThousand };

        // Act
        bool result = Check.That(num).IsAll(checksArray);

        // Assert
        result.ShouldBeTrue();
    }
}
