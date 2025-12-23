using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;
using System.Collections.ObjectModel;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class HttpOperationModel : ObservableObject, IOperationModel
    {
        /// <summary>
        /// http请求方式
        /// </summary>
        [ObservableProperty] private string httpMethod = "POST";

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowFromBodysVisibility))]
        [NotifyPropertyChangedFor(nameof(ShowNotFromBodysVisibility))]
        private string contentType = "application/json";
        /// <summary>
        /// Http请求路径
        /// </summary>
        [ObservableProperty] private string httpPath;
        /// <summary>
        /// 接口路径
        /// </summary>
        [ObservableProperty] private string apiPath;

        /// <summary>
        /// 创建Http请求头
        /// </summary>
        [ObservableProperty] private ObservableCollection<HttpItem> httpHeaders = new();

        /// <summary>
        /// 请求体内容
        /// </summary>
        [ObservableProperty] private string httpBody;


        public Visibility ShowFromBodysVisibility => contentType == "application/x-www-form-urlencoded"
            ? Visibility.Visible : Visibility.Collapsed;
        public Visibility ShowNotFromBodysVisibility => contentType != "application/x-www-form-urlencoded"
            ? Visibility.Visible : Visibility.Collapsed;
        [ObservableProperty] private ObservableCollection<HttpItem> fromBodys = new ObservableCollection<HttpItem>();

    }
}