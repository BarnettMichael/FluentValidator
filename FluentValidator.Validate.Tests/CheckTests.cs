using System;
using FluentValidator.Validate.Tests.Models;

namespace FluentValidator.Validate.Tests;
internal class CheckTests
{
    [Test]
    public void WhenGivenNull_ShouldNotAllowCreationOfASeed()
    {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        Should.Throw<ArgumentNullException>(() => Check.That<User>(value: null));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    }

    [Test]
    public void WhenGivenAnObject_ShouldReturnASeed()
    {
        var seed = Check.That(new User { Id = 1, UserName = "TestUserName" });

        seed.ShouldSatisfyAllConditions(
            () => seed.ShouldNotBeNull(),
            () => seed.Value.ShouldBeOfType<User>()
            );

        var user = seed.Value;
        user.ShouldSatisfyAllConditions(
            () => user.ShouldNotBeNull(),
            () => user.Id.ShouldBe(1),
            () => user.UserName.ShouldBe("TestUserName")
            );
    }
}
