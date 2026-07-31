using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

class App : Form {
    WebBrowser browser;

    [STAThread]
    static void Main() {
        // Enable IE11 Edge mode for modern CSS support
        SetWebBrowserFeature();
        Application.EnableVisualStyles();
        Application.Run(new App());
    }

    public App() {
        this.Text = "资源共享";
        this.ClientSize = new System.Drawing.Size(1200, 800);
        this.MinimumSize = new System.Drawing.Size(380, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Icon = new System.Drawing.Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "build", "icon.ico"));

        browser = new WebBrowser();
        browser.Dock = DockStyle.Fill;
        browser.ScriptErrorsSuppressed = true;
        browser.IsWebBrowserContextMenuEnabled = true;
        browser.AllowNavigation = false;

        // Load the HTML file
        string htmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www", "index.html");
        if (File.Exists(htmlPath)) {
            browser.Navigate(new Uri("file:///" + htmlPath.Replace("\\", "/")));
        }

        this.Controls.Add(browser);
        this.FormClosing += (s, e) => Application.Exit();
    }

    // Enable IE11 Edge mode
    static void SetWebBrowserFeature() {
        SetFeatureBrowserFeature(FEATURE_BROWSER_EMULATION, 11001);
        SetFeatureBrowserFeature(FEATURE_GPU_RENDERING, 1);
    }

    const int FEATURE_BROWSER_EMULATION = 2901;
    const int FEATURE_GPU_RENDERING = 2904;

    [DllImport("urlmon.dll")]
    [PreserveSig]
    static extern int CoInternetSetFeatureEnabled(int featureEntry, int flags, bool enable);

    static void SetFeatureBrowserFeature(int feature, int value) {
        try {
            using (var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION")) {
                key.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".exe", value, Microsoft.Win32.RegistryValueKind.DWord);
            }
        } catch {}
    }
}