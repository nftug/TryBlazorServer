using Microsoft.AspNetCore.Components;
using Pagination.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Components.Forms;
using Application.Todos.Models;
using Domain.Todos.Queries;
using Client.Data;
using Domain.Todos.ValueObjects;
using Client.Models;

namespace Client.Pages;

public partial class FetchTodo : MyComponentBase
{
    [Inject]
    protected TodoService TodoService { get; set; } = null!;

    [SupplyParameterFromQuery]
    [Parameter]
    public string Page { get; set; } = null!;

    [SupplyParameterFromQuery]
    [Parameter]
    public string Q { get; set; } = null!;

    [SupplyParameterFromQuery]
    [Parameter]
    public string State { get; set; } = null!;

    protected TodoCommandDTO TodoCommand { get; set; } = new TodoCommandDTO();
    protected Pagination<TodoResultDTO>? TodoItems { get; set; } = null;
    protected EditContext EditContext = null!;

    protected override void OnInitialized()
    {
        EditContext = new EditContext(TodoCommand);
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        await FetchData();
        await base.OnParametersSetAsync();
    }

    protected async Task FetchData()
    {
        // 画面更新時のバリデーションエラー抑制
        EditContext = new EditContext(TodoCommand);

        var param = new TodoQueryParameter
        {
            Limit = 10,
            Page = ParsePage(Page),
            Q = Q,
            State = ParseState(State)
        };

        await InvokeAsync(async () =>
        {
            TodoItems = await TodoService.GetList(param);
            StateHasChanged();
        });
    }

    protected void ResultToCommand(TodoResultDTO item)
    {
        TodoCommand = TodoService.ResultToCommand(item);
        EditContext = new EditContext(TodoCommand);
    }

    protected void ResetForm()
    {
        TodoCommand = new TodoCommandDTO();
        EditContext = new EditContext(TodoCommand);
    }


    protected async Task OnValidSubmit()
    {
        bool isNewData = TodoCommand.Id == null;
        if (isNewData)
            await TodoService.Create(TodoCommand);
        else
            await TodoService.Put((Guid)TodoCommand.Id!, TodoCommand);

        ResetForm();

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
            await FetchData();
    }

    protected async Task ChangeState(Guid id, TodoState state)
    {
        await TodoService.ChangeState(id, state);
        await FetchData();
    }

    protected async Task Delete(Guid id)
    {
        await TodoService.Delete(id);
        await FetchData();
        ResetForm();
    }

    // TODO: あとで文字列でも検索できるようにする (QueryParameterから書き換える)
    private static int? ParseState(string? value)
        => ParseIntParam(value, (x, canParse) => canParse ? x : null);
}
