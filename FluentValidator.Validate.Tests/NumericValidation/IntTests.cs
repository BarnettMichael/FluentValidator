using System;
using FluentValidator.Validate;
using NSubstitute;

namespace FluentValidator.Tests.NumericValidation;

/// <summary>
/// Unit tests that provide examples and code samples of the usage of the simple <see cref="Check{T}"/> class.
/// While return a simple <see cref="Result{T}"/> where there is no advanced Error reporting.
/// </summary>
[TestFixture]
internal class IntTests
{
    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheIntAsTrue()
    {
        // Arrange
        var num = 1;
        // Act
        bool result = Check.That(num).Is(i => i > 0);
        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheIntAsFalse()
    {
        // Arrange
        var num = -1;
        // Act
        bool result = Check.That(num).Is(i => i > 0);
        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassMultipleFunctions__IfOneFails_ThenReturnFalse()
    {
        // Arrange
        var num = 1;
        static bool isPositive(int i) => i > 0;
        static bool isZero(int i) => i == 0;
        // Act
        bool result = Check.That(num)
            .Is(isPositive)
            .And(isZero);
        // Assert
        result.ShouldBeFalse();
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassMultipleFunctionsOneOfWhichFails_NoChecksAfterFailureShouldBeRun()
    {
        // Arrange
        var num = 1;
        Func<int, bool> passingStub = Substitute.For<Func<int, bool>>();
        passingStub(Arg.Any<int>()).Returns(true);

        static bool isZero(int i) => i == 0;

        Func<int, bool> mockFunction = Substitute.For<Func<int, bool>>();
        mockFunction(Arg.Any<int>()).Returns(true);
        // Act
        bool result = Check.That(num)
            .Is(passingStub)
            .And(isZero)
            .And(mockFunction);
        // Assert
        result.ShouldSatisfyAllConditions(
            () => result.ShouldBeFalse(),
            () => passingStub.Received(1).Invoke(Arg.Any<int>()),
            () => mockFunction.DidNotReceive().Invoke(Arg.Any<int>()));
    }

    [Test]
    public void WhenGivenAnInt_ShouldBeAbleToPassMultipleFunctionsAndObtainTheResultOfAllOfThem_AllShouldPass()
    {
        // Arrange
        var num = 100;
        static bool isPositive(int i) => i > 0;
        static bool isGreaterThanFifty(int i) => i > 50;
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
        static bool isPositive(int i) => i > 0;
        static bool IsDivisibleByTen(int i) => i % 10 == 0;
        static bool isEven(int i) => i % 2 == 0;
        static bool isLessThanAThousand(int i) => i < 1000;

        Func<int, bool>[] checksArray = new[] { isPositive, IsDivisibleByTen, isEven, isLessThanAThousand };

        // Act
        bool result = Check.That(num).IsAll(checksArray);

        // Assert
        result.ShouldBeTrue();
    }

    [Test]
    public void WhenGivenAnArrayOfChecks_OneHardCoded_ShouldCheckAllOfThem()
    {
        // Arrange
        var num = 100;
        static bool isPositive(int i) => i > 0;
        static bool IsDivisibleByTen(int i) => i % 10 == 0;
        static bool isEven(int i) => i % 2 == 0;
        static bool isLessThanAThousand(int i) => i < 1000;


        // Act
        bool result = Check.That(num)
            .Is(isPositive)
            .AndAll(IsDivisibleByTen, isEven, isLessThanAThousand);

        // Assert
        result.ShouldBeTrue();
    }
}
