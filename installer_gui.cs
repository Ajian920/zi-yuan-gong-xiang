using System;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Threading;

class Installer : Form {
    Panel header, footer, content;
    Label title, subtitle, status;
    Button btnNext, btnBack, btnInstall, btnBrowse, btnLaunch, btnDone;
    TextBox txtPath;
    ProgressBar progress;
    string installDir = @"D:\ResShare";
    Color accent = Color.FromArgb(0, 120, 215);

    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.Run(new Installer());
    }

    public Installer() {
        this.Text = "资源共享 安装向导";
        this.ClientSize = new Size(520, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Microsoft YaHei UI", 9F);

        header = new Panel { Location = new Point(0, 0), Size = new Size(520, 70), BackColor = accent };
        header.Dock = DockStyle.Top;
        title = new Label { Text = "资源共享", ForeColor = Color.White, Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold), Location = new Point(20, 12), AutoSize = true };
        subtitle = new Label { Text = "资源合集分享工具  v1.0.0", ForeColor = Color.FromArgb(200, 220, 255), Location = new Point(22, 45), AutoSize = true };
        header.Controls.Add(title);
        header.Controls.Add(subtitle);

        footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.White };
        footer.Paint += (s, e) => e.Graphics.DrawLine(new Pen(Color.FromArgb(220, 220, 220)), 0, 0, footer.Width, 0);

        content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

        this.Controls.Add(content);
        this.Controls.Add(footer);
        this.Controls.Add(header);

        ShowWelcome();
    }

    void Clear() {
        content.Controls.Clear();
        footer.Controls.Clear();
    }

    void ShowWelcome() {
        Clear();
        var lbl = new Label {
            Text = "欢迎安装「资源共享」\n\n本程序将把资源共享安装到您的电脑上。\n安装目录默认为 D:\\ResShare\n\n请勾选下方协议后点击「下一步」。",
            Location = new Point(20, 20), Size = new Size(470, 120), Font = new Font("Microsoft YaHei UI", 10F)
        };
        var chk = new CheckBox { Text = "我已阅读并同意《用户协议》", Location = new Point(20, 160), AutoSize = true };
        var btnClose = new Button { Text = "取消", Size = new Size(75, 30), Location = new Point(330, 8), FlatStyle = FlatStyle.Flat };
        btnClose.Click += (s, e) => this.Close();
        btnNext = new Button { Text = "下一步", Size = new Size(85, 30), Location = new Point(420, 8), FlatStyle = FlatStyle.Flat, BackColor = accent, ForeColor = Color.White };
        btnNext.Click += (s, e) => {
            if (!chk.Checked) { MessageBox.Show("请先同意用户协议", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            ShowDirSelect();
        };
        content.Controls.Add(lbl);
        content.Controls.Add(chk);
        footer.Controls.Add(btnClose);
        footer.Controls.Add(btnNext);
    }

    void ShowDirSelect() {
        Clear();
        var lbl = new Label { Text = "选择安装位置", Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        var lbl2 = new Label { Text = "目标文件夹：", Location = new Point(20, 65), AutoSize = true };
        txtPath = new TextBox { Text = installDir, Location = new Point(20, 90), Size = new Size(380, 26) };
        btnBrowse = new Button { Text = "浏览...", Size = new Size(80, 26), Location = new Point(410, 89) };
        btnBrowse.Click += (s, e) => { var f = new FolderBrowserDialog(); if (f.ShowDialog() == DialogResult.OK) txtPath.Text = f.SelectedPath; };
        var info = new Label { Text = "所需空间：约 340 MB", Location = new Point(20, 130), ForeColor = Color.Gray, AutoSize = true };
        btnBack = new Button { Text = "上一步", Size = new Size(75, 30), Location = new Point(240, 8) };
        btnBack.Click += (s, e) => ShowWelcome();
        var btnClose = new Button { Text = "取消", Size = new Size(75, 30), Location = new Point(330, 8) };
        btnClose.Click += (s, e) => this.Close();
        btnNext = new Button { Text = "安装", Size = new Size(85, 30), Location = new Point(420, 8), FlatStyle = FlatStyle.Flat, BackColor = accent, ForeColor = Color.White };
        btnNext.Click += (s, e) => { installDir = txtPath.Text; ShowInstalling(); };
        content.Controls.AddRange(new Control[] { lbl, lbl2, txtPath, btnBrowse, info });
        footer.Controls.AddRange(new Control[] { btnBack, btnClose, btnNext });
    }

    void ShowInstalling() {
        Clear();
        var lbl = new Label { Text = "正在安装...", Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold), Location = new Point(20, 30), AutoSize = true };
        status = new Label { Text = "准备中...", Location = new Point(20, 80), AutoSize = true, ForeColor = Color.Gray };
        progress = new ProgressBar { Location = new Point(20, 110), Size = new Size(470, 28), Style = ProgressBarStyle.Continuous };
        content.Controls.AddRange(new Control[] { lbl, status, progress });
        new Thread(DoInstall) { IsBackground = true }.Start();
    }

    void DoInstall() {
        try {
            if (!Directory.Exists(installDir)) Directory.CreateDirectory(installDir);

            string zipPath = null;
            using (Stream res = Assembly.GetExecutingAssembly().GetManifestResourceStream("resources.zip")) {
                if (res != null) {
                    zipPath = Path.Combine(Path.GetTempPath(), "res.zip");
                    using (FileStream fs = File.Create(zipPath)) { byte[] buf = new byte[65536]; int n; while ((n = res.Read(buf, 0, buf.Length)) > 0) fs.Write(buf, 0, n); }
                }
            }

            if (zipPath != null && File.Exists(zipPath)) {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath)) {
                    int total = archive.Entries.Count, i = 0;
                    foreach (var entry in archive.Entries) {
                        i++;
                        int pct = (int)((float)i / total * 100);
                        this.Invoke((Action)delegate { progress.Value = pct; status.Text = "正在安装 (" + i + "/" + total + ")..."; });
                        string dest = Path.Combine(installDir, entry.FullName);
                        string dir = Path.GetDirectoryName(dest);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        if (entry.Length > 0) entry.ExtractToFile(dest, true);
                    }
                }
                File.Delete(zipPath);
            }

            this.Invoke((Action)delegate { status.Text = "创建快捷方式..."; });
            string exePath = Path.Combine(installDir, "ResShare.exe");

            // Desktop shortcut
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CreateShortcut(Path.Combine(desktop, "资源共享.lnk"), exePath, installDir);

            // Start menu shortcut
            string sm = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "资源共享");
            if (!Directory.Exists(sm)) Directory.CreateDirectory(sm);
            CreateShortcut(Path.Combine(sm, "资源共享.lnk"), exePath, installDir);

            this.Invoke((Action)delegate { ShowDone(); });
        } catch (Exception ex) {
            this.Invoke((Action)delegate { MessageBox.Show("安装失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); });
        }
    }

    void CreateShortcut(string lnkPath, string targetExe, string workDir) {
        try {
            Type shellType = Type.GetTypeFromProgID("WScript.Shell");
            object shell = Activator.CreateInstance(shellType);
            object shortcut = shellType.InvokeMember("CreateShortcut", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { lnkPath });
            Type shortcutType = shortcut.GetType();
            shortcutType.InvokeMember("TargetPath", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { targetExe });
            shortcutType.InvokeMember("WorkingDirectory", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { workDir });
            shortcutType.InvokeMember("IconLocation", System.Reflection.BindingFlags.SetProperty, null, shortcut, new object[] { targetExe + ",0" });
            shortcutType.InvokeMember("Save", System.Reflection.BindingFlags.InvokeMethod, null, shortcut, new object[] { });
            // COM objects released by GC
        } catch (Exception ex) {
            System.Diagnostics.Debug.WriteLine("CreateShortcut error: " + ex.Message);
        }
    }

    void ShowDone() {
        Clear();
        var icon = new Label { Text = "✔", Font = new Font("Segoe UI", 36F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 160, 0), Location = new Point(20, 15), AutoSize = true };
        var lbl = new Label { Text = "安装完成！", Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold), Location = new Point(80, 25), AutoSize = true };
        var info = new Label { Text = "资源共享 已安装到：" + installDir + "\n\n桌面和开始菜单已创建快捷方式。", Location = new Point(20, 90), Size = new Size(470, 80), Font = new Font("Microsoft YaHei UI", 10F) };
        content.Controls.AddRange(new Control[] { icon, lbl, info });
        btnLaunch = new Button { Text = "完成", Size = new Size(85, 30), Location = new Point(420, 8), FlatStyle = FlatStyle.Flat, BackColor = accent, ForeColor = Color.White };
        btnLaunch.Click += (s, e) => this.Close();
        var btnRun = new Button { Text = "立即启动", Size = new Size(85, 30), Location = new Point(320, 8) };
        btnRun.Click += (s, e) => { Process.Start(Path.Combine(installDir, "ResShare.exe")); this.Close(); };
        footer.Controls.Add(btnRun);
        footer.Controls.Add(btnLaunch);
    }
}