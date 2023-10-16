using Microsoft.EntityFrameworkCore;
using ReStore.Data;
using ReStore.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


#region DbContext

builder.Services.AddDbContext<RestoreDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

#endregion

#region CORS
builder.Services.AddCors();
#endregion


builder.Services.AddControllers();

#region Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();
#endregion

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(opt =>
{
    opt.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
});



app.UseHttpsRedirection();

app.UseAuthorization();



app.MapControllers();

app.Run();
