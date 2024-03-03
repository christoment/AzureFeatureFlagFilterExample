using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options
        .Connect(builder.Configuration.GetConnectionString("AppConfiguration"))
        .UseFeatureFlags();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ITargetingContextAccessor, SimpleTargetingContextAccessor>();
builder.Services.AddFeatureManagement()
    .AddFeatureFilter<PercentageFilter>()
    .AddFeatureFilter<TargetingFilter>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/featureflags", async (IFeatureManager fm) =>
    {
        Dictionary<string, bool> featureFlagStatus = [];
        await foreach (var name in fm.GetFeatureNamesAsync())
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                featureFlagStatus.Add(name, await fm.IsEnabledAsync(name));
            }
        }

        return TypedResults.Ok(featureFlagStatus);
    })
    .WithName("GetFeatureFlag")
    .WithOpenApi();

app.Run();
