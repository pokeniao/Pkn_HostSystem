namespace Pkn_HostSystem.Service.Stations.Interface
{
    public interface IStation
    {
        /// <summary>
        /// 主入口
        /// </summary>
        /// <returns></returns>
        Task<(bool Succeed, object Return)> Main(string station, CancellationTokenSource cts);
    }
}