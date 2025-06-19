using System;
using System.Diagnostics;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.CommonTasks;
using RegistryHive = WixSharp.RegistryHive;

namespace Pkn_HostSystem_Wix_V4_Bootstrapper
{
    public class Program
    {
        static string baseDir =
            @"C:\Users\admin\Desktop\space\C#Project\Pkn_HostSystem\Pkn_HostSystem\bin\Debug\net8.0-windows";


        static string iconFile =
            @"C:\Users\admin\Desktop\space\C#Project\Pkn_HostSystem\Pkn_HostSystem\Assets\Pkn_Install128.ico";

        static Guid upgradeCode = new Guid("F9A314C0-7F34-4A5D-ABF3-54C4C48B9F2C");

        static Guid productCode = new Guid("30854a0c-8c60-4957-8ba4-a0a57f14fa11");

        static string version = FileVersionInfo.GetVersionInfo($@"{baseDir}\Pkn_HostSystem.exe").FileVersion;

        static void Main()
        {
            string productMsi = BuildMsi();

            var bootstrapper =
                new Bundle("Pkn_HostSystemInstall",
                    new PackageGroupRef("NetFx462Web"),
                    new ExePackage(@"windowsdesktop-runtime-8.0.11-win-x64.exe")
                    {
                        Permanent = true, //此组件是永久性的，不会被 Bootstrapper 卸载。
                        // UninstallCommand = "/uninstall /quiet /norestart",
                        InstallArguments = "/install /quiet /norestart", //安装时调用的命令行参数（静默安装、无重启）
                        //DetectCondition = "DOTNET_VERSION >= \"8.0.0\" AND DOTNET_VERSION < \"9.0.0\"",  //如果当前系统中已满足这个条件，就不执行安装（即已安装 .NET 8，就不重复安装） , 不需要这行了, 即使安装器运行了，微软的 .NET Desktop Runtime 安装器会检测已有版本并自动退出
                        Compressed = true, //将安装包一起打进最终生成的 .exe 安装器（否则需要外部放置）
                        DisplayName = ".NET 8 Runtime" //安装引导界面中显示的组件名称
                    },
                    new MsiPackage(productMsi) { DisplayInternalUI = true });
            bootstrapper.Language= "zh-CN";
            bootstrapper.Version = new Version(version);
            bootstrapper.UpgradeCode = upgradeCode;
            bootstrapper.IconFile = iconFile;
            // bootstrapper.PreserveTempFiles = true;
            bootstrapper.Build("Pkn_HostSystemInstall.exe");
        }

