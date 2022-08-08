using Client.Data;
using Microsoft.AspNetCore.Components;

namespace Client.Models;

public abstract class MyComponentBase : ComponentBase
{
    [Inject]
    protected UserInfoService UserInfo { get; set; } = null!;
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = null!;

    protected static int? ParseIntParam(string? value, Func<int, bool, int?> func)
    {
        bool canParse = int.TryParse(value, out int _value);
        return func(_value, canParse);
    }

    protected static int? ParsePage(string? value)
        => ParseIntParam(value, (x, _) => x > 0 ? x : 1);
}
