using Domain.Todos;
using Domain.Comments;
using Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Infrastructure.DataModels;

namespace Infrastructure.Todos;

public class TodoRepository : ITodoRepository
{
    private readonly ApplicationDbContext _context;

    public TodoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Todo> CreateAsync(Todo todo)
    {
        var todoDataModel = ToDataModel(todo);
        await _context.Todo.AddAsync(todoDataModel);
        await _context.SaveChangesAsync();

        return ToModel(todoDataModel);
    }

    public async Task<Todo> UpdateAsync(Todo todo)
    {
        var foundTodoDataModel = await _context.Todo
                                               .Include(x => x.Comments)
                                               .FirstOrDefaultAsync(
                                                   x => x.Id == todo.Id
                                               );
        if (foundTodoDataModel == null)
            throw new NotFoundException();

        var todoDataModel = Transfer(todo, foundTodoDataModel);

        _context.Todo.Update(todoDataModel);
        await _context.SaveChangesAsync();

        return ToModel(todoDataModel);
    }

    private TodoDataModel ToDataModel(Todo todo)
    {
        return new TodoDataModel
        {
            Title = todo.Title.Value,
            Description = todo.Description?.Value,
            BeginDateTime = todo.Period?.BeginDateTimeValue,
            DueDateTime = todo.Period?.DueDateTimeValue,
            State = todo.State.Value,
            Comments = new List<CommentDataModel>(),
            CreatedDateTime = todo.CreatedDateTime,
            UpdatedDateTime = todo.UpdatedDateTime,
            OwnerUserId = todo.OwnerUserId
        };
    }

    private TodoDataModel Transfer(Todo todo, TodoDataModel todoDataModel)
    {
        todoDataModel.Title = todo.Title.Value;
        todoDataModel.Description = todo.Description?.Value;
        todoDataModel.BeginDateTime = todo.Period?.BeginDateTimeValue;
        todoDataModel.DueDateTime = todo.Period?.DueDateTimeValue;
        todoDataModel.State = todo.State.Value;
        todoDataModel.CreatedDateTime = todo.CreatedDateTime;
        todoDataModel.UpdatedDateTime = todo.UpdatedDateTime;
        todoDataModel.OwnerUserId = todo.OwnerUserId;

        return todoDataModel;
    }

    public async Task<Todo?> FindAsync(Guid id)
    {
        var todoDataModel = await _context.Todo
                                          .Include(x => x.Comments)
                                          .FirstOrDefaultAsync(
                                              x => x.Id == id
                                          );

        return todoDataModel != null ? ToModel(todoDataModel) : null;
    }

    private Todo ToModel(TodoDataModel todoDataModel)
    {
        var comments = todoDataModel.Comments.Select(x =>
            new Comment(
                x.Id,
                new CommentContent(x.Content),
                x.TodoId,
                x.CreatedDateTime,
                x.UpdatedDateTime,
                x.OwnerUserId
            )).ToList();

        return new Todo(
            id: todoDataModel.Id,
            title: new TodoTitle(todoDataModel.Title),
            description: !string.IsNullOrWhiteSpace(todoDataModel.Description)
                ? new TodoDescription(todoDataModel.Description) : null,
            period: new TodoPeriod(todoDataModel.BeginDateTime, todoDataModel.DueDateTime),
            state: new TodoState(todoDataModel.State),
            comments: comments,
            createdDateTime: todoDataModel.CreatedDateTime,
            updatedDateTime: todoDataModel.UpdatedDateTime,
            ownerUserId: todoDataModel?.OwnerUserId
        );
    }

    public async Task RemoveAsync(Guid id)
    {
        var todoDataModel = await _context.Todo.FindAsync(id);

        if (todoDataModel == null)
            throw new NotFoundException();

        _context.Todo.Remove(todoDataModel);
        await _context.SaveChangesAsync();
    }
}
