using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.NodifyControl.ParamOperationModel;

namespace Pkn_HostSystem.NodifyControl.Operation.Base
{
    public class BaseOperation
    {
        /// <summary>
        /// 动态遍历 动态参数的值
        /// </summary>
        public string GetParamValue(OperationParamModel operationParamModel)
        {

            if (operationParamModel.ParamMethod.Equals("动态获取"))
            {

                return GetParamValue(operationParamModel.DynParam);

            }
            else
            {
                return operationParamModel.ParamValue;
            }
        }
    }
}