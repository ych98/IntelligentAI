using IntelligentAI.ApiService;
using IntelligentAI.ApiService.Models;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

var apis = builder.Configuration.GetSection("ModelApis").Get<ModelApis>();
var keys = builder.Configuration.GetSection("ApiKeys").Get<ApiKeys>();
var swaggerConfig = builder.Configuration.GetSection("SwaggerConfig").Get<List<SwaggerConfig>>();

// Add HttpClients
builder.Services.AddHttpClients(apis);

// Add AiModels
builder.Services.AddAiModels(keys);

// Register controllers
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    foreach (var config in swaggerConfig)
    {
        c.SwaggerDoc(config.GroupName, new OpenApiInfo
        {
            Title = config.Title,
            Version = config.Version
        });
    }

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    c.IncludeXmlComments(xmlPath);
    c.OrderActionsBy(o => o.RelativePath);
});

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (var config in swaggerConfig)
        {
            c.SwaggerEndpoint($"/swagger/{config.GroupName}/swagger.json", $"{config.Title} {config.Version}");
        }
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
