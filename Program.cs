using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VedioChatApp_Server_.HubConfig;
using VedioChatApp_Server_.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDbContext<chat_AppContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("chatapp")));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowHeaders",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
    .AddCookie(options =>
    {
        options.Cookie.Name = "chatAppCookie";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        // other cookie options
    });

builder.Services.AddSignalR(options=>{
    options.EnableDetailedErrors=true;
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();



var app = builder.Build();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowHeaders");

app.UseEndpoints(Endpoint =>
{
    Endpoint.MapControllers();
    Endpoint.MapHub<chatHub>("/chatHub");
    
});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
