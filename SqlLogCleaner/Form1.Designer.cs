namespace SqlLogCleaner
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblSavedConn = new System.Windows.Forms.Label();
            this.cmbSavedConnections = new System.Windows.Forms.ComboBox();
            this.lblTemplate = new System.Windows.Forms.Label();
            this.cmbTemplates = new System.Windows.Forms.ComboBox();
            this.lblConnStr = new System.Windows.Forms.Label();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.lblConnName = new System.Windows.Forms.Label();
            this.txtConnName = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.grpConnection = new System.Windows.Forms.GroupBox();
            this.txtLogOutput = new System.Windows.Forms.TextBox();
            this.grpOperation = new System.Windows.Forms.GroupBox();
            this.dgvLogList = new System.Windows.Forms.DataGridView();
            this.btnBatchClearLog = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.grpConnection.SuspendLayout();
            this.grpOperation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogList)).BeginInit();
            this.SuspendLayout();
            // 
            // lblSavedConn
            // 
            this.lblSavedConn.AutoSize = true;
            this.lblSavedConn.Location = new System.Drawing.Point(15, 26);
            this.lblSavedConn.Name = "lblSavedConn";
            this.lblSavedConn.Size = new System.Drawing.Size(71, 12);
            this.lblSavedConn.TabIndex = 0;
            this.lblSavedConn.Text = "历史连接库:";
            // 
            // cmbSavedConnections
            // 
            this.cmbSavedConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSavedConnections.FormattingEnabled = true;
            this.cmbSavedConnections.Location = new System.Drawing.Point(92, 23);
            this.cmbSavedConnections.Name = "cmbSavedConnections";
            this.cmbSavedConnections.Size = new System.Drawing.Size(140, 20);
            this.cmbSavedConnections.TabIndex = 1;
            // 
            // lblTemplate
            // 
            this.lblTemplate.AutoSize = true;
            this.lblTemplate.Location = new System.Drawing.Point(240, 26);
            this.lblTemplate.Name = "lblTemplate";
            this.lblTemplate.Size = new System.Drawing.Size(59, 12);
            this.lblTemplate.TabIndex = 2;
            this.lblTemplate.Text = "快捷模板:";
            // 
            // cmbTemplates
            // 
            this.cmbTemplates.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTemplates.FormattingEnabled = true;
            this.cmbTemplates.Location = new System.Drawing.Point(302, 22);
            this.cmbTemplates.Name = "cmbTemplates";
            this.cmbTemplates.Size = new System.Drawing.Size(243, 20);
            this.cmbTemplates.TabIndex = 3;
            // 
            // lblConnStr
            // 
            this.lblConnStr.AutoSize = true;
            this.lblConnStr.Location = new System.Drawing.Point(15, 56);
            this.lblConnStr.Name = "lblConnStr";
            this.lblConnStr.Size = new System.Drawing.Size(71, 12);
            this.lblConnStr.TabIndex = 4;
            this.lblConnStr.Text = "连接字符串:";
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(92, 53);
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.Size = new System.Drawing.Size(344, 21);
            this.txtConnectionString.TabIndex = 5;
            // 
            // lblConnName
            // 
            this.lblConnName.AutoSize = true;
            this.lblConnName.Location = new System.Drawing.Point(15, 86);
            this.lblConnName.Name = "lblConnName";
            this.lblConnName.Size = new System.Drawing.Size(71, 12);
            this.lblConnName.TabIndex = 7;
            this.lblConnName.Text = "管理别名框:";
            // 
            // txtConnName
            // 
            this.txtConnName.Location = new System.Drawing.Point(92, 83);
            this.txtConnName.Name = "txtConnName";
            this.txtConnName.Size = new System.Drawing.Size(344, 21);
            this.txtConnName.TabIndex = 8;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(445, 51);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(100, 25);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "连接并加载";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(445, 81);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 25);
            this.btnSave.TabIndex = 9;
            this.btnSave.Text = "保存当前连接";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // grpConnection
            // 
            this.grpConnection.Controls.Add(this.lblSavedConn);
            this.grpConnection.Controls.Add(this.cmbSavedConnections);
            this.grpConnection.Controls.Add(this.lblTemplate);
            this.grpConnection.Controls.Add(this.cmbTemplates);
            this.grpConnection.Controls.Add(this.lblConnStr);
            this.grpConnection.Controls.Add(this.txtConnectionString);
            this.grpConnection.Controls.Add(this.btnConnect);
            this.grpConnection.Controls.Add(this.lblConnName);
            this.grpConnection.Controls.Add(this.txtConnName);
            this.grpConnection.Controls.Add(this.btnSave);
            this.grpConnection.Location = new System.Drawing.Point(12, 12);
            this.grpConnection.Name = "grpConnection";
            this.grpConnection.Size = new System.Drawing.Size(560, 115);
            this.grpConnection.TabIndex = 0;
            this.grpConnection.TabStop = false;
            this.grpConnection.Text = "1. 数据库实例管理与连接";
            // 
            // txtLogOutput
            // 
            this.txtLogOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.txtLogOutput.BackColor = System.Drawing.Color.OldLace;
            this.txtLogOutput.Font = new System.Drawing.Font("SimSun", 9.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtLogOutput.ForeColor = System.Drawing.Color.DarkRed;
            this.txtLogOutput.Location = new System.Drawing.Point(12, 135);
            this.txtLogOutput.Multiline = true;
            this.txtLogOutput.Name = "txtLogOutput";
            this.txtLogOutput.ReadOnly = true;
            this.txtLogOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLogOutput.Size = new System.Drawing.Size(560, 465);
            this.txtLogOutput.TabIndex = 1;
            // 
            // grpOperation
            // 
            this.grpOperation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpOperation.Controls.Add(this.dgvLogList);
            this.grpOperation.Controls.Add(this.btnBatchClearLog);
            this.grpOperation.Location = new System.Drawing.Point(585, 12);
            this.grpOperation.Name = "grpOperation";
            this.grpOperation.Size = new System.Drawing.Size(537, 588);
            this.grpOperation.TabIndex = 2;
            this.grpOperation.TabStop = false;
            this.grpOperation.Text = "2. 待整理资产列表 (支持行级自适应策略调配)";
            // 
            // dgvLogList
            // 
            this.dgvLogList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLogList.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvLogList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLogList.Location = new System.Drawing.Point(10, 23);
            this.dgvLogList.Name = "dgvLogList";
            this.dgvLogList.RowTemplate.Height = 23;
            this.dgvLogList.Size = new System.Drawing.Size(517, 510);
            this.dgvLogList.TabIndex = 0;
            // 
            // btnBatchClearLog
            // 
            this.btnBatchClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBatchClearLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnBatchClearLog.Font = new System.Drawing.Font("SimSun", 9.0F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBatchClearLog.ForeColor = System.Drawing.Color.White;
            this.btnBatchClearLog.Location = new System.Drawing.Point(347, 542);
            this.btnBatchClearLog.Name = "btnBatchClearLog";
            this.btnBatchClearLog.Size = new System.Drawing.Size(180, 35);
            this.btnBatchClearLog.TabIndex = 1;
            this.btnBatchClearLog.Text = "开始批量清理选中的日志";
            this.btnBatchClearLog.UseVisualStyleBackColor = false;
            this.btnBatchClearLog.Click += new System.EventHandler(this.btnBatchClearLog_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnHelp.Location = new System.Drawing.Point(12, 608);
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(100, 25);
            this.btnHelp.TabIndex = 3;
            this.btnHelp.Text = "❓ 帮助与关于";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1134, 642);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.grpOperation);
            this.Controls.Add(this.txtLogOutput);
            this.Controls.Add(this.grpConnection);
            this.MinimumSize = new System.Drawing.Size(960, 500);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SQL Server 事务日志批量安全清理工具 (.NET 4.0 绿色版)";
            this.grpConnection.ResumeLayout(false);
            this.grpConnection.PerformLayout();
            this.grpOperation.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLogList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSavedConn;
        private System.Windows.Forms.ComboBox cmbSavedConnections;
        private System.Windows.Forms.Label lblTemplate;
        private System.Windows.Forms.ComboBox cmbTemplates;
        private System.Windows.Forms.Label lblConnStr;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.Label lblConnName;
        private System.Windows.Forms.TextBox txtConnName;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.GroupBox grpConnection;
        private System.Windows.Forms.TextBox txtLogOutput;
        private System.Windows.Forms.GroupBox grpOperation;
        private System.Windows.Forms.DataGridView dgvLogList;
        private System.Windows.Forms.Button btnBatchClearLog;
        private System.Windows.Forms.Button btnHelp;
    }
}