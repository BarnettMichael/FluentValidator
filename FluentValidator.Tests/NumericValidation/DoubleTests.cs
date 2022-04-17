using System;

namespace FluentValidator.Tests.NumericValidation;

/// <summary>
/// Tests that enumerate examples using the <see cref="Check{T}"/> class.
/// Which return a <see cref="Result{T, E}"/> where options for more advanced error reporting are used.
/// </summary>
[TestFixture]
internal class DoubleTests
{
    class DeclaredError
    {
        enum DoubleErrors
        {
            Default,
            NotPositive,
        }
        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsTrue()
        {
            // Arrange
            var num = 1.0d;
            Func<double, bool> isPositive = (d) => d > 0;
            // Act
            var result = Check.That(num).Is(isPositive, DoubleErrors.NotPositive);
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeTrue(),
                () => result.Error.ShouldBe(DoubleErrors.Default)
                );
        }

        //[Test]
        //public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsFalse()
        //{
        //    // Arrange
        //    var num = -1d;
        //    Func<double, bool> isPositive = (d) => d > 0;
        //    Func<double, string> errorGenerator = (d) => $"The value: {d} failed validation.";
        //    // Act
        //    var result = Check.That(num).Is(isPositive, errorGenerator);
        //    // Assert
        //    result.ShouldSatisfyAllConditions(
        //        () => result.IsSuccess.ShouldBeFalse(),
        //        () => result.Error.ShouldBe("The value: -1 failed validation.")
        //        );
        //}

        //[Test]
        //public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_GivenOneWillFail_ShouldFailWithCorrectError()
        //{
        //    // Arrange
        //    var num = 1d;
        //    Func<double, bool> isPositive = d => d > 0;
        //    Func<double, string> isNotPositiveError = (d) => $"Validation failed: {d} is not positive.";
        //    Func<double, bool> isZero = d => d == 0;
        //    Func<double, string> isNotZeroError = (d) => $"Validation failed: {d} is not zero.";

        //    // Act
        //    var result = Check.That(num)
        //        .Is(isPositive, isNotPositiveError)
        //        .And(isZero, isNotZeroError);

        //    // Assert
        //    result.ShouldSatisfyAllConditions(
        //        () => result.IsSuccess.ShouldBeFalse(),
        //        () => result.Error.ShouldBe("Validation failed: 1 is not zero.")
        //        );
        //}

        //[Test]
        //public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctionsAndObtainTheResultOfAllOfThem_AllShouldPass()
        //{
        //    // Arrange
        //    var num = 100d;
        //    Func<double, bool> isPositive = d => d > 0;
        //    Func<double, bool> isGreaterThanFifty = d => d > 50;
        //    Func<double, string> errorGenerator = (d) => $"{d} did not pass validation";
        //    // Act
        //    bool result = Check.That(num)
        //        .Is(isPositive, errorGenerator)
        //        .And(isGreaterThanFifty, errorGenerator);
        //    // Assert
        //    result.ShouldBeTrue();
        //}

        //[Test]
        //public void WhenGivenAnArrayOfChecks_ShouldCheckAllOfThem()
        //{
        //    // Arrange
        //    var num = 100d;
        //    Func<double, bool> isPositive = d => d > 0;
        //    Func<double, bool> IsDivisibleByTen = d => d % 10 == 0;
        //    Func<double, bool> isEven = d => d % 2 == 0;
        //    Func<double, bool> isLessThanAThousand = d => d < 1000;

        //    Func<double, string> errorGenerator = d => $"{d} failed validation";

        //    (Func<double, bool> check, Func<double, string> errorGenerator)[] checksArray =
        //        new[] {
        //        (isPositive, errorGenerator),
        //        (IsDivisibleByTen, errorGenerator),
        //        (isEven, errorGenerator),
        //        (isLessThanAThousand, errorGenerator)
        //        };

        //    // Act
        //    bool result = Check.That(num).IsAll(checksArray);

        //    // Assert
        //    result.ShouldBeTrue();
        //}
    }

    class ErrorGenerator
    {
        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsTrue()
        {
            // Arrange
            var num = 1.0d;
            Func<double, bool> isPositive = (d) => d > 0;
            Func<double, string> errorGenerator = (d) => $"The value: {d} failed validation.";
            // Act
            var result = Check.That(num).Is(isPositive, errorGenerator);
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBeNull(),
                () => result.IsSuccess.ShouldBeTrue()
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsFalse()
        {
            // Arrange
            var num = -1d;
            Func<double, bool> isPositive = (d) => d > 0;
            Func<double, string> errorGenerator = (d) => $"The value: {d} failed validation.";
            // Act
            var result = Check.That(num).Is(isPositive, errorGenerator);
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("The value: -1 failed validation.")
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, string> isNotPositiveError = (d) => $"Validation failed: {d} is not positive.";
            Func<double, bool> isZero = d => d == 0;
            Func<double, string> isNotZeroError = (d) => $"Validation failed: {d} is not zero.";

            // Act
            var result = Check.That(num)
                .Is(isPositive, isNotPositiveError)
                .And(isZero, isNotZeroError);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("Validation failed: 1 is not zero.")
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctionsAndObtainTheResultOfAllOfThem_AllShouldPass()
        {
            // Arrange
            var num = 100d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> isGreaterThanFifty = d => d > 50;
            Func<double, string> errorGenerator = (d) => $"{d} did not pass validation";
            // Act
            bool result = Check.That(num)
                .Is(isPositive, errorGenerator)
                .And(isGreaterThanFifty, errorGenerator);
            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void WhenGivenAnArrayOfChecks_ShouldCheckAllOfThem()
        {
            // Arrange
            var num = 100d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> IsDivisibleByTen = d => d % 10 == 0;
            Func<double, bool> isEven = d => d % 2 == 0;
            Func<double, bool> isLessThanAThousand = d => d < 1000;

            Func<double, string> errorGenerator = d => $"{d} failed validation";

            (Func<double, bool> check, Func<double, string> errorGenerator)[] checksArray =
                new[] {
                (isPositive, errorGenerator),
                (IsDivisibleByTen, errorGenerator),
                (isEven, errorGenerator),
                (isLessThanAThousand, errorGenerator)
                };

            // Act
            bool result = Check.That(num).IsAll(checksArray);

            // Assert
            result.ShouldBeTrue();
        }
    }

}
