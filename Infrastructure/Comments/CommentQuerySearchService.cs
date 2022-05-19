using Microsoft.EntityFrameworkCore;
using Infrastructure.DataModels;
using Infrastructure.Shared;

namespace Infrastructure.Comments;

public class CommentQuerySearchService
{
    private readonly ApplicationDbContext _context;

    public CommentQuerySearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<CommentDataModel> GetFilteredQuery(CommentQueryParameter param)
    {
        var query = _context.Comment.Include(x => x.OwnerUser)
                                     .AsQueryable();

        // qで絞り込み
        QuerySearchManager.ForEachKeyword(param.q, q =>
        {
            query = query.Where(x =>
                x.Content.ToLower().Contains(q)
            );
        });

        // 内容で絞り込み
        QuerySearchManager.ForEachKeyword(param.Content, name =>
        {
            query = query.Where(x =>
                x.Content.ToLower().Contains(name)
            );
        });

        // ユーザー名で絞り込み
        QuerySearchManager.ForEachKeyword(param.UserName, username =>
        {
            query = query.Where(x =>
                x.OwnerUser!.UserName.ToLower().Contains(username)
            );
        });

        // ユーザーIDで絞り込み
        if (!string.IsNullOrEmpty(param.UserId))
            query = query.Where(x => x.OwnerUserId == param.UserId);

        return query;
    }
}
