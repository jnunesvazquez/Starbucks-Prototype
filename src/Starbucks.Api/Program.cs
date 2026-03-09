using Starbucks.Api.Extensions;
using Starbucks.Persistence;
using Starbucks.Application;
using Core.Mappy.Interfaces;
using Starbucks.Application.Categories.DTOs;
using Core.Mappy.Extensions;
using Starbucks.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();


var app = builder.Build();

var mapper = app.Services.GetRequiredService<IMapper>();
mapper.RegisterMappings(typeof(CategoryMappingProfile).Assembly);


await app.ApplyMigration(environment);

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();