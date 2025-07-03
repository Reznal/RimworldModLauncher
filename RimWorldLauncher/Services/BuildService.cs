using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace RimWorldLauncher.Services
{
    public class BuildService : IBuildService
    {
        public event EventHandler<string>? BuildOutputReceived;
        public event EventHandler<bool>? BuildCompleted;
        public event EventHandler<int>? BuildProgressChanged;

        public bool IsBuilding { get; private set; }

        public async Task<bool> BuildModAsync(string modPath)
        {
            if (IsBuilding) return false;

            IsBuilding = true;
            var success = false;

            try
            {
                // Find the .sln or .csproj file in the mod directory
                var projectFile = FindProjectFile(modPath);
                if (string.IsNullOrEmpty(projectFile))
                {
                    BuildOutputReceived?.Invoke(this, $"ERROR: No .sln or .csproj file found in {modPath}");
                    return false;
                }

                var fileExtension = Path.GetExtension(projectFile).ToLowerInvariant();
                BuildOutputReceived?.Invoke(this, $"Building project: {Path.GetFileName(projectFile)} ({fileExtension})");
                BuildProgressChanged?.Invoke(this, 10);

                var startInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = $"build \"{projectFile}\" --configuration Debug --verbosity normal",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    WorkingDirectory = Path.GetDirectoryName(projectFile)
                };

                using var process = new Process { StartInfo = startInfo };
                
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        BuildOutputReceived?.Invoke(this, e.Data);
                        
                        // Update progress based on output
                        if (e.Data.Contains("Build succeeded") || e.Data.Contains("Build completed"))
                        {
                            BuildProgressChanged?.Invoke(this, 90);
                        }
                        else if (e.Data.Contains("Build FAILED") || e.Data.Contains("Build failed"))
                        {
                            BuildProgressChanged?.Invoke(this, 50);
                        }
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        BuildOutputReceived?.Invoke(this, $"ERROR: {e.Data}");
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                await process.WaitForExitAsync();
                success = process.ExitCode == 0;
                
                if (success)
                {
                    BuildOutputReceived?.Invoke(this, "Build completed successfully!");
                    BuildProgressChanged?.Invoke(this, 100);
                }
                else
                {
                    BuildOutputReceived?.Invoke(this, $"Build failed with exit code: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                BuildOutputReceived?.Invoke(this, $"Build failed with exception: {ex.Message}");
                BuildOutputReceived?.Invoke(this, $"Stack trace: {ex.StackTrace}");
                success = false;
            }
            finally
            {
                IsBuilding = false;
                BuildCompleted?.Invoke(this, success);
            }

            return success;
        }

        private string? FindProjectFile(string modPath)
        {
            // If modPath is directly a .sln or .csproj file
            if (File.Exists(modPath))
            {
                var extension = Path.GetExtension(modPath).ToLowerInvariant();
                if (extension == ".sln" || extension == ".csproj")
                {
                    return modPath;
                }
            }

            // If modPath is a directory, search for project files
            if (Directory.Exists(modPath))
            {
                // Prioritize .sln files first
                var solutionFiles = Directory.GetFiles(modPath, "*.sln", SearchOption.TopDirectoryOnly);
                if (solutionFiles.Length > 0)
                {
                    return solutionFiles[0];
                }

                // Fall back to .csproj files
                var projectFiles = Directory.GetFiles(modPath, "*.csproj", SearchOption.TopDirectoryOnly);
                if (projectFiles.Length > 0)
                {
                    return projectFiles[0];
                }

                // Search in subdirectories for .sln files
                var solutionFilesRecursive = Directory.GetFiles(modPath, "*.sln", SearchOption.AllDirectories);
                if (solutionFilesRecursive.Length > 0)
                {
                    return solutionFilesRecursive[0];
                }

                // Search in subdirectories for .csproj files
                var projectFilesRecursive = Directory.GetFiles(modPath, "*.csproj", SearchOption.AllDirectories);
                if (projectFilesRecursive.Length > 0)
                {
                    return projectFilesRecursive[0];
                }
            }

            return null;
        }
    }
} 