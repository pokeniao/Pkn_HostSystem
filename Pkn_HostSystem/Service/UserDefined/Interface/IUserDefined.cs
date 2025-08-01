namespace Pkn_HostSystem.Service.UserDefined.Interface
{
    public interface IUserDefined
    {


        /// <summary>
        /// 执行主入口
        /// </summary>
        /// <returns></returns>
       Task<(bool Succeed, object Return)>  Main(CancellationTokenSource cts, params object[] args);

        /// <summary>
        /// 返回错误的信息
        /// </summary>
        /// <returns></returns>
        string ErrorMessage();
    }
}