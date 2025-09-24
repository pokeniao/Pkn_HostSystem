using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Views.Pages;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Pkn_HostSystem.ViewModels.Page
{
    public partial class S7ToolViewModel:ObservableRecipient
    {
        public SnackbarService SnackbarService { get; set; }
        public LogControl<S7ToolViewModel> Log;
        public S7ToolModel S7ToolModel { get; set; }

        public S7Base S7Base { get; set; } = new S7Base();

        /// <summary>
        /// Ip显示集合
        /// </summary>
        [ObservableProperty] private List<string> ips = ModbusBase.GetIpAddress().ToList();


        public List<S7MethodEnum> Methods { get; set; } =
            Enum.GetValues(typeof(S7MethodEnum)).Cast<S7MethodEnum>().ToList();


        public S7ToolViewModel()
        {

            SnackbarService = new SnackbarService();
            Log = new LogControl<S7ToolViewModel>(SnackbarService);
            //Model初始化
            S7ToolModel = new S7ToolModel();
        }

        [RelayCommand]
        public async void Connect()
        {
            if (S7ToolModel.RunButton.Equals("连接"))
            {
                (bool succeed, string message) = await S7Base.Connect(S7ToolModel.CpuType, S7ToolModel.Ip, S7ToolModel.Rack, S7ToolModel.Slot,
                    S7ToolModel.Port);
                if (succeed)
                {

                    S7ToolModel.RunButton = "断开";
                }
                else
                {
                    Log.ErrorAndShowTask(message);
                }
            }
            else
            {
                S7Base.Close();
                S7ToolModel.RunButton = "连接";
            }
        }

        [RelayCommand]
        public async void Read()
        {
            //检测是否连接成功
            // if (!S7Base.IsConnected)
            // {
            //     Log.ErrorAndShowTask("未连接请先连接通讯");
            //     return;
            // }
            S7MethodEnum s7MethodEnum = S7ToolModel.Method;
            string dataArea = S7ToolModel.DataArea;
            int numberData = S7ToolModel.NumberData;
            string offset = S7ToolModel.Offset;
            int num = S7ToolModel.Num;

            if (string.IsNullOrEmpty(dataArea))
            {
                Log.ErrorAndShowTask("内存区为空,请选择内存区");
                return;
            }

            switch (s7MethodEnum)
            {
                case S7MethodEnum.位:
                    await S7Base.Read<bool>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.Byte8位:
                    await S7Base.Read<byte>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.无符号16位:
                    await S7Base.Read<ushort>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.有符号16位:
                    await S7Base.Read<short>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.无符号32位:
                    await S7Base.Read<uint>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.有符号32位:
                    await S7Base.Read<int>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.浮点数:
                    await S7Base.Read<float>(s7MethodEnum, dataArea, numberData, offset, num);
                    break;
                case S7MethodEnum.字符串:
                    break;
            }
        }

        [RelayCommand]
        public async void Send()
        {
            //检测是否连接成功
            if (!S7Base.IsConnected)
            {
                Log.ErrorAndShowTask("未连接请先连接通讯");
                return;
            }


        }

        #region 弹窗SnackbarService

        public void setSnackbarPresenter(SnackbarPresenter snackbarPresenter)
        {
            SnackbarService.SetSnackbarPresenter(snackbarPresenter);
        }

        #endregion
    }
}