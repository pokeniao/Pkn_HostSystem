namespace Pkn_HostSystem.Base
{
    public enum ModbusEndian
    {
        BigEndian, // 高字在前，高字节在前
        LittleEndian, // 低字在前，低字节在前
        WordSwap, // 低字在前，高字节在前
        ByteSwap // 高字在前，低字节在前
    }

    /// <summary>
    /// ushort[] 转双寄存器 int,uint ,Float
    /// </summary>
    public static class ModbusDoubleRegisterTool
    {
        public static List<uint> ToUInt32List(ushort[] registers, ModbusEndian endian)
        {
            var result = new List<uint>();
            for (int i = 0; i < registers.Length - 1; i += 2)
            {
                uint value = ConvertToUInt32(registers[i], registers[i + 1], endian);
                result.Add(value);
            }

            return result;
        }

        public static List<int> ToInt32List(ushort[] registers, ModbusEndian endian)
        {
            var result = new List<int>();
            foreach (var u in ToUInt32List(registers, endian))
            {
                result.Add(unchecked((int)u));
            }

            return result;
        }

        public static List<float> ToFloatList(ushort[] registers, ModbusEndian endian)
        {
            var result = new List<float>();
            foreach (var u in ToUInt32List(registers, endian))
            {
                byte[] bytes = BitConverter.GetBytes(u);
                if (BitConverter.IsLittleEndian)
                {
                    // BitConverter 是小端，需要按实际顺序翻转
                    switch (endian)
                    {
                        case ModbusEndian.BigEndian:
                            Array.Reverse(bytes);
                            break;
                        case ModbusEndian.LittleEndian:
                            // 不动
                            break;
                        case ModbusEndian.WordSwap:
                            bytes = new byte[] { bytes[2], bytes[3], bytes[0], bytes[1] };
                            break;
                        case ModbusEndian.ByteSwap:
                            Array.Reverse(bytes);
                            break;
                    }
                }

                result.Add(BitConverter.ToSingle(bytes, 0));
            }

            return result;
        }

        private static uint ConvertToUInt32(ushort word1, ushort word2, ModbusEndian endian)
        {
            byte[] bytes = new byte[4];

            switch (endian)
            {
                case ModbusEndian.BigEndian:
                    // word1: 高位, word2: 低位
                    bytes[0] = (byte)(word1 >> 8);
                    bytes[1] = (byte)(word1 & 0xFF);
                    bytes[2] = (byte)(word2 >> 8);
                    bytes[3] = (byte)(word2 & 0xFF);
                    break;

                case ModbusEndian.LittleEndian:
                    // word1: 低位, word2: 高位
                    bytes[0] = (byte)(word1 & 0xFF);
                    bytes[1] = (byte)(word1 >> 8);
                    bytes[2] = (byte)(word2 & 0xFF);
                    bytes[3] = (byte)(word2 >> 8);
                    break;

                case ModbusEndian.WordSwap:
                    // word1: 低位, word2: 高位, 但字节顺序不换
                    bytes[0] = (byte)(word2 >> 8);
                    bytes[1] = (byte)(word2 & 0xFF);
                    bytes[2] = (byte)(word1 >> 8);
                    bytes[3] = (byte)(word1 & 0xFF);
                    break;

                case ModbusEndian.ByteSwap:
                    // word1: 高位, word2: 低位, 但字节顺序倒转
                    bytes[0] = (byte)(word2 & 0xFF);
                    bytes[1] = (byte)(word2 >> 8);
                    bytes[2] = (byte)(word1 & 0xFF);
                    bytes[3] = (byte)(word1 >> 8);
                    break;
            }

            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}