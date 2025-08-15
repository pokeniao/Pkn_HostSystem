using Pkn_HostSystem.Service.UserDefined.Interface;
using Pkn_HostSystem.Static;
using static OpenTK.Graphics.OpenGL.GL;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BT3651Test : IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts, params object[] args)
        {
            string message = args[0] as string;
            //处理字符串
            message = message.Replace(" ", "").Trim();

            string[] strings = message.Split(",");

            object[] objects = [false, false];
            if (strings.Length != 2)
            {
                objects = [false, false];
                return (false, objects);
            }
            //阻值 单位mΩ
            double value = double.Parse(strings[0]);
            //电压 单位V
            double value2 = double.Parse(strings[1]);
            try
            {
                double RHight = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[50]).ToString());
                double RLow = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[51]).ToString());
                double VHight = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[52]).ToString());
                double VLow = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[53]).ToString());
                if (!(RHight > value && value > RLow))
                {
                    Volatile.Write(ref StaticArrayRegister.ArrayRegister[56], "NG");

                    objects = [true, "电测NG"];
                    return (true, objects);
                }
                if (!(VHight > value2 && value2 > VLow))
                {
                    Volatile.Write(ref StaticArrayRegister.ArrayRegister[56], "NG");
                    objects = [true, "电测NG"];
                    return (true, objects);
                }
            }
            catch (Exception e)
            {
                objects = [false, $"{e.ToString()}"];
                return (false, objects);
            }
            //将电阻 电压写入到寄存器中

            Volatile.Write(ref StaticArrayRegister.ArrayRegister[54], value);
            Volatile.Write(ref StaticArrayRegister.ArrayRegister[55], value2);

            //写入OK
            Volatile.Write(ref StaticArrayRegister.ArrayRegister[56], "OK");
            objects = [true, $"OK"];
            return (true, objects);
        }

        public async Task<string> ErrorMessage(CancellationTokenSource cts, params object[] args)
        {
            try
            {
                string message = args[0] as string;
                //处理字符串
                message = message.Replace(" ", "").Trim();

                string[] strings = message.Split(",");

                if (strings.Length != 2)
                {
                    return "电测未正确返回数据";
                }
                //阻值 单位mΩ
                double value = double.Parse(strings[0]);

                double RHight = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[50]).ToString());
                double RLow = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[51]).ToString());
                double VHight = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[52]).ToString());
                double VLow = double.Parse(Volatile.Read(ref StaticArrayRegister.ArrayRegister[53]).ToString());
                if (!(RHight > value && value > RLow))
                {
                    return "电阻未达到条件";
                }


                //电压 单位V
                double value2 = double.Parse(strings[1]);

                if (!(VHight > value2 && value2 > VLow))
                {
                    return "电压未达到条件";
                }
            }
            catch (Exception e)
            {
                return e.ToString();
            }

            return "未知错误";
        }
    }
}