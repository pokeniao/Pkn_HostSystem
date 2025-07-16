using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Base;

public class JsonTool<T> where T : class, new()
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
    //         GlobalManager.AssemblyName // 文件夹名
    //     );

    private static readonly string SaveFile = Path.Combine(GlobalManager.AppFolder, "LocalCache");
    private static readonly string FilePath = Path.Combine(SaveFile, typeof(T).Name + ".json");


    private static LogBase<JsonTool<T>> Log = new();

    #region Json格式本地保存 , Json格式本地加载
    /// <summary>
    /// 用Json格式保存
    /// </summary>
    /// <param name="config"></param>
    /// <returns></returns>
    public static bool Save(T config)
    {
        try
        {
            //1.不存在文件夹,创建文件夹
            if (!Directory.Exists(SaveFile))
                Directory.CreateDirectory(SaveFile);
            //2.转成Json格式
            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            //3.JSON字符串写入到本地
            File.WriteAllTextAsync(FilePath, json);
            Log.Info($"{typeof(T)}程序保存成功");
            return true;
        }
        catch (Exception ex)
        {
            //保存失败
            Log.Error($"{typeof(T)}程序保存失败:{ex}");
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
            //1. 判断文件路径是否存在
            if (File.Exists(FilePath))
            {
                //2. 读取文件中的字符串
                var json = File.ReadAllText(FilePath);
                //3. 将字符串转成类
                //3.1 设置转换格式   new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }强制调用set方法 ,那么只要 get 不为 null，默认会走“就地填充”，不调用 set，即：如果在对象创建时就被初始化了（比如 constructor 或字段初始化），Newtonsoft 会把 JSON 中的数组元素一个个 .Add() 到 Numbers 里，而不是重新创建一个新 List 并调用 set Numbers(...)。
                return JsonConvert.DeserializeObject<T>(json,
                    new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }) ?? null;
            }
        }
        catch (Exception ex)
        {
            // 可加日志处理
            Log.Error($"程序重新加载本地缓冲失败:{ex}");
        }
        return null; // 文件不存在或解析失败时返回null,进行初始化
    }

    /// <summary>
    /// 删除保存
    /// </summary>
    public static void Reset()
    {
        //存在,删掉
        if (File.Exists(FilePath)) File.Delete(FilePath);
    }

    #endregion




    /// <summary>
    /// 尝试格式化JSON输出换行,并且校验是否为JSON格式 ,成功返回格式化后的JSON string字符串 , 否则返回原本JSON
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
            //1. 去除前后的空格
            var trimmed = response.Trim();
            //2. 检查是不是json格式的
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                //是Json格式
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


    #region 深拷贝

    /// <summary>
    /// 利用JSON 实现深拷贝 , 先将类转成JSON 在将JSON转成一个类
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

    #endregion

    #region 通过JSON修改类

    /// <summary>
    /// 传入一个JSON字符串,修改目标类
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static void PopulateObject(string partialJson, T target)
    {

        if (string.IsNullOrWhiteSpace(partialJson) || target == null)
            return;

        // JsonConvert.PopulateObject会将 JSON 中有的字段赋值到 targetObject 上；
        // 不会清空或影响 JSON 中没有的字段；
        // 不会创建新对象，只对已有对象赋值。
        JsonConvert.PopulateObject(partialJson, target);
    }

    /// <summary>
    /// 传入一个修改类, 修改目标类
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static void PopulateObject(T alter, T target)
    {
        //转成json
        var json = JsonConvert.SerializeObject(alter, Formatting.Indented);
        if (string.IsNullOrWhiteSpace(json) || target == null)
            return;
        // new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace }强制调用set方法 ,那么只要 get 不为 null，默认会走“就地填充”，不调用 set，即：如果在对象创建时就被初始化了（比如 constructor 或字段初始化），Newtonsoft 会把 JSON 中的数组元素一个个 .Add() 到 Numbers 里，而不是重新创建一个新 List 并调用 set Numbers(...)。
        var settings = new JsonSerializerSettings
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };
        // JsonConvert.PopulateObject会将 JSON 中有的字段赋值到 targetObject 上；
        // 不会清空或影响 JSON 中没有的字段；
        // 不会创建新对象，只对已有对象赋值。
        JsonConvert.PopulateObject(json, target, settings);
    }

    #endregion

}