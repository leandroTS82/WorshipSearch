using WorshipSearch.Prompts;
using WorshipSearch.Services;

var builder = WebApplication.CreateBuilder(args);

// MVC with Razor Views
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Respect [JsonPropertyName] attributes; don't override with camelCase
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
    });

// HttpClient registrations
builder.Services.AddHttpClient<ILlmKeyService, LlmKeyService>();
builder.Services.AddHttpClient<IGroqService, GroqService>();
builder.Services.AddHttpClient<IMusicSearchService, MusicSearchService>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0 Safari/537.36");
    client.Timeout = TimeSpan.FromSeconds(15);
});
builder.Services.AddHttpClient<IMusicHtmlParser, MusicHtmlParser>(client =>
{
    client.DefaultRequestHeaders.UserAgent.ParseAdd(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 Chrome/120.0 Safari/537.36");
    client.Timeout = TimeSpan.FromSeconds(20);
});

// Services
builder.Services.AddSingleton<IMusicJsonGenerator, MusicJsonGenerator>();
builder.Services.AddSingleton<IPromptBuilder, PromptBuilder>();
builder.Services.AddScoped<IMusicEnrichmentService, MusicEnrichmentService>();
builder.Services.AddSingleton<IMusicRepository, MusicRepository>();
builder.Services.AddSingleton<ISearchRestrictionsService, SearchRestrictionsService>();
builder.Services.AddSingleton<IJsonSearchEngine, JsonSearchEngine>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure /json folder exists
var jsonFolder = Path.Combine(app.Environment.ContentRootPath, "json");
Directory.CreateDirectory(jsonFolder);

app.Run();
