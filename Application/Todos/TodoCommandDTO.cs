using Domain.Todos;
using Application.Shared;

namespace Application.Todos;

public class TodoCommandDTO : ValidatableDTOBase
{
    public Guid Id { get; set; }
    public string? Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? BeginDateTime { get; set; }
    public DateTime? DueDateTime { get; set; }
    public int? State { get; set; }

    protected override void AssertCanCreate()
    {
        Todo.CreateNew(
            new TodoTitle(Title),
            new TodoDescription(Description),
            new TodoPeriod(BeginDateTime, DueDateTime),
            new TodoState(State ?? 0),
            string.Empty
        );
    }
}
