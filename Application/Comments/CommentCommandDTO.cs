using Application.Shared;
using Domain.Comments;

namespace Application.Comments;

public class CommentCommandDTO : ValidatableDTOBase
{
    public Guid Id { get; set; }
    public string? Content { get; set; } = null!;
    public Guid TodoId { get; set; }

    protected override void AssertCanCreate()
    {
        Comment.CreateNew(new CommentContent(Content), TodoId, string.Empty);
    }
}
