using System;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Text;

class App {
    static HttpListener listener;
    static string baseDir;

    static void Main() {
        baseDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "www");
        
        // Find a free port
        int port = 18080;
        listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:" + port + "/");
        
        try {
            listener.Start();
        } catch {
            // Try another port
            port = 18081;
            listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:" + port + "/");
            listener.Start();
        }

        // Open browser
        Process.Start("http://localhost:" + port + "/");

        // Serve requests
        while (true) {
            try {
                var ctx = listener.GetContext();
                string path = ctx.Request.Url.LocalPath;
                if (path == "/") path = "/index.html";
                
                string filePath = Path.Combine(baseDir, path.TrimStart('/'));
                
                if (File.Exists(filePath)) {
                    byte[] data = File.ReadAllBytes(filePath);
                    string contentType = "text/html";
                    if (filePath.EndsWith(".css")) contentType = "text/css";
                    else if (filePath.EndsWith(".js")) contentType = "application/javascript";
                    else if (filePath.EndsWith(".png")) contentType = "image/png";
                    else if (filePath.EndsWith(".ico")) contentType = "image/x-icon";
                    
                    ctx.Response.ContentType = contentType + "; charset=utf-8";
                    ctx.Response.ContentLength64 = data.Length;
                    ctx.Response.OutputStream.Write(data, 0, data.Length);
                } else {
                    ctx.Response.StatusCode = 404;
                    byte[] msg = Encoding.UTF8.GetBytes("Not Found");
                    ctx.Response.OutputStream.Write(msg, 0, msg.Length);
                }
                ctx.Response.Close();
            } catch {
                break;
            }
        }
    }
}