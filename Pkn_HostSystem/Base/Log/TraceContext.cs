namespace Pkn_HostSystem.Base.Log
{
    public static class TraceContext
    {
        /// <summary>
        /// 它类似于 ThreadLocal<T>，但 支持异步编程模型；每个**异步调用链（logical context）**维护自己的一份数据；是 .NET 中实现日志上下文、Tracing、DI 等的底层核心工具之一（比如 OpenTelemetry、HttpContext、LogContext 都是基于它）。
        /// </summary>
        private static AsyncLocal<string> _name = new AsyncLocal<string>();


        private static AsyncLocal<Dictionary<string , dynamic>> _param = new AsyncLocal<Dictionary<string, dynamic>>();


        // static TraceContext()
        // {
        //     Param = new Dictionary<string, dynamic>();
        // }

        public static Dictionary<string, dynamic>? Param
        {
            get => _param.Value;
            set => _param.Value = value;
        }


        public static string Name
        {
            get => _name.Value ?? "NuLL";
            set => _name.Value = value;
        }

        /// <summary>
        /// 获取对应key的参数值,没有返回null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static dynamic GetParam(string key)
        {
            bool tryGetValue = TraceContext.Param.TryGetValue(key, out dynamic value);

            if (tryGetValue)
            {
                return value;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 更新对应key的参数值,没有则添加其中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void UpdateParam(string key , object value)
        {

            if (TraceContext.Param == null)
            {
                
            }

            bool tryGetValue = TraceContext.Param.TryGetValue(key, out _);

            if (tryGetValue)
            {
                TraceContext.Param[key] = value;
            }
            else
            {
                TraceContext.Param.TryAdd(key, value);
            }
        }
    }
}