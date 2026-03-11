using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Services
{
    public class PythonRunner
    {
        private readonly ILogger<PythonRunner> _logger;

        public PythonRunner(ILogger<PythonRunner> logger)
        {
            _logger = logger;
        }

        public async Task<string> RunPythonScript(string scriptPath, string jsonInput)
        {
            string? workingDirectory = Path.GetDirectoryName(scriptPath);
            bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            
            if (!isWindows)
            {
                scriptPath = "/app/Services/ServiceUtilities/predict_car_price.py";
                workingDirectory = "/app/Services/ServiceUtilities";
            }
            
            string pythonExecutable = isWindows
                ? GetVenvPythonPath(workingDirectory)
                : "python3";

            var startInfo = new ProcessStartInfo
            {
                FileName = pythonExecutable, 
                Arguments = $"\"{scriptPath}\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = workingDirectory,
                CreateNoWindow = true
            };

            using (var process = Process.Start(startInfo))
            {
                if (process == null)
                    throw new InvalidOperationException("Failed to start Python process.");

                _logger.LogInformation($"Running Python script: {scriptPath} with {pythonExecutable}");
                _logger.LogInformation($"Working Directory: {workingDirectory}");
                _logger.LogInformation($"Input JSON: {jsonInput}");

                await process.StandardInput.WriteLineAsync(jsonInput);
                await process.StandardInput.FlushAsync();
                process.StandardInput.Close();

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                string output = await outputTask;
                string error = await errorTask;

                if (process.ExitCode != 0)
                {
                    _logger.LogError($"Python script error: {error}");
                    throw new Exception($"Python script failed with error: {error}");
                }

                _logger.LogInformation($"Python script output: {output}");
                return output.Trim();
            }

        }


        private string GetVenvPythonPath(string? workingDirectory)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                _logger.LogWarning("Working directory is null or empty, falling back to system Python");
                return "python";
            }

            string venvPythonPath = Path.Combine(workingDirectory, "venv", "Scripts", "python.exe");

            if (File.Exists(venvPythonPath))
            {
                _logger.LogInformation($"Using venv Python: {venvPythonPath}");
                return venvPythonPath;
            }

            _logger.LogWarning("Virtual environment not found, falling back to system Python");
            return "python";
        }
    }

}
