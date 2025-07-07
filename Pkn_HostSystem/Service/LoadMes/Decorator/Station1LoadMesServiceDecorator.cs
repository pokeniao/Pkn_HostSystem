using Azure;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes.Interface;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Service.LoadMes.Decorator
{

    /// <summary>
    /// 装饰模式,装饰器
    /// </summary>
    public class Station1LoadMesServiceDecorator : ILoadMesService
    {
        private readonly ILoadMesService _loadMesService;

        public Station1LoadMesServiceDecorator(ILoadMesService loadMesService ) 
        {
            _loadMesService = loadMesService;
            
            if (loadMesService is LoadMesService concrete)
            {
                concrete._self = this;
            }
        }



        //对需要装饰的方法进行重写,其他方法不需要重写

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

        public async Task<(bool succeed, string? response)> RunOne(string Name, string request, CancellationTokenSource cts)
        {
            return await _loadMesService.RunOne(Name, request, cts);
        }

        public async Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item, string request, CancellationTokenSource cts)
        {
            //进行工位日志记录
            if (item.NeedStationLog)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                    $"[{TraceContext.Name}]--发送内容: \r\n {request}");
            }
            StationManager.TraceContextStart(item.Station);
            dynamic eachStation = TraceContext.Param;

            eachStation.Station.Main(request, cts);
            (bool succeed, string? response)  = await _loadMesService.SendHttp(item, request, cts);

            if (succeed)
            {
                //进行工位日志记录
                if (item.NeedStationLog)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station,
                        $"[{TraceContext.Name}]--返回消息--成功--消息体:\r\n{item.Response}");
                }
            }
            else
            {
                //进行工位日志记录
                if (item.NeedStationLog)
                {
                    StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station,
                        $"[{TraceContext.Name}]--返回消息--失败--消息体:\r\n{item.Response}");
                }
            }
            eachStation.Station.Main(request, cts);

            return ( succeed, response);
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

        public async Task<(bool sueeced, string? result)> DynMessage(string request, string DynName, CancellationTokenSource cts)
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

        public async Task<(bool succeed, string response)> ReadTcpMessageAsync(DynCondition item, CancellationTokenSource cts)
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