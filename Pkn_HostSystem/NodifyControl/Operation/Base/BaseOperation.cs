using Pkn_HostSystem.Models.Core;

namespace Pkn_HostSystem.NodifyControl.Operation.Base
{
    public class BaseOperation
    {
        /// <summary>
        /// 动态遍历 动态参数的值
        /// </summary>
        public string GetParamValue(OperationParam operationParam)
        {

            if (operationParam.ParamMethod.Equals("动态获取"))
            {

                return GetParamValue(operationParam.DynParam);

            }
            else
            {
                return operationParam.ParamValue;
            }
        }
    }
}