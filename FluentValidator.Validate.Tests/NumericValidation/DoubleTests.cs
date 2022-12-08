using System;
using FluentValidator.Validate;
using FluentValidator.Validate.Tests.Models;

namespace FluentValidator.Tests.NumericValidation;

/// <summary>
/// Tests that enumerate basic examples of using the <see cref="Check{T}"/> class.
/// Which return a <see cref="Result{T, E}"/> where options for more advanced error reporting are used.
/// </summary>
[TestFixture]
internal class DoubleTests
{
    class ErrorString
    {
        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsFalse()
        {
            // Arrange
            var num = -1d;
            Func<double, bool> isPositive = (d) => d > 0;
            // Act
            var result = Check.That(num).Is(isPositive, "The value is not positive.");
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("The value is not positive.")
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> IsOdd = d => d % 2 == 1;
            Func<double, bool> isZero = d => d == 0;

            string isNotOddError = "The value is not odd";
            string isNotZeroError = "The value is not zero.";

            // Act
            var result = Check.That(num)
                .Is(isPositive, "The value is not positive.")
                .And(IsOdd, isNotOddError)
                .And(isZero, isNotZeroError);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("The value is not zero.")
                );
        }

        /*
         * Not sure if want to allow conversion from Result<TEntity> to Result<TEntity,TError>.
        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_OneHardcoded_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> IsOdd = d => d % 2 == 1;
            Func<double, bool> isZero = d => d == 0;

            string isNotPositiveError = "The value is not positive.";
            string isNotOddError = "The value is not odd";
            string isNotZeroError = "The value is not zero.";

            // Act
            var result = Check.That(num)
                .Is(isPositive, isNotPositiveError)
                .AndAll((IsOdd, isNotOddError), (isZero, isNotZeroError));

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("The value is not zero.")
                );
        }
        */
        [Test]
        public void WhenGivenAnArrayOfChecks_ShouldCheckInSeries_UntilOneFails()
        {
            // Arrange
            var num = 998d;
            (Func<double, bool> check, string error) isPositive = (d => d > 0, "value is not positive");
            (Func<double, bool> check, string error) isEven = (d => d % 2 == 0, "value is not even");
            (Func<double, bool> check, string error) IsDivisibleByTen = (d => d % 10 == 0, "value is not divisible by ten");
            (Func<double, bool> check, string error) isLessThanAThousand = (d => d < 1000, "value is not less than 1000");

            // Act
            var result = Check.That(num).IsAll(isPositive, isEven, IsDivisibleByTen, isLessThanAThousand);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBe("value is not divisible by ten"),
                () => result.IsSuccess.ShouldBeFalse()
                );
        }
    }

    class ErrorGenerator
    {

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
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_OneHardcoded_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> IsOdd = d => d % 2 == 1;
            Func<double, bool> isZero = d => d == 0;

            Func<double, string> isNotPositiveErrorGenerator = d => $"The value:{d} is not positive.";
            Func<double, string> isNotOddErrorGenerator = d => $"The value:{d} is not odd";
            Func<double, string> isNotZeroErrorGenerator = d => $"The value:{d} is not zero.";

            // Act
            var result = Check.That(num)
                .Is(isPositive, isNotPositiveErrorGenerator)
                .AndAll((IsOdd, isNotOddErrorGenerator), (isZero, isNotZeroErrorGenerator));

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe("The value:1 is not zero.")
                );
        }

        [Test]
        public void WhenGivenAnArrayOfChecks_ShouldCheckInSeries_UntilOneFails()
        {
            // Arrange
            var num = 998d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> isEven = d => d % 2 == 0;
            Func<double, bool> IsDivisibleByTen = d => d % 10 == 0;
            Func<double, bool> isLessThanAThousand = d => d < 1000;

            (Func<double, bool> check, Func<double, string> errorGenerator)[] checksArray =
                new (Func<double, bool>, Func<double,string>)[] {
                (isPositive, v => $"value:{v} is not positive"),
                (isEven, v => $"value:{v} is not even"),
                (IsDivisibleByTen, v => $"value:{v} is not divisible by ten"),
                (isLessThanAThousand, v => $"value:{v} is not less than 1000")
                };

            // Act
            var result = Check.That(num).IsAll(checksArray);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBe("value:998 is not divisible by ten"),
                () => result.IsSuccess.ShouldBeFalse()
                );
        }
    }

    class DeclaredError
    {
        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassAFunctionToValidateAPropertyOfTheDoubleAsFalse()
        {
            // Arrange
            var num = -1d;
            Func<double, bool> isPositive = (d) => d > 0;
            // Act
            var result = Check.That(num).Is(isPositive, ErrorType.NotPositive);
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe(ErrorType.NotPositive)
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> isZero = d => d == 0;

            // Act
            var result = Check.That(num)
                .Is(isPositive, ErrorType.NotPositive)
                .And(isZero, ErrorType.NotZero);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe(ErrorType.NotZero)
                );
        }

        [Test]
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctions_OneHardcoded_GivenOneWillFail_ShouldFailWithCorrectError()
        {
            // Arrange
            var num = 1d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> IsOdd = d => d % 2 == 1;
            Func<double, bool> isZero = d => d == 0;

            // Act
            var result = Check.That(num)
                .Is(isPositive, ErrorType.NotPositive)
                .AndAll((IsOdd, ErrorType.NotOdd), (isZero, ErrorType.NotZero));

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Error.ShouldBe(ErrorType.NotZero)
                );
        }

        [Test]
        public void WhenGivenAnArrayOfChecks_ShouldCheckInSeries_UntilOneFails()
        {
            // Arrange
            var num = 998d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> isEven = d => d % 2 == 0;
            Func<double, bool> IsDivisibleByTen = d => d % 10 == 0;
            Func<double, bool> isLessThanAThousand = d => d < 1000;

            (Func<double, bool> check, ErrorType error)[] checksArray =
                new [] {
                (isPositive, ErrorType.NotPositive),
                (isEven, ErrorType.NotEven),
                (IsDivisibleByTen, ErrorType.NotDivisibleBy10),
                (isLessThanAThousand, ErrorType.NotLessThan1000)
                };

            // Act
            var result = Check.That(num).IsAll(checksArray);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBe(ErrorType.NotDivisibleBy10),
                () => result.IsSuccess.ShouldBeFalse()
                );
        }

    }

    class ValidationSuccess
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
        public void WhenGivenADouble_ShouldBeAbleToPassMultipleFunctionsAndObtainTheResultOfAllOfThem_AllShouldPass()
        {
            // Arrange
            var num = 100d;
            Func<double, bool> isPositive = d => d > 0;
            Func<double, bool> isGreaterThanFifty = d => d > 50;
            Func<double, string> errorGenerator = (d) => $"{d} did not pass validation";
            // Act
            var result = Check.That(num)
                .Is(isPositive, errorGenerator)
                .And(isGreaterThanFifty, errorGenerator);
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBeNull(),
                () => result.IsSuccess.ShouldBeTrue()
                );
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
            var result = Check.That(num).IsAll(checksArray);

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Error.ShouldBeNull(),
                () => result.IsSuccess.ShouldBeTrue()
                );
        }

    }


}
