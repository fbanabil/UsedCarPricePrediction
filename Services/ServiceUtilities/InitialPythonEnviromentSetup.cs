using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Logging;

namespace Services
{
    public class InitialPythonEnviromentSetup
    {
        private readonly ILogger<InitialPythonEnviromentSetup> _logger;

        public InitialPythonEnviromentSetup(ILogger<InitialPythonEnviromentSetup> logger)
        {
            _logger = logger;
        }
        public async Task EnsureVirtualEnvironment(string workingDirectory)
        {
            string venvPath = Path.Combine(workingDirectory, "venv");
            _logger.LogInformation("Ensuring virtual environment at: {VenvPath}. \n   It might take some time if it is first time.", venvPath);

            if (!Directory.Exists(venvPath))
            {
                // Create venv
                _logger.LogInformation("Creating virtual environment...");
                await RunCommandAsync("python", "-m venv venv", workingDirectory);
            }

            string requirementsFilePath = Path.Combine(workingDirectory, "requirements.txt");
            string pythonPath = Path.Combine(venvPath, "Scripts", "python.exe");

            // Always check if requirements are satisfied
            if (! await AreRequirementsSatisfiedAsync(pythonPath, requirementsFilePath, workingDirectory))
            {
                _logger.LogInformation("Upgrading pip...");
                await RunCommandAsync(pythonPath, "-m pip install --upgrade pip", workingDirectory);

                _logger.LogInformation("Installing/updating requirements from: {RequirementsFilePath}", requirementsFilePath);
                await RunCommandAsync(pythonPath, "-m pip install -r " + requirementsFilePath, workingDirectory);

                // Verify installation was successful
                if (!await AreRequirementsSatisfiedAsync(pythonPath, requirementsFilePath, workingDirectory))
                {
                    throw new InvalidOperationException("Failed to install all required packages after installation attempt.");
                }
            }
            else
            {
                _logger.LogInformation("All required packages are already installed.");
            }
        }


        private async Task RunCommandAsync(string fileName, string arguments, string workingDirectory)
        {
            _logger.LogInformation("Running command: {FileName} {Arguments}", fileName, arguments);
            
            var processInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processInfo);
            if (process == null)
            {
                throw new InvalidOperationException($"Failed to start process: {fileName} {arguments}");
            }

            // Read output asynchronously to prevent deadlocks
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string output = await outputTask;
            string error = await errorTask;

            if (process.ExitCode != 0)
            {
                _logger.LogError("Command failed with exit code {ExitCode}. Error: {Error}", process.ExitCode, error);
                throw new InvalidOperationException($"Command failed: {fileName} {arguments}. Error: {error}");
            }

            if (!string.IsNullOrEmpty(output))
            {
                _logger.LogInformation("Command output: {Output}", output);
            }
        }


        private async Task<bool> AreRequirementsSatisfiedAsync(string pythonPath, string requirementsFilePath, string workingDirectory)
        {
            try
            {
                _logger.LogInformation("Checking if all requirements are satisfied...");

                var processInfo = new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = $"-m pip install --dry-run -r \"{requirementsFilePath}\"",
                    WorkingDirectory = workingDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(processInfo);
                if (process == null)
                {
                    return false;
                }

                var outputTask = process.StandardOutput.ReadToEndAsync();
                var errorTask = process.StandardError.ReadToEndAsync();

                await process.WaitForExitAsync();

                string output = await outputTask;
                string error = await errorTask;

                // If dry-run shows "Requirement already satisfied" and no "Would install", all packages are installed
                bool allSatisfied = output.Contains("Requirement already satisfied") && !output.Contains("Would install");

                _logger.LogInformation("Requirements check result: {Result}", allSatisfied ? "All satisfied" : "Some missing/outdated");

                return allSatisfied;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error checking requirements satisfaction, assuming requirements need installation");
                return false;
            }
        }
    }
}
