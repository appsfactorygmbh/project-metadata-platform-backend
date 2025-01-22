using System;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using ProjectMetadataPlatform.Api;
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Infrastructure;
using ProjectMetadataPlatform.Api.Swagger;



WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SchemaFilter<RequireNonNullablePropertiesSchemaFilter>();
    options.SupportNonNullableReferenceTypes();

    var xmlDocFiles = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory), "*.xml").ToList();

    xmlDocFiles.ForEach(path => options.IncludeXmlComments(path, true));
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your Bearer token like this: Bearer {your token}",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
});

builder.Services
    .AddApiDependencies()
    .AddApplicationDependencies()
    .AddInfrastructureDependencies();

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options
    => options.AddDefaultPolicy(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddControllers(
    options =>
    {
        options.Filters.Add(new ProducesResponseTypeAttribute(401));
        options.Filters.Add(new ProducesResponseTypeAttribute(500));
    }).AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });


var app = builder.Build();

app.Services.MigrateDatabase();
app.Services.AddAdminUser();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors();

app.Run();

/// <summary>
/// The entry point for the application.
/// </summary>
// ReSharper disable once ClassNeverInstantiated.Global - This is needed for the integration tests to work.
public partial class Program;

