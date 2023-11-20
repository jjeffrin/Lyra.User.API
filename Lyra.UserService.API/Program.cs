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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors(o => o.AddPolicy("LyraPolicy", builder =>
{
    //builder.WithOrigins()
            builder.AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => 
            { 
                if (host.Equals("https://localhost:3000")) return true; 
                return false;
            })
            .AllowCredentials();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("LyraPolicy");
app.UseAuthorization();

app.MapControllers();

app.Run();
