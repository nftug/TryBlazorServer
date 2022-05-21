using Application.Todos;
using Infrastructure.Todos;
using Microsoft.AspNetCore.Components;
using Pagination.EntityFrameworkCore.Extensions;
using Client.Shared;
using Microsoft.AspNetCore.Components.Forms;
using Domain.Todos;

namespace Client.Pages;

public class FetchTodoModel : MyComponentBase
{
    [SupplyParameterFromQuery]
    [Parameter]
    public string Page { get; set; } = null!;
    [SupplyParameterFromQuery]
    [Parameter]
    public string Q { get; set; } = null!;
    [SupplyParameterFromQuery]
    [Parameter]
    public string State { get; set; } = null!;

    protected TodoCommandDTO TodoData { get; set; } = new TodoCommandDTO();
    protected Pagination<TodoResultDTO>? TodoItems = null;
    protected EditContext EditContext = null!;

    protected override void OnInitialized()
    {
        EditContext = new EditContext(TodoData);
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        // ページ遷移時のバリデーションエラーの抑制
        EditContext = new EditContext(TodoData);

        var param = new TodoQueryParameter { Limit = 10 };
        param.Page = ParsePage(Page);
        param.q = Q;
        param.State = ParseState(State);

        TodoItems = await Mediator.Send(new List.Query(param, _userId));
    }

    protected async Task OnValidSubmit()
    {
        bool isNewData = TodoData.Id == new Guid();
        if (isNewData)
            await Mediator.Send(new Create.Command(TodoData, _userId));
        else
            await Mediator.Send(new Edit.Command(TodoData.Id, TodoData, _userId, false));

        OnClickReset();

        bool isParameterChanged = ParsePage(Page) != 1 || !string.IsNullOrEmpty(Q);

        if (isNewData && isParameterChanged)
            NavigationManager.NavigateTo(
                NavigationManager.GetUriWithQueryParameters(
                    new Dictionary<string, object?> {
                        {"page", null}, {"q", null}
                    })
            );
        else
            await OnParametersSetAsync();
    }

    protected void OnClickEdit(TodoResultDTO item)
    {
        TodoData = new TodoCommandDTO
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            BeginDateTime = item.BeginDateTime,
            DueDateTime = item.DueDateTime,
            State = item.State.Value
        };
        EditContext = new EditContext(TodoData);
    }

    protected async Task OnClickChangeState(TodoResultDTO item, TodoState state)
    {
        TodoData = new TodoCommandDTO
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            BeginDateTime = item.BeginDateTime,
            DueDateTime = item.DueDateTime,
            State = state.Value
        };
        await OnValidSubmit();
    }

    protected async Task OnClickDelete(Guid id)
    {
        await Mediator.Send(new Delete.Command(id, _userId));
        await OnParametersSetAsync();
        OnClickReset();
    }

    protected void OnClickReset()
    {
        TodoData = new TodoCommandDTO();
        EditContext = new EditContext(TodoData);
    }

    private int? ParseIntParam(string? value, Func<int, bool, int?> func)
    {
        int _value;
        bool canParse = Int32.TryParse(value, out _value);
        return func(_value, canParse);
    }

    private int? ParsePage(string? value)
        => ParseIntParam(value, (x, _) => x > 0 ? x : 1);

    // TODO: あとで文字列でも検索できるようにする (QueryParameterから書き換える)
    private int? ParseState(string? value)
        => ParseIntParam(value, (x, canParse) => canParse ? x : null);
}
