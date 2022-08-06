using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Domain.Comments.Entities;
using Infrastructure.Comments;
using Domain.Comments.Services;
using Domain.Todos.Entities;
using Infrastructure.Todos;
using Domain.Users.Entities;
using Infrastructure.Users;
using Domain.Todos.Services;
using Domain.Users.Services;
using Domain.Shared.Interfaces;
using Domain.Services;

namespace Client.Extensions;

internal static class ApplicationServiceExtension
{
    internal static IServiceCollection AddApplicationServices
        (this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<DataContext>(opt =>
        {
            // opt.UseInMemoryDatabase("TodoList");
            opt.UseSqlite(config.GetConnectionString("DefaultConnection"));
        });

        services.AddMediatR(typeof(Application.Todos.UseCases.List.Query).Assembly);

        // repositories
        services.AddTransient<IRepository<Todo>, TodoRepository>();
        services.AddTransient<IRepository<Comment>, CommentRepository>();
        services.AddTransient<IRepository<User>, UserRepository>();

        // Query services
        services.AddTransient<IFilterQueryService<Todo>, TodoFilterQueryService>();
        services.AddTransient<IFilterQueryService<Comment>, CommentFilterQueryService>();

        // domain services
        services.AddScoped<IDomainService<Todo>, TodoService>();
        services.AddScoped<IDomainService<Comment>, CommentService>();
        services.AddScoped<IDomainService<User>, UserService>();

        return services;
    }
}