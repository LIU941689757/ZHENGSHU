
using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main()
    {
        // 读取配置行（形如：D:\certs\client.pfx 123456 ；.cer可只有路径）
        string line = ParseConfig();

        // 静默安装（InstallCertificate 内部解析路径/密码并执行 certutil）
        InstallCertificate(line);
    }

    // 读取配置文件一行文本：路径 与（可选）密码在同一行
    static string ParseConfig()
    {
        string cfgFile = @"D:\testpath.txt";
        return File.ReadAllText(cfgFile).Trim();
    }

    // 从“路径+密码”的单一字符串中解析并执行静默安装
    static void InstallCertificate(string pathAndPassword)
    {
        // 拆分：第一个空格前=路径；空格后=密码（可空）
        string certPath;
        string password = "";
        int spaceIndex = pathAndPassword.IndexOf(' ');
        if (spaceIndex > 0)
        {
            certPath = pathAndPassword.Substring(0, spaceIndex).Trim('"');
            password = pathAndPassword.Substring(spaceIndex + 1).Trim();
        }
        else
        {
            certPath = pathAndPassword.Trim('"');
        }

        // 根据扩展名选择导入方式（pfx用 -importpfx；其它如 .cer 用 -addstore）
        string ext = Path.GetExtension(certPath).ToLowerInvariant();
        string args;

        if (ext == ".pfx")
        {
            // pfx 始终带 -p，空密码也写 -p ""，保证静默无交互
            args = string.IsNullOrEmpty(password)
                ? "-user -f -p \"\" -importpfx My \"" + certPath + "\""
                : "-user -f -p \"" + password + "\" -importpfx My \"" + certPath + "\"";
        }
        else
        {
            args = "-user -f -addstore My \"" + certPath + "\"";
        }

        // 静默执行 certutil，不打印任何信息
        var p = new Process();
        p.StartInfo.FileName = "certutil.exe";
        p.StartInfo.Arguments = args;
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.CreateNoWindow = true;
        p.Start();
        p.WaitForExit();
    }
}
