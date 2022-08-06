using System.Security.Claims;

namespace Client.Data;

public class UserInfoService
{
    private IHttpContextAccessor HttpContextAccessor { get; }

    public UserInfoService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => HttpContextAccessor.HttpContext?.User;

    public Guid Id
        => User != null
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier))
            : new Guid();

    public string Name => User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
}
