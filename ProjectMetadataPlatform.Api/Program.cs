
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlDocFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml").ToList();
    xmlDocFiles.Add(Path.Combine(AppContext.BaseDirectory, Assembly.GetExecutingAssembly()?.GetName().Name + ".xml"));
    
    xmlDocFiles.ForEach(path => options.IncludeXmlComments(path,true));
});
builder.Services
    .AddApplicationDependencies()
    .AddInfrastructureDependencies();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.Run();
