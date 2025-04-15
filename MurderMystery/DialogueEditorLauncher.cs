using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MurderMystery
{
    /// <summary>
    /// Helper class to launch the separate Dialogue Editor application
    /// </summary>
    public static class DialogueEditorLauncher
    {
        /// <summary>
        /// Launch the Dialogue Editor application
        /// </summary>
        public static void LaunchEditor()
        {
            try
            {
                // First try to find the editor in the same directory
                string editorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DialogueEditor.exe");

                if (!File.Exists(editorPath))
                {
                    // Try the parent directory (bin folder structure)
                    editorPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "DialogueEditor.exe");

                    if (!File.Exists(editorPath))
                    {
                        // Try looking in a Debug/Release folder structure
                        string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "DialogueEditor", "bin",
                            AppDomain.CurrentDomain.BaseDirectory.Contains("Debug") ? "Debug" : "Release", "DialogueEditor.exe");

                        if (File.Exists(configPath))
                        {
                            editorPath = configPath;
                        }
                        else
                        {
                            // As a last resort, look for it relative to the executable
                            string exePath = Assembly.GetEntryAssembly().Location;
                            string exeDir = Path.GetDirectoryName(exePath);
                            editorPath = Path.Combine(exeDir, "DialogueEditor.exe");

                            if (!File.Exists(editorPath))
                            {
                                // Output debugging information
                                Console.WriteLine($"Looking for DialogueEditor.exe at:");
                                Console.WriteLine($"- {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DialogueEditor.exe")}");
                                Console.WriteLine($"- {Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "DialogueEditor.exe")}");
                                Console.WriteLine($"- {configPath}");
                                Console.WriteLine($"- {editorPath}");

                                Console.WriteLine("Could not find DialogueEditor.exe. Please make sure it's properly built and included in your project.");
                                return;
                            }
                        }
                    }
                }

                // Make sure Dialogue.json exists and is accessible to the editor
                string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Dialogue.json");
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine("Warning: Dialogue.json not found in the current directory.");
                    Console.WriteLine("The Dialogue Editor will prompt you to locate the file.");
                }

                // Launch the editor as a separate process
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                Console.WriteLine("Dialogue Editor launched. You can continue using the console application.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error launching Dialogue Editor: {ex.Message}");
            }
        }
    }
}