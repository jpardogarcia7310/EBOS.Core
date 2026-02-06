namespace EBOS.Core.Primitives.Interfaces;

/// <summary>
/// Basic user information in the current context.
/// </summary>
public interface IUserInfo
{
    long Id { get; set; }
    string Username { get; set; }
    string Name { get; set; }
    long LanguageId { get; set; }
    string Email { get; set; }
    long ConnectionId { get; set; }
}
