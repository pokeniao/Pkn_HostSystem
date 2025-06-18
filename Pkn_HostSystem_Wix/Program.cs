using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using WixSharp;

namespace Pkn_HostSystem_Wix
{
    public class Program
    {
        static void Main()
        {
            string baseDir =
                @"C:\Users\admin\Desktop\space\C#Project\Pkn_HostSystem\Pkn_HostSystem\bin\Debug\net8.0-windows";

            var project = new Project("Pkn_HostSystem",
                new Dir(@"%ProgramFiles%\Pokeniao\Pkn_HostSystem",
                    new Files($@"{baseDir}\*.*")
                ),
                // 单独定义快捷方式, "[INSTALLDIR]" 是 安装目录占位符，会在安装时替换为实际安装路径
                new Dir(@"%Desktop%", //桌面
                    new ExeFileShortcut("Pkn_HostSystem", "[INSTALLDIR]Pkn_HostSystem.exe", "")//快捷方式的名称 , 快捷方式的可执行文件 , 参数可以填 --safe等
                    {
                        WorkingDirectory = "[INSTALLDIR]" // 快捷方式对应工作的地方
                    }
                )
            );



            // 固定 UpgradeCode,第一次生成后固定下来不要再改,用于标识同一“产品线”。
            project.UpgradeCode = new Guid("F9A314C0-7F34-4A5D-ABF3-54C4C48B9F2C");

            //获取版本集的版本
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo($@"{baseDir}\Pkn_HostSystem.exe");

            // 递增版本号（重要）
            project.Version = new Version(fileVersionInfo.FileVersion);

            // 自动升级设置,没有 MajorUpgrade，即使你更新了版本号，安装包也不会自动卸载旧版本，你必须手动卸载旧版才能安装新版。
            project.MajorUpgrade = new MajorUpgrade
            {
                AllowDowngrades = false,// 不允许降级安装
                Schedule = UpgradeSchedule.afterInstallInitialize, // 升级流程安排
                DowngradeErrorMessage = "不能安装比当前版本更低的版本。" // 错误提示
            };


            //设置这个 MSI 安装包的唯一标识符（ProductCode）。1.控制面板卸载时用它区分不同软件。 2.如果你重新生成一个新的 GUID，则表示是不同的软件版本（旧版无法升级，会并存） 3. 如果要支持升级，GUID 要固定，但 Version 要变
            project.GUID = new Guid("30854a0c-8c60-4957-8ba4-a0a57f14fa11");
            //发布者
            project.ControlPanelInfo.Manufacturer = "潘智高"; 
            project.ControlPanelInfo.ProductIcon =
                $@"{baseDir}\Pkn_HostSystem.exe"; //用程序图标作为控制面板显示图标（提取 .exe 的 embedded icon）
            project.Codepage = "936"; //65001 UTF-8 不支持 , 936 简体中文
            project.Language = "zh-CN";
            project.PreserveTempFiles = true; //当生成失败时，不删除临时的 .wxs XML 文件、.wixobj 等，便于调试。调试时建议开启。正式发布时可以关掉。


            Compiler.BuildMsi(project); // 编译生成 .msi 安装包，输出在当前项目的 bin\Debug\netX 目录下。
        }
    }
}