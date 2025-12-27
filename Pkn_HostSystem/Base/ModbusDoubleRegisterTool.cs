namespace Pkn_HostSystem.Base
{
    public enum ModbusEndian
    {
        BigEndian,              // 低字在前，高字节在后       FF00 FF01  ->  FF00FF01
        LittleEndian,           // 高字在前, 低字在后,逆序    FF00 FF01  ->  01FF00FF
        BigEndianByteSwap,      // 高字在前，低字在后         FF00 FF01  ->  FF01FF00
        LittleEndianByteSwap    // 低字在前，高字节在后,逆序  FF00 FF01  ->  00FF01FF
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
                    // 低字在前，高字节在后
                    bytes[0] = (byte)(word2 & 0xFF);
                    bytes[1] = (byte)(word2 >> 8);
                    bytes[2] = (byte)(word1 & 0xFF);
                    bytes[3] = (byte)(word1 >> 8);
                    break;

                case ModbusEndian.LittleEndian:
                    // ModbusEndian.BigEndian 的逆向
                    bytes[0] = (byte)(word1 >> 8);
                    bytes[1] = (byte)(word1 & 0xFF);
                    bytes[2] = (byte)(word2 >> 8);
                    bytes[3] = (byte)(word2 & 0xFF);
                    break;

                case ModbusEndian.BigEndianByteSwap:

                    bytes[0] = (byte)(word2 >> 8);
                    bytes[1] = (byte)(word2 & 0xFF);
                    bytes[2] = (byte)(word1 >> 8);
                    bytes[3] = (byte)(word1 & 0xFF);
                    break;

                case ModbusEndian.LittleEndianByteSwap:
                    bytes[0] = (byte)(word1 & 0xFF);
                    bytes[1] = (byte)(word1 >> 8);
                    bytes[2] = (byte)(word2 & 0xFF);
                    bytes[3] = (byte)(word2 >> 8);
                    break;
            }

            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}