using Microsoft.AspNetCore.Components.Authorization;
using Client.Areas.Identity;
using Client.Data;
using Infrastructure;
using Infrastructure.DataModels;
using Client.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
IConfiguration config = builder.Configuration;
builder.Services.AddApplicationServices(config);

builder.Services.AddDefaultIdentity<UserDataModel<Guid>>(opt =>
            opt.SignIn.RequireConfirmedAccount = true
        ).AddEntityFrameworkStores<DataContext>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<UserDataModel<Guid>>>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<UserInfoService>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<CountIncrementService>();
builder.Services.AddScoped<TodoService>();

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
