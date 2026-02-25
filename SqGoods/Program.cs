using Microsoft.Extensions.FileProviders;
using SqGoods.DomainLogic;
using SqGoods.DomainLogic.DataAccess;
using SqGoods.Infrastructure;
using SqGoods.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();

builder.Services.AddSqGoodsDomainLogic(options =>
{
    options.DatabaseType = builder.Configuration.GetValue<DatabaseType>("DbType");
    options.ConnectionString = builder.Configuration.GetValue<string>("ConnectionString")
        ?? throw new InvalidOperationException("Missing required configuration key 'ConnectionString'.");
});

builder.Services.AddSingleton<DatabaseCheckMiddleware>();

builder.Services.AddScoped<CatalogService>();
builder.Services.AddScoped<CategoriesService>();
builder.Services.AddScoped<AttributesService>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddProblemDetails();
builder.Services.AddResponseCompression();

builder.Services.AddSwaggerDocument();

var app = builder.Build();

await InitializeDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles();

var spaDistPath = TryResolveSpaDistPath(app.Environment.ContentRootPath);
if (spaDistPath == null)
{
    throw new InvalidOperationException("Missing compiled SPA assets. Expected index.html under ClientApp/dist.");
}
var spaFileProvider = new PhysicalFileProvider(spaDistPath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = spaFileProvider,
    OnPrepareResponse = context =>
    {
        if (Path.HasExtension(context.File.Name))
        {
            context.Context.Response.Headers.CacheControl = "public,max-age=31536000,immutable";
        }
    }
});

app.UseMiddleware<DatabaseCheckMiddleware>();

app.UseRouting();

app.UseOpenApi();
app.UseSwaggerUi();

app.MapControllers();
app.MapRazorPages();
app.MapFallbackToFile("index.html", new StaticFileOptions { FileProvider = spaFileProvider });

await app.RunAsync();

static string? TryResolveSpaDistPath(string contentRootPath)
{
    var distRoot = Path.Combine(contentRootPath, "ClientApp", "dist");

    var primaryCandidates = new[]
    {
        Path.Combine(distRoot, "browser"),
        distRoot
    };

    foreach (var path in primaryCandidates)
    {
        if (File.Exists(Path.Combine(path, "index.html")))
        {
            return path;
        }
    }

    if (!Directory.Exists(distRoot))
    {
        return null;
    }

    foreach (var path in Directory.EnumerateDirectories(distRoot))
    {
        if (File.Exists(Path.Combine(path, "index.html")))
        {
            return path;
        }
    }

    return null;
}

static async Task InitializeDatabaseAsync(WebApplication app)
{
    var dbManager = app.Services.GetRequiredService<IDatabaseManager>();
    var logger = app.Services
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    var initializationError = await dbManager.Initialize();
    if (initializationError == null)
    {
        return;
    }

    if (initializationError.Exception != null)
    {
        logger.LogError(initializationError.Exception, initializationError.CreateMessage());
    }
    else
    {
        logger.LogError(initializationError.CreateMessage());
    }
}
