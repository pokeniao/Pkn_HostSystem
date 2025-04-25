using KeyenceTool;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Pojo.Page.HomePage;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.Static;
using RestSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using LoadMesAddAndUpdateWindowModel = Pkn_HostSystem.Models.Windows.LoadMesAddAndUpdateWindowModel;

namespace Pkn_HostSystem.Server.LoadMes;

public class LoadMesServer
{
    private ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList;

    private MesLogBase<LoadMesServer> log;

    public LoadMesServer(ObservableCollection<LoadMesAddAndUpdateWindowModel> mesPojoList)
    {
        this.mesPojoList = mesPojoList;
        log = new MesLogBase<LoadMesServer>();
    }


    #region 获取当前行 与获取网络Id方法
    /// <summary>
    /// 循环查找当前行是否存在
    /// </summary>
    /// <param name="Name"></param>
    /// <returns></returns>
    public LoadMesAddAndUpdateWindowModel SelectByName(string Name)
    {
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
        var netWorkPoJoes = GlobalMannager.NetWorkDictionary.Items.ToList();
        foreach (var netWorkPoJo in netWorkPoJoes)
            if (netWorkPoJo.NetworkDetailed.Name == ConnectName)
                return netWorkPoJo.NetWorkId;

        return null;
    }



    #endregion

    #region 触发Http请求

    /// <summary>
    /// 触发单个请求
    /// </summary>
    public async Task<bool> RunOne(string Name,CancellationTokenSource cts)
    {
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = SelectByName(Name);
        //得到消息体
        return await SendHttp(item,cts);
    }

    /// <summary>
    /// 全部触发
    /// </summary>
    /// <returns></returns>
    public async Task<bool> RunAll()
    {
        foreach (var item in mesPojoList)
        {
            await SendHttp(item,new CancellationTokenSource());
        }

        return true;
    }

    #endregion

    #region 发送Http任务

    public async Task<bool> SendHttp(LoadMesAddAndUpdateWindowModel item,CancellationTokenSource cts)
    {
        //得到消息体
        var request = await PackRequest(item.Name);
        //日志显示发送内容
        log.Info($"{item.Name}--发送内容: {request}");
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
            RestResponse response = await client.ExecuteAsync(requestBody,cts.Token);
            //判断
            if (response.IsSuccessStatusCode)
            {
                item.Response = response.Content;
                log.Info($"{item.Name}--发送请求返回: {item.Response}");
                return true;
            }
            else
            {
                item.Response = response.ErrorMessage;
                log.Info($"{item.Name}--发送请求返回: {item.Response}");
                return false;
            }
        }

        return false;

    }

    #endregion

    #region 封装消息请求体方法
    /// <summary>
    /// 包装Request请求
    /// </summary>
    /// <param name="name"></param>
    public async Task<string> PackRequest(string name)
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
            switch (item.Method)
            {
                case "动态获取":
                    //获取动态的值
                    var value = await DynMessage(request, itemValue);
                    request = StaticMessage(request, itemKey, value);
                    break;
                case "常量":
                    //直接嵌入常量
                    request = StaticMessage(request, itemKey, itemValue);
                    break;
                case "方法集":
                    var value2 = await MethodMessage(request, itemValue);
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
    /// 动态嵌入内容
    /// </summary>
    /// <param name="request">整个消息体</param>
    /// <param name="DynName">嵌入名</param>
    /// <returns></returns>
    public async Task<string> DynMessage(string request, string DynName)
    {
        if (DynName == null)
        {
            return null;
        }

        var lookup = GlobalMannager.DynDictionary.Lookup(DynName);
        if (!lookup.HasValue) return string.Empty;
        var mesTcpPojo = lookup.Value;
        var message = mesTcpPojo.Message;
        if (message == null) return string.Empty;

        var result = "";
        // 1. 循环获取动态条件
        foreach (var item in mesTcpPojo.DynCondition)
        {
            var itemKey = item.Name;
            var methodName = item.MethodName;
            bool isSwitch = item.OpenSwitch;
            //2. 判断需要通过什么获取动态内容
            switch (methodName)
            {
                case "读寄存器":
                    string readReg = await ReadReg(item);
                    if (isSwitch)
                    {
                        readReg = SwitchGetMessage(readReg, item);
                    }

                    message = StaticMessage(message, itemKey, readReg);
                    break;
                case "读线圈":
                    string readCoid = await ReadCoid(item);
                    if (isSwitch)
                    {
                        readCoid = SwitchGetMessage(readCoid, item);
                    }

                    message = StaticMessage(message, itemKey, readCoid);
                    break;
                case "Socket返回":

                    string tcp = await ReadTcpMessageAsync(item);
                    if (isSwitch)
                    {
                        tcp = SwitchGetMessage(tcp, item);
                    }
                    message = StaticMessage(message, itemKey, tcp);
                    break;
                case "读DM寄存器":
                    string readDm = await KeyenceReadDM(item);
                    if (isSwitch)
                    {
                        readDm = SwitchGetMessage(readDm, item);
                    }
                    message = StaticMessage(message, itemKey, readDm);
                    break;
                case "读R线圈状态":
                    break;
            }
        }

        return message;
    }

    #endregion

    #region 方法集内容嵌入

    private async Task<string> MethodMessage(string request, string itemValue)
    {
        if (itemValue == null)
        {
            return null;
        }

        switch (itemValue)
        {
            case "当前时间(yyyy-MM-dd HH:mm:ss)":
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                break;
            case "当前时间(yyyy/MM/dd HH:mm:ss)":
                return DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                break;
            case "当前时间(yyyy-MM-dd)":
                return DateTime.Now.ToString("yyyy-MM-dd");
                break;
            case "当前时间(yyyy/MM/dd)":
                return DateTime.Now.ToString("yyyy/MM/dd");
                break;
            default:
                return string.Empty;
        }
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
                response = await tcpTool.SendAndWaitClientAsync(item.SocketSendMessage);
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
            log.Error("执行失败,TCP或RTU未连接上");
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
            log.Error("读取失败");
        }

        return result;
    }

    #endregion
}