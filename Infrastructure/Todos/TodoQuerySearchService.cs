using Microsoft.EntityFrameworkCore;
using Infrastructure.DataModels;
using Infrastructure.Shared;

namespace Infrastructure.Todos;

public class TodoQuerySearchService
{
    private readonly ApplicationDbContext _context;

    public TodoQuerySearchService(ApplicationDbContext context)
    {
        _context = context;
    }

    public IQueryable<TodoDataModel> GetFilteredQuery(TodoQueryParameter param)
    {
        var query = _context.Todo.Include(x => x.Comments)
                                  .Include(x => x.OwnerUser)
                                  .AsQueryable();

        // qで絞り込み
        QuerySearchManager.ForEachKeyword(param.q, q =>
        {
            query = query.Where(x =>
                x.Title.ToLower().Contains(q) ||
                x.Description!.ToLower().Contains(q) ||
                x.Comments.Any(x => x.Content.ToLower().Contains(q)) ||
                x.OwnerUser!.UserName.ToLower().Contains(q)
            );
        });

        // タイトルで絞り込み
        QuerySearchManager.ForEachKeyword(param.Title, name =>
        {
            query = query.Where(x =>
                x.Title.ToLower().Contains(name)
            );
        });

        // 説明文で絞り込み
        QuerySearchManager.ForEachKeyword(param.Description, name =>
        {
            query = query.Where(x =>
                x.Description!.ToLower().Contains(name)
            );
        });

        // コメントで絞り込み
        QuerySearchManager.ForEachKeyword(param.Comment, comment =>
        {
            query = query.Where(x =>
                x.Comments.Any(x => x.Content.ToLower().Contains(comment))
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

        // 状態で絞り込み
        if (param.State != null)
            query = query.Where(x => x.State == param.State);

        return query;
    }
}
