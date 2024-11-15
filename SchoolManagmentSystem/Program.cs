using Domain.Extensions;
using Domain.Helpers;
using Domain.Middlewares;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace SchoolManagmentSystem;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(new CustomJsonFormatter())
            .WriteTo.File(new CustomJsonFormatter(), "logs/logs.txt", rollingInterval: RollingInterval.Day)
            .WriteTo.File(new CustomJsonFormatter(), "logs/error_.txt", Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseSerilog();

        builder.Services.AddControllers();

        builder.Services.AddDbContext<SchoolManagmentDbContext>(options =>
             options.UseSqlServer(builder.Configuration.GetConnectionString("SchoolConnection")));

        builder.Services.ConfigureServices();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();
        app.UseMiddleware<ErrorHandlerMiddleware>();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
