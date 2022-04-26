using BlazorServerToDoApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContextFactory<ToDoDbContext>(dbContextBuilder =>
{
    var connStr = builder.Configuration.GetConnectionString(nameof(ToDoDbContext));
    dbContextBuilder.UseSqlServer(connStr);
});

builder.Services.AddSingleton<IToDoItemRepository, DatabaseToDoItemRepository>();

builder.Services.AddIgniteUIBlazor();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
using (var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>())
{
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
