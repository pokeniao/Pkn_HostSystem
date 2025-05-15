using KeyenceTool;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.Static;
using RestSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;


namespace Pkn_HostSystem.Server.LoadMes;

public class LoadMesService
{
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;

    private MesLogBase<LoadMesService> Log;

    public LoadMesService(ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList)
    {
        this.mesPojoList = mesPojoList;
        Log = new MesLogBase<LoadMesService>();
    }


    #region 获取当前行 与获取网络Id方法

    /// <summary>
    /// 循环查找当前行是否存在
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public LoadMesAddAndUpdateWindowModel SelectByName(string Name)
    {
        // Log.Info($"SelectByName执行,Name:{Name}");
        //判断是否有当前key 
        foreach (var item in mesPojoList)
            if (Name == item.Name)
                //返回当前行的数据
                return item;

        return null;
    }

    /// <summary>
    /// 循环遍历获取网络ID
    /// </summary>
    /// <returns></returns>
    public string getNetKey(string ConnectName)
    {
        //Log.Info($"getNetKey执行,ConnectName:{ConnectName}");
        var netWorkPoJoes = GlobalMannager.NetWorkDictionary.Items.ToList();
        foreach (var netWorkPoJo in netWorkPoJoes)
            if (netWorkPoJo.NetworkDetailed.Name == ConnectName)
                return netWorkPoJo.NetWorkId;

        return null;
    }

    #endregion

    #region 触发Http请求

    /// <summary>
    ///  触发单个请求
    /// </summary>
    /// <param name="Name">HTTP请求名称</param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string? response)> RunOne(string Name, CancellationTokenSource cts)
    {
        Log.Info($"[{TraceContext.Name}]--执行发送一次Http请求");
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = SelectByName(Name);
        //得到消息体
        var (succeed, request) = await PackRequest(item?.Name, cts);
        if (!succeed)
        {
            Log.Error($"[{TraceContext.Name}]--执行发送HTTP任务,消息体组装失败");
            return (false, null);
        }

        //得到消息体
        return await SendHttp(item, request, cts);
    }

    /// <summary>
    /// 触发单个请求
    /// </summary>
    /// <param name="Name">HTTP请求名称</param>
    /// <param name="request">请求体</param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string? response)> RunOne(string Name, string request, CancellationTokenSource cts)
    {
        Log.Info($"[{TraceContext.Name}]--执行发送一次Http请求");
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = SelectByName(Name);
        //得到消息体
        return await SendHttp(item, request, cts);
    }

    #endregion

    #region 发送Http任务

