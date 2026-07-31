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
    Label status;
    ProgressBar progress;
    string installDir = @"D:\ResShare";
    Color accent = Color.FromArgb(0, 120, 215);

    [STAThread]
    static void Main() { Application.EnableVisualStyles(); Application.Run(new Installer()); }

    public Installer() {
        this.Text = "\u8d44\u6e90\u5171\u4eab \u5b89\u88c5\u5411\u5bfc";
        this.ClientSize = new Size(520, 400);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Microsoft YaHei UI", 9F);
        header = new Panel { Dock = DockStyle.Top, Height = 70, BackColor = accent };
        var t = new Label { Text = "\u8d44\u6e90\u5171\u4eab", ForeColor = Color.White, Font = new Font("Microsoft YaHei UI", 18F, FontStyle.Bold), Location = new Point(20, 12), AutoSize = true };
        var st = new Label { Text = "\u8d44\u6e90\u5408\u96c6\u5206\u4eab\u5de5\u5177  v1.0", ForeColor = Color.FromArgb(200, 220, 255), Location = new Point(22, 45), AutoSize = true };
        header.Controls.Add(t); header.Controls.Add(st);
        footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.White };
        content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };
        this.Controls.AddRange(new Control[] { content, footer, header });
        ShowWelcome();
    }

    void Clear() { content.Controls.Clear(); footer.Controls.Clear(); }

    void ShowWelcome() {
        Clear();
        var lbl = new Label { Text = "\u6b22\u8fce\u5b89\u88c5\u300c\u8d44\u6e90\u5171\u4eab\u300d\n\n\u672c\u7a0b\u5e8f\u5c06\u628a\u8d44\u6e90\u5171\u4eab\u5b89\u88c5\u5230\u60a8\u7684\u7535\u8111\u4e0a\u3002\n\u9ed8\u8ba4\u5b89\u88c5\u5230 D:\\ResShare\n\n\u5b89\u88c5\u540e\u5c06\u521b\u5efa\u684c\u9762\u5feb\u6377\u65b9\u5f0f\u548c\u5378\u8f7d\u5de5\u5177\u3002", Location = new Point(20, 20), Size = new Size(470, 120), Font = new Font("Microsoft YaHei UI", 10F) };
        var chk = new CheckBox { Text = "\u6211\u5df2\u9605\u8bfb\u5e76\u540c\u610f\u300a\u7528\u6237\u534f\u8bae\u300b", Location = new Point(20, 160), AutoSize = true };
        var c = new Button { Text = "\u53d6\u6d88", Size = new Size(75, 30), Location = new Point(330, 8) }; c.Click += (s, e) => this.Close();
        var n = new Button { Text = "\u4e0b\u4e00\u6b65", Size = new Size(85, 30), Location = new Point(420, 8), BackColor = accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        n.Click += (s, e) => { if (!chk.Checked) { MessageBox.Show("\u8bf7\u5148\u540c\u610f\u7528\u6237\u534f\u8bae"); return; } ShowDir(); };
        content.Controls.AddRange(new Control[] { lbl, chk }); footer.Controls.AddRange(new Control[] { c, n });
    }

    void ShowDir() {
        Clear();
        var lbl = new Label { Text = "\u9009\u62e9\u5b89\u88c5\u4f4d\u7f6e", Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        var txt = new TextBox { Text = installDir, Location = new Point(20, 60), Size = new Size(380, 26) };
        var br = new Button { Text = "\u6d4f\u89c8...", Size = new Size(80, 26), Location = new Point(410, 59) };
        br.Click += (s, e) => { var f = new FolderBrowserDialog(); if (f.ShowDialog() == DialogResult.OK) txt.Text = f.SelectedPath; };
        var b = new Button { Text = "\u4e0a\u4e00\u6b65", Size = new Size(75, 30), Location = new Point(240, 8) }; b.Click += (s, e) => ShowWelcome();
        var c = new Button { Text = "\u53d6\u6d88", Size = new Size(75, 30), Location = new Point(330, 8) }; c.Click += (s, e) => this.Close();
        var n = new Button { Text = "\u5b89\u88c5", Size = new Size(85, 30), Location = new Point(420, 8), BackColor = accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        n.Click += (s, e) => { installDir = txt.Text; ShowInstalling(); };
        content.Controls.AddRange(new Control[] { lbl, txt, br }); footer.Controls.AddRange(new Control[] { b, c, n });
    }

    void ShowInstalling() {
        Clear();
        var lbl = new Label { Text = "\u6b63\u5728\u5b89\u88c5...", Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold), Location = new Point(20, 30), AutoSize = true };
        status = new Label { Text = "\u51c6\u5907\u4e2d...", Location = new Point(20, 80), AutoSize = true, ForeColor = Color.Gray };
        progress = new ProgressBar { Location = new Point(20, 110), Size = new Size(470, 28) };
        content.Controls.AddRange(new Control[] { lbl, status, progress });
        new Thread(DoInstall) { IsBackground = true }.Start();
    }

    void DoInstall() {
        try {
            if (!Directory.Exists(installDir)) Directory.CreateDirectory(installDir);
            string zipPath = null;
            using (Stream res = Assembly.GetExecutingAssembly().GetManifestResourceStream("resources.zip")) {
                if (res != null) {
                    zipPath = Path.Combine(Path.GetTempPath(), "rs.zip");
                    using (FileStream fs = File.Create(zipPath)) { byte[] buf = new byte[65536]; int n; while ((n = res.Read(buf, 0, buf.Length)) > 0) fs.Write(buf, 0, n); }
                }
            }
            if (zipPath != null) {
                using (ZipArchive a = ZipFile.OpenRead(zipPath)) {
                    int t = a.Entries.Count, i = 0;
                    foreach (var e in a.Entries) {
                        i++; int p = (int)((float)i / t * 100);
                        this.Invoke((Action)delegate { progress.Value = p; status.Text = i + " / " + t; });
                        string d = Path.Combine(installDir, e.FullName);
                        string dir = Path.GetDirectoryName(d);
                        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                        if (e.Length > 0) e.ExtractToFile(d, true);
                    }
                }
                try { File.Delete(zipPath); } catch {}
            }

            this.Invoke((Action)delegate { status.Text = "\u521b\u5efa\u5feb\u6377\u65b9\u5f0f..."; });
            string exe = Path.Combine(installDir, "ResShare.exe");
            
            // 创建桌面快捷方式
            string desk = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            CreateShortcut(desk + "\\\u8d44\u6e90\u5171\u4eab.lnk", exe);
            
            // 创建开始菜单
            string sm = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\\u8d44\u6e90\u5171\u4eab";
            if (!Directory.Exists(sm)) Directory.CreateDirectory(sm);
            CreateShortcut(sm + "\\\u8d44\u6e90\u5171\u4eab.lnk", exe);
            CreateShortcut(sm + "\\\u5378\u8f7d\u8d44\u6e90\u5171\u4eab.lnk", Path.Combine(installDir, "uninstall.exe"));
            
            this.Invoke((Action)delegate { ShowDone(); });
        } catch (Exception ex) {
            this.Invoke((Action)delegate { MessageBox.Show("Error: " + ex.Message); this.Close(); });
        }
    }

    void CreateShortcut(string lnkPath, string targetPath) {
        string workingDir = Path.GetDirectoryName(targetPath);
        string iconPath = targetPath + ",0";
        string cmd = string.Format(
            "echo Set ws=CreateObject(\"WScript.Shell\"):Set s=ws.CreateShortcut(\"{0}\"):s.TargetPath=\"{1}\":s.WorkingDirectory=\"{2}\":s.IconLocation=\"{3}\":s.Save > \"%TEMP%\\mklnk.vbs\" & cscript //nologo \"%TEMP%\\mklnk.vbs\" & del \"%TEMP%\\mklnk.vbs\"",
            lnkPath, targetPath, workingDir, iconPath);
        var psi = new ProcessStartInfo("cmd.exe", "/c " + cmd);
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        psi.CreateNoWindow = true;
        psi.UseShellExecute = false;
        using (var p = Process.Start(psi)) p.WaitForExit(10000);
    }

    void ShowDone() {
        Clear();
        var l = new Label { Text = "\u5b89\u88c5\u5b8c\u6210\uff01", Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 160, 0), Location = new Point(20, 30), AutoSize = true };
        var i = new Label { Text = "\u5df2\u5b89\u88c5\u5230\uff1a" + installDir + "\n\n\u684c\u9762\u5df2\u521b\u5efa\u300c\u8d44\u6e90\u5171\u4eab\u300d\u5feb\u6377\u65b9\u5f0f\n\u5f00\u59cb\u83dc\u5355\u5df2\u521b\u5efa\u5378\u8f7d\u5de5\u5177", Location = new Point(20, 80), Size = new Size(470, 80), Font = new Font("Microsoft YaHei UI", 10F) };
        content.Controls.AddRange(new Control[] { l, i });
        var r = new Button { Text = "\u7acb\u5373\u542f\u52a8", Size = new Size(85, 30), Location = new Point(320, 8) };
        r.Click += (s, ev) => { Process.Start(Path.Combine(installDir, "ResShare.exe")); this.Close(); };
        var f = new Button { Text = "\u5b8c\u6210", Size = new Size(85, 30), Location = new Point(420, 8), BackColor = accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        f.Click += (s, e) => this.Close();
        footer.Controls.AddRange(new Control[] { r, f });
    }
}