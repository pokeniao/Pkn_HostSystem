using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace Pkn_HostSystem.Static
{
    public class StaticArrayRegister
    {
        // 真正存数据的数组（线程安全 + Volatile 可用）
        public static object[] DataArrayRegister { get; set; }
        public static List<ConcurrentQueue<object>> DataQueueRegister { get; set; }
        /// <summary>
        /// 内部寄存器,为了响应UI显示
        /// </summary>
        [JsonIgnore]
        public static ObservableCollection<object> ArrayRegister { get; set; }
        /// <summary>
        /// 内部队列 ,队列初始化器,断电保存
        /// </summary>
        [JsonIgnore]
        public static ObservableCollection<ConcurrentQueue<object>> QueueRegister { get; set; }
        /// <summary>
        /// 读内部寄存器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static object ReadRegisterValue(int index)
        {
            return Volatile.Read(ref DataArrayRegister[index]);
        }
        /// <summary>
        /// 写内部寄存器
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public static void WriteRegisterValue(int index, object value)
        {
            Volatile.Write(ref DataArrayRegister[index], value);
            ArrayRegister[index] = value;
        }

        public static void SaveRegister()
        {
            JsonTool<StaticArrayRegister>.Save(new StaticArrayRegister());
        }
    }
}