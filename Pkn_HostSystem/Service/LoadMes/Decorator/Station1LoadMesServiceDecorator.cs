using Azure;
using log4net;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes.Interface;
using Pkn_HostSystem.Static;
using RestSharp;
using System.Collections.ObjectModel;
using System.IO;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace Pkn_HostSystem.Service.LoadMes.Decorator
{
    /// <summary>
    /// 装饰模式,装饰器
    /// </summary>
    public class Station1LoadMesServiceDecorator : ILoadMesService
    {
        private readonly ILoadMesService _loadMesService;

        public Station1LoadMesServiceDecorator(ILoadMesService loadMesService)
        {
            _loadMesService = loadMesService;

            if (loadMesService is LoadMesService concrete)
            {
                concrete._self = this;
            }
        }

        public LoadMesAddAndUpdateWindowModel SelectByName(string Name)
        {
            return _loadMesService.SelectByName(Name);
        }

        public string GetNetKey(string ConnectName)
        {
            return _loadMesService.GetNetKey(ConnectName);
        }

        public async Task<(bool succeed, string? response)> RunOne(string Name, CancellationTokenSource cts)
        {
            return await _loadMesService.RunOne(Name, cts);
        }

        public async Task<(bool succeed, string? response)> RunOne(string Name, string request,
            CancellationTokenSource cts)
        {
            return await _loadMesService.RunOne(Name, request, cts);
        }

        public async Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item,
            string request, CancellationTokenSource cts)
        {
            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                $"[{TraceContext.Name}]--发送内容: \r\n {request}");

            StationManager.TraceContextStart(item.Station);
            dynamic eachStation = TraceContext.Param["EachStation"];

            //调用工位逻辑
            TraceContext.UpdateParam("step", 1);
            TraceContext.UpdateParam("response", request);
            var result = await eachStation?.Station.Main(cts);

            if (!result.Item1)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                    $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为1, 错误原因:{result.Item2}");
            }


            //日志显示发送内容


            try
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
                    //判断是否是JSON格式,如果是转成输出
                    item.Response = AppJsonTool<Object>.TryFormatJson(item.Response, out bool isJson);

                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                        $"[{TraceContext.Name}]--返回消息--成功--消息体:\r\n{item.Response}");

                    TraceContext.UpdateParam("response", item.Response);
                    var result2 = await eachStation?.Station.Main(cts);

                    if (!result2.Item1)
                    {
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                            $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为2, 错误原因:{result2.Item2}");
                    }

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


                    //拦截工位错误

                    if (isJson)
                    {
                        JObject jObject = JObject.Parse(item.Response);
                        string? s = jObject["data"]?.ToString();
                        if (s ==
                            "Post接口请求接口http://hf-mes-fdkj2.ppp.com/api/mes-opm/snChangeFromMesOpenPlatform/snStageChange发生错误:I/O error on POST request for \"http://hf-mes-fdkj2.ppp.com/api/mes-opm/snChangeFromMesOpenPlatform/snStageChange\": Connection reset; nested exception is java.net.SocketException: Connection reset")
                        {
                            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                                $"[{TraceContext.Name}]--返回消息--成功(拦截)--消息体:\r\n{item.Response}");

                            TraceContext.UpdateParam("response", item.Response);
                            var result3 = await eachStation.Station.Main(cts);

                            if (!result3.Item1)
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                                    $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为2, 错误原因:{result3.Item2}");
                            }

                            return (true, item.Response);
                        }

                        if (s ==
                            "Post接口请求接口http://hf-mes-fdkj2.ppp.com/api/mes-jj/pro/ProPackagePo/addData发生错误:I/O error on POST request for \"http://hf-mes-fdkj2.ppp.com/api/mes-jj/pro/ProPackagePo/addData\": Connection reset; nested exception is java.net.SocketException: Connection reset")
                        {
                            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                                $"[{TraceContext.Name}]--返回消息--成功(拦截)--消息体:\r\n{item.Response}");

                            TraceContext.UpdateParam("response", item.Response);
                            var result4 = await eachStation.Station.Main(cts);

                            if (!result4.Item1)
                            {
                                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                                    $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为2, 错误原因:{result4.Item2}");
                            }

                            return (true, item.Response);
                        }
                    }

                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                        $"[{TraceContext.Name}]--返回消息--失败--消息体:\r\n{item.Response}");
                    TraceContext.UpdateParam("response", item.Response);
                    var result5 = await eachStation.Station.Main(cts);

                    if (!result5.Item1)
                    {
                        StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                            $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为2, 错误原因:{result5.Item2}");
                    }

                    return (false, item.Response);
                }
            }
            catch (Exception e)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                    $"[{TraceContext.Name}]--SendHttp发生不可预期错误: {e}");


                TraceContext.UpdateParam("step", 3);
                var result6 = await eachStation.Station.Main(cts);
                if (!result6.Item1)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                        $"[{TraceContext.Name}]-- 调用Main失败,当前step应该为2, 错误原因:{result6.Item2}");
                }
                return (false, item.Response);
            }
        }

        public async Task<(bool succeed, string? value)> PackRequest(string httpName, CancellationTokenSource cts)
        {
            return await _loadMesService.PackRequest(httpName, cts);
        }

        public string StaticMessage(string request, string itemKey, string itemValue)
        {
            return _loadMesService.StaticMessage(request, itemKey, itemValue);
        }

        public string StaticMessageSon(string request, string itemKey, string itemKeySon, string itemValue)
        {
            return _loadMesService.StaticMessageSon(request, itemKey, itemKeySon, itemValue);
        }

        public async Task<(bool sueeced, string? result)> DynMessage(string request, string DynName,
            CancellationTokenSource cts)
        {
            return await _loadMesService.DynMessage(request, DynName, cts);
        }

        public async Task<(bool succeed, string message)> Transpond(DynCondition model, string response)
        {
            return await _loadMesService.Transpond(model, response);
        }

        public bool VerityMessage(string message, DynVerify verify)
        {
            return _loadMesService.VerityMessage(message, verify);
        }

        public async Task<string> MethodMessage(string request, string itemValue, string itemMethodOtherValue)
        {
            return await _loadMesService.MethodMessage(request, itemValue, itemMethodOtherValue);
        }

        public DateTime DateTimeDispose(string itemMethodOtherValue)
        {
            return _loadMesService.DateTimeDispose(itemMethodOtherValue);
        }

        public string SwitchGetMessage(string message, DynCondition item)
        {
            return _loadMesService.SwitchGetMessage(message, item);
        }

        public async Task<(bool succeed, string response)> ReadTcpMessageAsync(DynCondition item,
            CancellationTokenSource cts)
        {
            return await _loadMesService.ReadTcpMessageAsync(item, cts);
        }

        public async Task<(bool succeed, string? result)> ReadCoid(DynCondition item)
        {
            return await _loadMesService.ReadCoid(item);
        }

        public async Task<(bool succeed, string? result)> ReadReg(DynCondition item)
        {
            return await _loadMesService.ReadReg(item);
        }

        public async Task<string> KeyenceReadDM(DynCondition item)
        {
            return await _loadMesService.KeyenceReadDM(item);
        }
    }
}