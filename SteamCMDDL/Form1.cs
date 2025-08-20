using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;

// Make sure this namespace matches your project name in the Solution Explorer
namespace SteamCMDDL
{
    public partial class Form1 : Form
    {
        #region Class-level Variables
        private readonly string steamCmdExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamcmd.exe");
        private readonly string steamCmdContentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "steamcmd", "steamapps", "workshop", "content");
        private static readonly HttpClient client = new HttpClient();
        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        #region Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("EVENT: Form_Load - Form is loading and controls are being initialized.");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Log($"EVENT: Form_Closing - Reason: {e.CloseReason.ToString()}");
        }
        #endregion

        private async Task<string> FetchModNameFromUrlAsync(string url)
        {
            try
            {
                // Download the HTML of the workshop item's page
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // The mod title is usually in a div with the class "workshopItemTitle"
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='workshopItemTitle']");

                if (node != null)
                {
                    // Return the clean text of the title
                    return System.Net.WebUtility.HtmlDecode(node.InnerText.Trim());
                }
            }
            catch (Exception ex)
            {
                // If something goes wrong, log it but don't crash
                Log($"Failed to fetch name for {url}: {ex.Message}");
            }

            // Return a default value if the name couldn't be found
            return "Unknown Mod Name";
        }

        #region Button Click Event Handlers
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnDownload_Click - 'Download All' button was clicked.");

            // --- App ID Auto-Detection Logic ---
            if (string.IsNullOrWhiteSpace(txtAppId.Text))
            {
                Log("App ID is empty. Attempting auto-detection...");
                // Find the first item in the list that looks like a URL
                var firstUrl = lvWorkshopItems.Items.Cast<ListViewItem>()
                                                    // THIS IS THE CORRECTED LINE:
                                                    .Select(item => item.SubItems[2].Text) // Look in the 3rd column
                                                    .FirstOrDefault(text => text.ToLower().StartsWith("http"));

                if (firstUrl != null)
                {
                    // Call our new method to get the App ID
                    string detectedAppId = await FetchAppIdFromWorkshopUrlAsync(firstUrl);
                    if (detectedAppId != null)
                    {
                        // If we found it, update the textbox!
                        txtAppId.Text = detectedAppId;
                    }
                    else
                    {
                        MessageBox.Show("Could not automatically detect the Steam App ID from the first item in the list.\n\nPlease enter it manually.", "Auto-Detect Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("The App ID is empty and no valid workshop links were found in the list to auto-detect it from.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            // --- END of logic ---

            // The rest of the download process continues as before
            if (!int.TryParse(txtAppId.Text, out _)) { MessageBox.Show("Please enter a valid numeric App ID.", "Error"); return; }

            var workshopIds = ParseWorkshopIdsFromListView();
            await StartDownloadProcessAsync(workshopIds);
        }

        private async void btnRetryFailed_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnRetryFailed_Click - Retrying failed items.");
            var failedItems = lvWorkshopItems.Items.Cast<ListViewItem>()
                .Where(item => item.SubItems[1].Text == "Failed")
                .ToList();

            if (failedItems.Count == 0)
            {
                MessageBox.Show("No failed items found to retry.", "Nothing to Retry", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var item in failedItems)
            {
                item.SubItems[1].Text = "Pending";
            }

            var idsToRetry = failedItems.Select(item => item.SubItems[0].Text).ToList();
            await StartDownloadProcessAsync(idsToRetry);
        }

        private async void btnAddItem_Click(object sender, EventArgs e)
        {
            string newItemText = txtAddItem.Text.Trim();
            if (string.IsNullOrEmpty(newItemText)) return;

            // Call our new, dedicated helper method
            await AddSingleItemToListAsync(newItemText);

            txtAddItem.Clear();
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (lvWorkshopItems.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lvWorkshopItems.SelectedItems)
                {
                    lvWorkshopItems.Items.Remove(item);
                }
            }
            else
            {
                MessageBox.Show("Please select one or more items to remove.", "No Item Selected");
            }
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            txtAppId.Clear();
            lvWorkshopItems.Items.Clear();
        }

        private void btnSaveList_Click(object sender, EventArgs e)
        {
            if (lvWorkshopItems.Items.Count == 0) { MessageBox.Show("There are no items to save.", "Nothing to Save"); return; }
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.Title = "Save Workshop List";
                sfd.FileName = "MyModList.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var linesToSave = new List<string> { txtAppId.Text };
                    linesToSave.AddRange(lvWorkshopItems.Items.Cast<ListViewItem>().Select(item => item.SubItems[0].Text));
                    File.WriteAllLines(sfd.FileName, linesToSave);
                }
            }
        }

        private void btnLoadList_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                ofd.Title = "Load Workshop List";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var allLines = File.ReadAllLines(ofd.FileName);
                    if (allLines.Length > 0)
                    {
                        lvWorkshopItems.Items.Clear();
                        txtAppId.Text = allLines[0];
                        foreach (string workshopId in allLines.Skip(1))
                        {
                            ListViewItem item = new ListViewItem(workshopId);
                            item.SubItems.Add("Pending");
                            lvWorkshopItems.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(steamCmdContentPath))
            {
                Process.Start("explorer.exe", steamCmdContentPath);
            }
            else
            {
                MessageBox.Show("The download folder has not been created yet.", "Folder Not Found");
            }
        }
        #endregion

        #region Core Logic & Helper Methods
        private async Task StartDownloadProcessAsync(List<string> workshopIds)
        {
            if (!workshopIds.Any()) { return; }
            bool isSteamCmdReady = await CheckAndInstallSteamCmdAsync();
            if (!isSteamCmdReady) { MessageBox.Show("Could not prepare SteamCMD.", "Error"); return; }

            SetControlsEnabled(false);
            progressBar.Value = 0;
            progressBar.Maximum = workshopIds.Count;
            txtLog.AppendText("--- Starting Download Process ---\n");

            // --- UPGRADED STATUS UPDATER ---
            // This logic now correctly finds the list item by its text,
            // which works for both full downloads and retrying failed items.
            var progress = new Progress<(string itemId, string status)>(update =>
            {
                var itemToUpdate = lvWorkshopItems.Items.Cast<ListViewItem>()
                    .FirstOrDefault(item => ParseWorkshopIds(new[] { item.SubItems[0].Text }).FirstOrDefault() == update.itemId);

                if (itemToUpdate != null)
                {
                    itemToUpdate.SubItems[1].Text = update.status;
                }

                if (update.status == "Success" || update.status == "Failed")
                {
                    if (progressBar.Value < progressBar.Maximum) { progressBar.Value++; }
                }
                if (itemToUpdate != null)
                {
                    itemToUpdate.SubItems[1].Text = update.status;

                    // --- NEW COLOR LOGIC ---
                    if (update.status == "Success")
                    {
                        itemToUpdate.SubItems[1].ForeColor = ColorTranslator.FromHtml("#A1CD44"); // Bright Green
                    }
                    else if (update.status == "Failed")
                    {
                        itemToUpdate.SubItems[1].ForeColor = ColorTranslator.FromHtml("#D23B2A"); // Bright Red
                    }
                    else
                    {
                        // Reset to default color for "Pending" or "Downloading"
                        itemToUpdate.SubItems[1].ForeColor = ColorTranslator.FromHtml("#C7D5E0"); // Light Gray
                    }
                }
            });

            try
            {
                await Task.Run(() => RunSteamCmd(txtAppId.Text, workshopIds, progress));
                Log("BACKGROUND TASK: Download task completed.");
            }
            catch (Exception ex)
            {
                Log($"CRITICAL FAILURE: {ex.Message}");
            }
            finally
            {
                progressBar.Value = progressBar.Maximum;
                SetControlsEnabled(true);
            }


        }

        private void RunSteamCmd(string appId, List<string> workshopIds, IProgress<(string itemId, string status)> progress)
        {
            string installDir = Path.GetDirectoryName(steamCmdExePath);
            var parsedIds = ParseWorkshopIds(workshopIds.ToArray()).ToList();

            foreach (var itemId in parsedIds)
            {
                progress.Report((itemId: itemId, status: "Downloading..."));
                bool downloadSucceeded = false;
                var process = new Process();
                var startInfo = new ProcessStartInfo
                {
                    FileName = steamCmdExePath,
                    Arguments = $"+force_install_dir \"{installDir}\" +login anonymous +workshop_download_item {appId} {itemId} +quit",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                };
                process.StartInfo = startInfo;
                process.OutputDataReceived += (s, args) => { if (args.Data != null && args.Data.Contains("Success. Downloaded item")) { downloadSucceeded = true; } };
                process.ErrorDataReceived += (s, args) => { }; // We can ignore stderr for this simplified success check

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                bool exited = process.WaitForExit(600000); // 10 minute timeout

                if (exited && downloadSucceeded)
                {
                    progress.Report((itemId: itemId, status: "Success"));
                }
                else
                {
                    progress.Report((itemId: itemId, status: "Failed"));
                    if (!exited) { process.Kill(); }
                }
            }
        }

        private async Task<string> FetchAppIdFromWorkshopUrlAsync(string url)
        {
            try
            {
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='breadcrumbs']//a[contains(@href, '/app/')]");
                if (node != null)
                {
                    string href = node.GetAttributeValue("href", string.Empty);
                    var match = Regex.Match(href, @"\d+");
                    if (match.Success) { return match.Value; }
                }
            }
            catch { /* Fail silently */ }
            return null;
        }

        private async Task AddItemsFromCollectionUrlAsync(string url)
        {
            Log($"Attempting to parse collection from URL: {url}");
            SetControlsEnabled(false); // Disable controls while we work

            try
            {
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                var itemNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'collectionItemDetails')]");

                if (itemNodes == null || itemNodes.Count == 0)
                {
                    Log("No collection items found. Treating as a single item link.");
                    // This now correctly calls our new awaitable helper method
                    await AddSingleItemToListAsync(url);
                    return;
                }

                Log($"Found {itemNodes.Count} items in the collection. Adding placeholders...");

                // This list will hold the items we need to update
                var itemsToUpdate = new List<(ListViewItem item, string itemUrl)>();

                foreach (var node in itemNodes)
                {
                    var linkNode = node.SelectSingleNode(".//a");
                    if (linkNode != null)
                    {
                        string itemUrl = linkNode.GetAttributeValue("href", string.Empty);
                        if (!string.IsNullOrEmpty(itemUrl))
                        {
                            // Instantly add the item to the list with a placeholder
                            ListViewItem newItem = new ListViewItem("Fetching name...");
                            newItem.SubItems.Add("Pending");
                            newItem.SubItems.Add(itemUrl);
                            lvWorkshopItems.Items.Add(newItem);

                            // Add the new item and its URL to a list for background processing
                            itemsToUpdate.Add((newItem, itemUrl));
                        }
                    }
                }

                // Now, fetch all the names in the background concurrently
                foreach (var tuple in itemsToUpdate)
                {
                    // We use a "fire-and-forget" task for each item.
                    // This starts all the downloads at once.
                    _ = Task.Run(async () =>
                    {
                        string modName = await FetchModNameFromUrlAsync(tuple.itemUrl);

                        // We must use Invoke to safely update the UI from a background thread
                        this.Invoke(new Action(() =>
                        {
                            tuple.item.Text = modName;
                        }));
                    });
                }
                Log($"Started fetching names for {itemsToUpdate.Count} items in the background.");
            }
            catch (Exception ex)
            {
                Log($"FAILURE: Could not parse collection. Error: {ex.Message}");
                MessageBox.Show($"Could not process the URL.\nError: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Re-enable controls
                SetControlsEnabled(true);
            }
        }

        private List<string> ParseWorkshopIdsFromListView()
        {
            return lvWorkshopItems.Items.Cast<ListViewItem>().Select(item => item.SubItems[0].Text).ToList();
        }

        private List<string> ParseWorkshopIds(string[] lines)
        {
            var idList = new List<string>();
            var regex = new Regex(@"\d+");
            foreach (var line in lines)
            {
                var match = regex.Match(line.Trim());
                if (match.Success) { idList.Add(match.Value); }
            }
            return idList.Distinct().ToList();
        }

        private void Log(string message)
        {
            if (string.IsNullOrEmpty(message) || txtLog.IsDisposed) return;
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => Log(message))); }
            else { txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}"); }
        }

        private async Task AddSingleItemToListAsync(string itemUrlOrId)
        {
            // Add the item to the list with a placeholder
            ListViewItem newItem = new ListViewItem("Fetching name..."); // Placeholder for Name
            newItem.SubItems.Add("Pending");                             // Status
            newItem.SubItems.Add(itemUrlOrId);                           // The actual ID/Link
            lvWorkshopItems.Items.Add(newItem);

            // Fetch the real name in the background.
            string modName = await Task.Run(() => FetchModNameFromUrlAsync(itemUrlOrId));

            // Update the placeholder text in the ListView.
            newItem.Text = modName;
            Log($"Added '{modName}' to the list.");
        }

        private void SetControlsEnabled(bool isEnabled)
        {
            foreach (Control c in this.Controls)
            {
                if (c is Button || c is TextBox || c is ListView)
                {
                    c.Enabled = isEnabled;
                }
            }
        }

        private async Task<bool> CheckAndInstallSteamCmdAsync()
        {
            if (File.Exists(steamCmdExePath)) { return true; }
            if (MessageBox.Show("SteamCMD is required. Download it now?", "Setup Required", MessageBoxButtons.YesNo) == DialogResult.No) return false;
            try
            {
                string steamCmdDir = Path.GetDirectoryName(steamCmdExePath);
                if (!Directory.Exists(steamCmdDir)) { Directory.CreateDirectory(steamCmdDir); }
                const string steamCmdUrl = "https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip";
                var zipPath = Path.Combine(Path.GetTempPath(), "steamcmd.zip");
                File.WriteAllBytes(zipPath, await client.GetByteArrayAsync(steamCmdUrl));
                ZipFile.ExtractToDirectory(zipPath, steamCmdDir, true);
                File.Delete(zipPath);
                var process = Process.Start(new ProcessStartInfo(steamCmdExePath, "+quit") { CreateNoWindow = true });
                await process.WaitForExitAsync();
                return true;
            }
            catch { return false; }
        }
        #endregion

        private void panelButtons_Paint(object sender, PaintEventArgs e)
        {

        }

        private void progressBar_Click(object sender, EventArgs e)
        {

        }

        private void topPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtAddItem_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtAppId_TextChanged(object sender, EventArgs e)
        {

        }
    }
}