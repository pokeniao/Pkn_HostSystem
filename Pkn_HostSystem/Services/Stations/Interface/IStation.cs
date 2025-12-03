namespace Pkn_HostSystem.Services.Stations.Interface
{
    public interface IStation
    {
        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        Task<(bool succeed, string message)> Main(CancellationTokenSource cts);
    }
}