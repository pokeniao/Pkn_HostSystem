using DynamicData.Aggregation;
using log4net;
using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.ViewModels.Page;

namespace Pkn_HostSystem.NodifyControl.Operation
{
    public class AddOperation : IOperation
    {
        private readonly Func<object[], object[]> _func;


        public AddOperation()
        {
                
            _func = (operands) =>
            {
                DesignViewModel.Log.Info($"执行{operands[0]}+{operands[1]}");
                double o = double.TryParse(operands[0].ToString(), out double d1) &&
                           double.TryParse(operands[1].ToString(), out double d2)
                    ? d2 + d1
                    : 0;
                DesignViewModel.Log.Info($"结果{o}");
                return new object[] { o };
            };
        }

        public object[] Execute(params object[] operands) => _func.Invoke(operands);
    }
}