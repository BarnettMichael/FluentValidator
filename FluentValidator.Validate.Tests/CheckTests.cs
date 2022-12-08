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
}
