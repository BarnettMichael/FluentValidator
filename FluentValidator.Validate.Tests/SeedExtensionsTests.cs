using System;
using FluentValidator.Validate.Tests.Models;

namespace FluentValidator.Validate.Tests;
/// <summary>
/// Unit tests around functionality of the extension methods in the <see cref="SeedExtensions"/>
/// class. These tests are not intended as examples of use cases.
/// </summary>
[TestFixture]
internal class SeedExtensionsTests
{
    class IsAll
    {
        class NoErrorReporting
        {
            [Test]
            public void WhenGivenAnEmptyArray_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[] { };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleValidCheck_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[]
                {
                    i => i > 50
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleValidChecks_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[]
                {
                    i => i > 50,
                    i => i % 2 == 0,
                    i => i < 1000
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleInvalidCheck_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[]
                {
                    i => i < 50
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleChecks_OneIsInvalid_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[]
                {
                    i => i > 50,
                    i => i % 2 == 1,
                    i => i < 1000
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleInvalidChecks_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new Func<int, bool>[]
                {
                    i => i < 50,
                    i => i % 2 == 1,
                    i => i > 1000
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(String.Empty),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

        }

        class WithSpecifiedErrorType
        {
            [Test]
            public void WhenGivenAnEmptyArray_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, ErrorType)[] { };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(ErrorType.None),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleValidCheck_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, string)[]
                {
                    (i => i > 50, "Not greater than 50")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(null),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleValidChecks_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, string)[]
                {
                    (i => i > 50, "Not greater than 50"),
                    (i => i % 2 == 0, "Not divisible by 2"),
                    (i => i < 1000, "Not less than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(null),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleInvalidCheck_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, string)[]
                {
                    (i => i < 50, "Not greater than 50")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Not greater than 50"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleChecks_OneIsInvalid_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, string)[]
                {
                    (i => i > 50, "Not greater than 50"),
                    (i => i % 2 == 1, "Is not odd"),
                    (i => i < 1000, "Not less than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Is not odd"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleInvalidChecks_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, string)[]
                {
                    (i => i < 50, "Not less than 50"),
                    (i => i % 2 == 1, "Not odd"),
                    (i => i > 1000, "Not more than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Not less than 50"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }
        }

        class WithErrorGenerator
        {
            [Test]
            public void WhenGivenAnEmptyArray_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[] { };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBeNull(),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleValidCheck_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[]
                {
                    (i => i > 50, i => $"Value: {i} not greater than 50")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(null),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleValidChecks_ShouldReturnTrue()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[]
                {
                    (i => i > 50, i => $"Value: {i} not greater than 50"),
                    (i => i % 2 == 0, i => $"Value: {i} not divisible by 2"),
                    (i => i < 1000, i => $"Value: {i} not less than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe(null),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeTrue()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingASingleInvalidCheck_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[]
                {
                    (i => i < 50, i => $"Value: {i} is not less than 50")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Value: 100 is not less than 50"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleChecks_OneIsInvalid_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[]
                {
                    (i => i > 50, i => $"Value: {i} not more than 50"),
                    (i => i % 2 == 1, i => $"Value: {i} is not odd"),
                    (i => i < 1000, i => $"Value: {i} not less than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

                // Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Value: 100 is not odd"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }

            [Test]
            public void WhenGivenAnArrayContainingMultipleInvalidChecks_ShouldReturnFalse()
            {
                // Arrange
                var value = 100;
                var checks = new (Func<int, bool>, Func<int, string>)[]
                {
                    (i => i < 50, i => $"Value: {i} is not less than 50"),
                    (i => i % 2 == 1, i => $"Value: {i} not odd"),
                    (i => i > 1000, i => $"Value: {i} not more than 1000")
                };

                // Act
                var result = Check.That(value).IsAll(checks);

// Assert
                result.ShouldSatisfyAllConditions(
                    () => result.Error.ShouldBe("Value: 100 is not less than 50"),
                    () => result.Value.ShouldBe(value),
                    () => result.IsSuccess.ShouldBeFalse()
                    );
            }
        }
    }
}
