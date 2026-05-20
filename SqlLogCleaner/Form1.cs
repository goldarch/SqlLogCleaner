using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

namespace SqlLogCleaner
{
    public partial class Form1 : Form
    {
        // ⭐【核心技术升级】：直接通过反射获取项目属性(AssemblyInfo)中的标准四位版本号（例如：2.0.0.1）
        // 这样以后只要在 VS 项目属性里改版本，代码完全不用动
        private readonly string AppVersion = "ver " + Application.ProductVersion;

        private readonly string configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conn_config.txt");
        private Dictionary<string, string> connectionDict = new Dictionary<string, string>();
        private int currentSqlMajorVersion = 11;
        private bool isFullyConnected = false;

        public Form1()
        {
            InitializeComponent();
            InitWarningMessage();
            InitTemplates();
            LoadSavedConnections();
            SetupGridColumns();

            // ⭐【核心修复一】：显式订阅 Shown 事件，确保在窗体完全暴露在屏幕上的最后一刻，强行把状态和标题拉满
            this.Shown += new EventHandler(Form1_Shown);
        }

        // ⭐【核心修复二】：在 Shown 事件中执行最终的界面统治权，彻底压制并修正生命周期导致的失效 Bug
        private void Form1_Shown(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.Text = string.Format("SQL Server 事务日志批量安全清理工具 {0} (.NET 4.0 绿色版)", AppVersion);
            this.Refresh();
        }

