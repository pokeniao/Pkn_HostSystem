using log4net;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;
using Pkn_HostSystem.Static;

namespace Pkn_HostSystem.Services.Core
{
    /// <summary>
    /// 这个写法在类上加括号, 等同于构造体传入log,并且复制给其内部的属性Log
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="log"></param>
    public class NetWorkTriggerLogic<T>(LogControl<T> log)
    {
        
        public Action RunFunc { get; set; }

        public async Task TriggerLogic(NetWorkTriggerModel model, CancellationTokenSource cts)
        {
            //触发通讯名
            NetWork netWork = GlobalManager.GetNetWork(model.NetworkName);
            if (netWork == null)
            {
                log.Error("获取通讯失败");
            }

            //判断是什么通讯
            switch (netWork.NetworkDetailed.NetMethod)
            {
                case "ModbusTcp":
                    //获取触发位
                    string currentMessage1 = await ModbusTrigger(model);
                    //判断是否触发
                    if (IsTrigger(model.TriggerMessage, currentMessage1))
                    {
                        log.Info($"[{TraceContext.Name}]--ModbusTcp已被触发");
                         RunFunc.Invoke();
                    }

                    break;
                case "ModbusRtu":
                    //获取触发位
                    string currentMessage2 = await ModbusTrigger(model);
                    //判断是否触发
                    if (IsTrigger(model.TriggerMessage, currentMessage2))
                    {
                        log.Info($"[{TraceContext.Name}]--ModbusRtu已被触发");
                        RunFunc.Invoke();
                    }

                    break;
                case "基恩士上位链路通讯":
                    //获取触发位
                    //判断是否触发
                    if (IsTrigger(model.TriggerMessage, await KeyenceHostLinkTrigger(model,cts)))
                    {
                        log.Info($"[{TraceContext.Name}]--基恩士上位链路通讯已被触发");
                        RunFunc.Invoke();
                    }

                    break;
                case "Socket":
                    break;
            }
        }


        /// <summary>
        /// ModbusTcp触发
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<string> ModbusTrigger(NetWorkTriggerModel model)
        {
            //触发通讯名
            NetWork netWork = GlobalManager.GetNetWork(model.NetworkName);
            if (netWork == null)
            {
                log.Error("获取通讯失败");
            }

            //获得ModBase对象
            ModbusBase modbusBase = netWork.ModbusBase;
            //读取寄存器
            ushort[] readHoldingRegisters03 = null;
            try
            {
                readHoldingRegisters03 = await modbusBase.ReadHoldingRegisters_03(
                    byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                    1);
            }
            catch (Exception e)
            {
                log.Error($"[{TraceContext.Name}]--循环读取:触发寄存器:{model.StartAddress},发送错误 :{e}");
                return string.Empty;
            }

            return readHoldingRegisters03[0].ToString();
        }


        private async Task<bool> ModbusTriggerWrite(NetWorkTriggerModel model, bool succeed)
        {
            try
            {
                //触发通讯名
                NetWork netWork = GlobalManager.GetNetWork(model.NetworkName);
                if (netWork == null)
                {
                    log.Error("获取通讯失败");
                }

                //获得ModBase对象
                ModbusBase modbusBase = netWork.ModbusBase;
                if (succeed)
                {
                    log.Info($"[{TraceContext.Name}]--modbus触发 返回成功触发消息:{model.SuccessResponseMessage}");
                    await modbusBase.WriteRegister_06(
                        byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                        ushort.Parse(model.SuccessResponseMessage));
                }
                else
                {
                    log.Error($"[{TraceContext.Name}]--modbus触发 返回失败触发消息:{model.FailResponseMessage}");
                    await modbusBase.WriteRegister_06(
                        byte.Parse(model.StationAddress), ushort.Parse(model.StartAddress),
                        ushort.Parse(model.FailResponseMessage));
                }
            }
            catch (Exception e)
            {
                log.Error($"[{TraceContext.Name}]--触发型Modbus写回失败 ,{e}");
                return false;
            }

            return true;
        }


        private async Task<string> KeyenceHostLinkTrigger(NetWorkTriggerModel model , CancellationTokenSource cts)
        {
            //触发通讯名
            NetWork netWork = GlobalManager.GetNetWork(model.NetworkName);
            if (netWork == null)
            {
                log.Error("获取通讯失败");
            }

            //获取基恩士对象
            KeyenceHostLinkTool netWorkKeyenceHostLinkTool = netWork.KeyenceHostLinkTool;

            (bool item1, ushort item2) =
                await netWorkKeyenceHostLinkTool.ReadDM<ushort>(int.Parse(model.StartAddress), cts, true);

            if (!item1)
            {
                log.Error($"[{TraceContext.Name}]--循环读取:触发寄存器DM:{model.StartAddress} 失败");
                return string.Empty;
            }

            return item2.ToString();
        }


        private async Task<bool> KeyenceHostLinkTriggerWrite(NetWorkTriggerModel model, CancellationTokenSource cts, bool succeed)
        {
            try
            {
                //触发通讯名
                NetWork netWork = GlobalManager.GetNetWork(model.NetworkName);
                if (netWork == null)
                {
                    log.Error("获取通讯失败");
                }

                //获取基恩士对象
                KeyenceHostLinkTool keyenceHostLinkTool = netWork.KeyenceHostLinkTool;
                if (succeed)
                {
                    log.Info($"[{TraceContext.Name}]--KeyenceHostLink触发 返回成功触发消息:{model.SuccessResponseMessage}");
                    return await keyenceHostLinkTool.WriteDM<ushort>(int.Parse(model.StartAddress),
                        ushort.Parse(model.SuccessResponseMessage), cts);
                }
                else
                {
                    log.Error($"[{TraceContext.Name}]--KeyenceHostLink触发 返回失败触发消息:{model.FailResponseMessage}");
                    return await keyenceHostLinkTool.WriteDM<ushort>(int.Parse(model.StartAddress),
                        ushort.Parse(model.FailResponseMessage), cts);
                }
            }
            catch (Exception e)
            {
                log.Error($"[{TraceContext.Name}]--KeyenceHostLink触发写回失败 ,{e}");
                return false;
            }

            return true;
        }

        public bool IsTrigger(string triggerMessage, string currentMessage)
        {
            if (triggerMessage == currentMessage)
                return true;
            return false;
        }
    }
}