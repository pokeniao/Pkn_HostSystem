using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Base;

public class AppJsonTool<T> where T : class, new()
{
    //1. Environment.SpecialFolder.ApplicationData 对应 C:\Users\你的用户名\AppData\Roaming
    //这是 Windows 推荐我们存放 用户级应用数据 的地方，比如配置文件、用户缓存、保存状态等。

    //2. Environment.GetFolderPath  获取系统中特殊文件夹的路径
    //Environment.GetFolderPath(Environment.SpecialFolder.Desktop); 返回：C:\Users\你的用户名\Desktop
    //Environment.SpecialFolder.LocalApplicationData  本地专用，不同步


    //3.  Path.Combine(...) 是 .NET 提供的路径拼接方法：它会自动添加斜杠 \，防止你手动拼接时出错。
    // private static readonly string AppFolder =
    //     Path.Combine(
    //         Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
    //         GlobalMannager.AssemblyName // 文件夹名
    //     );

    private static readonly string SaveFile = Path.Combine(GlobalMannager.AppFolder, "程序缓存");
    private static readonly string FilePath = Path.Combine(SaveFile, typeof(T).Name + ".json");

    private static LogBase<AppJsonTool<T>> log = new();
    /// <summary>
    /// 用json格式保存
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static bool Save(T config)
    {
        try
        {
            //不存在,创建
            if (!Directory.Exists(SaveFile))
                Directory.CreateDirectory(SaveFile);
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
    /// <summary>
    /// 用Json格式加载
    /// </summary>
    /// <returns></returns>
    public static T Load()
    {
        try
        {
            //存在就加载
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                // new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }强制调用set方法 ,那么只要 get 不为 null，默认会走“就地填充”，不调用 set，即：
                //如果在对象创建时就被初始化了（比如 constructor 或字段初始化），
                //  Newtonsoft 会把 JSON 中的数组元素一个个 .Add() 到 Numbers 里，而不是重新创建一个新 List 并调用 set Numbers(...)。
                return JsonConvert.DeserializeObject<T>(json,
                    new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }) ?? null;
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

    /// <summary>
    /// 尝试转换成JSON
    /// </summary>
    /// <param name="response"></param>
    /// <param name="isJson"></param>
    /// <returns></returns>
    public static string TryFormatJson(string response,out bool isJson)
    {
        if (string.IsNullOrWhiteSpace(response))
        {
            isJson = false;
            return response;
        }
          

        try
        {
            var trimmed = response.Trim();
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                var obj = JsonConvert.DeserializeObject<object>(trimmed);
                isJson = true;
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
        }
        catch
        {
            isJson = false;
            return response;
            // 非合法 JSON，忽略
        }

        isJson = false;
        return response;
    }


    /// <summary>
    /// 利用JSON 实现深拷贝
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T DeepClone(T obj)
    {
        //转成json
        var json = JsonConvert.SerializeObject(obj, Formatting.Indented);
        //反序列化
        return JsonConvert.DeserializeObject<T>(json,
            new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }) ?? null;
    }

    /// <summary>
    /// JsonConvert.PopulateObject会将 JSON 中有的字段赋值到 targetObject 上；
    /// 不会清空或影响 JSON 中没有的字段；
    /// 不会创建新对象，只对已有对象赋值。
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static void ApplyPartialJson(string partialJson, T target)
    {

        if (string.IsNullOrWhiteSpace(partialJson) || target == null)
            return;
        JsonConvert.PopulateObject(partialJson, target);
    }

    /// <summary>
    /// JsonConvert.PopulateObject会将 JSON 中有的字段赋值到 targetObject 上；
    /// 不会清空或影响 JSON 中没有的字段；
    /// 不会创建新对象，只对已有对象赋值。
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static void ApplyPartialJson(T alter, T target)
    {
        //转成json
        var json = JsonConvert.SerializeObject(alter, Formatting.Indented);
        if (string.IsNullOrWhiteSpace(json) || target == null)
            return;

        // new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }强制调用set方法 ,那么只要 get 不为 null，默认会走“就地填充”，不调用 set，即：
        //如果在对象创建时就被初始化了（比如 constructor 或字段初始化），
        //  Newtonsoft 会把 JSON 中的数组元素一个个 .Add() 到 Numbers 里，而不是重新创建一个新 List 并调用 set Numbers(...)。
        var settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
        JsonConvert.PopulateObject(json, target,settings);
    }
}