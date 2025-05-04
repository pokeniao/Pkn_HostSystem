using CommunityToolkit.Mvvm.DependencyInjection;
using KeyenceTool;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.Service.UserDefined;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using RestSharp;
using System.CodeDom;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

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
    public async Task<string> RunOne(string Name, CancellationTokenSource cts)
    {
        Log.Info($"RunOne执行,Name:{Name}");
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = SelectByName(Name);
        //得到消息体
        return await SendHttp(item, cts);
    }

    /// <summary>
    /// 全部触发
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RunAll()
    {
        Log.Info($"RunAll执行");
        foreach (var item in mesPojoList)
        {
            await SendHttp(item, new CancellationTokenSource());
        }

        return true;
    }

    #endregion

    #region 发送Http任务

    public async Task<string> SendHttp(LoadMesAddAndUpdateWindowModel item, CancellationTokenSource cts)
    {
        //得到消息体
        var request = await PackRequest(item.Name, cts);
        //日志显示发送内容
        Log.Info($"{nameof(LoadMesService)}--SendHttp--{item.Name}--发送内容: \r\n {request}");
        if (request != null)
        {
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

                item.Response = TryFormatJson(item.Response);
                Log.Info($"返回消息response结果为成功,状态码:{response.StatusCode}");
                Log.Info($"{item.Name}--发送请求返回:\r\n {item.Response}");
                return item.Response;
            }
            else
            {
                item.Response = response.ErrorMessage;
                if (item.Response == null)
                {
                    item.Response = response.Content;
                }

                item.Response = TryFormatJson(item.Response);

                Log.Info($"返回消息response结果为失败,状态码:{response.StatusCode}");
                Log.Info($"{item.Name}--发送请求返回:\r\n {item.Response}");
                return item.Response;
            }
        }

        return null;
    }

    public static string TryFormatJson(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return response;

        try
        {
            var trimmed = response.Trim();
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                var obj = JsonConvert.DeserializeObject<object>(trimmed);
                return JsonConvert.SerializeObject(obj, Formatting.Indented);
            }
        }
        catch
        {
            // 非合法 JSON，忽略
        }

        return response;
    }

    #endregion

    #region 封装消息请求体方法

    /// <summary>
    /// 包装Request请求
    /// </summary>
    /// <param name="name"></param>
    public async Task<string> PackRequest(string name, CancellationTokenSource cts)
    {
        //获得当前行的数据
        var loadMesAddAndUpdateWindowModel = SelectByName(name);
        if (loadMesAddAndUpdateWindowModel == null) return null;

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
                    Log.Info("正在动态嵌入内容");
                    var value = await DynMessage(request, itemValue, cts);
                    if (value == null)
                    {
                        return null;
                    }

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

        return request;
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
    /// 动态嵌入内容
    /// </summary>
    /// <param name="request">整个消息体</param>
    /// <param name="DynName">嵌入名</param>
    /// <returns></returns>
    public async Task<string> DynMessage(string request, string DynName, CancellationTokenSource cts)
    {
        if (DynName == null)
        {
            Log.Info($"{nameof(LoadMesService)}--正在动态嵌入内容的时候,动态获取名未设置(DynName) ");
            return null;
        }

        var lookup = GlobalMannager.DynDictionary.Lookup(DynName);
        if (!lookup.HasValue)
        {
            Log.Info($"{nameof(LoadMesService)}--正在动态嵌入内容的时候,--{DynName}--从动态字典DynDictionary找不到,返回空字符串");
            return string.Empty;
        }

        var mesTcpPojo = lookup.Value;
        var message = mesTcpPojo.Message;
        if (message == null)
        {
            Log.Info($"{nameof(LoadMesService)} 从动态字典DynDictionary找到的消息内容Message为Null");
            return string.Empty;
        }

        var result = "";
        // 1. 循环获取动态条件
        foreach (var item in mesTcpPojo.DynCondition)
        {
            var itemKey = item.Name;
            var methodName = item.MethodName;
            bool isSwitch = item.OpenSwitch;
            bool isVerify = item.OpenVerify;
            //2. 判断走什么形式的方法进行请求
            if (item.GetMessageType == "通讯")
            {
                //3. 判断需要通过什么获取动态内容
                switch (methodName)
                {
                    case "读寄存器":
                        Log.Info("动态嵌入内容:执行读寄存器中");
                        string readReg = await ReadReg(item);
                        if (isSwitch)
                        {
                            Log.Info("动态嵌入内容读寄存器需要进行消息转换Switch映射");
                            readReg = SwitchGetMessage(readReg, item);
                        }

                        message = StaticMessage(message, itemKey, readReg);
                        break;
                    case "读线圈":
                        Log.Info("动态嵌入内容:执行读线圈中");
                        string readCoid = await ReadCoid(item);
                        if (isSwitch)
                        {
                            Log.Info("动态嵌入内容读线圈需要进行消息转换Switch映射");
                            readCoid = SwitchGetMessage(readCoid, item);
                        }

                        message = StaticMessage(message, itemKey, readCoid);
                        break;
                    case "Socket返回":
                        Log.Info("动态嵌入内容:执行Socket消息发送");
                        string tcp = await ReadTcpMessageAsync(item);
                        //判断
                        if (tcp == null && item.DynFailReturnFail == true)
                        {
                            return null;
                        }

                        if (isSwitch)
                        {
                            Log.Info("动态嵌入内容Socket需要进行消息转换Switch映射");
                            tcp = SwitchGetMessage(tcp, item);
                        }

                        if (isVerify)
                        {
                            Log.Info("动态嵌入内容Socket需要进行消息校验");
                            foreach (var dynVerify in item.VerifyList)
                            {
                                if (!VerityMessage(tcp, dynVerify))
                                {
                                    Log.Info("校验到不匹配,撤回发送");
                                    return null;
                                }
                            }
                        }

                        message = StaticMessage(message, itemKey, tcp);
                        break;
                    case "读DM寄存器":
                        Log.Info("动态嵌入内容:执行读DM寄存器");
                        string readDm = await KeyenceReadDM(item);
                        if (isSwitch)
                        {
                            Log.Info("动态嵌入内容读DM寄存器需要进行消息转换Switch映射");
                            readDm = SwitchGetMessage(readDm, item);
                        }

                        message = StaticMessage(message, itemKey, readDm);
                        break;
                    case "读R线圈状态":
                        Log.Info("动态嵌入内容:执行读R线圈状态");
                        break;
                }
            }
            else if (item.GetMessageType == "HTTP")
            {
                var loadMesPage = Ioc.Default.GetRequiredService<LoadMesPage>();

                string response = await RunOne(item.HttpName, cts);
                JObject jObject = JObject.Parse(response);
                //获取内容
                foreach (var httpObject in item.HttpObjects)
                {
                    string JsonKey = httpObject.JsonKey;
                    //判断是否是自定义的JsonKey
                    var userDefined = RunUserDefined(JsonKey,out bool isReturn);
                    if (isReturn == true)
                    {
                        Log.Info("未选择工单!请选择工单后操作");
                        return null;
                    }

                    string jToken = null;
                    if (userDefined != null)
                    {
                        jToken = userDefined;
                    }
                    else
                    {
                         jToken = jObject.SelectToken(JsonKey).ToString();
                    }
                    Log.Info($"解析 {httpObject.JsonKey}:\r\n {jToken}");
                    message = StaticMessageSon(message, itemKey, httpObject.Name, jToken);
                }
            }
        }
        return message;
    }
    /// <summary>
    /// 用户自定义的
    /// </summary>
    /// <param name="JsonKey"></param>
    /// <returns></returns>
    public string RunUserDefined(string JsonKey ,out bool noReturn)
    {
        switch (JsonKey)
        {
            case "byd:Base003_OrderList:scheduleCode":
                string scheduleCode = new BydBase003OrderList().DynCurrentOrder("scheduleCode");
                if (scheduleCode == null)
                {
                    noReturn = true;
                }
                else
                {
                    noReturn = false;

                }
                return scheduleCode;
            case "byd:Base003_OrderList:orderCode":
                string orderCode = new BydBase003OrderList().DynCurrentOrder("orderCode");
                if (orderCode == null)
                {
                    noReturn = true;
                }
                else
                {
                    noReturn = false;
                }
                return orderCode;
            default:
                noReturn = false;
                return null;
        }
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
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length == len1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "字符长度检测!=":
                bool tryParse2 = int.TryParse(verify.Value, out int len2);
                if (!tryParse2)
                {
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length != len2)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case "字符长度检测>":
                bool tryParse3 = int.TryParse(verify.Value, out int len3);
                if (!tryParse3)
                {
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length > len3)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "字符长度检测<":
                bool tryParse4 = int.TryParse(verify.Value, out int len4);
                if (!tryParse4)
                {
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length < len4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "字符长度检测>=":
                bool tryParse5 = int.TryParse(verify.Value, out int len5);
                if (!tryParse5)
                {
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length >= len5)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "字符长度检测=<":
                bool tryParse6 = int.TryParse(verify.Value, out int len6);
                if (!tryParse6)
                {
                    Log.Info($"{nameof(LoadMesService)}--VerityMessage--在检测字符串的时候转换Int失败,检测填入的内容");
                    return false;
                }

                if (message.Length <= len6)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            case "字符=":
                if (message == verify.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "字符!=":
                if (message != verify.Value)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "正则表达式检测":

                if (Regex.IsMatch(message, verify.Type))
                {
                    return true;
                }
                else
                {
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
    /// 读tcp消息
    /// </summary>
    /// <param name="item"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task<string> ReadTcpMessageAsync(DynCondition item)
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

        string response = string.Empty;
        TcpTool tcpTool = curNetWork.TcpTool;
        //更具类型选择发送
        switch (netMethod)
        {
            case "Tcp客户端":
                Log.Info("执行Tcp客户端消息发送,并等待消息返回");
                response = await tcpTool.SendAndWaitClientAsync(item.SocketSendMessage);
                if (response == null && item.DynFailReturnFail == true)
                {
                    return null;
                }

                break;

            case "Tcp服务器":
                await tcpTool.BroadcastAsync(item.SocketSendMessage);
                break;
        }

        return response;
    }

    #endregion

    #region 动态获取Modbus通讯内容

    /// <summary>
    /// 读线圈
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<string> ReadCoid(DynCondition item)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netKey = getNetKey(itemConnectName);
        if (netKey == null) return null;
        var netWorkPoJo = GlobalMannager.NetWorkDictionary.Lookup(netKey).Value;
        //获得modbus
        var modbusBase = netWorkPoJo.ModbusBase;
        try
        {
            var bools = await modbusBase.ReadCoils_01((byte)item.StationAddress, (ushort)item.StartAddress,
                (ushort)item.EndAddress);

            return string.Join(",", Array.ConvertAll(bools, b => $"{b}"));
        }
        catch (Exception e)
        {
            throw;
        }
    }

    /// <summary>
    /// 读寄存器
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<string> ReadReg(DynCondition item)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netKey = getNetKey(itemConnectName);
        if (netKey == null) return null;
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
                    return result;
                case "单寄存器(有符号)":
                    result = string.Join(",",
                        Array.ConvertAll(holdingRegisters03,
                            p => $"{ModbusDataConverter.ConvertFromResponse<short>(p.ToString())}"));
                    return result;
                case "双寄存器;无符号;BigEndian":
                    List<uint> uInt32List1 =
                        ModbusDoubleRegisterConverter.ToUInt32List(holdingRegisters03, ModbusEndian.BigEndian);
                    return string.Join(",", Array.ConvertAll(uInt32List1.ToArray(), p => $"{p}"));
                case "双寄存器;无符号;LittleEndian":
                    List<uint> uInt32List2 =
                        ModbusDoubleRegisterConverter.ToUInt32List(holdingRegisters03, ModbusEndian.LittleEndian);
                    return string.Join(",", Array.ConvertAll(uInt32List2.ToArray(), p => $"{p}"));
                case "双寄存器;无符号;WordSwap":
                    List<uint> uInt32List3 =
                        ModbusDoubleRegisterConverter.ToUInt32List(holdingRegisters03, ModbusEndian.WordSwap);
                    return string.Join(",", Array.ConvertAll(uInt32List3.ToArray(), p => $"{p}"));
                case "双寄存器;无符号;ByteSwap":
                    List<uint> uInt32List4 =
                        ModbusDoubleRegisterConverter.ToUInt32List(holdingRegisters03, ModbusEndian.ByteSwap);
                    return string.Join(",", Array.ConvertAll(uInt32List4.ToArray(), p => $"{p}"));
                case "双寄存器;有符号;BigEndian":
                    List<int> int32List1 =
                        ModbusDoubleRegisterConverter.ToInt32List(holdingRegisters03, ModbusEndian.BigEndian);
                    return string.Join(",", Array.ConvertAll(int32List1.ToArray(), p => $"{p}"));
                case "双寄存器;有符号;LittleEndian":
                    List<int> int32List2 =
                        ModbusDoubleRegisterConverter.ToInt32List(holdingRegisters03, ModbusEndian.LittleEndian);
                    return string.Join(",", Array.ConvertAll(int32List2.ToArray(), p => $"{p}"));
                case "双寄存器;有符号;WordSwap":
                    List<int> int32List3 =
                        ModbusDoubleRegisterConverter.ToInt32List(holdingRegisters03, ModbusEndian.WordSwap);
                    return string.Join(",", Array.ConvertAll(int32List3.ToArray(), p => $"{p}"));
                case "双寄存器;有符号;ByteSwap":
                    List<int> int32List4 =
                        ModbusDoubleRegisterConverter.ToInt32List(holdingRegisters03, ModbusEndian.ByteSwap);
                    return string.Join(",", Array.ConvertAll(int32List4.ToArray(), p => $"{p}"));
                case "32位浮点数;BigEndian":
                    List<float> floatList1 =
                        ModbusDoubleRegisterConverter.ToFloatList(holdingRegisters03, ModbusEndian.BigEndian);
                    return string.Join(",", Array.ConvertAll(floatList1.ToArray(), p => $"{p}"));
                case "32位浮点数;LittleEndian":
                    List<float> floatList2 =
                        ModbusDoubleRegisterConverter.ToFloatList(holdingRegisters03, ModbusEndian.LittleEndian);
                    return string.Join(",", Array.ConvertAll(floatList2.ToArray(), p => $"{p}"));
                case "32位浮点数;WordSwap":
                    List<float> floatList3 =
                        ModbusDoubleRegisterConverter.ToFloatList(holdingRegisters03, ModbusEndian.WordSwap);
                    return string.Join(",", Array.ConvertAll(floatList3.ToArray(), p => $"{p}"));
                case "32位浮点数;ByteSwap":
                    List<float> floatList4 =
                        ModbusDoubleRegisterConverter.ToFloatList(holdingRegisters03, ModbusEndian.ByteSwap);
                    return string.Join(",", Array.ConvertAll(floatList4.ToArray(), p => $"{p}"));
                case "ASCII字符串":
                    var result_3 = new List<byte>();
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
                        result_3.Add(ByteLow);
                        result_3.Add(ByteHigh);
                    }

                    //输出ASCII码转换后的结果
                    return Encoding.ASCII.GetString(result_3.ToArray());
            }
        }
        catch (Exception e)
        {
            Log.Error("执行失败,TCP或RTU未连接上");
        }

        return result;
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
            Log.Error("读取失败");
        }

        return result;
    }

    #endregion
}