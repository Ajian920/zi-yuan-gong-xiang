using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

static class Program {
    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [STAThread]
    static void Main() {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Extract HTML to temp
        string tempDir = Path.Combine(Path.GetTempPath(), "ResShare");
        if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);
        string tempHtml = Path.Combine(tempDir, "index.html");

        try {
            using (Stream s = Assembly.GetExecutingAssembly().GetManifestResourceStream("ResShare.index.html")) {
                if (s != null) {
                    using (FileStream fs = File.Create(tempHtml)) {
                        byte[] buf = new byte[65536];
                        int n;
                        while ((n = s.Read(buf, 0, buf.Length)) > 0) fs.Write(buf, 0, n);
                    }
                } else {
                    MessageBox.Show("找不到内嵌资源文件。", "资源共享", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        } catch (Exception ex) {
            MessageBox.Show("释放资源失败：" + ex.Message, "资源共享", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Find Edge
        string edgePath = FindEdge();
        if (edgePath == null) {
            // Fallback: open in default browser
            try {
                Process.Start(tempHtml);
            } catch {
                MessageBox.Show("未找到Microsoft Edge浏览器，也无法打开默认浏览器。\n请安装Edge后重试。", "资源共享", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return;
        }

        // Launch Edge in app mode
        try {
            string url = "file:///" + tempHtml.Replace("\\", "/");
            ProcessStartInfo psi = new ProcessStartInfo {
                FileName = edgePath,
                Arguments = "--app=\"" + url + "\" --disable-features=msEdgeEnhanceContextMenu --no-first-run",
                UseShellExecute = false
            };
            Process.Start(psi);
        } catch (Exception ex) {
            MessageBox.Show("启动浏览器失败：" + ex.Message, "资源共享", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    static string FindEdge() {
        string[] paths = new string[] {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Microsoft", "Edge", "Application", "msedge.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft", "Edge", "Application", "msedge.exe"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Microsoft", "Edge", "Application", "msedge.exe")
        };
        foreach (string p in paths) {
            if (File.Exists(p)) return p;
        }
        // Try registry
        try {
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe");
            if (key != null) {
                string v = key.GetValue("") as string;
                if (v != null && File.Exists(v)) return v;
            }
        } catch {}
        return null;
    }
}
