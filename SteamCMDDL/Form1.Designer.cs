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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            btnDownload = new Button();
            progressBar = new ProgressBar();
            txtLog = new TextBox();
            btnSaveList = new Button();
            btnLoadList = new Button();
            btnOpenFolder = new Button();
            btnRetryFailed = new Button();
            txtAddItem = new TextBox();
            label1 = new Label();
            btnAddItem = new Button();
            txtAppId = new TextBox();
            lblAppId = new Label();
            btnRemoveSelected = new Button();
            btnClearAll = new Button();
            picModPreview = new PictureBox();
            lvWorkshopItems = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            lblModAuthor = new Label();
            lblModSize = new Label();
            lblModPosted = new Label();
            lblModVisitors = new Label();
            ((System.ComponentModel.ISupportInitialize)picModPreview).BeginInit();
            SuspendLayout();
            // 
            // btnDownload
            // 
            btnDownload.BackColor = Color.DimGray;
            btnDownload.FlatStyle = FlatStyle.Flat;
            btnDownload.Font = new Font("Microsoft Sans Serif", 10F);
            btnDownload.ForeColor = Color.White;
            btnDownload.Location = new Point(251, 475);
            btnDownload.Margin = new Padding(4, 3, 4, 3);
            btnDownload.Name = "btnDownload";
            btnDownload.Size = new Size(194, 48);
            btnDownload.TabIndex = 4;
            btnDownload.Text = "Download";
            btnDownload.UseVisualStyleBackColor = false;
            btnDownload.Click += btnDownload_Click;
            // 
            // progressBar
            // 
            progressBar.ForeColor = Color.FromArgb(102, 192, 244);
            progressBar.Location = new Point(60, 528);
            progressBar.Margin = new Padding(4, 3, 4, 3);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(1066, 43);
            progressBar.TabIndex = 5;
            progressBar.UseWaitCursor = true;
            // 
            // txtLog
            // 
            txtLog.BackColor = Color.FromArgb(24, 24, 24);
            txtLog.ForeColor = Color.Green;
            txtLog.Location = new Point(59, 577);
            txtLog.Margin = new Padding(4, 3, 4, 3);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.Size = new Size(1067, 285);
            txtLog.TabIndex = 6;
            // 
            // btnSaveList
            // 
            btnSaveList.BackColor = Color.DimGray;
            btnSaveList.FlatStyle = FlatStyle.Flat;
            btnSaveList.Font = new Font("Microsoft Sans Serif", 10F);
            btnSaveList.ForeColor = Color.FromArgb(216, 216, 216);
            btnSaveList.Location = new Point(157, 420);
            btnSaveList.Margin = new Padding(4, 3, 4, 3);
            btnSaveList.Name = "btnSaveList";
            btnSaveList.Size = new Size(156, 48);
            btnSaveList.TabIndex = 8;
            btnSaveList.Text = "Save List";
            btnSaveList.UseVisualStyleBackColor = false;
            btnSaveList.Click += btnSaveList_Click;
            // 
            // btnLoadList
            // 
            btnLoadList.BackColor = Color.DimGray;
            btnLoadList.FlatStyle = FlatStyle.Flat;
            btnLoadList.Font = new Font("Microsoft Sans Serif", 10F);
            btnLoadList.ForeColor = Color.FromArgb(216, 216, 216);
            btnLoadList.Location = new Point(820, 420);
            btnLoadList.Margin = new Padding(4, 3, 4, 3);
            btnLoadList.Name = "btnLoadList";
            btnLoadList.Size = new Size(141, 48);
            btnLoadList.TabIndex = 8;
            btnLoadList.Text = "Load List";
            btnLoadList.UseVisualStyleBackColor = false;
            btnLoadList.Click += btnLoadList_Click;
            // 
            // btnOpenFolder
            // 
            btnOpenFolder.BackColor = Color.DimGray;
            btnOpenFolder.FlatStyle = FlatStyle.Flat;
            btnOpenFolder.Font = new Font("Microsoft Sans Serif", 10F);
            btnOpenFolder.ForeColor = Color.FromArgb(216, 216, 216);
            btnOpenFolder.Location = new Point(376, 868);
            btnOpenFolder.Margin = new Padding(4, 3, 4, 3);
            btnOpenFolder.Name = "btnOpenFolder";
            btnOpenFolder.Size = new Size(378, 48);
            btnOpenFolder.TabIndex = 8;
            btnOpenFolder.Text = "Open Download Folder";
            btnOpenFolder.UseVisualStyleBackColor = false;
            btnOpenFolder.Click += btnOpenFolder_Click;
            // 
            // btnRetryFailed
            // 
            btnRetryFailed.BackColor = Color.DimGray;
            btnRetryFailed.FlatStyle = FlatStyle.Flat;
            btnRetryFailed.Font = new Font("Microsoft Sans Serif", 10F);
            btnRetryFailed.ForeColor = Color.FromArgb(216, 216, 216);
            btnRetryFailed.Location = new Point(691, 475);
            btnRetryFailed.Margin = new Padding(4, 3, 4, 3);
            btnRetryFailed.Name = "btnRetryFailed";
            btnRetryFailed.Size = new Size(213, 48);
            btnRetryFailed.TabIndex = 13;
            btnRetryFailed.Text = "Retry Failed";
            btnRetryFailed.UseVisualStyleBackColor = false;
            btnRetryFailed.Click += btnRetryFailed_Click;
            // 
            // txtAddItem
            // 
            txtAddItem.BackColor = Color.DimGray;
            txtAddItem.BorderStyle = BorderStyle.FixedSingle;
            txtAddItem.Font = new Font("Microsoft Sans Serif", 10F);
            txtAddItem.ForeColor = Color.FromArgb(199, 213, 224);
            txtAddItem.Location = new Point(357, 88);
            txtAddItem.Margin = new Padding(4, 3, 4, 3);
            txtAddItem.Name = "txtAddItem";
            txtAddItem.Size = new Size(604, 30);
            txtAddItem.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft Sans Serif", 10F);
            label1.Location = new Point(59, 93);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(276, 25);
            label1.TabIndex = 0;
            label1.Text = "Workshop Collection/Links/ID:";
            // 
            // btnAddItem
            // 
            btnAddItem.BackColor = Color.DimGray;
            btnAddItem.FlatStyle = FlatStyle.Flat;
            btnAddItem.Font = new Font("Microsoft Sans Serif", 10F);
            btnAddItem.ForeColor = Color.FromArgb(216, 216, 216);
            btnAddItem.Location = new Point(979, 45);
            btnAddItem.Margin = new Padding(4, 3, 4, 3);
            btnAddItem.Name = "btnAddItem";
            btnAddItem.Size = new Size(146, 73);
            btnAddItem.TabIndex = 11;
            btnAddItem.Text = "Add";
            btnAddItem.UseVisualStyleBackColor = false;
            btnAddItem.Click += btnAddItem_Click;
            // 
            // txtAppId
            // 
            txtAppId.AccessibleDescription = "Steam App ID";
            txtAppId.BackColor = Color.DimGray;
            txtAppId.BorderStyle = BorderStyle.FixedSingle;
            txtAppId.Font = new Font("Microsoft Sans Serif", 10F);
            txtAppId.ForeColor = Color.FromArgb(199, 213, 224);
            txtAppId.Location = new Point(316, 43);
            txtAppId.Margin = new Padding(4, 3, 4, 3);
            txtAppId.Name = "txtAppId";
            txtAppId.Size = new Size(212, 30);
            txtAppId.TabIndex = 1;
            // 
            // lblAppId
            // 
            lblAppId.AutoSize = true;
            lblAppId.Font = new Font("Microsoft Sans Serif", 10F);
            lblAppId.Location = new Point(60, 45);
            lblAppId.Margin = new Padding(4, 0, 4, 0);
            lblAppId.Name = "lblAppId";
            lblAppId.Size = new Size(227, 25);
            lblAppId.TabIndex = 0;
            lblAppId.Text = "Steam App ID (optional):";
            // 
            // btnRemoveSelected
            // 
            btnRemoveSelected.BackColor = Color.DimGray;
            btnRemoveSelected.FlatStyle = FlatStyle.Flat;
            btnRemoveSelected.Font = new Font("Microsoft Sans Serif", 10F);
            btnRemoveSelected.ForeColor = Color.FromArgb(216, 216, 216);
            btnRemoveSelected.Location = new Point(376, 421);
            btnRemoveSelected.Margin = new Padding(4, 3, 4, 3);
            btnRemoveSelected.Name = "btnRemoveSelected";
            btnRemoveSelected.Size = new Size(378, 48);
            btnRemoveSelected.TabIndex = 12;
            btnRemoveSelected.Text = "Remove selected";
            btnRemoveSelected.UseVisualStyleBackColor = false;
            btnRemoveSelected.Click += btnRemoveSelected_Click;
            // 
            // btnClearAll
            // 
            btnClearAll.BackColor = Color.DimGray;
            btnClearAll.FlatStyle = FlatStyle.Flat;
            btnClearAll.Font = new Font("Microsoft Sans Serif", 10F);
            btnClearAll.ForeColor = Color.FromArgb(216, 216, 216);
            btnClearAll.Location = new Point(453, 475);
            btnClearAll.Margin = new Padding(4, 3, 4, 3);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(230, 48);
            btnClearAll.TabIndex = 7;
            btnClearAll.Text = "Clear All";
            btnClearAll.UseVisualStyleBackColor = false;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // picModPreview
            // 
            picModPreview.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            picModPreview.Image = Properties.Resources.Untitled;
            picModPreview.Location = new Point(1132, 124);
            picModPreview.Name = "picModPreview";
            picModPreview.Size = new Size(437, 290);
            picModPreview.SizeMode = PictureBoxSizeMode.CenterImage;
            picModPreview.TabIndex = 14;
            picModPreview.TabStop = false;
            // 
            // lvWorkshopItems
            // 
            lvWorkshopItems.BackColor = Color.FromArgb(64, 64, 64);
            lvWorkshopItems.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3 });
            lvWorkshopItems.ForeColor = Color.Silver;
            lvWorkshopItems.FullRowSelect = true;
            lvWorkshopItems.GridLines = true;
            lvWorkshopItems.Location = new Point(59, 124);
            lvWorkshopItems.Name = "lvWorkshopItems";
            lvWorkshopItems.Size = new Size(1066, 290);
            lvWorkshopItems.TabIndex = 15;
            lvWorkshopItems.UseCompatibleStateImageBehavior = false;
            lvWorkshopItems.View = View.Details;
            lvWorkshopItems.SelectedIndexChanged += lvWorkshopItems_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "Name";
            columnHeader1.Width = 481;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "Status";
            columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "Link/ID";
            columnHeader3.Width = 481;
            // 
            // lblModAuthor
            // 
            lblModAuthor.AutoSize = true;
            lblModAuthor.Location = new Point(1304, 432);
            lblModAuthor.Name = "lblModAuthor";
            lblModAuthor.Size = new Size(115, 25);
            lblModAuthor.TabIndex = 16;
            lblModAuthor.Text = "Author: N/A";
            // 
            // lblModSize
            // 
            lblModSize.AutoSize = true;
            lblModSize.Location = new Point(1304, 487);
            lblModSize.Name = "lblModSize";
            lblModSize.Size = new Size(96, 25);
            lblModSize.TabIndex = 16;
            lblModSize.Text = "Size: N/A";
            // 
            // lblModPosted
            // 
            lblModPosted.AutoSize = true;
            lblModPosted.Location = new Point(1304, 546);
            lblModPosted.Name = "lblModPosted";
            lblModPosted.Size = new Size(118, 25);
            lblModPosted.TabIndex = 16;
            lblModPosted.Text = "Posted: N/A";
            // 
            // lblModVisitors
            // 
            lblModVisitors.AutoSize = true;
            lblModVisitors.Location = new Point(1304, 608);
            lblModVisitors.Name = "lblModVisitors";
            lblModVisitors.Size = new Size(121, 25);
            lblModVisitors.TabIndex = 16;
            lblModVisitors.Text = "Visitors: N/A";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(12F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(69, 69, 69);
            ClientSize = new Size(1621, 964);
            Controls.Add(lblModVisitors);
            Controls.Add(lblModPosted);
            Controls.Add(lblModSize);
            Controls.Add(lblModAuthor);
            Controls.Add(lvWorkshopItems);
            Controls.Add(picModPreview);
            Controls.Add(btnOpenFolder);
            Controls.Add(txtLog);
            Controls.Add(progressBar);
            Controls.Add(btnAddItem);
            Controls.Add(btnClearAll);
            Controls.Add(lblAppId);
            Controls.Add(btnLoadList);
            Controls.Add(label1);
            Controls.Add(txtAppId);
            Controls.Add(btnSaveList);
            Controls.Add(btnRetryFailed);
            Controls.Add(txtAddItem);
            Controls.Add(btnRemoveSelected);
            Controls.Add(btnDownload);
            Font = new Font("Microsoft Sans Serif", 10F);
            ForeColor = Color.FromArgb(188, 188, 188);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "Form1";
            Text = "SteamCMDDL";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)picModPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label lblWorkshopItem;
        private Button btnDownload;
        private ProgressBar progressBar;
        private TextBox txtLog;
        private Button btnSaveList;
        private Button btnLoadList;
        private Button btnOpenFolder;
        private Button btnRetryFailed;
        private TextBox txtAddItem;
        private Label label1;
        private Button btnAddItem;
        private TextBox txtAppId;
        private Label lblAppId;
        private Button btnRemoveSelected;
        private Button btnClearAll;
        private PictureBox picModPreview;
        private ListView lvWorkshopItems;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private Label lblModAuthor;
        private Label lblModSize;
        private Label lblModPosted;
        private Label lblModVisitors;
    }
}
