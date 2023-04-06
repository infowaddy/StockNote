using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StockNote.BusinessLayer.Interfaces;
using StockNote.BusinessLayer;
using StockNote.DataAccess;
using System.Runtime.InteropServices;
using Serilog;
using Serilog.Core;
using Microsoft.Extensions.Configuration;
using StockNote.WebAPI;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration)); ;

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(opt =>
{
    opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
    opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                    new HeaderApiVersionReader("api-version"),
                                                    new MediaTypeApiVersionReader("api-version"));
});

builder.Services.AddVersionedApiExplorer(options => {
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
});

if (builder.Environment.EnvironmentName != "Testing")
{    
    builder.Services.AddSwaggerGen();
    builder.Services.ConfigureOptions<SwaggerConfiguration>();
}

// Database and storage related-services
var dbConnectionString = builder.Configuration.GetConnectionString("StockNoteDb") ?? throw new InvalidOperationException("Missing connection string configuration");
builder.Services.AddDbContext<StockNoteDBContext>(options =>
    options.UseSqlServer(dbConnectionString));


// for dependancy injection
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("StockNoteSetting"));
builder.Services.AddTransient<IItemManager, ItemManager>();
builder.Services.AddTransient<IUnitManager, UnitManager>();
builder.Services.AddTransient<IWarehouseManager, WarehouseManager>();
builder.Services.AddTransient<IStockTransactionManager, StockTransactionManager>();
builder.Services.AddTransient<IStockInWarehouseManager, StockInWarehouseManager>();


var app = builder.Build();
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
    await EnsureDbAsync(app.Services, builder.Environment.EnvironmentName == "Testing");

// Configure the HTTP request pipeline.
if (app.Environment.EnvironmentName !="Testing")
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            if (description.IsDeprecated)
                o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                    $"StockNote - {description.GroupName.ToUpper()} [Deprecated]");
            else
                o.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                $"StockNote - {description.GroupName.ToUpper()}");
        }
    });
    app.UseHttpsRedirection();
}

app.UseSerilogRequestLogging();

app.UseAuthorization();

app.MapControllers();

app.Run();

static async Task EnsureDbAsync(IServiceProvider sp, bool test=false)
{
    await using var db = sp.CreateScope().ServiceProvider.GetRequiredService<StockNoteDBContext>();
    if(test)
        await db.Database.EnsureDeletedAsync();
    await db.Database.MigrateAsync();
}

public partial class Program { }
