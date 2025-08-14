using Services;
using ServicInterfaces;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUsedCarPricePredictionService, UsedCarPricePredictionService>();
builder.Services.AddScoped<InitialPythonEnviromentSetup>();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<PythonRunner>();

var app = builder.Build();

bool EnvironmentSetup = false;

using (var scope = app.Services.CreateScope())
{
    var predictionService = scope.ServiceProvider.GetRequiredService<IUsedCarPricePredictionService>();
    EnvironmentSetup = await predictionService.EnsureEnviroment();
}


if (!EnvironmentSetup)
{
    app.Run(async (context) =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "text/plain";
        await context.Response.WriteAsync("Failed to setup virtual environment.\n      Ensure Python 3.13.0 and Internet connection.");
    });
}


app.UseStaticFiles();
app.UseRouting();
app.MapControllers();

app.Run();
