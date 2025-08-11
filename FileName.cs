using System;
using System.Diagnostics;
using System.IO;

class FileName
{
    static void Main()
    {
        // 解析配置文件
        string certPath, password;
        ParseConfig(out certPath, out password);

        // 安装证书
        InstallCertificate(certPath, password);
    }

    // 解析配置文件，返回证书路径和密码
    static void ParseConfig(out string certPath, out string password)
    {
        string cfgFile = @"D:\testpath.txt";
        string line = File.ReadAllText(cfgFile).Trim();

        password = "";
        int spaceIndex = line.IndexOf(' ');

        if (spaceIndex > 0)
        {
            certPath = line.Substring(0, spaceIndex).Trim('"');
            password = line.Substring(spaceIndex + 1).Trim();
        }
        else
        {
            certPath = line.Trim('"');
        }
    }

    // 安装证书（调用 certutil）
    static void InstallCertificate(string certPath, string password)
    {
        string ext = Path.GetExtension(certPath).ToLowerInvariant();
        string args;

        if (ext == ".pfx")
        {
            args = string.IsNullOrEmpty(password)
                ? "-user -f -p \"\" -importpfx My \"" + certPath + "\""
                : "-user -f -p \"" + password + "\" -importpfx My \"" + certPath + "\"";
        }
        else
        {
            args = "-user -f -addstore My \"" + certPath + "\"";
        }

        var p = new Process();
        p.StartInfo.FileName = "certutil.exe";
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
    }
}