    public async Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item,
        string request,
        CancellationTokenSource cts)
    {
        //日志显示发送内容
        Log.InfoOverride($"[{TraceContext.Name}]--发送内容: \r\n {request}");

        //创建连接
        var client = new RestClient(item.HttpPath);
        RestRequest requestBody;
        //创建请求
        switch (item.Ajax)
        {
            case "POST":
                requestBody = new RestRequest(item.Api, Method.Post);
                break;
            case "GET":
                requestBody = new RestRequest(item.Api, Method.Get);
                break;
            case "DELETE":
                requestBody = new RestRequest(item.Api, Method.Delete);
                break;
            case "PUT":
                requestBody = new RestRequest(item.Api, Method.Put);
                break;
            default:
                requestBody = new RestRequest();
                break;
        }

        //添加请求头
        foreach (var header in item.HttpHeaders)
        {
            requestBody.AddHeader(header.Key, header.Value);
        }

        //添加请求体
        switch (item.RequestMethod)
        {
            case "JSON":
                //会自动设置 Content-Type: application/json，并把内容当作 JSON 处理。
                requestBody.AddStringBody(request, DataFormat.Json);
                break;
            case "XML":
                //表示数据格式是 XML。
                requestBody.AddStringBody(request, DataFormat.Xml);
                break;
            case "TEXT":
                //一般用于你想自己完全控制请求内容或用于 GET 请求等不带 body 的请求。
                requestBody.AddStringBody(request, DataFormat.None);
                break;
            default:
                requestBody.AddStringBody(request, DataFormat.None);
                break;
        }

        //发送请求
        RestResponse response = await client.ExecuteAsync(requestBody, cts.Token);
        //判断
        if (response.IsSuccessStatusCode)
        {
            item.Response = response.Content;
            //判断是否是JSON格式,如果是转成输出
            item.Response = AppJsonTool<Object>.TryFormatJson(item.Response, out bool isJson);
            Log.InfoOverride($"[{TraceContext.Name}]--返回消息--成功--状态码:{response.StatusCode}--消息体:\r\n{item.Response}");
            return (true, item.Response);
        }
        else
        {
            //尝试从错误消息中获取,获取不到就从消息内容中获取
            item.Response = response.ErrorMessage;
            if (item.Response == null)
            {
                item.Response = response.Content;
            }

            //判断是否是JSON格式,如果是转成输出
            item.Response = AppJsonTool<Object>.TryFormatJson(item.Response, out bool isJson);

            Log.ErrorOverride($"[{TraceContext.Name}]--返回消息--失败--状态码:{response.StatusCode}--消息体:\r\n{item.Response}");
            return (false, item.Response);
        }
    }

    #endregion

    #region 封装消息请求体方法

    /// <summary>
    /// 包装Request请求
    /// </summary>
    /// <param httpName="httpName"></param>
    public async Task<(bool succeed, string? value)> PackRequest(string httpName, CancellationTokenSource cts)
    {
        //获得当前行的数据
        var loadMesAddAndUpdateWindowModel = SelectByName(httpName);
        if (loadMesAddAndUpdateWindowModel == null) return (false, null);

        //获得当前行的条件  将   ObservableCollection<> 转成List
        var conditionItems = Enumerable.ToList<LoadMesCondition>(loadMesAddAndUpdateWindowModel.Condition);


        //获取请求体
        var request = loadMesAddAndUpdateWindowModel.Request;
        //对请求体进行嵌入内容
        //遍历当前条件,判断条件方式
        foreach (var item in conditionItems)
        {
            var itemKey = item.Key;
            var itemValue = item.Value;
            var itemMethodOtherValue = item.Method_OtherValue;

            switch (item.Method)
            {
                case "动态获取":
                    //获取动态的值
                    Log.Info($"[{TraceContext.Name}]--正在动态嵌入内容");
                    var (succeed, value) = await DynMessage(request, itemValue, cts);
                    if (!succeed)
                    {
                        Log.Error($"[{TraceContext.Name}]--执行动态嵌入内容时发送错误");
                        return (false, null);
                    }

                    Log.Info($"[{TraceContext.Name}]--嵌入内容: \r\n{value}");
                    request = StaticMessage(request, itemKey, value);
                    break;
                case "常量":
                    //直接嵌入常量
                    request = StaticMessage(request, itemKey, itemValue);
                    break;
                case "方法集":
                    var value2 = await MethodMessage(request, itemValue, itemMethodOtherValue);
                    request = StaticMessage(request, itemKey, value2);
                    break;
            }
        }

        return (true, request);
    }

    #endregion

    #region 静态嵌入和动态嵌入内容

    /// <summary>
    /// 嵌入静态内容
    /// </summary>
    /// <param name="request">消息体</param>
    /// <param name="itemKey">填充键</param>
    /// <param name="itemValue">填充值</param>
    /// <returns></returns>
    public string StaticMessage(string request, string itemKey, string itemValue)
    {
        var i = request.IndexOf($"[{itemKey}]");
        if (i != -1)
        {
            var keyLen = itemKey.Length;
            var requestA = request.Substring(0, i);
            var requestB = request.Substring(i + keyLen + 2);
            request = requestA + itemValue + requestB;
        }

        return request;
    }

    /// <summary>
    /// 嵌入静态子内容
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemKey"></param>
    /// <param name="itemKeySon"></param>
    /// <param name="itemValue"></param>
    /// <returns></returns>
    public string StaticMessageSon(string request, string itemKey, string itemKeySon, string itemValue)
    {
        var i = request.IndexOf($"[{itemKey}.{itemKeySon}]");
        if (i != -1)
        {
            var keyLen = itemKey.Length;
            var keysonLen = itemKeySon.Length;

            var sumLen = keyLen + keysonLen;
            var requestA = request.Substring(0, i);
            var requestB = request.Substring(i + sumLen + 3);
            request = requestA + itemValue + requestB;
        }

        return request;
    }

    /// <summary>
    /// 动态嵌入
    /// </summary>
    /// <param name="request">请求体内容</param>
    /// <param name="DynName">动态嵌入的名称</param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public async Task<(bool sueeced, string? result)> DynMessage(string request, string DynName,
        CancellationTokenSource cts)
    {
        if (DynName == null)
        {
            Log.Error($"[{TraceContext.Name}]--正在动态嵌入内容的时候,动态获取名未设置(DynName),无法从GlobalMannager.DynDictionary进行查找 ");
            return (false, null);
        }

        var lookup = GlobalMannager.DynDictionary.Lookup(DynName);
        if (!lookup.HasValue)
        {
            Log.Error($"[{TraceContext.Name}]--正在动态嵌入内容的时候,名为:{DynName},从动态字典GlobalMannager.DynDictionary找不到,返回空字符串");
            return (false, null);
        }

        var mesTcpPojo = lookup.Value;
        var message = mesTcpPojo.Message;
        if (message == null)
        {
            Log.Error($"[{TraceContext.Name}]--从动态字典GlobalMannager.DynDictionary找到的消息内容Message为Null");
            return (false, null);
        }

        var result = "";
        // 1. 循环获取动态条件
        foreach (var item in mesTcpPojo.DynCondition)
        {
            var itemKey = item.Name;
            var methodName = item.MethodName;
            bool isSwitch = item.OpenSwitch;
            bool isVerify = item.OpenVerify;
            bool ResultTranspond = item.ResultTranspond;
            //2. 判断走什么形式的方法进行请求
            if (item.GetMessageType == "通讯")
            {
                //3. 判断需要通过什么获取动态内容
                switch (methodName)
                {
                    case "读寄存器":
                        Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读寄存器中");
                        (bool succeed1, string readReg) = await ReadReg(item);
                        if (!succeed1)
                        {
                            Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读寄存器地址{item.StartAddress}失败");
                            return (false, null);
                        }

                        if (isSwitch)
                        {
                            Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}--进行Switch映射");
                            readReg = SwitchGetMessage(readReg, item);
                        }

                        message = StaticMessage(message, itemKey, readReg);
                        break;
                    case "读线圈":
                        Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读线圈中");
                        (bool succeed2, string readCoid) = await ReadCoid(item);
                        if (!succeed2)
                        {
                            Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读线圈地址{item.StartAddress}失败");
                            return (false, null);
                        }

                        if (isSwitch)
                        {
                            Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}--进行Switch映射");
                            readCoid = SwitchGetMessage(readCoid, item);
                        }

                        message = StaticMessage(message, itemKey, readCoid);
                        break;
                    case "Socket返回":
                        Log.Info($"[{TraceContext.Name}]--动态嵌入内容:执行Socket消息发送");
                        (bool succeed, string tcp) = await ReadTcpMessageAsync(item);
                        //判断
                        if (!succeed)
                        {
                            Log.Error($"[{TraceContext.Name}]--Socket返回发送错误");
                            return (false, null);
                        }

                        //进行switch替换
                        if (isSwitch)
                        {
                            Log.Info($"[{TraceContext.Name}]--Socket需要进行消息转换Switch映射");
                            tcp = SwitchGetMessage(tcp, item);
                        }

                        //进行校验
                        if (isVerify)
                        {
                            Log.Info($"[{TraceContext.Name}]---Socket需要进行消息校验");
                            foreach (var dynVerify in item.VerifyList)
                            {
                                if (!VerityMessage(tcp, dynVerify))
                                {
                                    Log.Error($"[{TraceContext.Name}]--校验到不匹配,撤回发送");
                                    return (false, null);
                                }
                            }
                        }

                        if (ResultTranspond)
                        {
                            Log.Info($"[{TraceContext.Name}]--需要对当前结果进行转发");
                            (bool succeed3, string message1) = await Transpond(item, tcp);
                            if (!succeed3)
                            {
                                Log.Info($"[{TraceContext.Name}]--转发失败");
                                return (false, null);
                            }
                        }

                        message = StaticMessage(message, itemKey, tcp);
                        break;
                    case "读DM寄存器":
                        Log.Info($"[{TraceContext.Name}]--执行读DM寄存器");
                        string readDm = await KeyenceReadDM(item);
                        if (isSwitch)
                        {
                            Log.Info($"[{TraceContext.Name}]--读DM寄存器需要进行消息转换Switch映射");
                            readDm = SwitchGetMessage(readDm, item);
                        }

                        message = StaticMessage(message, itemKey, readDm);
                        break;
                    case "读R线圈状态":
                        Log.Info($"[{TraceContext.Name}]--执行读R线圈状态");
                        break;
                }
            }
            else if (item.GetMessageType == "HTTP")
            {
                //检查是否循环嵌套


                //执行发送HTTP
                (bool succeed, string? response) = await RunOne(item.HttpName, cts);
                //需要判断返回结果一下是否是Json格式
                AppJsonTool<Object>.TryFormatJson(response, out bool isJson);
                JObject jObject;
                if (!isJson)
                {
                    Log.Error($"[{TraceContext.Name}]--返回的response 不是JSON格式");
                    //如果不是JSON对象直接退出
                    return (false, null);
                }

                //将Json转成对象
                jObject = JObject.Parse(response);
                //获取内容
                foreach (var httpObject in item.HttpObjects)
                {
                    string JsonKey = httpObject.JsonKey;
                    //判断是否是自定义的JsonKey
                    string value = RunUserDefined(JsonKey, out bool error, out bool userDefind,
                        out string errorMessage);

                    if (userDefind)
                    {
                        if (error)
                        {
                            Log.Error($"[{TraceContext.Name}]--" + errorMessage);
                            //已经执行完自定义的,需要退出
                            return (false, null);
                        }
                    }


                    //常规执行
                    string jToken;
                    if (userDefind)
                    {
                        jToken = value;
                    }
                    else
                    {
                        jToken = jObject.SelectToken(JsonKey).ToString();
                    }

                    Log.Info($"[{TraceContext.Name}]--解析 {httpObject.JsonKey}:\r\n {jToken}");
                    message = StaticMessageSon(message, itemKey, httpObject.Name, jToken);
                }
            }
        }

        return (true, message);
    }


    #region 消息转发

    /// <summary>
    /// 转发
    /// </summary>
    /// <param name="model"></param>
    /// <param name="response"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string message)> Transpond(DynCondition model, string response)
    {
        switch (model.TranspondModbusDetailed.TranspondMethod)
        {
            case "通讯":
                //通过名字搜索id
                string forwardingName = model.TranspondModbusDetailed.ConnectName;
                //获得网络名
                string netKey = getNetKey(forwardingName);
                //获得网络
                var netWork = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;

                //判断当前转发的通讯是什么类型的
                string networkDetailedNetMethod = netWork.NetworkDetailed.NetMethod;
                switch (networkDetailedNetMethod)
                {
                    case "ModbusTcp":
                        ModbusBase modbusBase = netWork.ModbusBase;
                        List<ushort> list = new List<ushort>();
                        try
                        {
                            for (int i = 0; i < response.Length; i += 2)
                            {
                                char high = response[i];
                                char low = (i + 1 < response.Length) ? response[i + 1] : '\0'; // 补0
                                ushort packed = (ushort)((high << 8) | low);
                                list.Add(packed);
                            }

                            ushort[] result = list.ToArray();

                            await modbusBase.WriteRegisters_10(byte.Parse(model.TranspondModbusDetailed.SlaveAddress),
                                ushort.Parse(model.TranspondModbusDetailed.StartAddress), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行通讯转发时发送错误,:{e}");
                            return (false, null);
                        }

                        break;
                }

                break;
            case "队列":
                break;
        }

        return (true, null);
    }

    #endregion

    /// <summary>
    /// 用户自定义的
    /// </summary>
    /// <param name="JsonKey"></param>
    /// <returns></returns>
    public string RunUserDefined(string JsonKey, out bool error, out bool userDefined, out string ErrorMessage)
    {
        string returnMessage = null;
        //格式  UserDefined:类名:属性
        if (JsonKey.Contains("UserDefined"))
        {
            userDefined = true;
            //1.去掉收尾空格
            string trim = JsonKey.Trim();
            //2.进行分割
            string[] split = trim.Split(':');
            //3.反射类
            //3.1 拼接命名空间
            string className = "Pkn_HostSystem.Service.UserDefined." + split[1];
            //获得示例对象
            Type? userObjectType = Type.GetType(className);
            if (userObjectType != null)
            {
                //创建示例对象
                object? userObject = Activator.CreateInstance(userObjectType);

                //获得方法,并且调用
                MethodInfo? methodInfo = userObjectType.GetMethod("GetPropertyValue");

                //执行方法
                object? invoke = methodInfo?.Invoke(userObject, [split[2]]);
                if (invoke == null)
                {
                    //返回错误
                    error = true;
                    MethodInfo? method = userObjectType.GetMethod("ErrorMessage");
                    try
                    {
                        var message = method?.Invoke(userObject, []);
                        ErrorMessage = message.ToString();
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }
                }
                else
                {
                    error = false;
                    ErrorMessage = String.Empty;
                    returnMessage = invoke.ToString();
                }
            }
        }
        else
        {
            userDefined = false;
        }

        error = false;
        ErrorMessage = String.Empty;
        return returnMessage;
    }

    #endregion

    #region Verity校验方法

    public bool VerityMessage(string message, DynVerify verify)
    {
        switch (verify.Type)
        {
            case "字符长度检测=":
                bool tryParse1 = int.TryParse(verify.Value, out int len1);
                if (!tryParse1)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length == len1)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度不等于{len1}");
                    return false;
                }
            case "字符长度检测!=":
                bool tryParse2 = int.TryParse(verify.Value, out int len2);
                if (!tryParse2)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length != len2)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度={len2}");
                    return false;
                }

            case "字符长度检测>":
                bool tryParse3 = int.TryParse(verify.Value, out int len3);
                if (!tryParse3)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length > len3)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度<={len3}");
                    return false;
                }
            case "字符长度检测<":
                bool tryParse4 = int.TryParse(verify.Value, out int len4);
                if (!tryParse4)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length < len4)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度>={len4}");
                    return false;
                }
            case "字符长度检测>=":
                bool tryParse5 = int.TryParse(verify.Value, out int len5);
                if (!tryParse5)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length >= len5)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度<{len5}");
                    return false;
                }
            case "字符长度检测=<":
                bool tryParse6 = int.TryParse(verify.Value, out int len6);
                if (!tryParse6)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length <= len6)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度>{len6}");
                    return false;
                }

            case "字符=":
                if (message == verify.Value)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,字符不等于 {verify.Value}");
                    return false;
                }
            case "字符!=":
                if (message != verify.Value)
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,字符等于 {verify.Value}");
                    return false;
                }
            case "正则表达式检测":

                if (Regex.IsMatch(message, verify.Type))
                {
                    return true;
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,不符合正则表达式");
                    return false;
                }
        }

        return false;
    }

    #endregion

    #region 方法集内容嵌入

    private async Task<string> MethodMessage(string request, string itemValue, string itemMethodOtherValue)
    {
        if (itemValue == null)
        {
            return null;
        }

        DateTime dateTime = DateTimeDispose(itemMethodOtherValue);
        switch (itemValue)
        {
            case "当前时间(yyyy-MM-dd HH:mm:ss)":
                //判断时间是否需要处理
                return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
            case "当前时间(yyyy/MM/dd HH:mm:ss)":
                return dateTime.ToString("yyyy/MM/dd HH:mm:ss");
            case "当前时间(yyyy-MM-dd)":
                return dateTime.ToString("yyyy-MM-dd");
            case "当前时间(yyyy/MM/dd)":
                return dateTime.ToString("yyyy/MM/dd");
            default:
                return string.Empty;
        }
    }

    //规则-,5M,5D,5H,5m,5s
    public DateTime DateTimeDispose(string itemMethodOtherValue)
    {
        if (itemMethodOtherValue == null)
        {
            return DateTime.Now;
        }

        //分割字符串
        string[] strings = itemMethodOtherValue.Split(",");

        //判断是-,还是+
        //1.1 获取字符的长度
        int length = itemMethodOtherValue.Length;
        //1.2 获取到减号的位置
        int subIndex = itemMethodOtherValue.IndexOf("-");
        //1.3 获取到加号的位置
        int addIndex = itemMethodOtherValue.IndexOf("+");

        DateTime time = DateTime.Now;


        foreach (var s in strings)
        {
            int Mindex = s.IndexOf("M");
            int Dindex = s.IndexOf("D");
            int Hindex = s.IndexOf("H");
            int mindex = s.IndexOf("m");
            int sindex = s.IndexOf("s");

            if (subIndex == 0)
            {
                if (Mindex != -1)
                {
                    string substring = s.Substring(0, Mindex);
                    int.TryParse(substring, out int mResult);
                    time.AddMonths(-mResult);
                }
                else if (Dindex != -1)
                {
                    string substring = s.Substring(0, Dindex);
                    int.TryParse(substring, out int mResult);
                    time = time - TimeSpan.FromDays(mResult);
                }
                else if (Hindex != -1)
                {
                    string substring = s.Substring(0, Hindex);
                    int.TryParse(substring, out int mResult);
                    time = time - TimeSpan.FromHours(mResult);
                }
                else if (mindex != -1)
                {
                    string substring = s.Substring(0, mindex);
                    int.TryParse(substring, out int mResult);
                    time = time - TimeSpan.FromMinutes(mResult);
                }
                else if (sindex != -1)
                {
                    string substring = s.Substring(0, sindex);
                    int.TryParse(substring, out int mResult);
                    time = time - TimeSpan.FromSeconds(mResult);
                }
            }
            else if (addIndex == 0)
            {
                if (Mindex != -1)
                {
                    string substring = s.Substring(0, Mindex);
                    int.TryParse(substring, out int mResult);
                    time.AddMonths(mResult);
                }
                else if (Dindex != -1)
                {
                    string substring = s.Substring(0, Dindex);
                    int.TryParse(substring, out int mResult);
                    time = time + TimeSpan.FromDays(mResult);
                }
                else if (Hindex != -1)
                {
                    string substring = s.Substring(0, Hindex);
                    int.TryParse(substring, out int mResult);
                    time = time + TimeSpan.FromHours(mResult);
                }
                else if (mindex != -1)
                {
                    string substring = s.Substring(0, mindex);
                    int.TryParse(substring, out int mResult);
                    time = time + TimeSpan.FromMinutes(mResult);
                }
                else if (sindex != -1)
                {
                    string substring = s.Substring(0, sindex);
                    int.TryParse(substring, out int mResult);
                    time = time + TimeSpan.FromSeconds(mResult);
                }
            }
        }

        return time;
    }

    #endregion

    #region Switch转换嵌入

    /// <summary>
    /// 通过Switch转换
    /// </summary>
    /// <param name="message"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public string SwitchGetMessage(string message, DynCondition item)
    {
        var dynSwitches = item.SwitchList;

        foreach (DynSwitch dynSwitch in dynSwitches)
        {
            if (dynSwitch.Case == message)
            {
                return dynSwitch.Value;
            }
        }

        return message;
    }

    #endregion

    #region 套接字通讯获取内容

    /// <summary>
    /// Socket套接字
    /// </summary>
    /// <param name="item">动态</param>
    /// <param name="parentName">调用的父类名称,用于日志显示</param>
    /// <returns></returns>
    public async Task<(bool succeed, string response)> ReadTcpMessageAsync(DynCondition item)
    {
        //判断是走客户端发送,还是走服务器发送
        string itemConnectName = item.ConnectName;
        string netMethod = "";

        NetWork curNetWork = null;

        //遍历取出判断当前的网络是什么类型
        foreach (var netWorkPoJo in GlobalMannager.NetWorkDictionary.Items)
        {
            if (netWorkPoJo.NetworkDetailed.Name == itemConnectName)
            {
                netMethod = netWorkPoJo.NetworkDetailed.NetMethod;
                curNetWork = netWorkPoJo;
            }
        }

        if (curNetWork == null)
        {
            Log.Error($"[{TraceContext.Name}]--执行Socket时--遍历获取网络时,未获取到 GlobalMannager.NetWorkDictionary中不存在");
            return (false, null);
        }

        string response = string.Empty;
        TcpTool tcpTool = curNetWork.TcpTool;
        //更具类型选择发送
        switch (netMethod)
        {
            case "Tcp客户端":
                Log.Info($"[{TraceContext.Name}]--执行Tcp客户端消息发送,并等待消息返回");
                (bool succeed, response) = await tcpTool.SendAndWaitClientAsync(item.SocketSendMessage);
                if (!succeed)
                {
                    Log.Error($"[{TraceContext.Name}]--执行Tcp客户端消息发送,等待消息返回时发生错误");
                    return (false, null);
                }

                break;

            case "Tcp服务器":
                await tcpTool.BroadcastAsync(item.SocketSendMessage);
                break;
        }

        return (true, response);
    }

    #endregion

    #region 动态获取Modbus通讯内容

    /// <summary>
    /// 读线圈
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string? result)> ReadCoid(DynCondition item)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netKey = getNetKey(itemConnectName);
        if (netKey == null) return (false, null);
        var netWorkPoJo = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;
        //获得modbus
        var modbusBase = netWorkPoJo.ModbusBase;
        try
        {
            var bools = await modbusBase.ReadCoils_01((byte)item.StationAddress, (ushort)item.StartAddress,
                (ushort)item.EndAddress);

            return (true, string.Join(",", Array.ConvertAll(bools, b => $"{b}")));
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--执行modbus读线圈失败,{e}");
            return (false, null);
        }
    }

    /// <summary>
    /// 读寄存器
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string? result)> ReadReg(DynCondition item)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netKey = getNetKey(itemConnectName);
        if (netKey == null) return (false, null);
        var netWorkPoJo = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;
        //获得modbus
        var modbusBase = netWorkPoJo.ModbusBase;
        var result = "";
        try
        {
            //获得读寄存器值
            var holdingRegisters03 = await modbusBase.ReadHoldingRegisters_03((byte)item.StationAddress,
                (ushort)item.StartAddress, (ushort)item.EndAddress);

            switch (item.BitNet)
            {
                case "单寄存器(无符号)":
                    //用逗号分割
                    result = string.Join(",", Array.ConvertAll(holdingRegisters03, p => $"{p}"));
                    return (true, result);
                case "单寄存器(有符号)":
                    result = string.Join(",",
                        Array.ConvertAll(holdingRegisters03, p => $"{(short)p}"));
                    return (true, result);
                case "双寄存器;无符号;BigEndian":
                    List<uint> uInt32List1 =
                        ModbusDoubleRegisterTool.ToUInt32List(holdingRegisters03, ModbusEndian.BigEndian);
                    return (true, string.Join(",", Array.ConvertAll(uInt32List1.ToArray(), p => $"{p}")));
                case "双寄存器;无符号;LittleEndian":
                    List<uint> uInt32List2 =
                        ModbusDoubleRegisterTool.ToUInt32List(holdingRegisters03, ModbusEndian.LittleEndian);
                    return (true, string.Join(",", Array.ConvertAll(uInt32List2.ToArray(), p => $"{p}")));
                case "双寄存器;无符号;WordSwap":
                    List<uint> uInt32List3 =
                        ModbusDoubleRegisterTool.ToUInt32List(holdingRegisters03, ModbusEndian.WordSwap);
                    return (true, string.Join(",", Array.ConvertAll(uInt32List3.ToArray(), p => $"{p}")));
                case "双寄存器;无符号;ByteSwap":
                    List<uint> uInt32List4 =
                        ModbusDoubleRegisterTool.ToUInt32List(holdingRegisters03, ModbusEndian.ByteSwap);
                    return (true, string.Join(",", Array.ConvertAll(uInt32List4.ToArray(), p => $"{p}")));
                case "双寄存器;有符号;BigEndian":
                    List<int> int32List1 =
                        ModbusDoubleRegisterTool.ToInt32List(holdingRegisters03, ModbusEndian.BigEndian);
                    return (true, string.Join(",", Array.ConvertAll(int32List1.ToArray(), p => $"{p}")));
                case "双寄存器;有符号;LittleEndian":
                    List<int> int32List2 =
                        ModbusDoubleRegisterTool.ToInt32List(holdingRegisters03, ModbusEndian.LittleEndian);
                    return (true, string.Join(",", Array.ConvertAll(int32List2.ToArray(), p => $"{p}")));
                case "双寄存器;有符号;WordSwap":
                    List<int> int32List3 =
                        ModbusDoubleRegisterTool.ToInt32List(holdingRegisters03, ModbusEndian.WordSwap);
                    return (true, string.Join(",", Array.ConvertAll(int32List3.ToArray(), p => $"{p}")));
                case "双寄存器;有符号;ByteSwap":
                    List<int> int32List4 =
                        ModbusDoubleRegisterTool.ToInt32List(holdingRegisters03, ModbusEndian.ByteSwap);
                    return (true, string.Join(",", Array.ConvertAll(int32List4.ToArray(), p => $"{p}")));
                case "32位浮点数;BigEndian":
                    List<float> floatList1 =
                        ModbusDoubleRegisterTool.ToFloatList(holdingRegisters03, ModbusEndian.BigEndian);
                    return (true, string.Join(",", Array.ConvertAll(floatList1.ToArray(), p => $"{p}")));
                case "32位浮点数;LittleEndian":
                    List<float> floatList2 =
                        ModbusDoubleRegisterTool.ToFloatList(holdingRegisters03, ModbusEndian.LittleEndian);
                    return (true, string.Join(",", Array.ConvertAll(floatList2.ToArray(), p => $"{p}")));
                case "32位浮点数;WordSwap":
                    List<float> floatList3 =
                        ModbusDoubleRegisterTool.ToFloatList(holdingRegisters03, ModbusEndian.WordSwap);
                    return (true, string.Join(",", Array.ConvertAll(floatList3.ToArray(), p => $"{p}")));
                case "32位浮点数;ByteSwap":
                    List<float> floatList4 =
                        ModbusDoubleRegisterTool.ToFloatList(holdingRegisters03, ModbusEndian.ByteSwap);
                    return (true, string.Join(",", Array.ConvertAll(floatList4.ToArray(), p => $"{p}")));
                case "ASCII字符串(低高位)":
                    var result_3 = new List<byte>();
                    foreach (var itemUshort in holdingRegisters03)
                    {
                        //转成16进制
                        var value = itemUshort.ToString("x4");
                        //从2索引截取到结尾
                        var low = value.Substring(2);
                        var high = value.Substring(0, 2);
                        var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                        var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                        //低位在前
                        result_3.Add(ByteLow);
                        result_3.Add(ByteHigh);
                    }

                    //输出ASCII码转换后的结果
                    return (true, Encoding.ASCII.GetString(result_3.ToArray()));
                case "ASCII字符串(高低位)":
                    var result_4 = new List<byte>();
                    foreach (var itemUshort in holdingRegisters03)
                    {
                        //转成16进制
                        var value = itemUshort.ToString("x4");
                        //从2索引截取到结尾
                        var high = value.Substring(2);
                        var low = value.Substring(0, 2);
                        var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                        var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                        //低位在前
                        result_4.Add(ByteLow);
                        result_4.Add(ByteHigh);
                    }

                    //输出ASCII码转换后的结果
                    return (true, Encoding.ASCII.GetString(result_4.ToArray()));
            }
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--执行Modbus读寄存器失败,错误:{e}");
            return (false, null);
        }

        return (false, result);
    }

    #endregion

    #region 动态获取基恩士上链路内容

    public async Task<string> KeyenceReadDM(DynCondition item)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netKey = getNetKey(itemConnectName);
        if (netKey == null) return null;
        var netWorkPoJo = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;
        //获得keyenceHostLinkTool
        KeyenceHostLinkTool keyenceHostLinkTool = netWorkPoJo.KeyenceHostLinkTool;
        var result = "";
        int startAddress = item.StartAddress;
        int num = item.EndAddress;
        try
        {
            switch (item.BitNet)
            {
                case "单寄存器(无符号)":
                    ushort readDm1 = keyenceHostLinkTool.ReadDM<ushort>(startAddress);
                    return readDm1.ToString();
                case "单寄存器(有符号)":
                    short readDm2 = keyenceHostLinkTool.ReadDM<short>(startAddress);
                    return readDm2.ToString();
                case "双寄存器(无符号)":
                    uint readDm3 = keyenceHostLinkTool.ReadDM<uint>(startAddress);
                    return readDm3.ToString();
                case "双寄存器(有符号)":
                    int readDm4 = keyenceHostLinkTool.ReadDM<int>(startAddress);
                    return readDm4.ToString();
                case "32位浮点数":
                    float readDm5 = keyenceHostLinkTool.ReadDM<float>(startAddress);
                    return readDm5.ToString();
                case "ASCII字符串":
                    ushort[] readDmWords = keyenceHostLinkTool.ReadDMWords(startAddress, num);

                    var result_4 = new List<byte>();
                    foreach (var itemUshort in readDmWords)
                    {
                        //转成16进制
                        var value = itemUshort.ToString("x4");
                        //从2索引截取到结尾
                        var high = value.Substring(2);
                        var low = value.Substring(0, 2);
                        var ByteLow = byte.Parse(low, NumberStyles.HexNumber);
                        var ByteHigh = byte.Parse(high, NumberStyles.HexNumber);

                        //低位在前
                        result_4.Add(ByteLow);
                        result_4.Add(ByteHigh);
                    }

                    //输出ASCII码转换后的结果
                    return Encoding.ASCII.GetString(result_4.ToArray());
            }
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--基恩士上链路读取DM失败 :{e}");
        }

        return result;
    }

    #endregion
}