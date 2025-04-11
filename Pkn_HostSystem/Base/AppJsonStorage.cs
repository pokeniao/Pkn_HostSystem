using System.IO;

namespace Pkn_HostSystem.Base;

public class AppJsonStorage<T> where T : class, new()
{
    //1. Environment.SpecialFolder.ApplicationData 对应 C:\Users\你的用户名\AppData\Roaming
    //这是 Windows 推荐我们存放 用户级应用数据 的地方，比如配置文件、用户缓存、保存状态等。

    //2. Environment.GetFolderPath  获取系统中特殊文件夹的路径
    //Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 返回：C:\Users\你的用户名\Desktop

    //3.  Path.Combine(...) 是 .NET 提供的路径拼接方法：它会自动添加斜杠 \，防止你手动拼接时出错。
    private static readonly string AppFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Pkn_HostSystem" // 这里你可以替换成自己的程序名
        );
}