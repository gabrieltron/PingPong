using Microsoft.EntityFrameworkCore;
using PingPong.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PingPongContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PingPongContext")));

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IDBConnectionFactory, DBConnectionFactory>();

builder.Services.AddScoped<PlayerRepository>();
builder.Services.AddScoped<TeamRepository>();

// Add services to the container.
builder.Services.AddControllersWithViews();
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
