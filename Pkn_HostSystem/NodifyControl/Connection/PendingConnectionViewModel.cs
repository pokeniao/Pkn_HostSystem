using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Pkn_HostSystem.Base.Enum;
using Pkn_HostSystem.NodifyControl.Editor;
using Pkn_HostSystem.NodifyControl.Node.Connector;

namespace Pkn_HostSystem.NodifyControl.Connection
{
    /// <summary>
    /// 预连接模块
    /// </summary>
    public partial class PendingConnectionViewModel : ObservableObject
    {
        //编辑器对象
        private readonly EditorViewModel _editor;

        //开始 连接端子
        private MyConnector _source;


        public PendingConnectionViewModel(EditorViewModel editor)
        {
            _editor = editor;
        }

        //记录开始连接端子
        [RelayCommand]
        private void Start(MyConnector source)
        {
            _source = source;
        }

        //预连接结束时判断 并进行连接
        [RelayCommand]
        private void Finish(MyConnector target)
        {
            //完毕的时候目标不为空,且目标不为自己
            if (target == null || target == _source)
            {
                return;
            }

            //自己的输入不能连自己的输出
            if (_source.NodeId == target.NodeId)
            {
                return;
            }

            //输入不能连输入,输出不能连输出
            if (_source.ConnectorType == ConnectorTypeEnum.Input && target.ConnectorType == ConnectorTypeEnum.Input)
            {
                return;
            }

            if (_source.ConnectorType == ConnectorTypeEnum.Output && target.ConnectorType == ConnectorTypeEnum.Output)
            {
                return;
            }

            //输入 不能 指向 输出

            if (_source.ConnectorType == ConnectorTypeEnum.Input && target.ConnectorType == ConnectorTypeEnum.Output)
            {
                return;
            }
        


            _editor.Connect(_source, target);
        }
    }
}