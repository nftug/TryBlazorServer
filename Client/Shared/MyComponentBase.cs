using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Components;

namespace Client.Shared;

public class MyComponentBase : ComponentBase
{
    [Inject]
    protected IMediator Mediator { get; set; } = null!;
    [Inject]
    protected IHttpContextAccessor httpContextAccessor { get; set; } = null!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    public string _userId =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? string.Empty;

    public string _userName =>
        httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name)
        ?? string.Empty;
}
