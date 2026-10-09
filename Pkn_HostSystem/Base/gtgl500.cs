using System;
using System.Runtime.InteropServices;

namespace GTN_Crd
{
    public class glink
    {
		public const short GLINK_MODE_DLL=0;
        public const short GLINK_MODE_DSP=1;

        [DllImport("gts.dll")]
        public static extern short GT_SetGLinkDo(short slaveno, ushort offset, ref byte pData, ushort bytelength);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkDo(short slaveno, ushort offset, ref byte pData, ushort bytelength);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkDi(short slaveno, ushort offset, out byte pData, ushort bytelength);
        [DllImport("gts.dll")]
        public static extern short GT_SetGLinkDoBit(short slaveno, short doIndex, byte value);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkDiBit(short slaveno, short diIndex, out byte pValue);
        [DllImport("gts.dll")]
        public static extern short GT_SetGLinkAo(short slaveno, ushort channel, ref short data, ushort count);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkAo(short slaveno, ushort channel, out short data, ushort count);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkAi(short slaveno, ushort channel, out short data, ushort count);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkOnlineSlaveNum(out byte pSlavenum);
        [DllImport("gts.dll")]
        public static extern short GT_SetGLinkModuleConfig(ref string pFile);
        [DllImport("gts.dll")]
        public static extern short GT_GLinkInit(short cardNum);
		[DllImport("gts.dll")]
        public static extern short GT_GLinkInitEx(short cardNum, short opMode);
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkDiEx(short slaveno, out byte pData, ushort count);
		 [DllImport("gts.dll")]
		//获取所有Glink从站信息，slavenum在线从站数量，slavetype 1DIO16 2AIO0606 4DI32  dilength dolength 长度 字节
		public static extern short  GT_GetGLinkModulesInfo(out char pslavenum,out char pslavetype,out char psubslavetype,out char pdilength,out char pdolength);
        public struct GLINK_COMM_STS
        {
            public byte onlineSlaveNum;
            public byte initSlaveNum;
            public byte commStatus;
            //public byte dump[13];
        }
        [DllImport("gts.dll")]
        public static extern short GT_GetGLinkCommStatus(out GLINK_COMM_STS commSts);
        [DllImport("gts.dll")]
        public static extern short GT_RelateGlinkToMcGpiBit(short gpi,short slaveno,short bitoffset,short byteOffset);
        [DllImport("gts.dll")]
        public static extern short GT_RelateGlinkToMcGpoBit(short gpo,short slaveno,short bitoffset,short byteOffset);
        [DllImport("gts.dll")]
		public static extern short GT_GLinkDeInit(UInt32 Param);
    }
}