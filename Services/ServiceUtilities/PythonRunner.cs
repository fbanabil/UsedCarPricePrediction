using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

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

            // Use venv Python executable instead of system Python
            string pythonExecutable = GetVenvPythonPath(workingDirectory);

            var startInfo = new ProcessStartInfo
            {
                FileName = pythonExecutable, // Use venv Python
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
                _logger.LogInformation($"Input JSON: {jsonInput}");

                using (var writer = process.StandardInput)
                {
                    writer.WriteLine(jsonInput);
                }

                string output = process.StandardOutput.ReadToEnd().Trim();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    _logger.LogError($"Python script error: {error}");
                    throw new Exception($"Python script failed with error: {error}");
                }

                return output;
            }

        }
        private string GetVenvPythonPath(string? workingDirectory)
        {
            string venvPythonPath = Path.Combine(workingDirectory, "venv", "Scripts", "python.exe");

            if (File.Exists(venvPythonPath))
            {
                _logger.LogInformation($"Using venv Python: {venvPythonPath}");
                return venvPythonPath;
            }

            _logger.LogWarning("Virtual environment not found, falling back to system Python");
            return "python"; // Fallback to system Python
        }
    }

}