        static string BuildMsi()
        {
            string baseDir =
                @"C:\Users\admin\Desktop\space\C#Project\Pkn_HostSystem\Pkn_HostSystem\bin\Debug\net8.0-windows";

            var project = new Project("Pkn_HostSystem",
                new Dir(@"%ProgramFiles%\Pokeniao\Pkn_HostSystem", //这是告诉 WixSharp 你想让这个目录作为安装根目录
                    new Files($@"{baseDir}\*.*")
                    // , new File($@"..\Pkn_HostSystem\runtime\windowsdesktop-runtime-8.0.11-win-x64.exe") //将环境也一起打包进去
                    // {
                    //     TargetFileName = "dotnet-runtime-installer.exe", //重命名
                    //     Condition = new Condition("NOT DOTNET_VERSION >< \"12.\"") //只有没装 .NET 8 时才安装它 , 有不安装
                    // }
                ),
                // 单独定义快捷方式, "[INSTALLDIR]" 是 安装目录占位符，会在安装时替换为实际安装路径
                new Dir(@"%Desktop%", //桌面
                    new ExeFileShortcut("Pkn_HostSystem", "[INSTALLDIR]Pkn_HostSystem.exe",
                            "") //快捷方式的名称 , 快捷方式的可执行文件 , 参数可以填 --safe等
                        {
                            WorkingDirectory = "[INSTALLDIR]", // 快捷方式对应工作的地方
                        }
                ),
                new Dir(@"%ProgramMenu%\Pkn_HostSystem", //开始菜单程序目录
                    new ExeFileShortcut("Pkn_HostSystem", "[INSTALLDIR]Pkn_HostSystem.exe",
                            "") //快捷方式的名称 , 快捷方式的可执行文件 , 参数可以填 --safe等
                        {
                            WorkingDirectory = "[INSTALLDIR]" // 快捷方式对应工作的地方
                        },
                    new ExeFileShortcut("卸载Pkn_HostSystem", "[SystemFolder]msiexec.exe", "/x [ProductCode]")
                    {
                        IconFile = $@"{baseDir}\Pkn_HostSystem.exe" // 提取主程序内嵌图标
                    })
            );

            #region 自动升级逻辑

            // 固定 UpgradeCode,第一次生成后固定下来不要再改,用于标识同一“产品线”。
            project.UpgradeCode = upgradeCode;
            // 递增版本号（重要）
            project.Version = new Version(version);

            // 自动升级设置,没有 MajorUpgrade，即使你更新了版本号，安装包也不会自动卸载旧版本，你必须手动卸载旧版才能安装新版。
            project.MajorUpgrade = new MajorUpgrade
            {
                AllowDowngrades = false, // 不允许降级安装
                Schedule = UpgradeSchedule.afterInstallInitialize, // 升级流程安排
                DowngradeErrorMessage = "不能安装比当前版本更低的版本。" // 错误提示
            };

            #endregion

            //安装器定义的属性（Property），用于控制安装流程、条件等。
            project.Properties = new[]
            {
                //RegValueProperty  WixSharp 内置类，表示一个“通过注册表获取值”的 Property。
                new RegValueProperty("DOTNET_VERSION", //安装属性的名称（变量名），可以在 LaunchCondition 中引用它。
                        RegistryHive.LocalMachine, //查询的是 HKEY_LOCAL_MACHINE 主键，等同于注册表编辑器里的 HKLM。
                        @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost", //注册表键路径：.NET 安装信息所在。
                        "Version") //注册表中该键下的值名（你在注册表右侧看到的字段名）。
                    {
                        Win64 = true
                    } // 64位必须要有这个不然获取不到
            };

            #region 用不了的 WixSharp 的高级钩子机制

            //这是 WixSharp 的高级钩子机制，当内部生成 WiX XML 文档（.wxs 文件）时，允许你拦截并修改它。
            // doc 是 XDocument 类型，代表整个 WiX 安装 XML。
            // project.WixSourceGenerated += document =>
            // {
            //     //找到 WiX 安装包 XML 的 <Product> 元素，它是整个安装逻辑的根元素。
            //     var product = document.Root.Select("Product");
            //
            //     //向 <Product> 添加一个新的 <Property> 元素，作用是：定义一个 MSI 安装属性，名称为DOTNET_VERSION。
            //     product.Add(
            //         new XElement("Property",
            //             new XAttribute("Id", "DOTNET_VERSION"), // 这表示这个属性的名称是 DOTNET_VERSION，这个变量后续可以在 LaunchCondition 或安装逻辑中使用。
            //             new XElement("RegistrySearch", //往这个属性中加入一个 <RegistrySearch>，表示从注册表中获取值并赋给 DOTNET_VERSION。
            //                 new XAttribute("Id", "DotNetSearch"), // 为这次搜索起一个 ID（可选的标识名，主要用于调试或其他引用）。
            //                 new XAttribute("Root", "HKLM"), //表示要查询的是注册表的 HKEY_LOCAL_MACHINE 主键。
            //                 new XAttribute("Key", @"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost"), // 注册表路径，指向 .NET Core/5/6/7/8 Runtime 安装路径的标准位置。
            //                 new XAttribute("Name", "Version"), // 要查询的值的名称（注册表项右边列的名称），我们要获取的是 .NET shared host 安装的 Version 字段。
            //                 new XAttribute("Type", "raw"), // 表示获取原始字符串值（而不是 DWORD 等数据类型）。
            //                  new XAttribute("Win64", "yes") // 告诉安装程序这是 64 位注册表路径（默认 MSI 读取 32 位注册表，需要手动开启 64 位支持）。
            //             )
            //         )
            //     );
            // };
            /***
             *相当于
             *<Property Id="DOTNET_VERSION">
                 <RegistrySearch Id="DotNetSearch"
                                 Root="HKLM"
                                 Key="SOFTWARE\dotnet\Setup\InstalledVersions\x64\sharedhost"
                                 Name="Version"
                                 Type="raw"
                                 Win64="yes" />
               </Property>
             */

            #endregion

            // LaunchConditions 用于条件判断 , 判断 DOTNET_VERSION 是否存在
            project.LaunchConditions.Add(
                new LaunchCondition(
                    "DOTNET_VERSION >= \"8.0.0\" AND DOTNET_VERSION < \"9.0.0\"",
                    ".NET 8 未安装，无法继续安装。\n请先下载安装：https://dotnet.microsoft.com/en-us/download/dotnet/8.0")
            );

            // 添加注册表内容, 通过[INSTALLDIR]标识的,在卸载的时候,会索引到进行卸载
            project.AddRegValue(
                new RegValue(RegistryHive.CurrentUser,
                    @"Software\Microsoft\Windows\CurrentVersion\Run",
                    "Pkn_HostSystem",
                    "\"[INSTALLDIR]Pkn_HostSystem.exe\"")
            );


            project.UI = WUI.WixUI_InstallDir; //安装器会自动弹出选择目录窗口。
            // project.LicenceFile = @"license.rtf"; // 你的协议文件

            //设置这个 MSI 安装包的唯一标识符（ProductCode）。1.控制面板卸载时用它区分不同软件。 2.如果你重新生成一个新的 GUID，则表示是不同的软件版本（旧版无法升级，会并存） 3. 如果要支持升级，GUID 要固定，但 Version 要变
            project.GUID = productCode;
            //发布者
            project.ControlPanelInfo.Manufacturer = "Pokeniao";
            project.ControlPanelInfo.ProductIcon =
                $@"{baseDir}\Pkn_HostSystem.exe"; //用程序图标作为控制面板显示图标（提取 .exe 的 embedded icon）
            project.Codepage = "936"; //65001 UTF-8 不支持 , 936 简体中文
            project.Language = "zh-CN";
            project.PreserveTempFiles = true; //当生成失败时，不删除临时的 .wxs XML 文件、.wixobj 等，便于调试。调试时建议开启。正式发布时可以关掉。

            // Compiler.BuildMsi(project); // 编译生成 .msi 安装包，输出在当前项目的 bin\Debug\netX 目录下。

            return project.BuildMsi();
        }
    }
}