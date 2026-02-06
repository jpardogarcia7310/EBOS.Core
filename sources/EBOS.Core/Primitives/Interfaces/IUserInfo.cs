namespace EBOS.Core.Primitives.Interfaces;

/// <summary>
/// Basic user information in the current context.
/// </summary>
public interface IUserInfo
{
    int Id { get; set; }
    string Username { get; set; }
    string Name { get; set; }
    int LanguageId { get; set; }
    string Email { get; set; }
    int ConnectionId { get; set; }
}