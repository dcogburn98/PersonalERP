namespace PERP_TaskScheduler
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dgvTasks = new System.Windows.Forms.DataGridView();
            this.pnlDetails = new System.Windows.Forms.Panel();
            this.lblTaskName = new System.Windows.Forms.Label();
            this.lblTaskNameValue = new System.Windows.Forms.Label();
            this.lblModule = new System.Windows.Forms.Label();
            this.lblModuleValue = new System.Windows.Forms.Label();
            this.lblActionType = new System.Windows.Forms.Label();
            this.lblActionTypeValue = new System.Windows.Forms.Label();
            this.lblInterval = new System.Windows.Forms.Label();
            this.lblIntervalValue = new System.Windows.Forms.Label();
            this.lblSecurityReview = new System.Windows.Forms.Label();
            this.txtActionData = new System.Windows.Forms.TextBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnEnable = new System.Windows.Forms.Button();
            this.btnRunNow = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lblHistory = new System.Windows.Forms.Label();
            this.dgvHistory = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).BeginInit();
            this.pnlDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvTasks
            // 
            this.dgvTasks.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTasks.AllowUserToAddRows = false;
            this.dgvTasks.AllowUserToDeleteRows = false;
            this.dgvTasks.ReadOnly = true;
            this.dgvTasks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTasks.MultiSelect = false;
            this.dgvTasks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTasks.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTasks.Location = new System.Drawing.Point(12, 12);
            this.dgvTasks.Name = "dgvTasks";
            this.dgvTasks.Size = new System.Drawing.Size(860, 180);
            this.dgvTasks.TabIndex = 0;
            // 
            // pnlDetails
            // 
            this.pnlDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDetails.Location = new System.Drawing.Point(12, 198);
            this.pnlDetails.Name = "pnlDetails";
            this.pnlDetails.Size = new System.Drawing.Size(860, 140);
            this.pnlDetails.TabIndex = 1;
            this.pnlDetails.Controls.Add(this.lblTaskName);
            this.pnlDetails.Controls.Add(this.lblTaskNameValue);
            this.pnlDetails.Controls.Add(this.lblModule);
            this.pnlDetails.Controls.Add(this.lblModuleValue);
            this.pnlDetails.Controls.Add(this.lblActionType);
            this.pnlDetails.Controls.Add(this.lblActionTypeValue);
            this.pnlDetails.Controls.Add(this.lblInterval);
            this.pnlDetails.Controls.Add(this.lblIntervalValue);
            this.pnlDetails.Controls.Add(this.lblSecurityReview);
            this.pnlDetails.Controls.Add(this.txtActionData);
            this.pnlDetails.Controls.Add(this.btnRefresh);
            this.pnlDetails.Controls.Add(this.btnEnable);
            this.pnlDetails.Controls.Add(this.btnRunNow);
            this.pnlDetails.Controls.Add(this.btnDelete);
            // 
            // lblTaskName
            // 
            this.lblTaskName.AutoSize = true;
            this.lblTaskName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTaskName.Location = new System.Drawing.Point(3, 5);
            this.lblTaskName.Name = "lblTaskName";
            this.lblTaskName.Size = new System.Drawing.Size(72, 13);
            this.lblTaskName.TabIndex = 0;
            this.lblTaskName.Text = "Task Name:";
            // 
            // lblTaskNameValue
            // 
            this.lblTaskNameValue.AutoSize = true;
            this.lblTaskNameValue.Location = new System.Drawing.Point(80, 5);
            this.lblTaskNameValue.Name = "lblTaskNameValue";
            this.lblTaskNameValue.Size = new System.Drawing.Size(0, 13);
            this.lblTaskNameValue.TabIndex = 1;
            // 
            // lblModule
            // 
            this.lblModule.AutoSize = true;
            this.lblModule.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblModule.Location = new System.Drawing.Point(250, 5);
            this.lblModule.Name = "lblModule";
            this.lblModule.Size = new System.Drawing.Size(52, 13);
            this.lblModule.TabIndex = 2;
            this.lblModule.Text = "Module:";
            // 
            // lblModuleValue
            // 
            this.lblModuleValue.AutoSize = true;
            this.lblModuleValue.Location = new System.Drawing.Point(308, 5);
            this.lblModuleValue.Name = "lblModuleValue";
            this.lblModuleValue.Size = new System.Drawing.Size(0, 13);
            this.lblModuleValue.TabIndex = 3;
            // 
            // lblActionType
            // 
            this.lblActionType.AutoSize = true;
            this.lblActionType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblActionType.Location = new System.Drawing.Point(3, 25);
            this.lblActionType.Name = "lblActionType";
            this.lblActionType.Size = new System.Drawing.Size(79, 13);
            this.lblActionType.TabIndex = 4;
            this.lblActionType.Text = "Action Type:";
            // 
            // lblActionTypeValue
            // 
            this.lblActionTypeValue.AutoSize = true;
            this.lblActionTypeValue.Location = new System.Drawing.Point(88, 25);
            this.lblActionTypeValue.Name = "lblActionTypeValue";
            this.lblActionTypeValue.Size = new System.Drawing.Size(0, 13);
            this.lblActionTypeValue.TabIndex = 5;
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblInterval.Location = new System.Drawing.Point(250, 25);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(54, 13);
            this.lblInterval.TabIndex = 6;
            this.lblInterval.Text = "Interval:";
            // 
            // lblIntervalValue
            // 
            this.lblIntervalValue.AutoSize = true;
            this.lblIntervalValue.Location = new System.Drawing.Point(308, 25);
            this.lblIntervalValue.Name = "lblIntervalValue";
            this.lblIntervalValue.Size = new System.Drawing.Size(0, 13);
            this.lblIntervalValue.TabIndex = 7;
            // 
            // lblSecurityReview
            // 
            this.lblSecurityReview.AutoSize = true;
            this.lblSecurityReview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSecurityReview.Location = new System.Drawing.Point(3, 48);
            this.lblSecurityReview.Name = "lblSecurityReview";
            this.lblSecurityReview.Size = new System.Drawing.Size(101, 13);
            this.lblSecurityReview.TabIndex = 8;
            this.lblSecurityReview.Text = "Security Review";
            // 
            // txtActionData
            // 
            this.txtActionData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtActionData.Location = new System.Drawing.Point(6, 64);
            this.txtActionData.Multiline = true;
            this.txtActionData.ReadOnly = true;
            this.txtActionData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtActionData.Name = "txtActionData";
            this.txtActionData.Size = new System.Drawing.Size(550, 60);
            this.txtActionData.TabIndex = 9;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(580, 5);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(80, 25);
            this.btnRefresh.TabIndex = 10;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            // 
            // btnEnable
            // 
            this.btnEnable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnable.Location = new System.Drawing.Point(670, 5);
            this.btnEnable.Name = "btnEnable";
            this.btnEnable.Size = new System.Drawing.Size(80, 25);
            this.btnEnable.TabIndex = 11;
            this.btnEnable.Text = "Enable";
            this.btnEnable.UseVisualStyleBackColor = true;
            // 
            // btnRunNow
            // 
            this.btnRunNow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRunNow.Location = new System.Drawing.Point(760, 5);
            this.btnRunNow.Name = "btnRunNow";
            this.btnRunNow.Size = new System.Drawing.Size(80, 25);
            this.btnRunNow.TabIndex = 12;
            this.btnRunNow.Text = "Run Now";
            this.btnRunNow.UseVisualStyleBackColor = true;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(760, 35);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(80, 25);
            this.btnDelete.TabIndex = 13;
            this.btnDelete.Text = "Delete";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // lblHistory
            // 
            this.lblHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHistory.AutoSize = true;
            this.lblHistory.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblHistory.Location = new System.Drawing.Point(12, 345);
            this.lblHistory.Name = "lblHistory";
            this.lblHistory.Size = new System.Drawing.Size(113, 13);
            this.lblHistory.TabIndex = 2;
            this.lblHistory.Text = "Execution History";
            // 
            // dgvHistory
            // 
            this.dgvHistory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvHistory.AllowUserToAddRows = false;
            this.dgvHistory.AllowUserToDeleteRows = false;
            this.dgvHistory.ReadOnly = true;
            this.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistory.Location = new System.Drawing.Point(12, 362);
            this.dgvHistory.Name = "dgvHistory";
            this.dgvHistory.Size = new System.Drawing.Size(860, 150);
            this.dgvHistory.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 521);
            this.Controls.Add(this.dgvTasks);
            this.Controls.Add(this.pnlDetails);
            this.Controls.Add(this.lblHistory);
            this.Controls.Add(this.dgvHistory);
            this.Name = "MainForm";
            this.Text = "Task Scheduler";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTasks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistory)).EndInit();
            this.pnlDetails.ResumeLayout(false);
            this.pnlDetails.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.DataGridView dgvTasks;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblTaskName;
        private System.Windows.Forms.Label lblTaskNameValue;
        private System.Windows.Forms.Label lblModule;
        private System.Windows.Forms.Label lblModuleValue;
        private System.Windows.Forms.Label lblActionType;
        private System.Windows.Forms.Label lblActionTypeValue;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.Label lblIntervalValue;
        private System.Windows.Forms.Label lblSecurityReview;
        private System.Windows.Forms.TextBox txtActionData;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnEnable;
        private System.Windows.Forms.Button btnRunNow;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Label lblHistory;
        private System.Windows.Forms.DataGridView dgvHistory;
    }
}
