namespace Pkn_HostSystem.Service.UserDefined
{
    public class BydIOT :IUserDefined
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