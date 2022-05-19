using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Client.Areas.Identity;
using Client.Data;
using Infrastructure;
using Application.Todos;
using Domain.Todos;
using Domain.Comments;
using Domain.Users;
using Infrastructure.Todos;
using Infrastructure.Comments;
using Infrastructure.Users;
using Infrastructure.DataModels;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
/* builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>(); */

builder.Services.AddDefaultIdentity<UserDataModel>(opt =>
            opt.SignIn.RequireConfirmedAccount = true
        ).AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<UserDataModel>>();

builder.Services.AddHttpContextAccessor();

// MediatR
builder.Services.AddMediatR(typeof(List.Query).Assembly);

// repositories
builder.Services.AddTransient<ITodoRepository, TodoRepository>();
builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();

// query services
builder.Services.AddTransient<TodoQuerySearchService>();
builder.Services.AddTransient<CommentQuerySearchService>();

builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
