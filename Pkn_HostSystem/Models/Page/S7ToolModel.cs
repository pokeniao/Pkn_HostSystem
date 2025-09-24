using CommunityToolkit.Mvvm.ComponentModel;
using DynamicData;
using Pkn_HostSystem.Base;
using Pkn_HostSystem.Base.Enum;
using S7.Net;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.Models.Page
{
    public partial class S7ToolModel : ObservableObject
    {
        /// <summary>
        /// CPU类型
        /// </summary>
        [ObservableProperty] private CpuType cpuType;

        /// <summary>
        /// Ip
        /// </summary>
        [ObservableProperty] private string ip;

        /// <summary>
        /// 端口号
        /// </summary>
        [ObservableProperty] private int port =102;

        /// <summary>
        /// 机架号
        /// </summary>
        [ObservableProperty] private int rack = 0;
        /// <summary>
        /// 槽号
        /// </summary>
        [ObservableProperty] private int slot = 1;

        [ObservableProperty] private string runButton = "连接";

        /// <summary>
        /// 读取/写入方式
        /// </summary>
        [NotifyPropertyChangedFor(nameof(showOffset))] // 只用于属性
        [ObservableProperty] private S7MethodEnum method = S7MethodEnum.位;
        partial void OnMethodChanged(S7MethodEnum value)
        {
            dataAreaList.Clear();
            dataAreaList .AddRange(S7Base.GetDataArea(method)); 
        }
        /// <summary>
        /// 内存区
        /// </summary>
        [NotifyPropertyChangedFor(nameof(showOffset))]
        [ObservableProperty] private string dataArea;
        /// <summary>
        /// 内存区地址
        /// </summary>
        [ObservableProperty] private int numberData = 0;

        /// <summary>
        /// 显示可选内存区
        /// </summary>
        [ObservableProperty] private ObservableCollection<string> dataAreaList = new ObservableCollection<string>(["DB", "M", "I", "Q"]);

        /// <summary>
        /// 偏移量
        /// </summary>
        
        [ObservableProperty] private string offset = "0";
        /// <summary>
        /// 数量
        /// </summary>
        [ObservableProperty] private int num = 1;

        /// <summary>
        /// 字符串
        /// </summary>
        [ObservableProperty] private string asciiStringMessage;



        /// <summary>
        /// 用于显示当前接收到的信息
        /// </summary>
        [ObservableProperty] private string acceptMessageText;
        /// <summary>
        /// 用于显示记录当前发送的信息
        /// </summary>

        [ObservableProperty] private string sendMessageText;


        public Visibility showOffset =>
            method == S7MethodEnum.位 || dataArea == "DB" ? Visibility.Visible : Visibility.Collapsed;
    }
}