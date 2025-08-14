namespace SteamCMDDL
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblAppId = new Label();
            txtAppId = new TextBox();
            lblWorkshopItem = new Label();
            rtbWorkshopIds = new RichTextBox();
            btnDownload = new Button();
            progressBar = new ProgressBar();
            txtLog = new TextBox();
            btnClearAll = new Button();
            btnSaveList = new Button();
            btnLoadList = new Button();
            btnOpenFolder = new Button();
            SuspendLayout();
            // 
            // lblAppId
            // 
            lblAppId.AutoSize = true;
            lblAppId.Font = new Font("Stencil", 14F, FontStyle.Bold);
            lblAppId.Location = new Point(172, 77);
            lblAppId.Name = "lblAppId";
            lblAppId.Size = new Size(221, 33);
            lblAppId.TabIndex = 0;
            lblAppId.Text = "Steam App ID:";
            // 
            // txtAppId
            // 
            txtAppId.Font = new Font("Stencil", 14F, FontStyle.Bold);
            txtAppId.Location = new Point(187, 136);
            txtAppId.Name = "txtAppId";
            txtAppId.Size = new Size(177, 41);
            txtAppId.TabIndex = 1;
            // 
            // lblWorkshopItem
            // 
            lblWorkshopItem.AutoSize = true;
            lblWorkshopItem.Font = new Font("Stencil", 14F, FontStyle.Bold);
            lblWorkshopItem.Location = new Point(566, 28);
            lblWorkshopItem.Name = "lblWorkshopItem";
            lblWorkshopItem.Size = new Size(647, 33);
            lblWorkshopItem.TabIndex = 2;
            lblWorkshopItem.Text = "Workshop Item Links/IDs (one per line):";
            // 
            // rtbWorkshopIds
            // 
            rtbWorkshopIds.Location = new Point(542, 79);
            rtbWorkshopIds.Name = "rtbWorkshopIds";
            rtbWorkshopIds.Size = new Size(695, 702);
            rtbWorkshopIds.TabIndex = 3;
            rtbWorkshopIds.Text = "";
            // 
            // btnDownload
            // 
            btnDownload.Font = new Font("Stencil", 14F, FontStyle.Bold);
            btnDownload.Location = new Point(171, 199);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(208, 53);
            btnDownload.TabIndex = 4;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = true;
            btnDownload.Click += btnDownload_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(42, 258);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(484, 34);
            progressBar.TabIndex = 5;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(42, 298);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(484, 483);
            txtLog.TabIndex = 6;
            // 
            // btnClearAll
            // 
            btnClearAll.Font = new Font("Stencil", 14F, FontStyle.Bold);
            btnClearAll.Location = new Point(76, 819);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(128, 41);
            btnClearAll.TabIndex = 7;
            btnClearAll.Text = "Clear All";
            btnClearAll.UseVisualStyleBackColor = true;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // btnSaveList
            // 
            btnSaveList.Font = new Font("Stencil", 14F, FontStyle.Bold);
            btnSaveList.Location = new Point(752, 819);
            btnSaveList.Name = "btnSaveList";
            btnSaveList.Size = new Size(209, 41);
            btnSaveList.TabIndex = 8;
            btnSaveList.Text = "Save List...";
            btnSaveList.UseVisualStyleBackColor = true;
            btnSaveList.Click += btnSaveList_Click;
            // 
            // btnLoadList
            // 
            btnLoadList.Font = new Font("Stencil", 14F, FontStyle.Bold);
            btnLoadList.Location = new Point(1004, 819);
            btnLoadList.Name = "btnLoadList";
            btnLoadList.Size = new Size(209, 41);
            btnLoadList.TabIndex = 8;
            btnLoadList.Text = "Load List...";
            btnLoadList.UseVisualStyleBackColor = true;
            btnLoadList.Click += btnLoadList_Click;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.Font = new Font("Stencil", 14F, FontStyle.Bold);
            btnOpenFolder.Location = new Point(250, 819);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(441, 41);
            btnOpenFolder.TabIndex = 8;
            btnOpenFolder.Text = "Open Download Folder";
            btnOpenFolder.UseVisualStyleBackColor = true;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1271, 904);
            Controls.Add(btnOpenFolder);
            Controls.Add(btnLoadList);
            Controls.Add(btnSaveList);
            Controls.Add(btnClearAll);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnDownload);
            Controls.Add(rtbWorkshopIds);
            Controls.Add(lblWorkshopItem);
            Controls.Add(txtAppId);
            Controls.Add(lblAppId);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAppId;
        private TextBox txtAppId;
        private Label lblWorkshopItem;
        private RichTextBox rtbWorkshopIds;
        private Button btnDownload;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private Button btnClearAll;
        private Button btnSaveList;
        private Button btnLoadList;
        private Button btnOpenFolder;
    }
}
