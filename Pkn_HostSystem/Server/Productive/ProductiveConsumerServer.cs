using DynamicData;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Pojo.Page.HomePage;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Server.Productive
{
    public class ProductiveConsumerServer
    {
        /// <summary>
        /// 消费队列
        /// </summary>
        public BlockingCollection<List<ushort>> Queue { get; set; }

        /// <summary>
        /// 一次消息获取方式
        /// </summary>
        public ObservableCollection<ProductiveDetailed> MessageList { get; set; }

        /// <summary>
        /// 生产者通讯方式
        /// </summary>
        public NetWork ProductiveNetWork { get; set; }

        /// <summary>
        /// 消费一个产品
        /// </summary>
        public NetWork ConsumeNetWork { get; set; }

        public LogBase<ProductiveConsumerServer> Log = new LogBase<ProductiveConsumerServer>();


        public ProductiveConsumerServer(BlockingCollection<List<ushort>> _Queue,
            ObservableCollection<ProductiveDetailed> _MessageList, NetWork _ProductiveNetWork, NetWork _ConsumeNetWork)
        {
            Queue = _Queue;
            MessageList = _MessageList;
            ProductiveNetWork = _ProductiveNetWork;
            ConsumeNetWork = _ConsumeNetWork;
        }

        public async Task<bool> Production()
        {
            //判断当前的通讯模式
            try
            {
                switch (ProductiveNetWork.NetworkDetailed.NetMethod)
                {
                    case "ModbusTcp":
                        List<ushort> ushorts = new List<ushort>();
                        ModbusBase modbusBase = ProductiveNetWork.ModbusBase;
                        foreach (var detailed in MessageList)
                        {
                            ushort[] holdingRegisters03 = await modbusBase.ReadHoldingRegisters_03(
                                byte.Parse(detailed.ProducterStationAddress),
                                ushort.Parse(detailed.ProducterStartAddress), ushort.Parse(detailed.ProducterLength));
                            ushorts.Add(holdingRegisters03);
                        }

                        Queue.Add(ushorts);
                        break;
                    case "ModbusRcp":
                        break;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> Consume()
        {
            //消费
            //判断当前的通讯模式
            try
            {
                switch (ConsumeNetWork.NetworkDetailed.NetMethod)
                {
                    case "ModbusTcp":
                        ModbusBase modbusBase = ConsumeNetWork.ModbusBase;

                        if (Queue.TryTake(out List<ushort> ushorts))
                        {
                            int num = 0;
                            int curPos = 0;
                            int endPos = 0;
                            foreach (ProductiveDetailed detailed in MessageList)
                            {
                                //需要读取出来的长度
                                num = int.Parse(detailed.ProducterLength);
                                endPos = num + curPos;
                                List<ushort> loadUshorts = new List<ushort>();
                                for (; curPos < endPos; curPos++)
                                {
                                    loadUshorts.Add(ushorts[curPos]);
                                }

                                //发送
                                await modbusBase.WriteRegisters_10(byte.Parse(detailed.ConsumerStationAddress),
                                    ushort.Parse(detailed.ConsumerStartAddress), loadUshorts.ToArray());
                            }
                        }
                        else
                        {
                            Log.Error("队列中无数据");
                        }

                        break;
                    case "ModbusRcp":
                        break;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }
    }
}