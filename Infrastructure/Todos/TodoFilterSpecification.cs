using Infrastructure.DataModels;
using System.Text.RegularExpressions;
using Domain.Todos.Entities;
using Domain.Todos.Queries;
using Domain.Shared.Queries;
using Infrastructure.Shared.Specifications.Filter.Extensions;
using Infrastructure.Shared.Specifications.Filter;

namespace Infrastructure.Todos;

internal class TodoFilterSpecification : FilterSpecificationBase<Todo, TodoDataModel>
{
    public TodoFilterSpecification(DataContext context)
        : base(context)
    {
    }

    protected override IQueryable<IDataModel<Todo>> GetQueryByParameter(
        IQueryable<IDataModel<Todo>> source,
        IQueryParameter<Todo> param
    )
    {
        var _param = (TodoQueryParameter)param;

        ExpressionGroups.AddSimpleSearch(_param.UserId, x => x.OwnerUserId == _param.UserId);

        ExpressionGroups.AddSimpleSearch(_param.State, x => x.State == _param.State);

        ExpressionGroups.AddSearch(_param.Q, k =>
            ExpressionCombiner.OrElse(
                k.Contains("Title"),
                k.Contains("Description"),
                k.ContainsInChildren<CommentDataModel>("Comments", "Content")));

        ExpressionGroups.AddSearch(_param.Title, k => k.Contains("Title"));

        ExpressionGroups.AddSearch(_param.Description, k => k.Contains("Description"));

        ExpressionGroups.AddSearch(_param.Comment, k =>
            ContainsInChildren<CommentDataModel>(k, "Comments", "Content"));

        ExpressionGroups.AddSearch(_param.UserName, k => k.ContainsInChild("OwnerUser", "UserName"));

        return source.OfType<TodoDataModel>().ApplyExpressionGroup(ExpressionGroups);
    }

    protected override IQueryable<TodoDataModel> OrderQuery(
        IQueryable<IDataModel<Todo>> query,
        IQueryParameter<Todo> param
    )
    {
        var _query = query.Cast<TodoDataModel>();
        bool isDescending = Regex.IsMatch(param.Sort, "^-");
        string orderBy = Regex.Replace(param.Sort, "^-", "");

        return orderBy switch
        {
            "createdOn" => _query.OrderByAscDesc(x => x.CreatedOn, isDescending),
            "updatedOn" => _query.OrderByAscDesc(x => x.UpdatedOn, isDescending),
            "title" => _query.OrderByAscDesc(x => x.Title, isDescending),
            _ => query.Cast<TodoDataModel>()
        };
    }
}
