using Azure;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Views.Pages;
using System.Threading;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class SerialToolViewModel :ObservableRecipient
    {


        public SnackbarService SnackbarService { get; set; }
        public LogControl<SerialToolViewModel> Log;
        public SerialToolModel SerialToolModel { get; set; }


        public event Action<string> OnMessageReceived ;


        public ScpiSerialTool ScpiSerialTool { get; set; }

        public SerialToolViewModel()
        {
            SnackbarService = new SnackbarService();
            Log = new LogControl<SerialToolViewModel>(SnackbarService);
            //Model初始化
            SerialToolModel = new SerialToolModel();

            ScpiSerialTool = new ScpiSerialTool();
        }


        [RelayCommand]
        public void Connect(SerialToolPage page)
        {
            if (SerialToolModel.ConnectButton == "连接")
            {
                try
                {
                    bool open = ScpiSerialTool.Open(SerialToolModel.Com,int.Parse(SerialToolModel.BaudRate), SerialToolModel.Paritie , int.Parse(SerialToolModel.DataBits) ,SerialToolModel.StopBits , SerialToolModel.TimeOut , SerialToolModel.NewLine);

                    if (!open)
                    {
                        Log.ErrorAndShowTask($"连接失败");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.ErrorAndShowTask($"连接发生错误:{e}");
                    return;
                }
                SerialToolModel.ConnectButton = "断开";
            }
            else
            {
                ScpiSerialTool.Close();
                SerialToolModel.ConnectButton = "连接";
            }
        }

        private CancellationTokenSource ctsRead;
        [RelayCommand]
        public void WhileRead(SerialToolPage page)
        {

            if (SerialToolModel.WhileReadButton.Equals("循环读取"))
            {
                if (ScpiSerialTool.serialPort == null || ! ScpiSerialTool.IsOpen)
                {
                    Log.WarningAndShowTask("请先连接串口");
                    return;
                }
                ctsRead = new();
                OnMessageReceived = s =>
                {

                    SerialToolModel.AcceptMessageText += $"[{DateTime.Now:HH:mm:ss}]--{s}\r\n";
                };
                ScpiSerialTool.OnMessageReceived += OnMessageReceived;

                ScpiSerialTool.ReadLineEvenTask(ctsRead);
                page.OneReadButton.IsEnabled = false;
                SerialToolModel.WhileReadButton = "关闭读取";
            }
            else
            {
                
                ctsRead.Cancel();
                ScpiSerialTool.OnMessageReceived -= OnMessageReceived;
                OnMessageReceived = null;
                page.OneReadButton.IsEnabled = true;
                SerialToolModel.WhileReadButton = "循环读取";
            }
            
        }

        [RelayCommand]
        public async void OneRead(SerialToolPage page)
        {
            if (ScpiSerialTool.serialPort == null || !ScpiSerialTool.IsOpen)
            {
                Log.WarningAndShowTask("请先连接串口");
                return;
            }
            (bool succeed, string? response) = await ScpiSerialTool.ReadLine();
            if (succeed)
            {
                SerialToolModel.AcceptMessageText += $"[{DateTime.Now:HH:mm:ss}]--{response}\r\n";
            }
            else
            {
                Log.ErrorAndShowTask($"{response}");
            }
            
        }

        [RelayCommand]
        public async void WriteLine()
        {
            if (ScpiSerialTool.serialPort == null || !ScpiSerialTool.IsOpen)
            {
                Log.WarningAndShowTask("请先连接串口");
                return;
            }

            await ScpiSerialTool.WriteLine(SerialToolModel.SendMessageText);


        }
        [RelayCommand]

        public async void Write()
        {
            if (ScpiSerialTool.serialPort == null || !ScpiSerialTool.IsOpen)
            {
                Log.WarningAndShowTask("请先连接串口");
                return;
            }

            await ScpiSerialTool.Write(SerialToolModel.SendMessageText);
        }

        [RelayCommand]
        public async void WriteAndWaitResponse()
        {
            if (ScpiSerialTool.serialPort == null || !ScpiSerialTool.IsOpen)
            {
                Log.WarningAndShowTask("请先连接串口");
                return;
            }

            (bool succeed, string? response) = await ScpiSerialTool.WriteLineAndWaitResponse(SerialToolModel.SendMessageText,SerialToolModel.WriteTimeOut);

            if (succeed)
            {
                SerialToolModel.AcceptMessageText += $"[{DateTime.Now:HH:mm:ss}]--{response}\r\n";
            }
            else
            {
                Log.ErrorAndShowTask("读取超时");
            }
        }


        [RelayCommand]
        public void Clear(SerialToolPage page)
        {
            SerialToolModel.AcceptMessageText = "";
        }

        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }
        #endregion
    }
}