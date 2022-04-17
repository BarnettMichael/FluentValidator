namespace FluentValidator.Tests.Parsing.ParseUserModel;

// Dto as if from an API or client etc.
internal record UserPostDto(int? Id, string? Content, bool? IsPublished);
internal record UserDto(int? Id, string? UserName, string? DisplayName, string? FirstName, string? LastName, UserPostDto[]? Posts);

// Domain models that data needs to be parsed into.
internal record UserPost(int Id, string Content, bool IsPublished);
internal record User(int Id, string UserName, string DisplayName, string FirstName, string LastName, UserPost[] Posts);