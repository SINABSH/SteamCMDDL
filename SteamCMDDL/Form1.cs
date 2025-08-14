// These 'using' statements are necessary for the code to work
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

// Make sure this namespace matches your project name in the Solution Explorer
namespace SteamCMDDL
{
    public partial class Form1 : Form
    {
        // Define a path for steamcmd within our application's folder.
        private readonly string steamCmdExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamcmd.exe");

        private readonly string steamCmdContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamapps", "workshop", "content");

        // Use a static HttpClient for performance benefits.
        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            // This is the first code that runs.
            InitializeComponent();
            // We can't log here yet because the log textbox itself hasn't been created.
        }

        // This event runs just before the form is displayed for the first time.
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("EVENT: Form_Load - Form is loading and controls are being initialized.");
        }

        // This event runs right after the form is displayed.
        private void Form1_Shown(object sender, EventArgs e)
        {
            Log("EVENT: Form_Shown - Form has been successfully displayed to the user.");
        }

        // This event runs when the form is about to close.
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // This log is crucial. It tells us WHY the form is closing.
            Log($"EVENT: Form_Closing - Reason: {e.CloseReason.ToString()}");
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnDownload_Click - 'Download All' button was clicked.");

            // --- Setup and Validation (this part is the same) ---
            bool isSteamCmdReady = await CheckAndInstallSteamCmdAsync();
            if (!isSteamCmdReady) { /* ... */ return; }
            if (!int.TryParse(txtAppId.Text, out int appId)) { /* ... */ return; }
            var workshopIds = ParseWorkshopIds(rtbWorkshopIds.Lines);
            if (!workshopIds.Any()) { /* ... */ return; }

            SetControlsEnabled(false);
            progressBar.Value = 0;
            progressBar.Maximum = workshopIds.Count;
            txtLog.AppendText("--- Starting Download Process ---\n");

            // --- THIS IS THE NEW PART ---
            // Create a progress reporter. The code inside the parentheses
            // is the action that will run on the UI thread every time progress is reported.
            var progress = new Progress<int>(value =>
            {
                progressBar.Value = value;
                Log($"UI: Progress bar updated to {value}/{progressBar.Maximum}");
            });
            // --- END OF NEW PART ---

            try
            {
                Log("STEP 4: Starting background task for downloads.");
                // We now pass the 'progress' reporter to our background task.
                await Task.Run(() => RunSteamCmd(appId.ToString(), workshopIds, progress));

                Log("BACKGROUND TASK: Download task completed.");
                MessageBox.Show("Downloads completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Log($"CRITICAL FAILURE: An unhandled error occurred in the download task: {ex.Message}");
                MessageBox.Show($"An error occurred during the download process:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // To ensure the progress bar shows 100% at the end.
                progressBar.Value = progressBar.Maximum;
                Log("STEP 5: Re-enabling UI.");
                SetControlsEnabled(true);
            }
        }

        private async Task<bool> CheckAndInstallSteamCmdAsync()
        {
            Log("Checking for SteamCMD existence...");
            if (File.Exists(steamCmdExePath))
            {
                Log($"SteamCMD found at: {steamCmdExePath}");
                return true;
            }

            Log("SteamCMD not found. Prompting user for download.");
            var userChoice = MessageBox.Show("SteamCMD is required and was not found. Would you like to download and set it up automatically?", "Setup Required", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (userChoice == DialogResult.No)
            {
                Log("User chose not to download SteamCMD.");
                return false;
            }

            Log("User agreed to download. Starting setup process...");
            try
            {
                string steamCmdZipPath = Path.Combine(Path.GetTempPath(), "steamcmd.zip");
                string steamCmdDir = Path.GetDirectoryName(steamCmdExePath);

                if (!Directory.Exists(steamCmdDir))
                {
                    Log($"Creating directory: {steamCmdDir}");
                    Directory.CreateDirectory(steamCmdDir);
                }

                Log("Downloading steamcmd.zip...");
                const string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
                byte[] fileBytes = await client.GetByteArrayAsync(steamCmdUrl);
                File.WriteAllBytes(steamCmdZipPath, fileBytes);
                Log("Download complete.");

                Log($"Extracting files to {steamCmdDir}...");
                ZipFile.ExtractToDirectory(steamCmdZipPath, steamCmdDir);
                Log("Extraction complete.");
                File.Delete(steamCmdZipPath);
                Log("Cleaned up zip file.");

                Log("Initializing SteamCMD (running it once to update)...");
                var processInfo = new ProcessStartInfo(steamCmdExePath, "+quit") { CreateNoWindow = true, UseShellExecute = false };
                var process = Process.Start(processInfo);
                await Task.Run(() => process.WaitForExit());
                Log("SteamCMD initialization complete.");

                return true;
            }
            catch (Exception ex)
            {
                Log($"CRITICAL FAILURE during SteamCMD setup: {ex.Message}");
                string steamCmdDir = Path.GetDirectoryName(steamCmdExePath);
                if (Directory.Exists(steamCmdDir))
                {
                    Directory.Delete(steamCmdDir, true);
                }
                return false;
            }
        }

        // Notice the new "IProgress<int> progress" parameter at the end
        // Notice the new "IProgress<int> progress" parameter at the end
        private void RunSteamCmd(string appId, List<string> workshopIds, IProgress<int> progress)
        {
            int itemsCompleted = 0;

            // Get the correct installation directory for steamcmd
            string installDir = Path.GetDirectoryName(steamCmdExePath);

            foreach (var itemId in workshopIds)
            {
                Log($"Starting download process for item: {itemId}");

                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    FileName = steamCmdExePath,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };

                process.StartInfo = startInfo;
                process.OutputDataReceived += (sender, args) => Log($"[SteamCMD]: {args.Data}");
                process.ErrorDataReceived += (sender, args) => Log($"[SteamCMD ERR]: {args.Data}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                using (StreamWriter sw = process.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        // --- THIS IS THE NEW PART ---
                        // Force steamcmd to use the correct directory before doing anything else.
                        // The quotes are important for paths with spaces.
                        sw.WriteLine($"force_install_dir \"{installDir}\"");
                        // --- END OF NEW PART ---

                        sw.WriteLine("login anonymous");
                        sw.WriteLine($"workshop_download_item {appId} {itemId}");
                        sw.WriteLine("quit");
                    }
                }

                Log($"Waiting for item {itemId} to finish downloading...");
                bool exited = process.WaitForExit(3000); // 5 minute timeout

                if (!exited)
                {
                    Log($"WARNING: Item {itemId} took too long. Forcing shutdown of its process.");
                    process.Kill();
                }

                Log($"Finished processing item: {itemId}.");

                itemsCompleted++;
                progress.Report(itemsCompleted);
            }
        }

        #region UI Helper Methods

        private List<string> ParseWorkshopIds(string[] lines)
        {
            Log($"Parsing {lines.Length} lines of input.");
            var idList = new List<string>();
            var regex = new Regex(@"\d+");
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var match = regex.Match(line.Trim());
                if (match.Success)
                {
                    idList.Add(match.Value);
                }
            }
            var distinctList = idList.Distinct().ToList();
            Log($"Parsed {distinctList.Count} unique IDs.");
            return distinctList;
        }

        private void Log(string message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // This is a thread-safe way to append text to the textbox.
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}")));
            }
            else
            {
                txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }

        private void UpdateProgressBar()
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(UpdateProgressBar));
            }
            else
            {
                if (progressBar.Value < progressBar.Maximum)
                {
                    progressBar.Value++;
                    Log($"UI: Progress bar updated to {progressBar.Value}/{progressBar.Maximum}");
                }
            }
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            txtAppId.Enabled = isEnabled;
            rtbWorkshopIds.Enabled = isEnabled;
            btnDownload.Enabled = isEnabled;
            btnClearAll.Enabled = isEnabled;
            btnSaveList.Enabled = isEnabled;
            btnLoadList.Enabled = isEnabled;
            btnOpenFolder.Enabled = isEnabled; // You might choose to keep this enabled always
            Log($"UI: Controls have been {(isEnabled ? "Enabled" : "Disabled")}.");
        }

        #endregion

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnClearAll_Click - Clearing all input fields.");
            txtAppId.Clear();
            rtbWorkshopIds.Clear();
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnSaveList_Click - Opening Save File Dialog.");
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.Title = "Save Workshop List";
                sfd.FileName = "MyModList.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Create a list to hold all lines to be saved
                        var linesToSave = new List<string>();

                        // Add the App ID as the very first line
                        linesToSave.Add(txtAppId.Text);

                        // Add all the workshop item links after the App ID
                        linesToSave.AddRange(rtbWorkshopIds.Lines);

                        // Write the combined list to the file
                        File.WriteAllLines(sfd.FileName, linesToSave);
                        Log($"SUCCESS: List and App ID saved to {sfd.FileName}");
                    }
                    catch (Exception ex)
                    {
                        Log($"FAILURE: Could not save file. Error: {ex.Message}");
                        MessageBox.Show($"Could not save the list.\nError: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Log("User cancelled the save operation.");
                }
            }
        }

        private void btnLoadList_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnLoadList_Click - Opening Open File Dialog.");
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.Title = "Load Workshop List";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // Read all lines from the saved file
                        var allLines = File.ReadAllLines(ofd.FileName);

                        // Check if the file is not empty
                        if (allLines.Length > 0)
                        {
                            // The first line is the App ID
                            txtAppId.Text = allLines[0];

                            // The rest of the lines are the workshop items
                            // We use Skip(1) to ignore the first line we already used
                            rtbWorkshopIds.Lines = allLines.Skip(1).ToArray();

                            Log($"SUCCESS: List and App ID loaded from {ofd.FileName}");
                        }
                        else
                        {
                            // If the file is empty, clear the boxes
                            txtAppId.Clear();
                            rtbWorkshopIds.Clear();
                            Log("Loaded file was empty.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log($"FAILURE: Could not load file. Error: {ex.Message}");
                        MessageBox.Show($"Could not load the list.\nError: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Log("User cancelled the load operation.");
                }
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnOpenFolder_Click - Attempting to open download folder.");
            if (Directory.Exists(steamCmdContentPath))
            {
                Log($"SUCCESS: Found folder at {steamCmdContentPath}. Opening...");
                Process.Start("explorer.exe", steamCmdContentPath);
            }
            else
            {
                Log("FAILURE: Download folder does not exist yet. A download must be completed first.");
                MessageBox.Show("The download folder has not been created yet.\nPlease complete at least one download first.", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }
    }
}