using Microsoft.AspNetCore.Components;
using Pagination.EntityFrameworkCore.Extensions;
using Application.Todos.Models;
using Domain.Todos.Queries;
using Client.Data;
using Domain.Todos.ValueObjects;
using Client.Models;
using Client.Components;

namespace Client.Pages;

public partial class FetchTodo : MyComponentBase
{
    [Inject]
    private TodoService TodoService { get; set; } = null!;

    [SupplyParameterFromQuery]
    [Parameter]
    public string Page { get; set; } = null!;
    [SupplyParameterFromQuery]
    [Parameter]
    public string Q { get; set; } = null!;
    [SupplyParameterFromQuery]
    [Parameter]
    public string State { get; set; } = null!;

    private TodoCommandDTO TodoCommand { get; set; } = new TodoCommandDTO();
    private Pagination<TodoResultDTO>? TodoItems { get; set; } = null;
    private TodoForm? TodoForm { get; set; } = null!;
    private bool IsLoading { get; set; }

    protected override void OnParametersSet()
    {
        FetchData();
    }

    protected void FetchData()
    {
        var param = new TodoQueryParameter
        {
            Limit = 10,
            Page = ParsePage(Page),
            Q = Q,
            State = ParseState(State)
        };

        InvokeAsync(async () =>
        {
            IsLoading = true;
            // await Task.Delay(500);

            TodoItems = await TodoService.GetList(param);

            IsLoading = false;
            StateHasChanged();
        });
    }

    protected void ResultToCommand(TodoResultDTO item)
    {
        TodoCommand = TodoService.ResultToCommand(item);
    }

    protected async Task OnValidSubmit()
    {
        bool isNewData = TodoCommand.Id == null;
        if (isNewData)
            await TodoService.Create(TodoCommand);
        else
            await TodoService.Put((Guid)TodoCommand.Id!, TodoCommand);

        TodoForm?.ResetForm();

        bool isParameterChanged =
            ParsePage(Page) != 1
             || !string.IsNullOrEmpty(Q)
             || !string.IsNullOrEmpty(State);

        if (isParameterChanged)
            NavigationManager.NavigateTo(
                NavigationManager.GetUriWithQueryParameters(
                    new Dictionary<string, object?> {
                        {"page", null}, {"q", null}, {"state", null}
                    }));
        else
            FetchData();
    }

    protected async Task ChangeState(Guid id, TodoState state)
    {
        await TodoService.ChangeState(id, state);
        FetchData();
    }

    protected async Task Delete(Guid id)
    {
        await TodoService.Delete(id);
        FetchData();
    }

    // TODO: あとで文字列でも検索できるようにする (QueryParameterから書き換える)
    private static int? ParseState(string? value)
        => ParseIntParam(value, (x, canParse) => canParse ? x : null);
}
