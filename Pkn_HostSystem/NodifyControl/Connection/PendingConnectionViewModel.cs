using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private Connector _source;


        public PendingConnectionViewModel(EditorViewModel editor)
        {
            _editor = editor;
        }

        //记录开始连接端子
        [RelayCommand]
        private void Start(Connector source)
        {
            _source = source;
        }

        //预连接结束时判断 并进行连接
        [RelayCommand]
        private void Finish(Connector target)
        {
            //完毕的时候目标不为空,且目标不为自己
            if (target != null && target!= _source)
                _editor.Connect(_source, target);
        }

    }
}