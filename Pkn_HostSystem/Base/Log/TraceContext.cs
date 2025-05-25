namespace Pkn_HostSystem.Base.Log
{
    public static class TraceContext
    {
        /// <summary>
        /// 它类似于 ThreadLocal<T>，但 支持异步编程模型；每个**异步调用链（logical context）**维护自己的一份数据；是 .NET 中实现日志上下文、Tracing、DI 等的底层核心工具之一（比如 OpenTelemetry、HttpContext、LogContext 都是基于它）。
        /// </summary>
        private static AsyncLocal<string> _name = new AsyncLocal<string>();

        public static string Name
        {
            get => _name.Value ?? "未设置";
            set => _name.Value = value;
        }
    }
}