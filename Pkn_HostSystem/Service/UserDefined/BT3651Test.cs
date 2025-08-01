using Pkn_HostSystem.Service.UserDefined.Interface;
using static OpenTK.Graphics.OpenGL.GL;

namespace Pkn_HostSystem.Service.UserDefined
{
    public class BT3651Test: IUserDefined
    {
        public async Task<(bool Succeed, object Return)> Main(CancellationTokenSource cts ,params object[] args)
        {
            string message = args[0] as string;
            //处理字符串
            message = message.Replace(" ", "").Trim();

            string[] strings = message.Split(",");
            //阻值 单位mΩ
            double value = double.Parse(strings[0]);



            //电压 单位V
            double value2 = double.Parse(strings[1]);

            return (true, true);
        }

        public string ErrorMessage()
        {
            throw new NotImplementedException();
        }
    }
}