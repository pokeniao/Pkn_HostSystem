using Azure.Core;
using Newtonsoft.Json.Linq;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Core.Interface;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Service.LoadMes.Interface;
using Pkn_HostSystem.Service.Stations;
using Pkn_HostSystem.Static;
using RestSharp;

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

        public async Task<(bool succeed, string? response)> RunOne(string Name, CancellationTokenSource cts)
        {
            return await _loadMesService.RunOne(Name, cts);
        }

        public async Task<(bool succeed, string? response)> RunOne(string Name, string request,
            CancellationTokenSource cts)
        {
            return await _loadMesService.RunOne(Name, request, cts);
        }

        //
        public async Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item,
            string request, CancellationTokenSource cts)
        {
            var eachStation = TraceContext.GetParam("EachStation");
            EachStation<Station1>? station1 = eachStation as EachStation<Station1>;

            StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station, request , false);
            (bool succeed, string? response) = await _loadMesService.SendHttp(item, request, cts);
            if (succeed)
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Info, item.Station, response , false);
            }
            else
            {
                StationManager.StationLog(StationLogEnum.UserLog, InfoAndErrorEnum.Error, item.Station, response, false);
            }

            return (succeed, response);
        }

        public async Task<(bool succeed, string? value)> PackRequest(string httpName, CancellationTokenSource cts)
        {
            var eachStation = TraceContext.GetParam("EachStation");
            EachStation<Station1>? station1 = eachStation as EachStation<Station1>;
            station1.Station.Main(cts);
            (bool succeed, string? value) = await _loadMesService.PackRequest(httpName, cts);
            station1.Station.Main(cts);
            return (succeed, value);
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

        public async Task<(bool sueeced, string? result)> DynMessage(string DynName, CancellationTokenSource cts)
        {
            return await _loadMesService.DynMessage(DynName, cts);
        }

        public async Task<(bool succeed, string message)> Transpond(DynCondition model, string response)
        {
            return await _loadMesService.Transpond(model, response);
        }

        public async Task<(bool succeed, string response)> VerityMessage(string message, DynVerify verify,
            CancellationTokenSource cts)
        {
            return await _loadMesService.VerityMessage(message, verify, cts);
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

        public async Task<(bool succeed, string response)> KeyenceReadDM(DynCondition item, CancellationTokenSource cts)
        {
            return await _loadMesService.KeyenceReadDM(item, cts);
        }

        public async Task<(bool succeed, string? result)> KeyenceReadCoid(DynCondition item,
            CancellationTokenSource cts)
        {
            return await _loadMesService.KeyenceReadCoid(item, cts);
        }

        public async Task<(bool succeed, string message)> LateProcess(DynCondition item, string response,
            CancellationTokenSource cts)
        {
            return await _loadMesService.LateProcess(item, response, cts);
        }


        public async Task<(bool succeed, string response)> ScpiSerialAsync(DynCondition item,
            CancellationTokenSource cts)
        {
            return await _loadMesService.ScpiSerialAsync(item, cts);
        }
    }
}