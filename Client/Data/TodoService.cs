using Application.Shared.Enums;
using Application.Todos.Models;
using Application.Todos.UseCases;
using Domain.Todos.Queries;
using Domain.Todos.ValueObjects;
using MediatR;
using Pagination.EntityFrameworkCore.Extensions;

namespace Client.Data;

public class TodoService
{
    private readonly UserInfoService _userInfo;
    private readonly IMediator _mediator;

    public TodoService(IMediator mediator, UserInfoService userInfo)
    {
        _mediator = mediator;
        _userInfo = userInfo;
    }

    public static TodoCommandDTO ResultToCommand(TodoResultDTO origin)
        => new()
        {
            Id = origin.Id,
            Title = origin.Title,
            Description = origin.Description,
            StartDate = origin.StartDate,
            EndDate = origin.EndDate,
            State = origin.State.Value
        };

    public async Task<Pagination<TodoResultDTO>> GetList(TodoQueryParameter param)
        => await _mediator.Send(new List.Query(param, _userInfo.Id));

    public async Task<TodoResultDTO> Get(Guid id)
        => await _mediator.Send(new Details.Query(id, _userInfo.Id));

    public async Task<TodoResultDTO> Put(Guid id, TodoCommandDTO command)
        => await _mediator.Send(new Edit.Command(id, command, _userInfo.Id, EditMode.Put));

    public async Task<TodoResultDTO> Patch(Guid id, TodoCommandDTO command)
        => await _mediator.Send(new Edit.Command(id, command, _userInfo.Id, EditMode.Patch));

    public async Task<TodoResultDTO> Create(TodoCommandDTO command)
        => await _mediator.Send(new Create.Command(command, _userInfo.Id));

    public async Task Delete(Guid id)
        => await _mediator.Send(new Delete.Command(id, _userInfo.Id));

    public async Task<TodoResultDTO> ChangeState(Guid id, TodoState state)
    {
        var command = new TodoStateCommand { State = state.Value };
        return await _mediator.Send(new EditState.Command(id, command, _userInfo.Id, EditMode.Patch));
    }
}
