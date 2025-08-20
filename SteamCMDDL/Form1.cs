// These 'using' statements are necessary for the code to work
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

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
            // This line is required to allow the status colors to change.
           
        }

        #region Form Events
        private void Form1_Load(object sender, EventArgs e)
        {
            Log("EVENT: Form_Load - Form is loading and controls are being initialized.");
        }

        // This is the method with the main fix. Added "async" keyword.
        private async void lvWorkshopItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvWorkshopItems.SelectedItems.Count > 0)
            {
                ListViewItem selectedItem = lvWorkshopItems.SelectedItems[0];
                string itemUrl = selectedItem.SubItems[2].Text;

                lblModAuthor.Text = "Author: Loading...";
                lblModSize.Text = "File Size: Loading...";
                lblModPosted.Text = "Posted: Loading...";
                lblModVisitors.Text = "Visitors: Loading...";
                picModPreview.Image = null;

                // The 'await' call now works correctly because the method is async
                ModDetails details = await FetchModDetailsAsync(itemUrl);

                if (details != null)
                {
                    lblModAuthor.Text = $"Author: {details.Author ?? "N/A"}";
                    lblModSize.Text = $"File Size: {details.FileSize ?? "N/A"}";
                    lblModPosted.Text = $"Posted: {details.DatePosted ?? "N/A"}";
                    lblModVisitors.Text = $"Visitors: {details.Visitors ?? "N/A"}";
                    if (!string.IsNullOrEmpty(details.ImageUrl))
                    {
                        picModPreview.LoadAsync(details.ImageUrl);
                    }
                }
                else
                {
                    lblModAuthor.Text = "Author: Failed to load";
                    lblModSize.Text = "File Size: Failed to load";
                    lblModPosted.Text = "Posted: Failed to load";
                    lblModVisitors.Text = "Visitors: Failed to load";
                }
            }
        }
        #endregion

        #region Button Click Event Handlers
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnDownload_Click - 'Download All' button was clicked.");
            if (string.IsNullOrWhiteSpace(txtAppId.Text))
            {
                var firstUrl = lvWorkshopItems.Items.Cast<ListViewItem>().Select(item => item.SubItems[2].Text).FirstOrDefault(text => text.ToLower().StartsWith("http"));
                if (firstUrl != null)
                {
                    string detectedAppId = await FetchAppIdFromWorkshopUrlAsync(firstUrl);
                    if (detectedAppId != null) { txtAppId.Text = detectedAppId; }
                    else { MessageBox.Show("Could not automatically detect the Steam App ID.", "Auto-Detect Failed"); return; }
                }
                else { MessageBox.Show("The App ID is empty and no valid links were found to auto-detect from.", "Error"); return; }
            }
            if (!int.TryParse(txtAppId.Text, out _)) { MessageBox.Show("Please enter a valid numeric App ID.", "Error"); return; }
            var workshopIds = ParseWorkshopIdsFromListView();
            await StartDownloadProcessAsync(workshopIds);
        }

        private async void btnRetryFailed_Click(object sender, EventArgs e)
        {
            Log("EVENT: btnRetryFailed_Click - Retrying failed items.");
            var failedItems = lvWorkshopItems.Items.Cast<ListViewItem>().Where(item => item.SubItems[1].Text == "Failed").ToList();
            if (failedItems.Count == 0) { MessageBox.Show("No failed items found to retry.", "Nothing to Retry"); return; }
            foreach (var item in failedItems) { item.SubItems[1].Text = "Pending"; }
            // Corrected: Get the ID/Link from the 3rd column
            var idsToRetry = failedItems.Select(item => item.SubItems[2].Text).ToList();
            await StartDownloadProcessAsync(idsToRetry);
        }

        private async void btnAddItem_Click(object sender, EventArgs e)
        {
            string newItemText = txtAddItem.Text.Trim();
            if (string.IsNullOrEmpty(newItemText)) { return; }
            await AddSingleItemToListAsync(newItemText);
            txtAddItem.Clear();
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            if (lvWorkshopItems.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in lvWorkshopItems.SelectedItems) { lvWorkshopItems.Items.Remove(item); }
            }
            else { MessageBox.Show("Please select one or more items to remove.", "No Item Selected"); }
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
                sfd.FileName = "MyModList.txt";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var linesToSave = new List<string> { txtAppId.Text };
                    // Corrected: Save the ID/Link from the 3rd column
                    linesToSave.AddRange(lvWorkshopItems.Items.Cast<ListViewItem>().Select(item => item.SubItems[2].Text));
                    File.WriteAllLines(sfd.FileName, linesToSave);
                }
            }
        }

        private void btnLoadList_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var allLines = File.ReadAllLines(ofd.FileName);
                    if (allLines.Length > 0)
                    {
                        lvWorkshopItems.Items.Clear();
                        txtAppId.Text = allLines[0];
                        foreach (string workshopUrl in allLines.Skip(1))
                        {
                            _ = AddSingleItemToListAsync(workshopUrl);
                        }
                    }
                }
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(steamCmdContentPath)) { Process.Start("explorer.exe", steamCmdContentPath); }
            else { MessageBox.Show("The download folder has not been created yet.", "Folder Not Found"); }
        }
        #endregion

        #region Core Logic & Helper Methods
        private async Task StartDownloadProcessAsync(List<string> workshopUrls)
        {
            if (!workshopUrls.Any()) { return; }
            bool isSteamCmdReady = await CheckAndInstallSteamCmdAsync();
            if (!isSteamCmdReady) { MessageBox.Show("Could not prepare SteamCMD.", "Error"); return; }
            SetControlsEnabled(false);
            progressBar.Value = 0;
            progressBar.Maximum = workshopUrls.Count;
            var progress = new Progress<(string itemId, string status)>(update =>
            {
                var itemToUpdate = lvWorkshopItems.Items.Cast<ListViewItem>().FirstOrDefault(item => ParseWorkshopIds(new[] { item.SubItems[2].Text }).FirstOrDefault() == update.itemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.SubItems[1].Text = update.status;
                    if (update.status == "Success") { itemToUpdate.SubItems[1].ForeColor = ColorTranslator.FromHtml("#A1CD44"); }
                    else if (update.status == "Failed") { itemToUpdate.SubItems[1].ForeColor = ColorTranslator.FromHtml("#D23B2A"); }
                    else { itemToUpdate.SubItems[1].ForeColor = this.ForeColor; }
                }
                if (update.status == "Success" || update.status == "Failed") { if (progressBar.Value < progressBar.Maximum) { progressBar.Value++; } }
            });
            try { await Task.Run(() => RunSteamCmd(txtAppId.Text, workshopUrls, progress)); }
            finally { progressBar.Value = progressBar.Maximum; SetControlsEnabled(true); }
        }

        private void RunSteamCmd(string appId, List<string> workshopUrls, IProgress<(string itemId, string status)> progress)
        {
            string installDir = Path.GetDirectoryName(steamCmdExePath);
            var parsedWorkshopIds = ParseWorkshopIds(workshopUrls.ToArray());
            foreach (var itemId in parsedWorkshopIds)
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
                process.ErrorDataReceived += (s, args) => { Log($"[SteamCMD ERR]: {args.Data}"); };
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                System.Threading.Thread.Sleep(2000);
                bool exited = process.WaitForExit(600000);
                if (exited && downloadSucceeded) { progress.Report((itemId: itemId, status: "Success")); }
                else { progress.Report((itemId: itemId, status: "Failed")); if (!exited) { process.Kill(); } }
            }
        }

        private async Task AddSingleItemToListAsync(string itemUrlOrId)
        {
            ListViewItem newItem = new ListViewItem("Fetching name...");
            newItem.SubItems.Add("Pending");
            newItem.SubItems.Add(itemUrlOrId);
            lvWorkshopItems.Items.Add(newItem);
            string modName = await Task.Run(() => FetchModNameFromUrlAsync(itemUrlOrId));
            newItem.Text = modName;
        }

        private async Task AddItemsFromCollectionUrlAsync(string url)
        {
            SetControlsEnabled(false);
            try
            {
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);
                var itemNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'collectionItemDetails')]");
                if (itemNodes == null || itemNodes.Count == 0) { await AddSingleItemToListAsync(url); }
                else
                {
                    foreach (var node in itemNodes)
                    {
                        var linkNode = node.SelectSingleNode(".//a");
                        if (linkNode != null) { _ = AddSingleItemToListAsync(linkNode.GetAttributeValue("href", string.Empty)); }
                    }
                }
            }
            finally { SetControlsEnabled(true); }
        }

        private async Task<string> FetchModNameFromUrlAsync(string url)
        {
            try
            {
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);
                var node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='workshopItemTitle']");
                if (node != null) { return System.Net.WebUtility.HtmlDecode(node.InnerText.Trim()); }
            }
            catch { /* Fail silently */ }
            return "Unknown Mod Name";
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
                    var match = Regex.Match(node.GetAttributeValue("href", string.Empty), @"\d+");
                    if (match.Success) { return match.Value; }
                }
            }
            catch { /* Fail silently */ }
            return null;
        }

        private List<string> ParseWorkshopIdsFromListView()
        {
            return lvWorkshopItems.Items.Cast<ListViewItem>().Select(item => item.SubItems[2].Text).ToList();
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

        private void SetControlsEnabled(bool isEnabled)
        {
            Action<Control.ControlCollection> enableDisable = null;
            enableDisable = (controls) =>
            {
                foreach (Control c in controls)
                {
                    if (c is Button || c is TextBox || c is ListView) { c.Enabled = isEnabled; }
                    if (c.HasChildren) { enableDisable(c.Controls); }
                }
            };
            enableDisable(this.Controls);
        }

        private void Log(string message)
        {
            if (string.IsNullOrEmpty(message) || this.IsDisposed || txtLog.IsDisposed) return;
            if (txtLog.InvokeRequired) { txtLog.Invoke(new Action(() => Log(message))); }
            else { txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}"); }
        }

        private async Task<bool> CheckAndInstallSteamCmdAsync()
        {
            if (File.Exists(steamCmdExePath)) { return true; }
            if (MessageBox.Show("SteamCMD is required. Download it now?", "Setup Required", MessageBoxButtons.YesNo) == DialogResult.No) return false;
            try
            {
                string steamCmdDir = Path.GetDirectoryName(steamCmdExePath);
                if (!Directory.Exists(steamCmdDir)) { Directory.CreateDirectory(steamCmdDir); }
                var zipPath = Path.Combine(Path.GetTempPath(), "steamcmd.zip");
                File.WriteAllBytes(zipPath, await client.GetByteArrayAsync("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip"));
                ZipFile.ExtractToDirectory(zipPath, steamCmdDir, true);
                File.Delete(zipPath);
                var process = Process.Start(new ProcessStartInfo(steamCmdExePath, "+quit") { CreateNoWindow = true });
                await process.WaitForExitAsync();
                return true;
            }
            catch { return false; }
        }


        private async Task<ModDetails> FetchModDetailsAsync(string url)
        {
            var details = new ModDetails
            {
                Author = "N/A",
                FileSize = "N/A",
                DatePosted = "N/A",
                Visitors = "N/A"
            };

            try
            {
                var html = await client.GetStringAsync(url);
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(html);

                // Image
                details.ImageUrl =
                    htmlDoc.DocumentNode.SelectSingleNode("//img[@id='previewImageMain']")
                            ?.GetAttributeValue("src", null)
                    ?? htmlDoc.DocumentNode.SelectSingleNode("//img[contains(@class, 'workshopItemPreviewImage')]")
                        ?.GetAttributeValue("src", null);

                // Author (breadcrumbs fallback)
                var breadcrumb = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='breadcrumbs']//a[contains(@href, 'profiles') or contains(@href, 'id/')]");
                if (breadcrumb != null)
                    details.Author = breadcrumb.InnerText.Trim();

                // File Size, Posted, Updated: parse rightDetailsBlock as label-value pairs
                var rightBlock = htmlDoc.DocumentNode.SelectSingleNode("//div[contains(@class, 'rightDetailsBlock')]");
                if (rightBlock != null)
                {
                    var divs = rightBlock.SelectNodes("./div");
                    if (divs != null)
                    {
                        for (int i = 0; i < divs.Count - 1; i++)
                        {
                            var label = divs[i].InnerText.Trim();
                            var value = divs[i + 1].InnerText.Trim();

                            if (label.Equals("File Size", StringComparison.OrdinalIgnoreCase))
                                details.FileSize = value;
                            else if (label.Equals("Posted", StringComparison.OrdinalIgnoreCase))
                                details.DatePosted = value;
                            else if (label.Equals("Unique Visitors", StringComparison.OrdinalIgnoreCase))
                                details.Visitors = value;
                        }
                    }
                }

                // Fallback: try to extract using regex if HTML parsing fails
                if (details.FileSize == "N/A")
                {
                    var fileSizeMatch = Regex.Match(html, @"File Size<\/div>\s*<div[^>]*>([^<]+)<\/div>", RegexOptions.IgnoreCase);
                    if (fileSizeMatch.Success)
                        details.FileSize = fileSizeMatch.Groups[1].Value.Trim();
                }
                if (details.DatePosted == "N/A")
                {
                    var postedMatch = Regex.Match(html, @"Posted<\/div>\s*<div[^>]*>([^<]+)<\/div>", RegexOptions.IgnoreCase);
                    if (postedMatch.Success)
                        details.DatePosted = postedMatch.Groups[1].Value.Trim();
                }

                return details;
            }
            catch (Exception ex)
            {
                Log($"Failed to fetch mod details for {url}: {ex.Message}");
                return null;
            }
        }
    }
    #endregion
    public class ModDetails
    {
        public string ImageUrl { get; set; }
        public string Author { get; set; }
        public string FileSize { get; set; }
        public string DatePosted { get; set; }
        public string Visitors { get; set; }
    }
}