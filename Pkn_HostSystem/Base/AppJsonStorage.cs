using System.IO;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Log;

namespace Pkn_HostSystem.Base;

public class AppJsonStorage<T> where T : class, new()
{
    //1. Environment.SpecialFolder.ApplicationData 对应 C:\Users\你的用户名\AppData\Roaming
    //这是 Windows 推荐我们存放 用户级应用数据 的地方，比如配置文件、用户缓存、保存状态等。

    //2. Environment.GetFolderPath  获取系统中特殊文件夹的路径
    //Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 返回：C:\Users\你的用户名\Desktop
    //Environment.SpecialFolder.LocalApplicationData  本地专用，不同步


    //3.  Path.Combine(...) 是 .NET 提供的路径拼接方法：它会自动添加斜杠 \，防止你手动拼接时出错。
    private static readonly string AppFolder =
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Pkn_HostSystem" // 文件夹名
        );

    private static readonly string FilePath = Path.Combine(AppFolder, typeof(T).Name + ".json");

    private static LogBase<AppJsonStorage<T>> log = new();

    public static bool Save(T config)
    {
        try
        {
            //不存在,创建
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);
            //转成json
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            //写入到本地
            File.WriteAllTextAsync(FilePath, json);
            log.Info($"{typeof(T)}程序保存成功");
            return true;
        }
        catch (Exception ex)
        {
            // 可加日志处理
            log.Error($"{typeof(T)}程序保存失败:{ex}");
            return false;
        }
    }

    public static T Load()
    {
        try
        {
            //存在就加载
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                return JsonConvert.DeserializeObject<T>(json) ?? null;
            }
        }
        catch (Exception ex)
        {
            // 可加日志处理
            log.Error($"程序重新加载本地缓冲失败:{ex}");
        }

        return null; // 文件不存在或解析失败时返回null,进行初始化
    }

    /// <summary>
    /// 重置
    /// </summary>
    public static void Reset()
    {
        //存在,删掉
        if (File.Exists(FilePath)) File.Delete(FilePath);
    }
}