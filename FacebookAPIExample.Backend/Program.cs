using System.Net.Http.Headers;
using Carter;
using FacebookAPIExample.Backend.Integrations.Facebook;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCarter();

builder.Services.Configure<FacebookOptions>(builder.Configuration.GetSection("facebook"));

var app = builder.Build();

app.UseCors(x=>x
    .AllowAnyHeader()
    .AllowAnyOrigin()
    .AllowAnyMethod());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapCarter();

app.Run();