using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operations.Interface
{
    public interface IOperation
    {


        Task Execute(CancellationTokenSource cts);
        /// <summary>
        /// 用于显示参数页面
        /// </summary>
        /// <returns></returns>
        FrameworkElement GetConfigView();

    }
}
