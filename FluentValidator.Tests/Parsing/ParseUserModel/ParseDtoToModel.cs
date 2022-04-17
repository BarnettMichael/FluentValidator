using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentValidator.Tests.Parsing.ParseUserModel;
internal class ParseDtoToModel
{
    private User ConvertToUser(UserDto dto)
    {
        // Check if any of the mandatory fields are missing.
        if (dto.Id is null || dto.Id <= 0)
        {
            throw new ArgumentException("User.Id must be populated with a positive integer");
        }
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            throw new ArgumentException("FirstName cannot be empty");
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            throw new ArgumentException("LastName cannot be empty");
        }
        // Clean up the remaining fields so that they are acceptable within the domain.
        var id = dto.Id.Value;
        var firstName = dto.FirstName.Trim();
        var lastName = dto.LastName.Trim();
        var userName = dto.UserName?.Trim() ?? String.Empty;
        var displayName = dto.DisplayName?.Trim() ?? String.Empty;
        var posts = new List<UserPost>();
        foreach (var post in dto.Posts ?? Enumerable.Empty<UserPostDto>())
        {
            if (post.Id is not null && post.Id > 0)
            {
                var p = new UserPost(post.Id.Value, post.Content?.Trim() ?? String.Empty, post?.IsPublished ?? false);
                posts.Add(p);
            }
        }

        return new(id, userName, displayName, firstName, lastName, posts.ToArray());

    }

    [Test]
    public void WhenGivenADto_ShouldBeAbleToParseToAModel()
    {
        // Arrange
        var inputPosts = new UserPostDto[] {
            new UserPostDto(1, "Hello this is my first test post", IsPublished: true),
            new UserPostDto(2, "Post number two", IsPublished: false),
            new UserPostDto(3, "Oh no I didn't post publish my second post...", IsPublished: true)
            };

        var inputUser = new UserDto(4, "TestUser1", "DispName01", "John", "Smith", inputPosts);
        // Act
        var result = Parse<UserDto>.This(inputUser).As(ConvertToUser);

        result.Value.ShouldNotBeNull();

        // Assert
        result.ShouldSatisfyAllConditions(
            () => result.IsParsedSuccessfully.ShouldBeTrue(),
            () => result.Value.Id.ShouldBe(4),
            () => result.Value.FirstName.ShouldBe("John"),
            () => result.Value.LastName.ShouldBe("Smith"),
            () => result.Value.DisplayName.ShouldBe("DispName01"),
            () => result.Value.UserName.ShouldBe("TestUser1")
            );
    }
}
