namespace Pkn_HostSystem.Service.UserDefined
{
    public interface IUserDefined
    {
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <returns></returns>
        public object GetPropertyValue(string key);

        public string ErrorMessage();
    }
}