        private void SetupGridColumns()
        {
            dgvLogList.Columns.Clear();
            dgvLogList.AutoGenerateColumns = false;
            dgvLogList.AllowUserToAddRows = false;
            dgvLogList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn
            {
                Name = "colSelect",
                HeaderText = "选择",
                Width = 45,
                TrueValue = true,
                FalseValue = false
            };
            dgvLogList.Columns.Add(chkCol);

            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDbName", HeaderText = "数据库名称", DataPropertyName = "DbName", Width = 110, ReadOnly = true });
            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLogName", HeaderText = "日志逻辑名", DataPropertyName = "LogFileName", Width = 110, ReadOnly = true });
            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTotalSize", HeaderText = "总大小(MB)", DataPropertyName = "LogSizeMB", Width = 90, ReadOnly = true });
            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUsedSize", HeaderText = "已用(MB)", DataPropertyName = "UsedSpaceMB", Width = 80, ReadOnly = true });
            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPct", HeaderText = "已用率(%)", DataPropertyName = "LogSpaceUsedPct", Width = 80, ReadOnly = true });

            DataGridViewComboBoxColumn cmbCol = new DataGridViewComboBoxColumn
            {
                Name = "colStrategy",
                HeaderText = "收缩策略(可手工调整)",
                DataPropertyName = "Strategy",
                Width = 140,
                FlatStyle = FlatStyle.Flat
            };
            cmbCol.Items.Add("直接指定大小(10M)");
            cmbCol.Items.Add("循环递减法(20%)");
            dgvLogList.Columns.Add(cmbCol);

            dgvLogList.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "处理状态", DataPropertyName = "Status", Width = 85, ReadOnly = true });
        }

        private void InitWarningMessage()
        {
            txtLogOutput.Text = "⚠️ 【标准运维规范与批量操作风险提示】" + Environment.NewLine +
                             "------------------------------------------------------------------------------------------------------------------" + Environment.NewLine +
                             "1. 跨版本智能自适应：SQL 2000~2008 自动路由至经典截断；SQL 2012~2022+ 采用简单模式收缩逻辑。" + Environment.NewLine +
                             "2. 核心规程（三步保命法）：" + Environment.NewLine +
                             "   第一步：批量清理前，请务必在原生 SSMS 中对目标生产库执行【完整备份】以确保安全。" + Environment.NewLine +
                             "   （提示：此时即使日志极大，利用微软原生 SSMS 也能强行备份成功，只要目标磁盘空间足够）。" + Environment.NewLine + Environment.NewLine +
                             "   第二步：前置备份成功后，勾选右侧需要处理的账套，点击【开始批量清理选中的日志】进行安全收缩。" + Environment.NewLine + Environment.NewLine +
                             "   第三步：清理完成后，立即再次执行【完整备份】，重新建立干净健康的事务日志链。" + Environment.NewLine +
                             "------------------------------------------------------------------------------------------------------------------" + Environment.NewLine +
                             "💡 【行级自适应策略审计规范：】" + Environment.NewLine +
                             "   本工具已将控制粒度下沉至『账套行级』。当连接成功后，系统会自动审计右侧资产清单：" + Environment.NewLine +
                             "   ▶ 对体积平稳（<=50GB）的普通账套，行内默认分派【直接指定大小(10M)】策略以提升清洗效率；" + Environment.NewLine +
                             "   ▶ 对体积肥大（>50GB）的超大账套，行内将自动越级并分派【循环递减法(20%)】安全下楼梯策略。" + Environment.NewLine +
                             "   ※ 管理自主权：运维人员可直接在列表中单独点击修改任意一行的收缩策略，提交后系统将精准精细化执行。" + Environment.NewLine +
                             "------------------------------------------------------------------------------------------------------------------" + Environment.NewLine +
                             "3. 就绪状态：安全清理核心工坊 " + AppVersion + " 已就绪，等待连接数据库服务器..." + Environment.NewLine + Environment.NewLine;

            txtLogOutput.SelectionStart = txtLogOutput.Text.Length;
            txtLogOutput.ScrollToCaret();
        }

        private void InitTemplates()
        {
            cmbTemplates.Items.Add(new ToolTemplate { Name = "选择模板加载...", Value = "" });
            cmbTemplates.Items.Add(new ToolTemplate { Name = "模板：本地Windows身份验证(免密)", Value = "Server=.;Database=master;Integrated Security=True;" });
            cmbTemplates.Items.Add(new ToolTemplate { Name = "模板：本地SQL身份验证(sa)", Value = "Server=.;Database=master;User Id=sa;Password=你的密码;" });
            cmbTemplates.Items.Add(new ToolTemplate { Name = "模板：远程SQL身份验证", Value = "Server=192.168.1.X,1433;Database=master;User Id=sa;Password=你的密码;" });
            cmbTemplates.SelectedIndex = 0;
        }

        private void LoadSavedConnections()
        {
            cmbSavedConnections.SelectedIndexChanged -= cmbSavedConnections_SelectedIndexChanged;
            cmbSavedConnections.Items.Clear();
            connectionDict.Clear();

            string defaultName = "本地默认连接 (Windows验证)";
            string defaultConn = "Server=.;Database=master;Integrated Security=True;";
            connectionDict.Add(defaultName, defaultConn);
            cmbSavedConnections.Items.Add(defaultName);

            try
            {
                if (File.Exists(configPath))
                {
                    string[] lines = File.ReadAllLines(configPath, Encoding.UTF8);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrEmpty(line) || !line.Contains("|")) continue;
                        string[] parts = line.Split('|');
                        if (parts.Length >= 2)
                        {
                            string name = parts[0].Trim();
                            string conn = parts[1].Trim();
                            if (!connectionDict.ContainsKey(name))
                            {
                                connectionDict.Add(name, conn);
                                cmbSavedConnections.Items.Add(name);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLogLine("❌ 加载历史连接失败: " + ex.Message);
            }

            cmbSavedConnections.SelectedIndex = 0;
            cmbSavedConnections.SelectedIndexChanged += cmbSavedConnections_SelectedIndexChanged;

            ResetOperationGroup();
        }

        private void ResetOperationGroup()
        {
            isFullyConnected = false;
            if (dgvLogList != null)
            {
                dgvLogList.DataSource = null;
                if (dgvLogList.Rows.Count > 0) dgvLogList.Rows.Clear();
            }
            this.Text = string.Format("SQL Server 事务日志批量安全清理工具 {0} (.NET 4.0 绿色版)", AppVersion);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string name = txtConnName.Text.Trim();
            string connStr = txtConnectionString.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("请输入连接管理别名！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(connStr))
            {
                MessageBox.Show("连接字符串不能为空！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (connectionDict.ContainsKey(name)) connectionDict[name] = connStr;
                else connectionDict.Add(name, connStr);

                List<string> linesToSave = new List<string>();
                foreach (var kvp in connectionDict)
                {
                    if (kvp.Key == "本地默认连接 (Windows验证)") continue;
                    linesToSave.Add(string.Format("{0}|{1}", kvp.Key, kvp.Value));
                }

                File.WriteAllLines(configPath, linesToSave.ToArray(), Encoding.UTF8);
                MessageBox.Show("连接保存成功！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadSavedConnections();
                if (cmbSavedConnections.Items.Contains(name)) cmbSavedConnections.SelectedItem = name;
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbSavedConnections_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSavedConnections.SelectedItem == null) return;
            string selectedName = cmbSavedConnections.SelectedItem.ToString();
            if (connectionDict.ContainsKey(selectedName))
            {
                txtConnectionString.Text = connectionDict[selectedName];
                txtConnName.Text = selectedName.Contains("本地默认连接") ? "" : selectedName;
                ResetOperationGroup();
                AppendLogLine(string.Format("🔄 切换到历史连接 [{0}]，右侧已整理列表清场，请重新点击连接加载。", selectedName));
            }
        }

        private void cmbTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToolTemplate template = cmbTemplates.SelectedItem as ToolTemplate;
            if (template != null && !string.IsNullOrEmpty(template.Value))
            {
                txtConnectionString.Text = template.Value;
                ResetOperationGroup();
                AppendLogLine(string.Format("🔄 切换到快捷模板 [{0}]，右侧已整理列表清场，请重新点击连接加载。", template.Name));
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            string connStr = txtConnectionString.Text.Trim();
            string versionSql = "SELECT CAST(SERVERPROPERTY('ProductVersion') AS VARCHAR(50));";

            string getLogDetailsSql = @"
                DECLARE @LogSpace TABLE ( DbName sysname, LogSizeMB VARCHAR(50), LogSpaceUsedPct VARCHAR(50), Status int );
                INSERT INTO @LogSpace EXEC('DBCC SQLPERF(LOGSPACE)');
                IF OBJECT_ID('sys.databases') IS NOT NULL
                BEGIN
                    SELECT ls.DbName, f.name AS LogFileName, 
                           CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 THEN CAST(ls.LogSizeMB AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSizeMB,
                           CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 AND ISNUMERIC(ls.LogSpaceUsedPct) = 1 
                                THEN (CAST(ls.LogSizeMB AS MONEY) * CAST(ls.LogSpaceUsedPct AS MONEY) / 100.0) ELSE 0 END AS DECIMAL(18,2)) AS UsedSpaceMB,
                           CAST(CASE WHEN ISNUMERIC(ls.LogSpaceUsedPct) = 1 THEN CAST(ls.LogSpaceUsedPct AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSpaceUsedPct
                    FROM @LogSpace ls INNER JOIN sys.databases d ON ls.DbName = d.name
                    INNER JOIN sys.master_files f ON d.database_id = f.database_id WHERE f.type = 1 AND d.database_id > 4 AND d.state = 0
                    ORDER BY LogSizeMB DESC;
                END
                ELSE
                BEGIN
                    SELECT ls.DbName, f.name AS LogFileName, 
                           CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 THEN CAST(ls.LogSizeMB AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSizeMB,
                           CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 AND ISNUMERIC(ls.LogSpaceUsedPct) = 1 
                                THEN (CAST(ls.LogSizeMB AS MONEY) * CAST(ls.LogSpaceUsedPct AS MONEY) / 100.0) ELSE 0 END AS DECIMAL(18,2)) AS UsedSpaceMB,
                           CAST(CASE WHEN ISNUMERIC(ls.LogSpaceUsedPct) = 1 THEN CAST(ls.LogSpaceUsedPct AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSpaceUsedPct
                    FROM @LogSpace ls INNER JOIN master..sysaltfiles f ON db_name(f.dbid) = ls.DbName
                    WHERE (f.status & 0x40) <> 0 AND f.dbid > 4 ORDER BY LogSizeMB DESC;
                END";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    conn.Open();

                    using (SqlCommand cmdVer = new SqlCommand(versionSql, conn))
                    {
                        object verObj = cmdVer.ExecuteScalar();
                        if (verObj != null && verObj != DBNull.Value)
                        {
                            string verStr = verObj.ToString();
                            if (verStr.Contains("."))
                            {
                                string majorPart = verStr.Split('.')[0];
                                int.TryParse(majorPart, out currentSqlMajorVersion);
                            }
                        }
                    }

                    SqlDataAdapter da = new SqlDataAdapter(getLogDetailsSql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        ResetOperationGroup();
                        AppendLogLine("⚠️ 未找到可用的用户数据库日志资产！");
                        return;
                    }

                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("Strategy", typeof(string));

                    int autoGradientCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        row["Status"] = "未开始";
                        double sizeMB = Convert.ToDouble(row["LogSizeMB"]);
                        if (sizeMB > 51200)
                        {
                            row["Strategy"] = "循环递减法(20%)";
                            autoGradientCount++;
                        }
                        else
                        {
                            row["Strategy"] = "直接指定大小(10M)";
                        }
                    }

                    dgvLogList.DataSource = dt;
                    foreach (DataGridViewRow row in dgvLogList.Rows) row.Cells["colSelect"].Value = false;

                    string friendlyVerName = GetVersionFriendlyName(currentSqlMajorVersion);
                    this.Text = string.Format("SQL Server 事务日志批量安全清理工具 {0} [已连接: {1}]", AppVersion, friendlyVerName);
                    isFullyConnected = true;

                    AppendLogLine(string.Format("✅ 成功建立连接！当前服务器环境：{0}，已全盘加载 {1} 个账套资产。", friendlyVerName, dt.Rows.Count));
                    if (autoGradientCount > 0)
                    {
                        AppendLogLine(string.Format("   💡 [策略组提示]：系统已自动识别并为右侧 [{0}] 个大于 50GB 的超大数据库自适应开启了【循环递减法(20%)】保命策略，请核对。", autoGradientCount));
                    }
                }
            }
            catch (Exception ex)
            {
                ResetOperationGroup();
                AppendLogLine("❌ 连接失败: " + ex.Message);
            }
        }

        private void btnBatchClearLog_Click(object sender, EventArgs e)
        {
            if (!isFullyConnected || dgvLogList.Rows.Count == 0)
            {
                MessageBox.Show("当前未建立有效连接，无法执行清理！", "管理防呆提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<BatchTaskItem> selectedTasks = new List<BatchTaskItem>();
            foreach (DataGridViewRow row in dgvLogList.Rows)
            {
                object chkVal = row.Cells["colSelect"].Value;
                if (chkVal != null && (bool)chkVal == true)
                {
                    string rowChosenStrategy = "直接指定大小(10M)";
                    if (row.Cells["colStrategy"].Value != null)
                    {
                        rowChosenStrategy = row.Cells["colStrategy"].Value.ToString();
                    }

                    selectedTasks.Add(new BatchTaskItem
                    {
                        GridRowIndex = row.Index,
                        DbName = row.Cells["colDbName"].Value.ToString(),
                        LogFileName = row.Cells["colLogName"].Value.ToString(),
                        TotalSizeMB = row.Cells["colTotalSize"].Value.ToString(),
                        ChosenStrategy = rowChosenStrategy
                    });
                }
            }

            if (selectedTasks.Count == 0)
            {
                MessageBox.Show("请先在右侧列表中勾选需要批量清理日志的数据库！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult dr = MessageBox.Show(
                string.Format("您当前共勾选了 [{0}] 个数据库账套进行批量日志收缩！" + Environment.NewLine +
                              "系统将严格根据列表【每行独立指定的策略】执行精细化清洗。" + Environment.NewLine + Environment.NewLine +
                              "⚠️ 重要安全确认：请务必确保这些生产库在操作前已执行【完整备份】，是否继续？",
                              selectedTasks.Count),
                "批量操作刚性安全确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
            );

            if (dr != DialogResult.Yes) return;

            AppendLogLine(string.Format("\r\n🚀 开始执行流水线清洗任务... 预备处理账套数: {0} ------------------", selectedTasks.Count));
            int successCount = 0; int failCount = 0;
            string connStr = txtConnectionString.Text.Trim();

            foreach (var task in selectedTasks)
            {
                dgvLogList.Rows[task.GridRowIndex].Cells["colStatus"].Value = "处理中...";
                dgvLogList.Update();

                double initialSize = Convert.ToDouble(task.TotalSizeMB);
                AppendLogLine(string.Format("⏳ [{0}] 路由触发 -> 选定策略:【{1}】(日志当前: {2} MB)", task.DbName, task.ChosenStrategy, task.TotalSizeMB));

                try
                {
                    using (SqlConnection conn = new SqlConnection(connStr))
                    {
                        conn.Open();

                        if (currentSqlMajorVersion > 10)
                        {
                            string setSimpleSql = string.Format("USE [master]; ALTER DATABASE [{0}] SET RECOVERY SIMPLE WITH NO_WAIT;", task.DbName);
                            using (SqlCommand cmd = new SqlCommand(setSimpleSql, conn)) cmd.ExecuteNonQuery();
                        }

                        if (task.ChosenStrategy == "直接指定大小(10M)")
                        {
                            string directSql = string.Empty;
                            if (currentSqlMajorVersion <= 10)
                                directSql = string.Format("USE [master]; BACKUP LOG [{0}] WITH NO_LOG; EXEC('USE [{0}]; DBCC SHRINKFILE (N''{1}'', 10, TRUNCATEONLY);');", task.DbName, task.LogFileName.Replace("'", "''"));
                            else
                                directSql = string.Format("EXEC('USE [{0}]; DBCC SHRINKFILE (N''{1}'', 10, TRUNCATEONLY);');", task.DbName, task.LogFileName.Replace("'", "''"));

                            using (SqlCommand cmd = new SqlCommand(directSql, conn)) cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            double targetSize = initialSize;
                            double previousSize = initialSize;

                            while (targetSize > 100)
                            {
                                targetSize = Math.Round(targetSize * 0.8);
                                if (targetSize < 10) targetSize = 10;

                                string shrinkSql = string.Empty;
                                if (currentSqlMajorVersion <= 10)
                                    shrinkSql = string.Format("USE [master]; BACKUP LOG [{0}] WITH NO_LOG; EXEC('USE [{0}]; DBCC SHRINKFILE (N''{1}'', {2});');", task.DbName, task.LogFileName.Replace("'", "''"), targetSize);
                                else
                                    shrinkSql = string.Format("EXEC('USE [{0}]; DBCC SHRINKFILE (N''{1}'', {2});');", task.DbName, task.LogFileName.Replace("'", "''"), targetSize);

                                using (SqlCommand cmd = new SqlCommand(shrinkSql, conn)) cmd.ExecuteNonQuery();

                                double realCurrentSize = GetSingleLogSizeMBQuietly(conn, task.DbName);
                                if (realCurrentSize >= previousSize) break;
                                previousSize = realCurrentSize;

                                AppendLogLine(string.Format("    ⚡ 梯度下移中 -> 当前物理体积已降至: {0} MB", Math.Round(realCurrentSize)));
                                txtLogOutput.Refresh();
                            }

                            string finalTruncate = string.Format("EXEC('USE [{0}]; DBCC SHRINKFILE (N''{1}'', 10, TRUNCATEONLY);');", task.DbName, task.LogFileName.Replace("'", "''"));
                            using (SqlCommand cmd = new SqlCommand(finalTruncate, conn)) cmd.ExecuteNonQuery();
                        }

                        if (currentSqlMajorVersion > 10)
                        {
                            string setFullSql = string.Format("USE [master]; ALTER DATABASE [{0}] SET RECOVERY FULL WITH NO_WAIT;", task.DbName);
                            using (SqlCommand cmd = new SqlCommand(setFullSql, conn)) cmd.ExecuteNonQuery();
                        }

                        successCount++;
                        dgvLogList.Rows[task.GridRowIndex].Cells["colStatus"].Value = "🟢 完成";
                        AppendLogLine(string.Format("    ┗ 🟢 成功：数据库 [{0}] 账套收缩任务顺利完成。", task.DbName));
                    }
                }
                catch (Exception ex)
                {
                    failCount++;
                    dgvLogList.Rows[task.GridRowIndex].Cells["colStatus"].Value = "🔴 失败";
                    AppendLogLine(string.Format("    ┗ 🔴 失败：数据库 [{0}] 清洗阻断！异常原因: {1}", task.DbName, ex.Message));
                }

                txtLogOutput.Refresh(); dgvLogList.Refresh();
            }

            AppendLogLine(string.Format("🏁 集中清洗任务执行完毕！成功: {0} 个，失败: {1} 个。请立刻执行规范中的后置完整备份！\r\n", successCount, failCount));
            RefreshLogListQuietly(selectedTasks);
        }

        private double GetSingleLogSizeMBQuietly(SqlConnection openConn, string dbName)
        {
            string sql = string.Format(@"
                DECLARE @LogSpace TABLE ( DbName sysname, LogSizeMB money, LogSpaceUsedPct money, Status int );
                INSERT INTO @LogSpace EXEC('DBCC SQLPERF(LOGSPACE)');
                SELECT LogSizeMB FROM @LogSpace WHERE DbName = '{0}';", dbName.Replace("'", "''"));
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, openConn))
                {
                    object res = cmd.ExecuteScalar();
                    return res != null ? Convert.ToDouble(res) : 0;
                }
            }
            catch { return 0; }
        }

        private void RefreshLogListQuietly(List<BatchTaskItem> oldTasks)
        {
            string connStr = txtConnectionString.Text.Trim();

            // ⭐【铁腕彻底修复】：回盘脚本同步升级为 VARCHAR 容错缓冲技术，彻底绝杀奇葩库名引发的 money 转换报错
            string getLogDetailsSql = @"
        DECLARE @LogSpace TABLE ( 
            DbName sysname, 
            LogSizeMB VARCHAR(50), 
            LogSpaceUsedPct VARCHAR(50), 
            Status int 
        );
        INSERT INTO @LogSpace EXEC('DBCC SQLPERF(LOGSPACE)');
        
        IF OBJECT_ID('sys.databases') IS NOT NULL
        BEGIN
            SELECT ls.DbName, f.name AS LogFileName, 
                   CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 THEN CAST(ls.LogSizeMB AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSizeMB,
                   CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 AND ISNUMERIC(ls.LogSpaceUsedPct) = 1 
                        THEN (CAST(ls.LogSizeMB AS MONEY) * CAST(ls.LogSpaceUsedPct AS MONEY) / 100.0) ELSE 0 END AS DECIMAL(18,2)) AS UsedSpaceMB,
                   CAST(CASE WHEN ISNUMERIC(ls.LogSpaceUsedPct) = 1 THEN CAST(ls.LogSpaceUsedPct AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSpaceUsedPct
            FROM @LogSpace ls INNER JOIN sys.databases d ON ls.DbName = d.name
            INNER JOIN sys.master_files f ON d.database_id = f.database_id WHERE f.type = 1 AND d.database_id > 4 AND d.state = 0
            ORDER BY LogSizeMB DESC;
        END
        ELSE
        BEGIN
            SELECT ls.DbName, f.name AS LogFileName, 
                   CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 THEN CAST(ls.LogSizeMB AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSizeMB,
                   CAST(CASE WHEN ISNUMERIC(ls.LogSizeMB) = 1 AND ISNUMERIC(ls.LogSpaceUsedPct) = 1 
                        THEN (CAST(ls.LogSizeMB AS MONEY) * CAST(ls.LogSpaceUsedPct AS MONEY) / 100.0) ELSE 0 END AS DECIMAL(18,2)) AS UsedSpaceMB,
                   CAST(CASE WHEN ISNUMERIC(ls.LogSpaceUsedPct) = 1 THEN CAST(ls.LogSpaceUsedPct AS MONEY) ELSE 0 END AS DECIMAL(18,2)) AS LogSpaceUsedPct
            FROM @LogSpace ls INNER JOIN master..sysaltfiles f ON db_name(f.dbid) = ls.DbName
            WHERE (f.status & 0x40) <> 0 AND f.dbid > 4 ORDER BY LogSizeMB DESC;
        END";

            try
            {
                using (SqlConnection conn = new SqlConnection(connStr))
                {
                    SqlDataAdapter da = new SqlDataAdapter(getLogDetailsSql, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dt.Columns.Add("Status", typeof(string));
                    dt.Columns.Add("Strategy", typeof(string));

                    foreach (DataRow row in dt.Rows)
                    {
                        string dName = row["DbName"].ToString();
                        var matched = oldTasks.Find(t => t.DbName == dName);
                        if (matched != null)
                        {
                            object currentGridStatus = dgvLogList.Rows[matched.GridRowIndex].Cells["colStatus"].Value;
                            row["Status"] = currentGridStatus ?? "未开始";
                            row["Strategy"] = matched.ChosenStrategy;
                        }
                        else
                        {
                            double sizeMB = Convert.ToDouble(row["LogSizeMB"]);
                            row["Status"] = "未开始";
                            row["Strategy"] = sizeMB > 51200 ? "循环递减法(20%)" : "直接指定大小(10M)";
                        }
                    }

                    // 1. 重新绑定更新后的 DataTable 数据源
                    dgvLogList.DataSource = dt;

                    // 2. 批量复位所有行的复选框
                    foreach (DataGridViewRow row in dgvLogList.Rows)
                    {
                        row.Cells["colSelect"].Value = false;
                    }

                    // 3. 击穿重绘缓存
                    dgvLogList.Invalidate();
                    dgvLogList.Refresh();

                    AppendLogLine("🔄 已自动完成资产回盘，最新物理体积已与服务器完全同步。");
                }
            }
            catch (Exception ex)
            {
                AppendLogLine("❌ 自动回盘刷新失败: " + ex.Message);
            }
        }

        private void AppendLogLine(string text)
        {
            if (txtLogOutput != null)
            {
                txtLogOutput.AppendText("[" + DateTime.Now.ToString("HH:mm:ss") + "] " + text + Environment.NewLine);
            }
        }

        private string GetVersionFriendlyName(int majorVersion)
        {
            switch (majorVersion)
            {
                case 8: return "SQL Server 2000";
                case 9: return "SQL Server 2005";
                case 10: return "SQL Server 2008/R2";
                case 11: return "SQL Server 2012";
                case 12: return "SQL Server 2014";
                case 13: return "SQL Server 2016";
                case 14: return "SQL Server 2017";
                case 15: return "SQL Server 2019";
                case 16: return "SQL Server 2022";
                default: return "SQL Server (Version " + majorVersion + ".X)";
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Form aboutForm = new Form
            {
                Text = "关于作者与重要说明",
                Size = new System.Drawing.Size(430, 290),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblTitle = new Label
            {
                Text = "SQL Server 事务日志安全清理工具 " + AppVersion,
                Font = new System.Drawing.Font("SimSun", 11F, System.Drawing.FontStyle.Bold),
                Location = new System.Drawing.Point(20, 15),
                AutoSize = true
            };

            Label lblAuthor = new Label { Text = "开 发： GoldArch", Location = new System.Drawing.Point(20, 50), AutoSize = true };
            Label lblEmail = new Label { Text = "邮 箱： 99490008@qq.com", Location = new System.Drawing.Point(20, 75), AutoSize = true };

            Label lblBlogTag = new Label { Text = "博 客： ", Location = new System.Drawing.Point(20, 100), AutoSize = true };
            LinkLabel lnkBlog = new LinkLabel { Text = "https://www.cnblogs.com/goldarch", Location = new System.Drawing.Point(75, 100), AutoSize = true };
            lnkBlog.LinkClicked += (s, ev) => { try { System.Diagnostics.Process.Start(lnkBlog.Text); } catch { } };

            Label lblGitTag = new Label { Text = "GitHub：", Location = new System.Drawing.Point(20, 125), AutoSize = true };
            LinkLabel lnkGit = new LinkLabel { Text = "https://github.com/goldarch", Location = new System.Drawing.Point(75, 125), AutoSize = true };
            lnkGit.LinkClicked += (s, ev) => { try { System.Diagnostics.Process.Start(lnkGit.Text); } catch { } };

            Label lblDesc = new Label
            {
                Text = "说明：本工具专门解决企业级 ERP 账套及各类 SQL 实例日志暴涨引发的磁盘空间危机。具备全版本 (SQL 2000 ~ 2022+) 智能自适应路由清除功能，纯绿色单文件，不写注册表，无外部组件依赖。",
                Location = new System.Drawing.Point(20, 160),
                Size = new System.Drawing.Size(370, 50),
                ForeColor = System.Drawing.Color.Gray
            };

            Button btnClose = new Button
            {
                Text = "确定",
                Size = new System.Drawing.Size(80, 28),
                Location = new System.Drawing.Point(310, 215),
                DialogResult = DialogResult.OK
            };

            aboutForm.Controls.Add(lblTitle);
            aboutForm.Controls.Add(lblAuthor);
            aboutForm.Controls.Add(lblEmail);
            aboutForm.Controls.Add(lblBlogTag);
            aboutForm.Controls.Add(lnkBlog);
            aboutForm.Controls.Add(lblGitTag);
            aboutForm.Controls.Add(lnkGit);
            aboutForm.Controls.Add(lblDesc);
            aboutForm.Controls.Add(btnClose);

            aboutForm.AcceptButton = btnClose;
            aboutForm.ShowDialog(this);
        }
    }
}