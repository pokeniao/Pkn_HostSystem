
using System;
namespace WPF_NET.Base;
public class SnowflakeIdGenerator
{
    private const long Twepoch = 1672531200000L; // 自定义起始时间戳 (2023-01-01 00:00:00 UTC)
    private const int WorkerIdBits = 5;  // 机器 ID 位数
    private const int DatacenterIdBits = 5; // 数据中心 ID 位数
    private const int SequenceBits = 12; // 序列号位数

    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits); // 最大机器 ID
    private const long MaxDatacenterId = -1L ^ (-1L << DatacenterIdBits); // 最大数据中心 ID
    private const long MaxSequence = -1L ^ (-1L << SequenceBits); // 最大序列号

    private const int WorkerIdShift = SequenceBits; // 机器 ID 左移位数
    private const int DatacenterIdShift = SequenceBits + WorkerIdBits; // 数据中心 ID 左移位数
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits + DatacenterIdBits; // 时间戳左移位数

    private readonly long _workerId;
    private readonly long _datacenterId;
    private long _sequence = 0L;
    private long _lastTimestamp = -1L;

    private readonly object _lock = new object();

    public SnowflakeIdGenerator(long workerId, long datacenterId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
            throw new ArgumentException($"Worker ID 必须在 0 - {MaxWorkerId} 之间");
        if (datacenterId > MaxDatacenterId || datacenterId < 0)
            throw new ArgumentException($"Datacenter ID 必须在 0 - {MaxDatacenterId} 之间");

        _workerId = workerId;
        _datacenterId = datacenterId;
    }
    /// <summary>
    /// 获得雪花
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public long GetId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
                throw new InvalidOperationException("时钟回拨异常");

            if (timestamp == _lastTimestamp)
            {
                _sequence = (_sequence + 1) & MaxSequence;
                if (_sequence == 0)
                {
                    timestamp = WaitForNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0;
            }

            _lastTimestamp = timestamp;

            return ((timestamp - Twepoch) << TimestampLeftShift) |
                   (_datacenterId << DatacenterIdShift) |
                   (_workerId << WorkerIdShift) |
                   _sequence;
        }
    }

    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long WaitForNextMillis(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}
