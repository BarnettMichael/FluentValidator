using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FluentValidator.Validate.Tests;
/// <summary>
/// Unit tests around functionality of the <see cref="Result{TEntity}"/> and <see cref="Result{TEntity, TError}"/>
/// classes, not already covered in the example scenario tests.
/// </summary>
[TestFixture]
internal class ResultExtensionsTests
{
    class AndAll
    {
        /*
         *  Not sure if want to allow conversion from Result<TEntity> to Result<TEntity, TError>.
        [Test]
        public void GivenAResultTEntity_WhenCheckingWithErrorGenerators_ShouldReturnResultTEntityTError()
        {
            // Arrange
            var r = new Result<decimal>(25.465m, IsSuccess: true);
            Func<decimal, bool> isPositive = d => d > 0;
            Func<decimal, bool> isEven = d => d % 2 == 0;

            Func<decimal, string> isPositiveErrorGenerator = d => $"{d} is not positive";
            Func<decimal, string> isEvenErrorGenerator = d => $"{d} is not even";

            // Act
            var result = r.AndAll(
                (isPositive, isPositiveErrorGenerator),
                (isEven, isEvenErrorGenerator)
                );

            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.IsSuccess.ShouldBeFalse(),
                () => result.Value.ShouldBe(25.465m),
                () => result.Error.ShouldBeOfType(typeof(string)),
                () => result.Error.ShouldBe("25.465 is not even")
                );
        }
        */
    }
}
