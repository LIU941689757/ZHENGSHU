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
    static void InstallCertificate(string certPath, string password) // 安装证书，参数1=证书完整路径，参数2=密码(可空)
    {
        string ext = Path.GetExtension(certPath).ToLowerInvariant(); // 获取证书文件扩展名并转成小写
        string args; // 用来存拼接好的 certutil 命令参数

        if (ext == ".pfx") // 如果是 pfx 格式（含私钥）
        {
            args = string.IsNullOrEmpty(password) // 判断密码是否为空
                ? "-user -f -p \"\" -importpfx My \"" + certPath + "\"" // 密码为空：指定空密码，静默导入 pfx 到当前用户(My)
                : "-user -f -p \"" + password + "\" -importpfx My \"" + certPath + "\""; // 密码不为空：用给定密码静默导入 pfx
        }
        else // 如果是 cer/crt 等公钥证书
        {
            args = "-user -f -addstore My \"" + certPath + "\""; // 直接导入到当前用户(My)存储区
        }

        var p = new Process(); // 创建一个新进程对象
        p.StartInfo.FileName = "certutil.exe"; // 设置要启动的程序名为 certutil.exe（系统自带证书工具）
        p.StartInfo.Arguments = args; // 设置传递给 certutil 的参数
        p.StartInfo.UseShellExecute = false; // 禁用外壳启动，方便控制执行行为
        p.StartInfo.CreateNoWindow = true; // 不显示命令行窗口（静默）
        p.Start(); // 启动 certutil 进程
        p.WaitForExit(); // 等待 certutil 执行完成
    }

}

