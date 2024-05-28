
using ProjectMetadataPlatform.Application;
using ProjectMetadataPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services
        .AddApplicationDependencies()
        .AddInfrastructureDependencies();
}

var app = builder.Build();
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}