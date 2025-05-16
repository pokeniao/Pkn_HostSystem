using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using DynamicData.Binding;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Service.Productive;
using Pkn_HostSystem.Static;
using Pkn_HostSystem.Views.Pages;
using System.Collections.ObjectModel;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class ProductiveViewModel : ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogBase<ProductiveViewModel> Log;
        public ProductiveModel ProductiveModel { get; set; }

        public ProductiveViewModel()
        {
            ProductiveModel = AppJsonTool<ProductiveModel>.Load();

            if (ProductiveModel == null)
            {
                //Model初始化
                ProductiveModel = new ProductiveModel()
                {
                    Productives = new ObservableCollection<Productive>(),
                    ProducerList = new ObservableCollectionExtended<NetWork>(),
                    ConsumerList = new ObservableCollectionExtended<NetWork>()
                };
            }
            else
            {
            }

            SnackbarService = new SnackbarService();
            Log = new LogBase<ProductiveViewModel>(SnackbarService);
            GlobalMannager.NetWorkDictionary.Connect().Bind(ProductiveModel.ProducerList).Subscribe();
            GlobalMannager.NetWorkDictionary.Connect().Bind(ProductiveModel.ConsumerList).Subscribe();
        }

        #region 执行运行

        [RelayCommand]
        public async Task RunProductionAndConsumer(ProductivePage page)
        {
            //循环触发进程
            //1. 选中当前行数据
            Productive? item = page.PruAndConsumerDataGrid.SelectedItem as Productive;

            //2. 判断是否循环触发,还是消息触发的方式
            if (item.Run)
            {
                TriggerCyc(item);
            }
            //3. 停止任务逻辑
            else
            {
                //停止
                item.ctsProductive.Cancel();
                item.ctsConsumer.Cancel();
                item.ReConnectionCts.Cancel();
                item.ctsProductive = new CancellationTokenSource();
                item.ctsConsumer = new CancellationTokenSource();
                item.ReConnectionCts = new CancellationTokenSource();
                item.TaskProductive = null;
                item.TaskConsumer = null;
                Log.SuccessAndShow("关闭成功", $"{nameof(ProductiveViewModel)}--{item.Name}--生产者消费者模式关闭成功");
            }
        }

        #endregion

        #region 消息触发

        /// <summary>
        /// 触发型
        /// </summary>
        public void TriggerCyc(Productive item)
        {
            //1. 创建令牌
            item.ctsProductive = new CancellationTokenSource();
            item.ctsConsumer = new CancellationTokenSource();
            item.ReConnectionCts = new CancellationTokenSource();
            //2. 读取当前的通讯模式
            HomePageViewModel viewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
            //2.1获得所有通讯
            ObservableCollection<NetworkDetailed> networkDetaileds = viewModel.HomePageModel.SetConnectDg;
            //生产者与消费者详细信息
            NetworkDetailed producerNetworkDetailed = null;
            NetworkDetailed consumerNetworkDetailed = null;
            foreach (var detailed in networkDetaileds)
            {
                if (item.ProducerName == detailed.Name)
                {
                    producerNetworkDetailed = detailed;
                }

                if (item.ConsumerName == detailed.Name)
                {
                    consumerNetworkDetailed = detailed;
                }
            }

            //3. 获取网络
            NetWork netWorkProducer;
            NetWork netWorkConsumer;
            try
            {
                netWorkProducer = GlobalMannager.NetWorkDictionary.Lookup(producerNetworkDetailed?.Id).Value;
                netWorkConsumer = GlobalMannager.NetWorkDictionary.Lookup(consumerNetworkDetailed?.Id).Value;
            }
            catch (Exception e)
            {
                try
                {
                    Log.ErrorAndShowTask("启动失败,网络未开启", $"{nameof(ProductiveViewModel)}--{item.Name}--:启动失败,网络未开启");
                }
                catch (Exception exception)
                {
                    Log.Error($"{nameof(ProductiveViewModel)}--{item.Name}--启动失败,网络未开启");
                }

                item.Run = false;
                return;
            }

            //4.创建生产者消费者服务
            ProductiveConsumerService productiveConsumerService =
                new ProductiveConsumerService(item.Queue, item.MessageList, netWorkProducer, netWorkConsumer,
                    item.ConsumerName, item.ProducerName);
            //5. 创建单利任务
            item.TaskProductive = new Lazy<Task>(() => RunTrigger(item.ctsProductive, netWorkProducer,
                item.ProductiveStationAddress, item.ProductiveStartAddress, item.ProductiveTriggerValue,
                item.ProductiveTriggerCyc, true, productiveConsumerService));
            item.TaskConsumer = new Lazy<Task>(() => RunTrigger(item.ctsConsumer, netWorkConsumer,
                item.ConsumerStationAddress, item.ConsumerStartAddress, item.ConsumerTriggerValue,
                item.ConsumerTriggerCyc, false, productiveConsumerService));
            item.ReConnectionTask = new Lazy<Task>(() => ReConnection(item.ReConnectionCts, item, productiveConsumerService));
            //6. 运行
            Task task = item.TaskProductive.Value;
            Task task2 = item.TaskConsumer.Value;
            Task value = item.ReConnectionTask.Value;
            Log.SuccessAndShow("启动成功", $"{nameof(ProductiveViewModel)}--{item.Name}--生产者消费者模式启动成功");
        }


        public async Task ReConnection(CancellationTokenSource cts, Productive item, ProductiveConsumerService productiveConsumerService)
        {
            bool netWorkProducerReConnection = false;
            bool netWorkConsumerReConnection = false;
            //循环判断是否断线
            while (!cts.Token.IsCancellationRequested)
            {
                HomePageViewModel viewModel = Ioc.Default.GetRequiredService<HomePageViewModel>();
                //2.1获得所有通讯
                ObservableCollection<NetworkDetailed> networkDetaileds = viewModel.HomePageModel.SetConnectDg;
                //生产者与消费者详细信息
                NetworkDetailed producerNetworkDetailed = null;
                NetworkDetailed consumerNetworkDetailed = null;
                foreach (var detailed in networkDetaileds)
                {
                    if (item.ProducerName == detailed.Name)
                    {
                        producerNetworkDetailed = detailed;
                    }

                    if (item.ConsumerName == detailed.Name)
                    {
                        consumerNetworkDetailed = detailed;
                    }
                }
                //3. 获取网络
                NetWork netWorkProducer = null;
                NetWork netWorkConsumer = null;

                try
                {
                    netWorkProducer = GlobalMannager.NetWorkDictionary.Lookup(producerNetworkDetailed?.Id).Value;
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(ProductiveViewModel)}--{item.Name}--生产者对象连接失败,请开启或检查连接");
                    netWorkProducerReConnection = true;//获取不到的时候,需要进行重连
                    item.ctsProductive.Cancel();
                    item.ctsProductive = new CancellationTokenSource();
                }
                try
                {
                    netWorkConsumer = GlobalMannager.NetWorkDictionary.Lookup(consumerNetworkDetailed?.Id).Value;
                }
                catch (Exception e)
                {
                    Log.Error($"{nameof(ProductiveViewModel)}--{item.Name}--消费者对象连接失败,请开启或检查连接");
                    netWorkConsumerReConnection = true;//获取不到的时候,需要进行重连
                    item.ctsConsumer.Cancel();
                    item.ctsConsumer = new CancellationTokenSource();
                }

                //需要进行重连
                if (netWorkProducerReConnection == true && netWorkProducer != null)
                {
                    productiveConsumerService.ProductiveNetWork = netWorkProducer;

                    item.TaskProductive = new Lazy<Task>(() => RunTrigger(item.ctsProductive, netWorkProducer,
                        item.ProductiveStationAddress, item.ProductiveStartAddress, item.ProductiveTriggerValue,
                        item.ProductiveTriggerCyc, true, productiveConsumerService));
                    _ = item.TaskProductive.Value;
                    netWorkProducerReConnection = false;
                }

                if (netWorkConsumerReConnection == true && netWorkConsumer != null)
                {
                    productiveConsumerService.ConsumeNetWork = netWorkConsumer;
                    item.TaskConsumer = new Lazy<Task>(() => RunTrigger(item.ctsConsumer, netWorkConsumer,
                        item.ConsumerStationAddress, item.ConsumerStartAddress, item.ConsumerTriggerValue,
                        item.ConsumerTriggerCyc, false, productiveConsumerService));
                    _ = item.TaskConsumer.Value;
                    netWorkConsumerReConnection = false;
                }

                await Task.Delay(100);
            }
        }

        public async Task RunTrigger(CancellationTokenSource cts, NetWork netWork, string stationAddress,
            string startAddress, string triggerValue, string triggerCyc, bool isProductive,
            ProductiveConsumerService productiveConsumerService)
        {
            //1.启动后消息触发循环
            while (!cts.Token.IsCancellationRequested)
            {
                //尝试判断是否连接,是否需要从连


                //2. 判读更具什么进行的通讯
                switch (netWork.NetworkDetailed.NetMethod)
                {
                    case "ModbusTcp":
                        //获取触发位
                        string currentMessage1 = await ModbusTcpTrigger(netWork.ModbusBase, byte.Parse(stationAddress),
                            ushort.Parse(startAddress));

                        if (currentMessage1 == null)
                        {
                        }

                        //判断是否触发
                        if (IsTrigger(triggerValue, currentMessage1))
                        {
                            bool success = false;
                            //发送消息队列
                            if (isProductive)
                            {
                                success = await productiveConsumerService.Production();
                            }
                            else
                            {
                                success = await productiveConsumerService.Consume(cts);
                            }

                            //完成后给触发位停止
                            if (success)
                            {
                                await ModbusTcpTriggerWrite(netWork.ModbusBase, byte.Parse(stationAddress),
                                    ushort.Parse(startAddress), true);
                            }
                            else
                            {
                                await ModbusTcpTriggerWrite(netWork.ModbusBase, byte.Parse(stationAddress),
                                    ushort.Parse(startAddress), false);
                            }
                        }

                        break;
                    case "ModbusRtu":
                        //获取触发位
                        string currentMessage2 = await ModbusTcpTrigger(netWork.ModbusBase, byte.Parse(stationAddress),
                            ushort.Parse(startAddress));
                        //判断是否触发
                        if (IsTrigger(triggerValue, currentMessage2))
                        {
                            //发送消息队列


                            //完成后给触发位停止
                            if (true)
                            {
                                await ModbusTcpTriggerWrite(netWork.ModbusBase, byte.Parse(stationAddress),
                                    ushort.Parse(startAddress), true);
                            }
                            else
                            {
                                await ModbusTcpTriggerWrite(netWork.ModbusBase, byte.Parse(stationAddress),
                                    ushort.Parse(startAddress), false);
                            }
                        }

                        break;
                }

                await Task.Delay(int.Parse(triggerCyc), cts.Token);
            }
        }

        private async Task<string> ModbusTcpTrigger(ModbusBase modbusBase, byte staionAddress, ushort startAddress)
        {
            //读取寄存器
            ushort[] readHoldingRegisters03;
            try
            {
                readHoldingRegisters03 = await modbusBase.ReadHoldingRegisters_03(
                    staionAddress, startAddress,
                    1);
            }
            catch (Exception e)
            {
                Log.Error("ModbusTcpTrigger--readHoldingRegisters03--modbus执行失败,TCP或RTU未连接上");
                return null;
            }
            return readHoldingRegisters03[0].ToString();
        }

        private async Task<bool> ModbusTcpTriggerWrite(ModbusBase modbusBase, byte staionAddress, ushort startAddress,
            bool succeed)
        {
            try
            {
                if (succeed)
                {
                    await modbusBase.WriteRegister_06(
                        staionAddress, startAddress, 2);
                }
                else
                {
                    await modbusBase.WriteRegister_06(
                        staionAddress, startAddress, 3);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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

        #endregion

        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion

        #region 保存程序

        [RelayCommand]
        public void Save()
        {
            AppJsonTool<ProductiveModel>.Save(ProductiveModel);
        }

        #endregion
    }
}