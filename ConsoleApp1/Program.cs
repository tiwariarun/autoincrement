using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ConsoleApp1
{
    public class Program
    {
        private static string repoPath = @"E:\Projects\gitautoincrement\Autoincrement"; // Update to your repo path
        private static string counterFile = @"counter.txt"; // The file to store the counter
        private static string commitMessage = "Automated check-in, increment count";
        private static string branchName = "main"; // Your branch name
        private static int intervalInMilliseconds = 3600000; // Interval in milliseconds (e.g., 1 hour = 3600000ms)

        static void Main(string[] args)
        {
            // Set up a timer to call the CheckIn function at the specified interval
            Timer timer = new Timer(CheckIn, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(intervalInMilliseconds));

            // Keep the application running so the timer can keep executing
            Console.WriteLine("Press [Enter] to exit...");
            Console.ReadLine();
        }

        // This function will be called at each interval
        private static void CheckIn(object state)
        {
            try
            {
                Console.WriteLine("Starting check-in process...");

                // Step 1: Pull the latest changes from the repository
                GitPull();

                // Step 2: Increment the counter in the file
                IncrementCounter();

                // Step 3: Commit and push changes
                GitCommitAndPush();

                Console.WriteLine("Check-in completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during check-in: {ex.Message}");
            }
        }

        // Pull the latest changes from GitHub repository
        private static void GitPull()
        {
            ExecuteGitCommand("pull", branchName);
        }

        // Increment the counter stored in the counter.txt file
        private static void IncrementCounter()
        {
            try
            {
                int count = 0;
                if (File.Exists(counterFile))
                {
                    string content = File.ReadAllText(counterFile);
                    if (int.TryParse(content, out count))
                    {
                        count++; // Increment the counter
                    }
                }

                // Write the incremented counter back to the file
                File.WriteAllText(counterFile, count.ToString());
                Console.WriteLine($"Counter incremented to {count}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while incrementing counter: {ex.Message}");
            }
        }

        // Add changes, commit, and push to GitHub
        private static void GitCommitAndPush()
        {
            ExecuteGitCommand("add", counterFile);
            ExecuteGitCommand("commit", "-m \"" + commitMessage + "\"");
            ExecuteGitCommand("push", branchName);
        }

        // Helper function to execute Git commands
        private static void ExecuteGitCommand(string command, string arguments)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = $"{command} {arguments}",
                WorkingDirectory = repoPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = new Process { StartInfo = processStartInfo };
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.ErrorDataReceived += (sender, e) => Console.WriteLine("Error: " + e.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();
        }
    }

}
