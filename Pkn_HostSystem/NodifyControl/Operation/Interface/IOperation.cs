using Pkn_HostSystem.Models.Core;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operation.Interface
{
    public interface IOperation
    {
        void Execute();
        /// <summary>
        /// 用于显示参数页面
        /// </summary>
        /// <returns></returns>
        FrameworkElement GetConfigView();

    }
}
