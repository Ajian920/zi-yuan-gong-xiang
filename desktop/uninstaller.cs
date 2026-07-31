using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

class Uninstaller : Form {
    Label status;
    ProgressBar progress;
    Button btnClose;
    string installDir;
    Color accent = Color.FromArgb(0, 120, 215);

    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.Run(new Uninstaller());
    }

    public Uninstaller() {
        installDir = Path.GetDirectoryName(Application.ExecutablePath);
        this.Text = "资源共享 卸载";
        this.ClientSize = new Size(420, 200);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Microsoft YaHei UI", 9F);

        var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = accent };
        header.Controls.Add(new Label { Text = "资源共享 卸载程序", ForeColor = Color.White, Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true });

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
        var lbl = new Label { Text = "确定要卸载「资源共享」吗？\n所有程序文件将被删除。", Location = new Point(15, 15), Size = new Size(380, 50), Font = new Font("Microsoft YaHei UI", 10F) };
        status = new Label { Text = "", Location = new Point(15, 70), Size = new Size(380, 20), ForeColor = Color.Gray };
        progress = new ProgressBar { Location = new Point(15, 95), Size = new Size(380, 22), Visible = false };

        var btnUninstall = new Button { Text = "卸载", Size = new Size(85, 30), Location = new Point(220, 125), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(220, 50, 50), ForeColor = Color.White };
        btnUninstall.Click += (s, e) => { btnUninstall.Enabled = false; DoUninstall(); };
        btnClose = new Button { Text = "取消", Size = new Size(75, 30), Location = new Point(315, 125) };
        btnClose.Click += (s, e) => this.Close();

        content.Controls.AddRange(new Control[] { lbl, status, progress, btnUninstall, btnClose });
        this.Controls.Add(content);
        this.Controls.Add(header);
    }

    void DoUninstall() {
        progress.Visible = true;
        progress.Style = ProgressBarStyle.Marquee;
        status.Text = "正在卸载...";

        // Remove shortcuts
        try {
            string desktop = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "资源共享.lnk");
            if (File.Exists(desktop)) File.Delete(desktop);

            string sm = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "资源共享");
            if (Directory.Exists(sm)) Directory.Delete(sm, true);
        } catch {}

        // Remove registry entry
        try {
            var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);
            if (key != null) key.DeleteSubKeyTree("ResShare", false);
        } catch {}
        try {
            var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true);
            if (key != null) key.DeleteSubKeyTree("ResShare", false);
        } catch {}

        // Schedule self-deletion
        status.Text = "卸载完成！";
        progress.Visible = false;
        MessageBox.Show("资源共享 已成功卸载。\n\n安装目录中的部分文件可能需要手动删除。", "卸载完成", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Try to delete files
        try {
            string exe = Path.Combine(installDir, "ResShare.exe");
            if (File.Exists(exe)) File.Delete(exe);
            string uninstaller = Application.ExecutablePath;
            // Schedule self-deletion via cmd
            Process.Start(new ProcessStartInfo {
                FileName = "cmd.exe",
                Arguments = "/c ping 127.0.0.1 -n 2 > nul & rmdir /s /q \"" + installDir + "\"",
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true
            });
        } catch {}

        this.Close();
    }
}
