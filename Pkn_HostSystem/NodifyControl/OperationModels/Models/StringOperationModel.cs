using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class StringOperationModel : ObservableObject, IOperationModel
    {
        /// <summary>
        /// 字符串处理方式
        /// </summary>
        [ObservableProperty]
        private string method = "拼接";

        /// <summary>
        /// 字符串拼接
        /// </summary>
        public string OldMethod { get; set; }
    }
}