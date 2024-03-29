using Lyra.UserService.API.DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(Environment.GetEnvironmentVariable("SQLAZURECONNSTR_AzureSQLDatabaseConnection"));    
});

builder.Services.AddCors(o => o.AddPolicy("LyraPolicy", builder =>
{    
    builder.AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed((host) => true)
    .AllowCredentials();
}));

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.MapHealthChecks("/health");
app.UseHttpsRedirection();
app.UseCors("LyraPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
