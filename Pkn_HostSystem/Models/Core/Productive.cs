using CommunityToolkit.Mvvm.ComponentModel;
using Newtonsoft.Json;
using Pkn_HostSystem.Base;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;


namespace Pkn_HostSystem.Models.Core
{
    public partial class Productive:ObservableObject
    {
        /// <summary>
        /// 唯一标识名称
        /// </summary>
        public string Name { get; set; } = new SnowflakeIdGenerator(1,1).GetId().ToString();

        /// <summary>
        /// 生产者名
        /// </summary>
        public string ProducerName { get; set; }

        /// <summary>
        /// 消费者名
        /// </summary>
        public string ConsumerName { get; set; }

        /// <summary>
        /// 消息体
        /// </summary>
        public ObservableCollection<ProductiveDetailed> MessageList { get; set; } = new ObservableCollection<ProductiveDetailed>();

        #region 生产者触发
        /// <summary>
        /// 生产者触发字符串
        /// </summary>
        public string ProductiveTriggerValue { get; set; } = "1";

        /// <summary>
        /// 触发循环读取时间
        /// </summary>
        public string ProductiveTriggerCyc { get; set; } = "100";

        /// <summary>
        /// 生产者触发成功返回
        /// </summary>
        public string ProductiveTriggerSucceed { get; set; } = "2";

        /// <summary>
        /// 生产者触发失败返回
        /// </summary>
        public string ProductiveTriggerFail { get; set; } = "3";

        /// <summary>
        /// 生产者触发的站地址
        /// </summary>
        public string ProductiveStationAddress { get; set; } = "1";

        /// <summary>
        /// 生产者触发的起始地址
        /// </summary>
        public string ProductiveStartAddress { get; set; } = "0";


        #endregion

        #region 消费者触发
        /// <summary>
        /// 消费者触发字符串
        /// </summary>
        public string ConsumerTriggerValue { get; set; } = "1";

        /// <summary>
        /// 触发循环读取时间
        /// </summary>
        public string ConsumerTriggerCyc { get; set; } = "100";

        /// <summary>
        /// 消费者触发成功返回
        /// </summary>
        public string ConsumerTriggerSucceed { get; set; } = "2";

        /// <summary>
        /// 消费者触发失败返回
        /// </summary>
        public string ConsumerTriggerFail { get; set; } = "3";

        /// <summary>
        /// 消费者触发的站地址
        /// </summary>
        public string ConsumerStationAddress { get; set; } = "1";

        /// <summary>
        /// 消费者触发的起始地址
        /// </summary>
        public string ConsumerStartAddress { get; set; } = "0";


        #endregion

        /// <summary>
        /// 当前生产者消费者模式是否开启
        /// </summary>
        [ObservableProperty] public bool run;

        /// <summary>
        /// 用于显示当前`触发设定`文本
        /// </summary>
        public string ProductiveTriggerSettingShow
        {
            get
            {
                return $"站地址{ProductiveStationAddress} ,起始地址{ProductiveStartAddress}";
            }
        }

        /// <summary>
        /// 用于显示消费者文本
        /// </summary>
        public string ConsumerTriggerSettingShow
        {
            get
            {
                return $"站地址{ConsumerStationAddress} ,起始地址{ConsumerStartAddress}";
            }
        }

        /// <summary>
        /// 令牌 循环进程任务
        /// </summary>
        [JsonIgnore] public CancellationTokenSource ctsProductive { get; set; }

        /// <summary>
        /// 当前Http进程任务
        /// </summary>
        [JsonIgnore] public Lazy<Task> TaskProductive { get; set; }

        /// <summary>
        /// 令牌 循环进程任务
        /// </summary>
        [JsonIgnore] public CancellationTokenSource ctsConsumer { get; set; }

        /// <summary>
        /// 当前Http进程任务
        /// </summary>
        [JsonIgnore] public Lazy<Task> TaskConsumer { get; set; }

        /// <summary>
        /// 令牌 循环进程任务
        /// </summary>
        [JsonIgnore] public CancellationTokenSource ReConnectionCts { get; set; }

        /// <summary>
        /// 当前Http进程任务
        /// </summary>
        [JsonIgnore] public Lazy<Task> ReConnectionTask { get; set; }


        /// <summary>
        /// 当前消息队列
        /// </summary>
        [JsonIgnore]public BlockingCollection<List<ushort>> Queue { get; set; } = new BlockingCollection<List<ushort>>(30);

        // 用于序列化/反序列化的临时缓存
        [JsonProperty("Queue")]
        private List<List<ushort>> QueueRaw
        {
            get => Queue?.ToList();
            set
            {
                if (value != null)
                {
                    Queue = new BlockingCollection<List<ushort>>(30);
                    foreach (var item in value)
                        Queue.Add(item);
                }
            }
        }
    }
}