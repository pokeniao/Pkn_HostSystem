using System;
using System.Threading.Tasks;
using Pkn_WinForm.Base;
using Pkn_WinForm.Pojo;
using Pkn_WinForm.Pojo.byd;
using RestSharp;

namespace Pkn_WinForm.Server
{
    public class MesServer
    {
        private ModbusBase modbus = new ModbusBase();
        private LogBase logBase = new LogBase();

        public MesServer()
        {
        }

        public async Task ConnectTcp(UploadSetPojo data)
        {
            modbus.CloseTCP();
            await modbus.OpenTcpMaster(data.plcIp, data.plcPort);
        }

        public async Task CloseTcp()
        {
            modbus.CloseTCP();
        }

        public async Task<ushort[]> getPLCstate(UploadSetPojo data)
        {
            if (!modbus.IsTCPConnect())
            {
                await modbus.OpenTcpMaster(data.plcIp, data.plcPort);
            }

            try
            {
                if (modbus.IsTCPConnect())
                {
                    return await modbus.ReadHoldingRegisters_03(1, (ushort)data.plcStatus, 1);
                }
                else
                {
                    await logBase.WriteLog("连接PLC失败");

                    throw new Exception("连接PLC失败");
                }
            }
            catch (Exception e)
            {
                await logBase.WriteLog("连接读取失败");
                throw;
            }
        }

        public async Task<string> uploadState(int status, UploadSetPojo data)
        {
            RestClient client = new RestClient($"http://{data.MesIp}");
            var request = new RestRequest("/processing/api/product/upload/deviceStatus", Method.Post);
            //var request = new RestRequest("/postTest", Method.Post);

            request.AddJsonBody(new DeviceStatusDataPojo
            {
                deviceCode = data.deviceCode,
                lineCode = data.lineCode,
                stationCode = data.stationCode,
                status = status,
                time = DateTime.Now.ToString()
            });


            RestResponse response = await client.ExecuteAsync(request);
            string message;
            if (response.Content == null)
            {
                message = response.ErrorException.InnerException.Message;
            }
            else
            {
                message = response.Content;
            }

            await logBase.WriteLog(message);
            return message;
        }
        private bool hreadBool = false;
        public async Task hread(UploadSetPojo data)
        {
            if (hreadBool)
            {
                modbus.WriteCoil_05(1, data.hread, true);
                hreadBool = false;
            }
            else
            {
                modbus.WriteCoil_05(1, data.hread, false);
                hreadBool = true;
            }

        }
    }
}