namespace NetworkRenameTool {
    partial class MainForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.NetworksGrid = new System.Windows.Forms.DataGridView();
            this.KeyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Category = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnAddNetworkProfile = new System.Windows.Forms.Button();
            this.cmbNetworkAdapters = new System.Windows.Forms.ComboBox();
            this.txbFakeIp = new System.Windows.Forms.TextBox();
            this.lblNetworkAdapters = new System.Windows.Forms.Label();
            this.lblFakeIp = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSetNdisDeviceType = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // NetworksGrid
            // 
            this.NetworksGrid.AllowUserToAddRows = false;
            this.NetworksGrid.AllowUserToResizeColumns = false;
            this.NetworksGrid.AllowUserToResizeRows = false;
            this.NetworksGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.NetworksGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.KeyColumn,
            this.NameColumn,
            this.DescriptionColumn,
            this.Category});
            this.NetworksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NetworksGrid.Location = new System.Drawing.Point(0, 102);
            this.NetworksGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.NetworksGrid.Name = "NetworksGrid";
            this.NetworksGrid.RowHeadersWidth = 51;
            this.NetworksGrid.Size = new System.Drawing.Size(1130, 326);
            this.NetworksGrid.TabIndex = 0;
            this.NetworksGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.NetworksGrid_CellValueChanged);
            this.NetworksGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.NetworksGrid_UserDeletingRow);
            this.NetworksGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.NetworksGrid_KeyDown);
            // 
            // KeyColumn
            // 
            this.KeyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.KeyColumn.HeaderText = "Registry Key";
            this.KeyColumn.MinimumWidth = 6;
            this.KeyColumn.Name = "KeyColumn";
            this.KeyColumn.ReadOnly = true;
            this.KeyColumn.Width = 6;
            // 
            // NameColumn
            // 
            this.NameColumn.HeaderText = "Profile Name";
            this.NameColumn.MinimumWidth = 6;
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ToolTipText = "The name is displayed on connection list.";
            this.NameColumn.Width = 250;
            // 
            // DescriptionColumn
            // 
            this.DescriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescriptionColumn.HeaderText = "Description";
            this.DescriptionColumn.MinimumWidth = 6;
            this.DescriptionColumn.Name = "DescriptionColumn";
            this.DescriptionColumn.ToolTipText = "This description is not actually displayed anywhere.";
            // 
            // Category
            // 
            this.Category.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Category.HeaderText = "Category (0pub/1priv/2dom)";
            this.Category.MinimumWidth = 6;
            this.Category.Name = "Category";
            this.Category.ToolTipText = "Category indicates whether this network is public or private, so different securi" +
    "ty policies can apply.";
            // 
            // btnAddNetworkProfile
            // 
            this.btnAddNetworkProfile.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAddNetworkProfile.Location = new System.Drawing.Point(15, 15);
            this.btnAddNetworkProfile.Name = "btnAddNetworkProfile";
            this.btnAddNetworkProfile.Size = new System.Drawing.Size(229, 28);
            this.btnAddNetworkProfile.TabIndex = 1;
            this.btnAddNetworkProfile.Text = "Add Network Profile";
            this.btnAddNetworkProfile.UseVisualStyleBackColor = true;
            this.btnAddNetworkProfile.Click += new System.EventHandler(this.btnAddNetworkProfile_Click);
            // 
            // cmbNetworkAdapters
            // 
            this.cmbNetworkAdapters.Dock = System.Windows.Forms.DockStyle.Top;
            this.cmbNetworkAdapters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNetworkAdapters.FormattingEnabled = true;
            this.cmbNetworkAdapters.Location = new System.Drawing.Point(20, 20);
            this.cmbNetworkAdapters.Name = "cmbNetworkAdapters";
            this.cmbNetworkAdapters.Size = new System.Drawing.Size(621, 23);
            this.cmbNetworkAdapters.TabIndex = 2;
            // 
            // txbFakeIp
            // 
            this.txbFakeIp.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txbFakeIp.Location = new System.Drawing.Point(20, 57);
            this.txbFakeIp.Name = "txbFakeIp";
            this.txbFakeIp.Size = new System.Drawing.Size(621, 25);
            this.txbFakeIp.TabIndex = 3;
            this.txbFakeIp.Text = "25.255.255.254";
            // 
            // lblNetworkAdapters
            // 
            this.lblNetworkAdapters.AutoSize = true;
            this.lblNetworkAdapters.Location = new System.Drawing.Point(30, 23);
            this.lblNetworkAdapters.Name = "lblNetworkAdapters";
            this.lblNetworkAdapters.Size = new System.Drawing.Size(143, 15);
            this.lblNetworkAdapters.TabIndex = 4;
            this.lblNetworkAdapters.Text = "Choose an Adapter";
            // 
            // lblFakeIp
            // 
            this.lblFakeIp.AutoSize = true;
            this.lblFakeIp.Location = new System.Drawing.Point(46, 60);
            this.lblFakeIp.Name = "lblFakeIp";
            this.lblFakeIp.Size = new System.Drawing.Size(127, 15);
            this.lblFakeIp.TabIndex = 5;
            this.lblFakeIp.Text = "Fake Gateway IP";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnSetNdisDeviceType);
            this.panel1.Controls.Add(this.btnAddNetworkProfile);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(871, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(15);
            this.panel1.Size = new System.Drawing.Size(259, 102);
            this.panel1.TabIndex = 6;
            // 
            // btnSetNdisDeviceType
            // 
            this.btnSetNdisDeviceType.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSetNdisDeviceType.Location = new System.Drawing.Point(15, 57);
            this.btnSetNdisDeviceType.Name = "btnSetNdisDeviceType";
            this.btnSetNdisDeviceType.Size = new System.Drawing.Size(229, 30);
            this.btnSetNdisDeviceType.TabIndex = 2;
            this.btnSetNdisDeviceType.Text = "Set *NdisDeviceType=1";
            this.btnSetNdisDeviceType.UseVisualStyleBackColor = true;
            this.btnSetNdisDeviceType.Click += new System.EventHandler(this.btnSetNdisDeviceType_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel4);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Cursor = System.Windows.Forms.Cursors.Default;
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1130, 102);
            this.panel2.TabIndex = 7;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.cmbNetworkAdapters);
            this.panel4.Controls.Add(this.txbFakeIp);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(210, 0);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(20);
            this.panel4.Size = new System.Drawing.Size(661, 102);
            this.panel4.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lblNetworkAdapters);
            this.panel3.Controls.Add(this.lblFakeIp);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(210, 102);
            this.panel3.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1130, 428);
            this.Controls.Add(this.NetworksGrid);
            this.Controls.Add(this.panel2);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "MainForm";
            this.Text = "Network Rename Tool";
            ((System.ComponentModel.ISupportInitialize)(this.NetworksGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView NetworksGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn KeyColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Category;
        private System.Windows.Forms.Button btnAddNetworkProfile;
        private System.Windows.Forms.ComboBox cmbNetworkAdapters;
        private System.Windows.Forms.TextBox txbFakeIp;
        private System.Windows.Forms.Label lblNetworkAdapters;
        private System.Windows.Forms.Label lblFakeIp;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSetNdisDeviceType;
    }
}

