using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes.Interface;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.ViewModels.Page;
using RestSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Pkn_HostSystem.Service.LoadMes;

public class LoadMesService : ILoadMesService
{


    public LogControl<LoadMesService> Log { get; set; }


    public ILoadMesService _self { get; set; }

    public LoadMesService()
    {
        _self = this;
        Log = new LogControl<LoadMesService>();
    }


    #region 触发Http请求

    /// <summary>
    ///  触发单个请求
    /// </summary>
    /// <param name="Name">HTTP请求名称</param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public  async Task<(bool succeed, string? response)> RunOne(string Name, CancellationTokenSource cts)
    {
        Log.Info($"[{TraceContext.Name}]--执行发送一次Http请求");
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = GlobalManager.ProcessTask.Lookup(Name).Value;
        //得到消息体
        var (succeed, request) = await _self.PackRequest(item?.Name, cts);
        if (!succeed)
        {
            Log.Error($"[{TraceContext.Name}]--执行发送HTTP任务,消息体组装失败");
            return (false, null);
        }

        //得到消息体
        return await _self.SendHttp(item, request, cts);
    }

    /// <summary>
    /// 触发单个请求
    /// </summary>
    /// <param name="Name">HTTP请求名称</param>
    /// <param name="request">请求体</param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public  async Task<(bool succeed, string? response)> RunOne(string Name, string request,
        CancellationTokenSource cts)
    {
        Log.Info($"[{TraceContext.Name}]--执行发送一次Http请求");
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = GlobalManager.ProcessTask.Lookup(Name).Value;
        //得到消息体
        return await _self.SendHttp(item, request, cts);
    }

    #endregion

    #region 发送Http任务

    /// <summary>
    /// 发送Http任务
    /// </summary>
    /// <param name="item"></param>
    /// <param name="request"></param>
    /// <param name="cts"></param>
    /// <returns></returns>
    public  async Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item,
        string request,
        CancellationTokenSource cts)
    {
        //创建连接
        var client = new RestClient(item.HttpPath);
        RestRequest requestBody;
        //创建请求
        switch (item.Ajax)
        {
            case "POST":
                //日志显示发送内容
                Log.Info($"[{TraceContext.Name}]--发送POST请求,内容: \r\n {request}");

                requestBody = new RestRequest(item.Api, Method.Post);
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

                break;
            case "GET":
                //检查路径是否需要嵌入内容
                //获得数据
                var loadMesAddAndUpdateWindowModel = GlobalManager.ProcessTask.Lookup(item.Name).Value;
                if (loadMesAddAndUpdateWindowModel == null) return (false, null);
                //获得当前行的条件  将   ObservableCollection<> 转成List
                var conditionItems = Enumerable.ToList<LoadMesCondition>(loadMesAddAndUpdateWindowModel.Condition);
                //获取请求体
                var Api = item.Api;
                //对请求体进行嵌入内容
                //遍历当前条件,判断条件方式
                foreach (var c in conditionItems)
                {
                    var itemKey = c.Key;
                    var itemValue = c.Value;
                    var itemMethodOtherValue = c.Method_OtherValue;

                    //检查是否存在
                    var i = Api.IndexOf($"[{itemKey}]");

                    if (i == -1)
                    {
                        continue;
                    }

                    switch (c.Method)
                    {
                        case "动态获取":
                            //获取动态的值
                            Log.Info($"[{TraceContext.Name}]--正在动态嵌入内容");
                            var (succeed, value) = await _self.DynMessage(Api, itemValue, cts);
                            if (!succeed)
                            {
                                Log.Error($"[{TraceContext.Name}]--执行动态嵌入内容时发送错误:{value}");
                                return (false, null);
                            }

                            Log.Info($"[{TraceContext.Name}]--嵌入内容: \r\n{value}");
                            Api = _self.StaticMessage(Api, itemKey, value);
                            break;
                        case "常量":
                            //直接嵌入常量
                            Api = _self.StaticMessage(Api, itemKey, itemValue);
                            break;
                        case "方法集":
                            var value2 = await _self.MethodMessage(Api, itemValue, itemMethodOtherValue);
                            Api = _self.StaticMessage(Api, itemKey, value2);
                            break;
                    }
                }

                //日志显示发送内容
                Log.Info($"[{TraceContext.Name}]--发送GET请求,路径:{Api}");
                requestBody = new RestRequest(Api, Method.Get);
                break;
            case "DELETE":
                requestBody = new RestRequest(item.Api, Method.Delete);
                break;
            case "PUT":

                //日志显示发送内容
                Log.Info($"[{TraceContext.Name}]--发送PUT请求,内容: \r\n {request}");
                requestBody = new RestRequest(item.Api, Method.Put);
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

        //发送请求
        RestResponse response = await client.ExecuteAsync(requestBody, cts.Token);
        //判断
        if (response.IsSuccessStatusCode)
        {
            item.Response = response.Content;
            //判断是否是JSON格式,如果是转成输出
            item.Response = JsonTool<Object>.TryFormatJson(item.Response, out bool isJson);
            Log.Info($"[{TraceContext.Name}]--返回消息--成功--状态码:{response.StatusCode}--消息体:\r\n{item.Response}");


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

            //判断是否是JSON格式,如果是转成输出item.Response = JsonTool<Object>.TryFormatJson(item.Response, out bool isJson);

            Log.Error($"[{TraceContext.Name}]--返回消息--失败--状态码:{response.StatusCode}--消息体:\r\n{item.Response}");


            return (false, item.Response);
        }
    }

    #endregion

    #region 封装消息请求体方法

    /// <summary>
    /// 包装Request请求
    /// </summary>
    /// <param httpName="httpName"></param>
    public  async Task<(bool succeed, string? value)> PackRequest(string httpName, CancellationTokenSource cts)
    {
        //获得当前行的数据
        var loadMesAddAndUpdateWindowModel = GlobalManager.ProcessTask.Lookup(httpName).Value;
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

            //检查是否存在
            var i = request.IndexOf($"[{itemKey}]");

            if (i == -1)
            {
                continue;
            }


            switch (item.Method)
            {
                case "动态获取":
                    //获取动态的值
                    Log.Info($"[{TraceContext.Name}]--正在动态嵌入内容");
                    var (succeed, value) = await _self.DynMessage(itemValue, cts);
                    if (!succeed)
                    {
                        Log.Error($"[{TraceContext.Name}]--执行动态嵌入内容时发送错误:{value}");
                        return (false, null);
                    }

                    Log.Info($"[{TraceContext.Name}]--嵌入内容: \r\n{value}");
                    request = _self.StaticMessage(request, itemKey, value);
                    break;
                case "常量":
                    //直接嵌入常量
                    request = _self.StaticMessage(request, itemKey, itemValue);
                    break;
                case "方法集":
                    var value2 = await _self.MethodMessage(request, itemValue, itemMethodOtherValue);
                    request = _self.StaticMessage(request, itemKey, value2);
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
    public  string StaticMessage(string request, string itemKey, string itemValue)
    {
        var i = request.IndexOf($"[{itemKey}]");

        string messageBefore = request;
        if (i != -1)
        {
            var keyLen = itemKey.Length;
            var requestA = request.Substring(0, i);
            var requestB = request.Substring(i + keyLen + 2);
            request = requestA + itemValue + requestB;
        }

        //防止堆栈溢出,重复嵌套调用
        if (request == messageBefore)
        {
            Log.Error($"[{TraceContext.Name}]--进行嵌入后,前后一样,避免循环嵌套堆栈溢出,退出嵌套");
            return request;
        }


        return request.IndexOf($"[{itemKey}]") != -1 ? StaticMessage(request, itemKey, itemValue) : request;
    }

    /// <summary>
    /// 嵌入静态子内容
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemKey"></param>
    /// <param name="itemKeySon"></param>
    /// <param name="itemValue"></param>
    /// <returns></returns>
    public  string StaticMessageSon(string request, string itemKey, string itemKeySon, string itemValue)
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
        CancellationTokenSource cts,bool noLog = false)
    {
        if (DynName == null)
        {
            Log.Error($"[{TraceContext.Name}]--正在动态嵌入内容的时候,动态获取名未设置(DynName),无法从GlobalMannager.DynDictionary进行查找 ");
            return (false, null);
        }

        var lookup = GlobalManager.DynDictionary.Lookup(DynName);
        if (!lookup.HasValue)
        {
            Log.Error($"[{TraceContext.Name}]--正在动态嵌入内容的时候,名为:{DynName},从动态字典GlobalMannager.DynDictionary找不到,返回空字符串");
            return (false, null);
        }

        var mesTcpPojo = lookup.Value;
        var message = request;
        if (message == null)
        {
            Log.Error($"[{TraceContext.Name}]--从动态字典GlobalMannager.DynDictionary找到的消息内容Message为Null");
            return (false, null);
        }

        //通过正则表达式匹配对应数量的[]格式的字符
        MatchCollection matches = Regex.Matches(message, @"\[.*?\]");
        foreach (Match match in matches)
        {
            foreach (var item in mesTcpPojo.DynCondition)
            {
                var itemKey = item.Name;
                var methodName = item.MethodName;
                //检查是否存在
                var i = match.Value.IndexOf($"[{itemKey}]");

                if (i == -1)
                {
                    continue;
                }

                //2. 判断走什么形式的方法进行请求
                if (item.GetMessageType == "通讯")
                {
                    //3. 判断需要通过什么获取动态内容
                    switch (methodName)
                    {
                        case "读寄存器":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读寄存器中");
                            }
                            (bool succeed1, string readReg) = await _self.ReadReg(item);
                            if (!succeed1)
                            {

                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读寄存器地址{item.StartAddress}失败");
                                return (false, null);
                            }

                            (bool b1, string? responseLateProcess1) = await _self.LateProcess(item, readReg, cts);
                            if (b1)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess1);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess1}");
                                return (false, null);
                            }

                            break;
                        case "读线圈":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读线圈中");
                            }
                   
                            (bool succeed2, string readCoid) = await _self.ReadCoid(item);
                            if (!succeed2)
                            {
                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读线圈地址{item.StartAddress}失败");
                                return (false, null);
                            }

                            (bool b2, string? responseLateProcess2) = await _self.LateProcess(item, readCoid, cts);

                            if (b2)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess2);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess2}");
                                return (false, null);
                            }

                            break;
                        case "Socket返回":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--动态嵌入内容:执行Socket消息发送");
                            }
                            (bool succeed, string tcp) = await _self.ReadTcpMessageAsync(item, cts);
                            //判断
                            if (!succeed)
                            {
                                Log.Error($"[{TraceContext.Name}]--Socket返回发送错误");
                                return (false, null);
                            }

                            (bool b3, string? responseLateProcess3) = await _self.LateProcess(item, tcp, cts);
                            if (b3)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess3);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess3}");
                                return (false, null);
                            }

                            break;
                        case "读DM寄存器":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--执行读DM寄存器");
                            }
                    
                            (bool succeed4, string? s) = await _self.KeyenceReadDM(item, cts);

                            if (!succeed4)
                            {
                                return (false, null);
                            }

                            (bool b4, string? responseLateProcess4) = await _self.LateProcess(item, s, cts);
                            if (b4)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess4);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess4}");
                                return (false, null);
                            }

                            break;
                        case "读R线圈状态":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--执行读R线圈状态");
                            }
                            (bool succeed3, string? result) = await _self.KeyenceReadCoid(item, cts);
                            if (!succeed3)
                            {
                                return (false, null);
                            }
                            (bool b6, string? responseLateProcess6) = await _self.LateProcess(item, result, cts);
                            if (b6)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess6);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess6}");
                                return (false, null);
                            }


                            break;
                        case "串口通讯":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行串口发送中");
                            }
                        
                            (bool b, string? response) = await _self.ScpiSerialAsync(item, cts);
                            if (!b)
                            {
                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--串口发送返回失败");
                                return (false, null);
                            }

                            (bool b5, string? responseLateProcess5) = await _self.LateProcess(item, response, cts);

                            if (b5)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess5);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess5}");
                                return (false, null);
                            }

                            break;
                    }
                }
                else if (item.GetMessageType == "HTTP")
                {
                    //检查是否循环嵌套

                    //获取到当前的HTTP

                    LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
                    var loadMesAddAndUpdateWindowModels = loadMesPageViewModel.LoadMesPageModel.MesPojoList;
                    LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel = null;
                    foreach (var model in loadMesAddAndUpdateWindowModels)
                    {
                        if (model.Name == item.HttpName)
                        {
                            loadMesAddAndUpdateWindowModel = model;
                        }
                    }

                    if (loadMesAddAndUpdateWindowModel == null)
                    {
                        return (false, $"[{TraceContext.Name}]--获取HTTP子程序的时候,程序为空!");
                    }

                    loadMesAddAndUpdateWindowModel.cts = cts;
                    loadMesAddAndUpdateWindowModel.LoadMesService = this;

                    //触发HTTP返回结果
                    (bool succeed, string? s) =
                        await loadMesPageViewModel.ExecutionCondition(loadMesAddAndUpdateWindowModel);
                    if (!succeed)
                    {
                        return (false, null);
                    }


                    //是否定义返回结果
                    if (item.NeedInteriorTriggerUserSetReturn)
                    {
                        string InteriorMessage = item.InteriorTriggerReturnMessage.Value;

                        foreach (var httpObject in item.HttpObjects)
                        {
                            int indexOf = InteriorMessage.IndexOf($"[{httpObject.Name}]");
                            if (indexOf == -1)
                            {
                                continue;
                            }

                            string res = null;
                            switch (httpObject.Method)
                            {
                                case "常量":
                                    res = httpObject.staticParam;
                                    break;
                                case "结果Json解析":
                                    //判断是否为字符串格式
                                    JsonTool<object>.TryFormatJson(s, out bool isJson);
                                    if (isJson)
                                    {
                                        JObject jObject = JObject.Parse(s);
                                        if (httpObject.JsonParam != null)
                                        {
                                            res = jObject.SelectToken($"{httpObject.JsonParam}")?.ToString();

                                            if (res == null)
                                            {
                                                Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析到结果为NULL");
                                                return (false,
                                                    $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析到结果为NULL");
                                            }
                                        }
                                        else
                                        {
                                            Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析路径参数为NULL");
                                            return (false,
                                                $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析路径参数为NULL");
                                        }
                                    }
                                    else
                                    {
                                        Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,返回的不是Json字符串");
                                        return (false, $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,返回的不是Json字符串");
                                    }

                                    break;
                                case "方法集":
                                    res = String.Empty;
                                    break;
                            }

                            int length = httpObject.Name.Length;
                            var requestA = InteriorMessage.Substring(0, indexOf);
                            var requestB = InteriorMessage.Substring(indexOf + length + 2);
                            InteriorMessage = requestA + res + requestB;
                        }

                        s = InteriorMessage;
                    }

                    //是否转发
                    (bool b1, string? responseLateProcess1) = await _self.LateProcess(item, s, cts);
                    if (b1)
                    {
                        message = _self.StaticMessage(message, itemKey,
                            item.InteriorTriggerReturn ? responseLateProcess1 : String.Empty);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess1}");
                        return (false, null);
                    }
                }
                else if (item.GetMessageType == "自定义")
                {
                    Type userDefined = item.UserDefined;
                    //实例化
                    var objInstance = Activator.CreateInstance(userDefined);
                    //获取方法
                    var method = userDefined.GetMethod("Main");

                    //执行方法
                    var invoke = method.Invoke(objInstance, [cts]);

                    // 转换为具体元组类型
                    var (succeed, returnValue) = await (Task<(bool Succeed, object Return)>)invoke;

                    if (succeed)
                    {
                        //静态嵌入
                        message = _self.StaticMessage(message, itemKey, returnValue.ToString());
                    }
                    else
                    {
                        return (false, returnValue?.ToString());
                    }
                }
                else if (item.GetMessageType == "内部")
                {
                    //判断是集合还是队列

                    switch (item.MethodName)
                    {
                        case "读取(集合)":
                            message = _self.StaticMessage(message, itemKey,
                                Volatile.Read(ref GlobalManager.ArrayRegister[item.InteriorArrayIndex])?.ToString());
                            break;
                        case "读取(队列)":
                            bool tryPeek = GlobalManager.QueueRegister[item.InteriorQueueIndex]
                                .TryDequeue(out object a);

                            if (!tryPeek)
                            {
                                Log.Error($"[{TraceContext.Name}]--在取出队列中元素时失败");
                                return (false, null);
                            }

                            if (a == null)
                            {
                                Log.Error($"[{TraceContext.Name}]--在取出队列中元素时为null");
                                return (false, null);
                            }

                            message = _self.StaticMessage(message, itemKey, a?.ToString());
                            break;
                        case "写入(集合)":
                            message = _self.StaticMessage(message, itemKey, item.InteriorWriteMessage);
                            Volatile.Write(ref GlobalManager.ArrayRegister[item.InteriorArrayIndex],
                                item.InteriorWriteMessage);
                            break;
                        case "写入(队列)":
                            message = _self.StaticMessage(message, itemKey, item.InteriorWriteMessage);
                            GlobalManager.QueueRegister[item.InteriorQueueIndex].Enqueue(item.InteriorWriteMessage);
                            break;
                    }
                }
            }
        }
        // 1. 循环获取动态条件


        return (true, message);
    }

    public async Task<(bool sueeced, string? result)> DynMessage(string DynName,
    CancellationTokenSource cts, bool noLog = false)
    {
        if (DynName == null)
        {
            Log.Error($"[{TraceContext.Name}]--正在动态嵌入内容的时候,动态获取名未设置(DynName),无法从GlobalMannager.DynDictionary进行查找 ");
            return (false, null);
        }

        var lookup = GlobalManager.DynDictionary.Lookup(DynName);
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

        //通过正则表达式匹配对应数量的[]格式的字符
        MatchCollection matches = Regex.Matches(message, @"\[.*?\]");
        foreach (Match match in matches)
        {
            foreach (var item in mesTcpPojo.DynCondition)
            {
                var itemKey = item.Name;
                var methodName = item.MethodName;

                //检查是否存在
                var i = match.Value.IndexOf($"[{itemKey}]");

                if (i == -1)
                {
                    continue;
                }

                //2. 判断走什么形式的方法进行请求
                if (item.GetMessageType == "通讯")
                {
                    //3. 判断需要通过什么获取动态内容
                    switch (methodName)
                    {
                        case "读寄存器":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读寄存器中");
                            }
                           
                            (bool succeed1, string readReg) = await _self.ReadReg(item);
                            if (!succeed1)
                            {
                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读寄存器地址{item.StartAddress}失败");
                                return (false, null);
                            }

                            (bool b1, string? responseLateProcess1) = await _self.LateProcess(item, readReg, cts);
                            if (b1)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess1);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess1}");
                                return (false, null);
                            }

                            break;
                        case "读线圈":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行读线圈中");
                            }
                            (bool succeed2, string readCoid) = await _self.ReadCoid(item);
                            if (!succeed2)
                            {
                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--读线圈地址{item.StartAddress}失败");
                                return (false, null);
                            }

                            (bool b2, string? responseLateProcess2) = await _self.LateProcess(item, readCoid, cts);

                            if (b2)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess2);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess2}");
                                return (false, null);
                            }

                            break;
                        case "Socket返回":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--动态嵌入内容:执行Socket消息发送");
                            }
                        
                            (bool succeed, string tcp) = await _self.ReadTcpMessageAsync(item, cts);
                            //判断
                            if (!succeed)
                            {
                                Log.Error($"[{TraceContext.Name}]--Socket返回发送错误");
                                return (false, null);
                            }

                            (bool b3, string? responseLateProcess3) = await _self.LateProcess(item, tcp, cts);
                            if (b3)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess3);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess3}");
                                return (false, null);
                            }

                            break;
                        case "读DM寄存器":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--执行读DM寄存器");
                            }
                            
                            (bool succeed4, string? s) = await _self.KeyenceReadDM(item, cts);

                            if (!succeed4)
                            {
                                return (false, null);
                            }

                            (bool b4, string? responseLateProcess4) = await _self.LateProcess(item, s, cts);
                            if (b4)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess4);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess4}");
                                return (false, null);
                            }

                            break;
                        case "读R线圈状态":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--执行读R线圈状态");
                            }
                       

                            (bool succeed3, string? result) = await _self.KeyenceReadCoid(item, cts);
                            if (!succeed3)
                            {
                                return (false, null);
                            }

                            (bool b6, string? responseLateProcess6) = await _self.LateProcess(item, result, cts);
                            if (b6)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess6);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess6}");
                                return (false, null);
                            }


                            break;
                        case "串口通讯":
                            if (!noLog)
                            {
                                Log.Info($"[{TraceContext.Name}]--嵌入值:{item.Name}:执行串口发送中");
                            }
                       
                            (bool b, string? response) = await _self.ScpiSerialAsync(item, cts);
                            if (!b)
                            {
                                Log.Error($"[{TraceContext.Name}]--嵌入值:{item.Name}--串口发送返回失败");
                                return (false, null);
                            }

                            (bool b5, string? responseLateProcess5) = await _self.LateProcess(item, response, cts);

                            if (b5)
                            {
                                message = _self.StaticMessage(message, itemKey, responseLateProcess5);
                            }
                            else
                            {
                                Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess5}");
                                return (false, null);
                            }

                            break;
                    }
                }
                else if (item.GetMessageType == "HTTP")
                {
                    //检查是否循环嵌套

                    //获取到当前的HTTP

                    LoadMesPageViewModel loadMesPageViewModel = Ioc.Default.GetRequiredService<LoadMesPageViewModel>();
                    var loadMesAddAndUpdateWindowModels = loadMesPageViewModel.LoadMesPageModel.MesPojoList;
                    LoadMesAddAndUpdateWindowModel loadMesAddAndUpdateWindowModel = null;
                    foreach (var model in loadMesAddAndUpdateWindowModels)
                    {
                        if (model.Name == item.HttpName)
                        {
                            loadMesAddAndUpdateWindowModel = model;
                        }
                    }

                    if (loadMesAddAndUpdateWindowModel == null)
                    {
                        return (false, $"[{TraceContext.Name}]--获取HTTP子程序的时候,程序为空!");
                    }

                    loadMesAddAndUpdateWindowModel.cts = cts;
                    loadMesAddAndUpdateWindowModel.LoadMesService = this;

                    //触发HTTP返回结果
                    (bool succeed, string? s) =
                        await loadMesPageViewModel.ExecutionCondition(loadMesAddAndUpdateWindowModel);
                    if (!succeed)
                    {
                        return (false, null);
                    }


                    //是否定义返回结果
                    if (item.NeedInteriorTriggerUserSetReturn)
                    {
                        string InteriorMessage = item.InteriorTriggerReturnMessage.Value;

                        foreach (var httpObject in item.HttpObjects)
                        {
                            int indexOf = InteriorMessage.IndexOf($"[{httpObject.Name}]");
                            if (indexOf == -1)
                            {
                                continue;
                            }

                            string res = null;
                            switch (httpObject.Method)
                            {
                                case "常量":
                                    res = httpObject.staticParam;
                                    break;
                                case "结果Json解析":
                                    //判断是否为字符串格式
                                    JsonTool<object>.TryFormatJson(s, out bool isJson);
                                    if (isJson)
                                    {
                                        JObject jObject = JObject.Parse(s);
                                        if (httpObject.JsonParam != null)
                                        {
                                            res = jObject.SelectToken($"{httpObject.JsonParam}")?.ToString();

                                            if (res == null)
                                            {
                                                Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析到结果为NULL");
                                                return (false,
                                                    $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析到结果为NULL");
                                            }
                                        }
                                        else
                                        {
                                            Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析路径参数为NULL");
                                            return (false,
                                                $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,解析路径参数为NULL");
                                        }
                                    }
                                    else
                                    {
                                        Log.Error($"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,返回的不是Json字符串");
                                        return (false, $"[{TraceContext.Name}]--解析内部调用Http程序,中的JSON字符串时,返回的不是Json字符串");
                                    }

                                    break;
                                case "方法集":
                                    res = String.Empty;
                                    break;
                            }

                            int length = httpObject.Name.Length;
                            var requestA = InteriorMessage.Substring(0, indexOf);
                            var requestB = InteriorMessage.Substring(indexOf + length + 2);
                            InteriorMessage = requestA + res + requestB;
                        }

                        s = InteriorMessage;
                    }

                    //是否转发
                    (bool b1, string? responseLateProcess1) = await _self.LateProcess(item, s, cts);
                    if (b1)
                    {
                        message = _self.StaticMessage(message, itemKey,
                            item.InteriorTriggerReturn ? responseLateProcess1 : String.Empty);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--后期处理方法发送错误{responseLateProcess1}");
                        return (false, null);
                    }
                }
                else if (item.GetMessageType == "自定义")
                {
                    Type userDefined = item.UserDefined;
                    //实例化
                    var objInstance = Activator.CreateInstance(userDefined);
                    //获取方法
                    var method = userDefined.GetMethod("Main");

                    //执行方法
                    var invoke = method.Invoke(objInstance, [cts]);

                    // 转换为具体元组类型
                    var (succeed, returnValue) = await (Task<(bool Succeed, object Return)>)invoke;

                    if (succeed)
                    {
                        //静态嵌入
                        message = _self.StaticMessage(message, itemKey, returnValue.ToString());
                    }
                    else
                    {
                        return (false, returnValue?.ToString());
                    }
                }
                else if (item.GetMessageType == "内部")
                {
                    //判断是集合还是队列

                    switch (item.MethodName)
                    {
                        case "读取(集合)":
                            message = _self.StaticMessage(message, itemKey,
                                Volatile.Read(ref GlobalManager.ArrayRegister[item.InteriorArrayIndex])?.ToString());
                            break;
                        case "读取(队列)":
                            bool tryPeek = GlobalManager.QueueRegister[item.InteriorQueueIndex]
                                .TryDequeue(out object a);

                            if (!tryPeek)
                            {
                                Log.Error($"[{TraceContext.Name}]--在取出队列中元素时失败");
                                return (false, null);
                            }

                            if (a == null)
                            {
                                Log.Error($"[{TraceContext.Name}]--在取出队列中元素时为null");
                                return (false, null);
                            }

                            message = _self.StaticMessage(message, itemKey, a?.ToString());
                            break;
                        case "写入(集合)":
                            message = _self.StaticMessage(message, itemKey, item.InteriorWriteMessage);
                            Volatile.Write(ref GlobalManager.ArrayRegister[item.InteriorArrayIndex],
                                item.InteriorWriteMessage);
                            break;
                        case "写入(队列)":
                            message = _self.StaticMessage(message, itemKey, item.InteriorWriteMessage);
                            GlobalManager.QueueRegister[item.InteriorQueueIndex].Enqueue(item.InteriorWriteMessage);
                            break;
                    }
                }
            }
        }
        // 1. 循环获取动态条件


        return (true, message);
    }

    #region 执行可选后期处理

    public async Task<(bool succeed, string message)> LateProcess(DynCondition item, string response,
        CancellationTokenSource cts)
    {
        var itemKey = item.Name;
        var methodName = item.MethodName;
        bool isSwitch = item.OpenSwitch;
        bool isVerify = item.OpenVerify;
        bool ResultTranspond = item.ResultTranspond;
        try
        {
            //进行校验
            if (isVerify)
            {
                Log.Info($"[{TraceContext.Name}]---Socket需要进行消息校验");
                foreach (var dynVerify in item.VerifyList)
                {
                    (bool succeed, response) = await _self.VerityMessage(response, dynVerify, cts);

                    if (!succeed)
                    {
                        Log.Error($"[{TraceContext.Name}]--校验到不匹配,撤回发送");
                        return (false, null);
                    }
                }
            }

            //进行switch替换
            if (isSwitch)
            {
                Log.Info($"[{TraceContext.Name}]--Socket需要进行消息转换Switch映射");
                response = _self.SwitchGetMessage(response, item);
            }
            if (ResultTranspond)
            {
                Log.Info($"[{TraceContext.Name}]--需要对当前结果进行转发");
                (bool succeed3, string message1) = await _self.Transpond(item, response);
                if (!succeed3)
                {
                    Log.Info($"[{TraceContext.Name}]--转发失败");
                    return (false, null);
                }

                if (item.TranspondModbusDetailed.NoReturn)
                {
                    return (true, string.Empty);
                }
            }

            return (true, response);
        }
        catch (Exception e)
        {
            return (false, e.Message);
        }
    }

    #endregion


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
                var netWork = GlobalManager.GetNetWork(forwardingName);
                //获得网络
                if (netWork == null)
                {
                    Log.Error($"[{TraceContext.Name}]--进行通讯转发时发送错误,GetNetWork无法获取到通讯");
                    return (false, null);
                }

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

                            await modbusBase.WriteRegisters_10(
                                byte.Parse(model.TranspondModbusDetailed.SlaveAddress.ToString()),
                                ushort.Parse(model.TranspondModbusDetailed.StartAddress.ToString()), result);
                        }
                        catch (Exception e)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行通讯转发时发送错误,:{e}");
                            return (false, null);
                        }
                        break;
                    case "基恩士上位链路通讯":
                        KeyenceHostLinkTool netWorkKeyenceHostLinkTool = netWork.KeyenceHostLinkTool;
                        try
                        {
                            //将字符串转换为字节数组
                            
                            // await netWorkKeyenceHostLinkTool.WriteDM(model.TranspondModbusDetailed.StartAddress,string ,cts );
                        }
                        catch (Exception e)
                        {
                            Log.Error($"[{TraceContext.Name}]--进行基恩士上位链路通讯转发时发送错误,:{e}");
                            return (false, null);
                        }
                        break;
                }

                break;
            case "内部地址":
                Volatile.Write(
                    ref GlobalManager.ArrayRegister[int.Parse(model.TranspondModbusDetailed.InteriorAddress)],
                    response);
                break;
            case "队列":
                GlobalManager.QueueRegister[int.Parse(model.TranspondModbusDetailed.InteriorAddress)].Enqueue(response);
                break;
        }

        return (true, null);
    }

    #endregion

    #endregion

    #region Verity校验方法

    /// <summary>
    /// Verity校验方法
    /// </summary>
    /// <param name="message"></param>
    /// <param name="verify"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string response)> VerityMessage(string message, DynVerify verify, CancellationTokenSource cts)
    {
        bool tryParse = false;
        bool tryParse2 = false;
        int len = 0;
        int len2 = 0;
        //预处理一下message
        message = message.Replace(" ", "").Trim();

        switch (verify.Type)
        {
            case "字符长度检测=":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length == len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度不等于{len}");
                    return (false, message);
                }
            case "字符长度检测!=":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length != len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度={len}");
                    return (false, message);
                }

            case "字符长度检测>":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length > len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度<={len}");
                    return (false, message);
                }
            case "字符长度检测<":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length < len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度>={len}");
                    return (false, message);
                }
            case "字符长度检测>=":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length >= len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度<{len}");
                    return (false, message);
                }
            case "字符长度检测=<":
                tryParse = int.TryParse(verify.Value, out len);
                if (!tryParse)
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败,检测填入的内容");
                    return (false, message);
                }

                if (message.Length <= len)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,长度>{len}");
                    return (false, message);
                }

            case "字符=":
                if (message == verify.Value)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,字符不等于 {verify.Value}");
                    return (false, message);
                }
            case "字符!=":
                if (message != verify.Value)
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,字符等于 {verify.Value}");
                    return (false, message);
                }
            case "正则表达式检测":

                if (Regex.IsMatch(message, verify.Type))
                {
                    return (true, message);
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--校验失败,不符合正则表达式");
                    return (false, message);
                }
            case "数据>":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len > len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据>{verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "数据>=":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len >= len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据>={verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "数据<":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len < len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据<{verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "数据<=":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len <= len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据<={verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "数据=":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len == len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据=={verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "数据!=":
                tryParse = int.TryParse(message, out len);
                tryParse2 = int.TryParse(verify.Value, out len2);
                if (tryParse && tryParse2)
                {
                    if (len != len2)
                    {
                        return (true, message);
                    }
                    else
                    {
                        Log.Error($"[{TraceContext.Name}]--校验失败,不符合数据!={verify.Value}");
                        return (false, message);
                    }
                }
                else
                {
                    Log.Info($"[{TraceContext.Name}]--在检测字符串的时候转换Int失败");
                    return (false, message);
                }
            case "自定义复杂逻辑校验":
                Type ComplexValue = verify.ComplexValue;
                //实例化
                var objInstance = Activator.CreateInstance(ComplexValue);
                //获取方法
                var method = ComplexValue.GetMethod("Main");

                var ErrorReturn = ComplexValue.GetMethod("ErrorMessage");

                //执行方法
                var invoke = method.Invoke(objInstance, [cts, (object[])[message]]);

                // 转换为具体元组类型
                var (succeed, returnValue) = await (Task<(bool Succeed, object Return)>)invoke;


                object[] returnValues = returnValue as object[];
                if (succeed)
                {
                    if (!(bool)returnValues[0])
                    {
                        var Error =
                            await (Task<string>)ErrorReturn.Invoke(objInstance, [cts, (object[])[message]]);

                        Log.Error($"[{TraceContext.Name}]--校验失败:{Error}");

                    }


                    return ((bool)returnValues[0], returnValues[1].ToString());
                }
                else
                {
                    Log.Error($"[{TraceContext.Name}]--在校验是否满足条件时,发生故障");
                    return (false, message);
                }


                break;
        }

        return (false, message);
    }

    #endregion

    #region 方法集内容嵌入

    /// <summary>
    /// 方法集内容嵌入
    /// </summary>
    /// <param name="request"></param>
    /// <param name="itemValue"></param>
    /// <param name="itemMethodOtherValue"></param>
    /// <returns></returns>
    public async Task<string> MethodMessage(string request, string itemValue, string itemMethodOtherValue)
    {
        if (itemValue == null)
        {
            return null;
        }

        DateTime dateTime = _self.DateTimeDispose(itemMethodOtherValue);
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
            case "当前时间(13位时间戳)":
                return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds().ToString();
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 时间计算方法 规则: -,5M,5D,5H,5m,5s
    /// </summary>
    /// <param name="itemMethodOtherValue"></param>
    /// <returns></returns>
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
            else if (dynSwitch.Case == "default")
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
    public  async Task<(bool succeed, string response)> ReadTcpMessageAsync(DynCondition item,
        CancellationTokenSource cts)
    {
        //判断是走客户端发送,还是走服务器发送
        string itemConnectName = item.ConnectName;
        string netMethod = "";
        NetWork curNetWork = null;
        //遍历取出判断当前的网络是什么类型
        foreach (var netWorkPoJo in GlobalManager.NetWorkDictionary.Items)
        {
            if (netWorkPoJo.NetworkDetailed.Name == itemConnectName)
            {
                netMethod = netWorkPoJo.NetworkDetailed.NetMethod;
                curNetWork = netWorkPoJo;
            }
        }

        if (curNetWork == null)
        {
            Log.Error($"[{TraceContext.Name}]--执行Socket时--遍历获取网络时,未获取到 GlobalManager.NetWorkDictionary中不存在");
            return (false, null);
        }

        string response = string.Empty;
        TcpTool tcpTool = curNetWork.TcpTool;
        //获取需要发送的内容
        object sendMessage = null;
        switch (item.SendMessageMethod)
        {
            case "常量":
                sendMessage = item.SocketSendMessage;
                break;
            case "内部地址":
                sendMessage = Volatile.Read(
                    ref GlobalManager.ArrayRegister[int.Parse(item.InteriorGetRegisterMessageIndex)]
                );

                if (sendMessage == null)
                {
                    Log.Error($"[{TraceContext.Name}]--执行Socket时--读取内部地址为null");

                    return (false, null);
                }

                break;
            case "队列":

                GlobalManager.QueueRegister[int.Parse(item.InteriorGetRegisterMessageIndex)]
                    .TryDequeue(out sendMessage);
                if (sendMessage == null)
                {
                    Log.Error($"[{TraceContext.Name}]--执行Socket时--读取队列地址为null");
                    return (false, null);
                }

                break;
        }


        //更具类型选择发送
        switch (netMethod)
        {
            case "Tcp客户端":
                Log.Info($"[{TraceContext.Name}]--执行Tcp客户端消息发送,并等待消息返回");
                (bool succeed, response) = await tcpTool.SendAndWaitClientAsync(sendMessage.ToString(), cts);
                if (!succeed)
                {
                    Log.Error($"[{TraceContext.Name}]--执行Tcp客户端消息发送,等待消息返回时发生错误");
                    return (false, response);
                }

                break;
            case "Tcp服务器":
                Log.Info($"[{TraceContext.Name}]--执行Tcp服务器消息发送,并等待消息返回");
                (bool succeed2, response) = await tcpTool.ServerSendWaitResponseOneToOne(sendMessage.ToString(), cts);

                if (!succeed2)
                {
                    Log.Error($"[{TraceContext.Name}]--执行Tcp服务器消息发送,等待消息返回时发生错误");
                    return (false, response);
                }

                break;
        }

        return (true, response);
    }

    #endregion

    #region 串口获取内容

    public async Task<(bool succeed, string response)> ScpiSerialAsync(DynCondition item,
        CancellationTokenSource cts)
    {
        try
        {
            var itemKey = item.Name;
            var itemConnectName = item.ConnectName;
            var methodName = item.MethodName;
            //获得网络,遍历获取对应的网络
            var netWork = GlobalManager.GetNetWork(itemConnectName);
            if (netWork == null) return (false, null);

            ScpiSerialTool scpiSerialTool = netWork.ScpiSerialTool;


            //获取需要发送的内容
            object sendMessage = null;
            switch (item.SendMessageMethod)
            {
                case "常量":
                    sendMessage = item.SerialSendMessage;
                    break;
                case "内部地址":
                    sendMessage = Volatile.Read(
                        ref GlobalManager.ArrayRegister[int.Parse(item.InteriorGetRegisterMessageIndex)]
                    );

                    if (sendMessage == null)
                    {
                        Log.Error($"[{TraceContext.Name}]--执行Socket时--读取内部地址为null");

                        return (false, null);
                    }

                    break;
                case "队列":

                    GlobalManager.QueueRegister[int.Parse(item.InteriorGetRegisterMessageIndex)]
                        .TryDequeue(out sendMessage);
                    if (sendMessage == null)
                    {
                        Log.Error($"[{TraceContext.Name}]--执行Socket时--读取队列地址为null");
                        return (false, null);
                    }

                    break;
            }

            (bool b, string? response) =
                await scpiSerialTool.WriteLineAndWaitResponse(sendMessage.ToString(),
                    item.SendTimeOut);

            if (b)
            {
                return (true, response);
            }
            else
            {
                return (false, null);
            }
        }
        catch (Exception e)
        {
            return (false, null);
        }
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
        var netWork = GlobalManager.GetNetWork(itemConnectName);
        if (netWork == null) return (false, null);
        //获得modbus
        var modbusBase = netWork.ModbusBase;
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
        var netWork = GlobalManager.GetNetWork(itemConnectName);
        if (netWork == null) return (false, null);
        //获得modbus
        var modbusBase = netWork.ModbusBase;
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
                    return (true, Encoding.ASCII.GetString(result_3.ToArray()).Trim('\0'));
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
                    return (true, Encoding.ASCII.GetString(result_4.ToArray()).Trim('\0'));
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

    /// <summary>
    /// 动态获取基恩士上链路内容
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<(bool succeed, string response)> KeyenceReadDM(DynCondition item,
        CancellationTokenSource cts)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netWork = GlobalManager.GetNetWork(itemConnectName);
        if (netWork == null) return (false, null);

        //获得keyenceHostLinkTool
        KeyenceHostLinkTool keyenceHostLinkTool = netWork.KeyenceHostLinkTool;

        int startAddress = item.StartAddress;
        int num = item.EndAddress;
        try
        {
            switch (item.BitNet)
            {
                case "单寄存器(无符号)":

                    (bool b, ushort item3) = await keyenceHostLinkTool.ReadDM<ushort>(startAddress, cts);

                    return (b, item3.ToString());
                case "单寄存器(有符号)":

                    (bool b1, short s) = await keyenceHostLinkTool.ReadDM<short>(startAddress, cts);
                    return (b1, s.ToString());
                case "双寄存器(无符号)":

                    (bool b2, uint u) = await keyenceHostLinkTool.ReadDM<uint>(startAddress, cts);
                    return (b2, u.ToString());
                case "双寄存器(有符号)":

                    (bool b3, int i) = await keyenceHostLinkTool.ReadDM<int>(startAddress, cts);
                    return (b3, i.ToString());
                case "32位浮点数":
                    (bool b4, float q) = await keyenceHostLinkTool.ReadDM<float>(startAddress, cts);
                    return (b4, q.ToString());
                case "ASCII字符串":

                    (bool succeed, ushort[]? response) = await keyenceHostLinkTool.ReadDMWords(startAddress, num, cts);

                    if (!succeed || response == null)
                    {
                        Log.Error($"[{TraceContext.Name}]--基恩士上链路读取DM失败");
                        return (false, null);
                    }

                    ushort[] readDmWords = response;

                    var bytes = new List<byte>();
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
                        bytes.Add(ByteLow);
                        bytes.Add(ByteHigh);
                    }

                    //输出ASCII码转换后的结果
                    return (true, Encoding.ASCII.GetString(bytes.ToArray()));
            }
        }
        catch (Exception e)
        {
            Log.Error($"[{TraceContext.Name}]--基恩士上链路读取DM失败 :{e}");
            return (false, null);
        }

        return (false, "没有进入对应类型的解析Switch");
    }

    public  async Task<(bool succeed, string? result)> KeyenceReadCoid(DynCondition item,
        CancellationTokenSource cts)
    {
        var itemKey = item.Name;
        var itemConnectName = item.ConnectName;
        var methodName = item.MethodName;
        //获得网络,遍历获取对应的网络
        var netWork = GlobalManager.GetNetWork(itemConnectName);
        if (netWork == null) return (false, null);

        //获得keyenceHostLinkTool
        KeyenceHostLinkTool keyenceHostLinkTool = netWork.KeyenceHostLinkTool;
        var result = "";
        int startAddress = item.StartAddress;
        int num = item.EndAddress;

        return await keyenceHostLinkTool.ReadR(startAddress.ToString(), cts);
    }

    #endregion
}