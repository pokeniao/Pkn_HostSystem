using Pkn_HostSystem.Base.Log;
using Pkn_HostSystem.Models.Core;
using Pkn_HostSystem.Models.Windows;

namespace Pkn_HostSystem.Service.LoadMes.Interface
{

    public interface ILoadMesService
    {


        /// <summary>
        ///  触发单个请求
        /// </summary>
        /// <param name="Name">HTTP请求名称</param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool succeed, string? response)> RunOne(string Name, CancellationTokenSource cts);

        /// <summary>
        /// 触发单个请求
        /// </summary>
        /// <param name="Name">HTTP请求名称</param>
        /// <param name="request">请求体</param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool succeed, string? response)> RunOne(string Name, string request, CancellationTokenSource cts);

        /// <summary>
        /// 发送Http任务
        /// </summary>
        /// <param name="item"></param>
        /// <param name="request"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool succeed, string? response)> SendHttp(LoadMesAddAndUpdateWindowModel item,
            string request,
            CancellationTokenSource cts);
        /// <summary>
        /// 包装Request请求
        /// </summary>
        /// <param httpName="httpName"></param>
        Task<(bool succeed, string? value)> PackRequest(string httpName, CancellationTokenSource cts);

        /// <summary>
        /// 嵌入静态内容
        /// </summary>
        /// <param name="request">消息体</param>
        /// <param name="itemKey">填充键</param>
        /// <param name="itemValue">填充值</param>
        /// <returns></returns>
        string StaticMessage(string request, string itemKey, string itemValue);

        /// <summary>
        /// 嵌入静态子内容
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemKey"></param>
        /// <param name="itemKeySon"></param>
        /// <param name="itemValue"></param>
        /// <returns></returns>
        string StaticMessageSon(string request, string itemKey, string itemKeySon, string itemValue);

        /// <summary>
        /// 动态嵌入
        /// </summary>
        /// <param name="request">请求体内容</param>
        /// <param name="DynName">动态嵌入的名称</param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool sueeced, string? result)> DynMessage(string request, string DynName,
            CancellationTokenSource cts, bool noLog = false);
        /// <summary>
        /// 动态嵌入
        /// </summary>
        /// <param name="DynName"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool sueeced, string? result)> DynMessage(string DynName,
            CancellationTokenSource cts, bool noLog = false);

        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="model"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<(bool succeed, string message)> Transpond(DynCondition model, string response, CancellationTokenSource cts);

        /// <summary>
        /// Verity校验方法
        /// </summary>
        /// <param name="message"></param>
        /// <param name="verify"></param>
        /// <returns></returns>
        Task<(bool succeed, string response)> VerityMessage(string message, DynVerify verify, CancellationTokenSource cts);

        /// <summary>
        /// 方法集内容嵌入
        /// </summary>
        /// <param name="request"></param>
        /// <param name="itemValue"></param>
        /// <param name="itemMethodOtherValue"></param>
        /// <returns></returns>
        Task<string> MethodMessage(string request, string itemValue, string itemMethodOtherValue);

        /// <summary>
        /// 时间计算方法 规则: -,5M,5D,5H,5m,5s
        /// </summary>
        /// <param name="itemMethodOtherValue"></param>
        /// <returns></returns>
        DateTime DateTimeDispose(string itemMethodOtherValue);

        /// <summary>
        /// 通过Switch转换
        /// </summary>
        /// <param name="message"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        string SwitchGetMessage(string message, DynCondition item);

        /// <summary>
        /// Socket套接字
        /// </summary>
        /// <param name="item">动态</param>
        /// <param name="parentName">调用的父类名称,用于日志显示</param>
        /// <returns></returns>
        Task<(bool succeed, string response)> ReadTcpMessageAsync(DynCondition item,
            CancellationTokenSource cts);

        /// <summary>
        /// 读线圈
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<(bool succeed, string? result)> ReadCoid(DynCondition item);
        /// <summary>
        /// 读寄存器
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<(bool succeed, string? result)> ReadReg(DynCondition item);

        /// <summary>
        /// 动态获取基恩士上链路内容
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<(bool succeed, string response)> KeyenceReadDM(DynCondition item , CancellationTokenSource cts);
        /// <summary>
        /// 动态获取基恩士上链路内容
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<(bool succeed, string? result)> KeyenceReadCoid(DynCondition item,CancellationTokenSource cts);
        /// <summary>
        /// 执行可选后期处理
        /// </summary>
        /// <param name="item"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        Task<(bool succeed, string message)> LateProcess(DynCondition item, string response, CancellationTokenSource cts);
        /// <summary>
        /// 串口通讯
        /// </summary>
        /// <param name="item"></param>
        /// <param name="cts"></param>
        /// <returns></returns>
        Task<(bool succeed, string response)> ScpiSerialAsync(DynCondition item,
            CancellationTokenSource cts);
    }
}