using CommunityToolkit.Mvvm.ComponentModel;
using Pkn_HostSystem.NodifyControl.OperationModels.Interface;

namespace Pkn_HostSystem.NodifyControl.OperationModels.Models
{
    public partial class StringOperationModel : ObservableObject, IOperationModel
    {

        /// <summary>
        /// 字符串处理方式
        /// </summary>
        [ObservableProperty] private string method;
        /// <summary>
        /// 凭借方式
        /// </summary>
        [ObservableProperty] private string message;

        /// <summary>
        /// 切割符
        /// </summary>
        [ObservableProperty] private string slicing;


        /// <summary>
        /// 切割符起始位置
        /// </summary>
        [ObservableProperty] private string slicingStartIndex ="0";

        /// <summary>
        /// 切割符结束位置
        /// </summary>
        [ObservableProperty] private string slicingEndIndex = "1";


    }
}