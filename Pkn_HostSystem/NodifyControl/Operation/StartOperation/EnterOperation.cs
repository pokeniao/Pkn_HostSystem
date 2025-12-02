using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.NodifyControl.Node;
using Pkn_HostSystem.NodifyControl.Operation.Interface;
using Pkn_HostSystem.NodifyControl.Operation.MiddleOperation;
using Pkn_HostSystem.Views.UserControls.NodeOperation;
using System;
using System.Windows;

namespace Pkn_HostSystem.NodifyControl.Operation.StartOperation
{
    public class EnterOperation : IStartOperation
    {
        private readonly Action _func;

        public EnterOperationNode node;

        public LogControl<AddOperation> Log;
        public EnterOperation(EnterOperationNode _node)
        {
            node = _node;
            Log = new LogControl<AddOperation>();
            _func = Func;
        }

        private void Func()
        {
            
        }


        public FrameworkElement GetConfigView()
        {
            var view = new EnterOperationUserControl();
            view.DataContext = node;
            return view;
        }

        public void Execute() => _func.Invoke();
    }
}