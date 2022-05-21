using Application.Shared;
using Domain.Users;

namespace Application.Users;

public class UserCommandDTO : ValidatableDTOBase
{
    public string? Username { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;

    protected override void AssertCanCreate()
    {
        new UserUserName(Username);
        new UserEmail(Email);
    }
}
