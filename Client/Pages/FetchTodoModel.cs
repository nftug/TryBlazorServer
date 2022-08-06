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

        var param = new TodoQueryParameter
        {
            Limit = 10,
            Page = ParsePage(Page),
            q = Q,
            State = ParseState(State)
        };

        TodoItems = await Mediator.Send(new List.Query(param, _userId));
    }

    protected async Task OnValidSubmit()
    {
        bool isNewData = TodoData.Id == new Guid();
        if (isNewData)
            await Mediator.Send(new Create.Command(TodoData, _userId));
        else
            await Mediator.Send(new Edit.Command(TodoData.Id, TodoData, _userId));

        OnClickReset();

        bool isParameterChanged =
            ParsePage(Page) != 1
             || !string.IsNullOrEmpty(Q)
             || !string.IsNullOrEmpty(State);

        if (isNewData && isParameterChanged)
            NavigationManager.NavigateTo(
                NavigationManager.GetUriWithQueryParameters(
                    new Dictionary<string, object?> {
                        {"page", null}, {"q", null}, {"state", null}
                    }));
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
        var todo = new TodoCommandDTO
        {
            Id = item.Id,
            Title = item.Title,
            Description = item.Description,
            BeginDateTime = item.BeginDateTime,
            DueDateTime = item.DueDateTime,
            State = state.Value
        };
        await Mediator.Send(new Edit.Command(todo.Id, todo, _userId));
        await OnParametersSetAsync();
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

    private static int? ParseIntParam(string? value, Func<int, bool, int?> func)
    {
        bool canParse = int.TryParse(value, out int _value);
        return func(_value, canParse);
    }

    private static int? ParsePage(string? value)
        => ParseIntParam(value, (x, _) => x > 0 ? x : 1);

    // TODO: あとで文字列でも検索できるようにする (QueryParameterから書き換える)
    private static int? ParseState(string? value)
        => ParseIntParam(value, (x, canParse) => canParse ? x : null);
}
