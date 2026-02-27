using Microsoft.Extensions.Logging;
using ServicInterfaces;
using DataModels.Models;
using UsedCarPricePrediction.Enums;

namespace Services
{
    public class UsedCarPricePredictionService : IUsedCarPricePredictionService
    {
        private readonly PythonRunner _pythonRunner;
        private readonly ILogger<UsedCarPricePredictionService> _logger;
        InitialPythonEnviromentSetup _initialPythonEnviromentSetup;
        public UsedCarPricePredictionService(PythonRunner pythonRunner, ILogger<UsedCarPricePredictionService> logger, InitialPythonEnviromentSetup initialPythonEnviromentSetup)
        {
            _pythonRunner = pythonRunner;
            _logger = logger;
            _initialPythonEnviromentSetup = initialPythonEnviromentSetup;
        }

        public async Task<string> PredictPrice(PredictionInputs predictionInputs)
        {
            var input = new
            {
                year = predictionInputs.year,
                manufacturer = predictionInputs.manufacturer.GetDisplayName(),
                model = predictionInputs.model.GetDisplayName(),
                odometer = predictionInputs.odometer,
                condition = predictionInputs.condition.GetDisplayName(),
                cylinders = predictionInputs.cylinders.GetDisplayName(),
                fuel = predictionInputs.fuel.GetDisplayName(),
                title_status = predictionInputs.title_status.GetDisplayName(),
                transmission = predictionInputs.transmission.GetDisplayName(),
                drive = predictionInputs.drive.GetDisplayName(),
                type = predictionInputs.type.GetDisplayName(),
                lat = predictionInputs.Lat,
                Long = predictionInputs.Long,
                description = predictionInputs.description
            };
            string basePath = AppContext.BaseDirectory;
            string relativePath = Path.Combine("Services","ServiceUtilities", "predict_car_price.py");
            string scriptPath = Path.Combine(basePath, relativePath);
            scriptPath = scriptPath.Replace("\\UsedCarPricePrediction\\bin\\Debug\\net8.0", "");
            scriptPath = scriptPath.Replace("\\", "/");
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            string jsonInput = System.Text.Json.JsonSerializer.Serialize(input, options);
            string result =await  _pythonRunner.RunPythonScript(scriptPath, jsonInput);
            return result;
        }



        public async Task<bool> EnsureEnviroment()
        {
            string directory = Directory.GetCurrentDirectory();
            directory = directory.Replace("\\UsedCarPricePrediction", "");
            directory= directory+ "\\UsedCarPricePrediction\\Services\\ServiceUtilities";
            _logger.LogInformation("Ensuring Python environment setup in directory: {Directory}", directory);
            try
            {
                await _initialPythonEnviromentSetup.EnsureVirtualEnvironment(directory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ensure Python environment setup.");
                return false;
            }
            return true;
        }

    }
}
