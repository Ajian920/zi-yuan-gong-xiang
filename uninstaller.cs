using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

class Uninstaller : Form {
    Color accent = Color.FromArgb(220, 50, 50);
    string installDir = @"D:\ResShare";

    [STAThread]
    static void Main() { Application.EnableVisualStyles(); Application.Run(new Uninstaller()); }

    public Uninstaller() {
        this.Text = "\u8d44\u6e90\u5171\u4eab \u5378\u8f7d\u5411\u5bfc";
        this.ClientSize = new Size(420, 300);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(245, 245, 250);
        this.Font = new Font("Microsoft YaHei UI", 9F);

        // Header
        var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = accent };
        var t = new Label { Text = "\u8d44\u6e90\u5171\u4eab \u5378\u8f7d\u7a0b\u5e8f", ForeColor = Color.White, Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        header.Controls.Add(t);

        // Content
        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        var icon = new Label { Text = "\u26a0", Font = new Font("Segoe UI", 28F), ForeColor = Color.FromArgb(255, 160, 0), Location = new Point(20, 15), AutoSize = true };
        var msg = new Label { Text = "\u786e\u5b9a\u8981\u5378\u8f7d\u300c\u8d44\u6e90\u5171\u4eab\u300d\u5417\uff1f\n\n\u5378\u8f7d\u5c06\u5220\u9664\u4ee5\u4e0b\u5185\u5bb9\uff1a\n\u2022 \u5b89\u88c5\u76ee\u5f55\uff1a" + installDir + "\n\u2022 \u684c\u9762\u5feb\u6377\u65b9\u5f0f\n\u2022 \u5f00\u59cb\u83dc\u5355\u5feb\u6377\u65b9\u5f0f", Location = new Point(70, 15), Size = new Size(320, 120), Font = new Font("Microsoft YaHei UI", 10F) };
        content.Controls.AddRange(new Control[] { icon, msg });

        // Footer
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 50, BackColor = Color.White };
        var btnCancel = new Button { Text = "\u53d6\u6d88", Size = new Size(80, 32), Location = new Point(220, 10), FlatStyle = FlatStyle.Flat };
        btnCancel.Click += (s, e) => this.Close();
        var btnUninstall = new Button { Text = "\u5378\u8f7d", Size = new Size(80, 32), Location = new Point(320, 10), BackColor = accent, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnUninstall.Click += (s, e) => DoUninstall();
        footer.Controls.AddRange(new Control[] { btnCancel, btnUninstall });

        this.Controls.AddRange(new Control[] { content, footer, header });
    }

    void DoUninstall() {
        // 隐藏按钮，显示进度
        this.Controls.Clear();
        var header = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = accent };
        var t = new Label { Text = "\u8d44\u6e90\u5171\u4eab \u5378\u8f7d\u7a0b\u5e8f", ForeColor = Color.White, Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
        header.Controls.Add(t);

        var content = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        var status = new Label { Text = "\u6b63\u5728\u5378\u8f7d...", Location = new Point(20, 20), AutoSize = true, Font = new Font("Microsoft YaHei UI", 12F) };
        var progress = new ProgressBar { Location = new Point(20, 60), Size = new Size(380, 25), Style = ProgressBarStyle.Marquee };
        content.Controls.AddRange(new Control[] { status, progress });
        this.Controls.AddRange(new Control[] { content, header });

        // 执行卸载
        try {
            // 1. 删除桌面快捷方式
            status.Text = "\u5220\u9664\u684c\u9762\u5feb\u6377\u65b9\u5f0f...";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            DeleteFile(desktop + "\\\u8d44\u6e90\u5171\u4eab.lnk");

            // 2. 删除开始菜单
            status.Text = "\u5220\u9664\u5f00\u59cb\u83dc\u5355...";
            string sm = Environment.GetFolderPath(Environment.SpecialFolder.Programs) + "\\\u8d44\u6e90\u5171\u4eab";
            DeleteDir(sm);

            // 3. 关闭主程序
            status.Text = "\u5173\u95ed\u4e3b\u7a0b\u5e8f...";
            foreach (var p in Process.GetProcessesByName("ResShare")) {
                try { p.Kill(); p.WaitForExit(3000); } catch {}
            }

            // 4. 删除安装目录
            status.Text = "\u5220\u9664\u5b89\u88c5\u76ee\u5f55...";
            DeleteDir(installDir);

            // 完成
            status.Text = "\u5378\u8f7d\u5b8c\u6210\uff01";
            progress.Style = ProgressBarStyle.Continuous;
            progress.Value = 100;
            MessageBox.Show("\u8d44\u6e90\u5171\u4eab\u5df2\u6210\u529f\u5378\u8f7d\uff01", "\u5378\u8f7d\u5b8c\u6210", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        } catch (Exception ex) {
            MessageBox.Show("\u5378\u8f7d\u51fa\u9519\uff1a" + ex.Message, "\u9519\u8bef", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.Close();
        }
    }

    void DeleteFile(string path) {
        try { if (File.Exists(path)) File.Delete(path); } catch {}
    }

    void DeleteDir(string path) {
        try { if (Directory.Exists(path)) Directory.Delete(path, true); } catch {}
    }
}