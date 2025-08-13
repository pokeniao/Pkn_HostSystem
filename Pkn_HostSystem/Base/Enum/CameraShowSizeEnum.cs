namespace Pkn_HostSystem.Base.Enum
{
    public enum CameraShowSizeEnum
    {
        适应窗口模式,
        _100p,
        _50p,
        _25p,
    }

    public static class CameraShowSizeEnumExtensions
    {
        public static string GetDescription(this CameraShowSizeEnum value)
        {
            return value switch
            {
                CameraShowSizeEnum.适应窗口模式 => "适应窗口模式",
                CameraShowSizeEnum._100p => "100%",
                CameraShowSizeEnum._50p => "50%",
                CameraShowSizeEnum._25p => "25%",
                _ => value.ToString()
            };
        }
    }
}