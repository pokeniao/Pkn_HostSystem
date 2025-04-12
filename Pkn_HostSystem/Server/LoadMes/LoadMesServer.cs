using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Pojo.Page.MESTcp;
using Pkn_HostSystem.Pojo.Windows.LoadMesAddAndUpdateWindow;
using Pkn_HostSystem.Static;
using RestSharp;
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
    /// 包装Request请求
    /// </summary>
    /// <param name="name"></param>
    public async Task<string> PackRequest(string name)
    {
        //获得当前行的数据
        var loadMesAddAndUpdateWindowModel = SelectByName(name);
        if (loadMesAddAndUpdateWindowModel == null) return null;

        //获得当前行的条件
        var conditionItems = Enumerable.ToList<ConditionItem>(loadMesAddAndUpdateWindowModel.Condition);


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

    private async Task<string> MethodMessage(string request, string itemValue)
    {
        if (itemValue == null)
        {
            return null;
        }

        switch (itemValue)
        {
            case "当前时间":
                return DateTime.Now.ToString();
                break;
            default:
                return string.Empty;
        }
    }


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
    /// <param name="request"></param>
    /// <param name="DynName"></param>
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
        // 获取其中的条件
        foreach (var item in mesTcpPojo.DynCondition)
        {
            var itemKey = item.Name;
            var methodName = item.MethodName;
            //判断什么方式的获取值
            switch (methodName)
            {
                case "读寄存器":
                    message = StaticMessage(message, itemKey, await ReadReg(item));
                    break;
                case "读线圈":
                    message = StaticMessage(message, itemKey, await ReadCoid(item));
                    break;
                case "Socket返回":
                    break;
            }
        }

        return message; //测试
    }

    /// <summary>
    /// 循环遍历获取网络
    /// </summary>
    /// <returns></returns>
    public string getNetKey(string ConnectName)
    {
        var netWorkPoJoes = GlobalMannager.NetWorkDictionary.Items.ToList();
        foreach (var netWorkPoJo in netWorkPoJoes)
            if (netWorkPoJo.ConnectPojo.Name == ConnectName)
                return netWorkPoJo.NetWorkId;

        return null;
    }

    /// <summary>
    /// 读线圈
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public async Task<string> ReadCoid(DynConditionItem item)
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
    public async Task<string> ReadReg(DynConditionItem item)
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
                case "单寄存器":
                    //用逗号分割
                    result = string.Join(",", Array.ConvertAll(holdingRegisters03, p => $"{p}"));
                    return result;
                case "双寄存器":
                    var result_2 = new List<string>();
                    for (var i = 0; i < holdingRegisters03.Length - 1; i += 2)
                    {
                        var low = holdingRegisters03[i].ToString("X4");
                        var high = holdingRegisters03[i + 1].ToString("X4");
                        var value = high + low;
                        var valueInt = int.Parse(value, NumberStyles.HexNumber);
                        //存入当前数值
                        result_2.Add(valueInt.ToString());

                        var nextInt = i + 2;
                        if (nextInt == holdingRegisters03.Length - 1)
                        {
                            var value2 = holdingRegisters03[nextInt].ToString("X8");
                            var valueInt2 = int.Parse(value2, NumberStyles.HexNumber);
                            result_2.Add(valueInt2.ToString());
                        }
                    }

                    //用逗号分割
                    return string.Join(",", Array.ConvertAll(result_2.ToArray(), p => $"{p}"));
                case "ASCII字符串":

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
                        result_3.Add(ByteHigh);
                        result_3.Add(ByteLow);
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


    /// <summary>
    /// 手动发送HTTP消息
    /// </summary>
    public async Task<bool> RunOne(string Name)
    {
        //获取当前Name的行数据
        LoadMesAddAndUpdateWindowModel item = SelectByName(Name);
        //得到消息体
        return await SendHttp(item);
    }

    public async Task<bool> RunAll()
    {
        foreach (var item in mesPojoList)
        {
            await SendHttp(item);
        }

        return true;
    }

    public async Task<bool> SendHttp(LoadMesAddAndUpdateWindowModel item)
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
            RestResponse response = await client.ExecuteAsync(requestBody);
            //判断
            if (response.IsSuccessStatusCode)
            {
                item.Response = response.Content;
                log.Info($"{item.Name}--发送请求返回: {item.Response}");
            }
            else
            {
                item.Response = response.ErrorMessage;
                log.Info($"{item.Name}--发送请求返回: {item.Response}");
            }
        }

        return true;
    }
}