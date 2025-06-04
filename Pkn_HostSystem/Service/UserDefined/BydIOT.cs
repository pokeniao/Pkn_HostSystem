namespace Pkn_HostSystem.Service.UserDefined
{
    public class PppIOT :IUserDefined
    {
        public object GetPropertyValue(string key)
        {
            return null;
        }

        public (bool Succeed, object Return) Main()
        {
            return (true,"OK");
        }

        public string ErrorMessage()
        {
            return "";
        }
    }
}