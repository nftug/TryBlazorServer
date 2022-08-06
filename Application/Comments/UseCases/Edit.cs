using Domain.Comments.Entities;
using Application.Shared.UseCases;
using Application.Comments.Models;
using Domain.Shared.Interfaces;
using Domain.Services;

namespace Application.Comments.UseCases;

public class Edit
    : EditBase<Comment, CommentResultDTO, CommentCommandDTO>
{
    public class Handler : HandlerBase
    {
        public Handler(
            IRepository<Comment> repository,
            IDomainService<Comment> domain
        ) : base(repository, domain)
        {
        }

        protected override CommentResultDTO CreateDTO(Comment item)
            => new(item);

        protected override void Patch(Comment origin, CommentCommandDTO item)
        {
            origin.Edit(item.Content != null ? new(item.Content) : origin.Content);
        }

        protected override void Put(Comment origin, CommentCommandDTO item)
        {
            origin.Edit(new(item.Content));
        }
    }
}
