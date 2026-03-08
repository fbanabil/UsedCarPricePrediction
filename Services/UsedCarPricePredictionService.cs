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

            string scriptPath = GetPythonScriptPath();
            var options = new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase
            };
            string jsonInput = System.Text.Json.JsonSerializer.Serialize(input, options);

            string result = await _pythonRunner.RunPythonScript(scriptPath, jsonInput);
            
            return result;
        }

        private string GetPythonScriptPath()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            if (isWindows)
            {
                string basePath = AppContext.BaseDirectory;
                string scriptPath = Path.Combine(basePath, "Services", "ServiceUtilities", "predict_car_price.py");
                scriptPath = scriptPath.Replace("\\UsedCarPricePrediction\\bin\\Debug\\net8.0", "");
                return scriptPath.Replace("\\", "/");
            }
            else
            {
                // Docker/Linux: Python files are always at /app/Services/ServiceUtilities/
                // This works for both VS fast mode (Debug) and Release, since the
                // Dockerfile copies scripts to /app/Services/ServiceUtilities/ in the base stage.
                return "/app/Services/ServiceUtilities/predict_car_price.py";
            }
        }

        public async Task<bool> EnsureEnviroment()
        {
            bool isWindows = System.Runtime.InteropServices.RuntimeInformation
                .IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows);

            if (!isWindows)
            {
                // In Docker, Python packages are installed globally during image build
                _logger.LogInformation("Running in Docker/Linux — skipping venv setup.");
                return true;
            }

            string currentDir = Directory.GetCurrentDirectory();
            string? solutionRoot = Directory.GetParent(currentDir)?.FullName;

            if (string.IsNullOrEmpty(solutionRoot))
            {
                throw new InvalidOperationException("Could not determine solution root directory.");
            }

string directory = Path.Combine(solutionRoot, "Services", "ServiceUtilities");
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
