using BusinessLocationsWebAPI.Controllers;
using BusinessLocationsWebAPI.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
};

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//app.MapBusinessLocationsEndpoints();

app.Run();
