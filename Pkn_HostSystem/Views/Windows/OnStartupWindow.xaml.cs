using Pkn_HostSystem.Base;
using Pkn_HostSystem.Models.Page;
using Pkn_HostSystem.Static;
using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace Pkn_HostSystem.Views.Windows
{
    /// <summary>
    /// OnStartupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OnStartupWindow
    {
        private string publicKey = PknPublicKey.Key;


        public OnStartupModel OnStartupModel { get; set; } = JsonTool<OnStartupModel>.Load();

        public OnStartupWindow()
        {
            InitializeComponent();
            if (OnStartupModel == null)
            {
                OnStartupModel = new OnStartupModel();
            }

            Main();
        }

        public void Main()
        {
            string cpuId = GetCpuId();
            string mainBoardId = GetMainBoardId();
            // string diskId = GetDiskId();
            // string macAddress = GetMacAddress();

            // 生成机器码
            string raw = cpuId + mainBoardId;
            using var sha256 = SHA256.Create();
            byte[] hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(raw));
            string machineCode = BitConverter.ToString(hash).Replace("-", "");

            OnStartupModel.MachineCode = machineCode;
            bool showDialog = false;
            //未设置License时，显示验证窗口
            if (OnStartupModel.License == null)
            {
                VerifyWindow verifyWindow = new(OnStartupModel);
                verifyWindow.ShowDialog();
                showDialog = verifyWindow.DialogResult.Value;
            }
            else
            {
                showDialog = true;
            }

            if (showDialog)
            {
                bool validateLicense = false;
                while (!validateLicense)
                {
                    if (OnStartupModel.License != null)
                    {
                        //验证License
                        try
                        {
                            validateLicense = ValidateLicense(machineCode, publicKey, OnStartupModel.License);
                        }
                        catch (FormatException e)
                        {
                            Wpf.Ui.Controls.MessageBox messageBox = new()
                            {
                                Title = "错误",
                                Content = $"证书格式不正确"
                            };
                            messageBox.ShowDialogAsync();
                        }
                        catch (Exception e)
                        {
                            Wpf.Ui.Controls.MessageBox messageBox = new()
                            {
                                Title = "错误",
                                Content = $"{e}"
                            };
                            messageBox.ShowDialogAsync();
                        }
                        if (!validateLicense)
                        {
                            Wpf.Ui.Controls.MessageBox messageBox = new()
                            {
                                Title = "错误",
                                Content = $"证书不正确"
                            };
                            messageBox.ShowDialogAsync();
                        }
                        else
                        {
                            JsonTool<OnStartupModel>.Save(OnStartupModel);
                        }
                    }

                    //如果验证失败，显示验证窗口
                    if (!validateLicense)
                    {
                        VerifyWindow verifyWindow = new VerifyWindow(OnStartupModel);
                        verifyWindow.ShowDialog();
                        showDialog = verifyWindow.DialogResult.Value;
                    }

                    if (!showDialog)
                    {
                        Close();
                        return;
                    }
                }
            }
            else
            {
                Close();
            }
        }

        public static bool ValidateLicense(string machineCode, string publicKey, string license)
        {
            try
            {
                byte[] data = Encoding.UTF8.GetBytes(machineCode);
                byte[] signature = Convert.FromBase64String(license);

                using var rsa = RSA.Create();
                rsa.ImportSubjectPublicKeyInfo(Convert.FromBase64String(publicKey), out _);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
            catch (Exception e)
            {
                throw;
            }
        }


        //CPU 序列号
        string GetCpuId()
        {
            string cpuId = "";
            using (var mc = new ManagementClass("Win32_Processor"))
            {
                foreach (var mo in mc.GetInstances())
                {
                    cpuId = mo["ProcessorId"]?.ToString();
                    break;
                }
            }

            return cpuId ?? "";
        }

        //硬盘序列号
        string GetDiskId()
        {
            string diskId = "";
            using (var mc = new ManagementClass("Win32_DiskDrive"))
            {
                foreach (var mo in mc.GetInstances())
                {
                    diskId = mo["SerialNumber"]?.ToString();
                    if (!string.IsNullOrEmpty(diskId))
                        break;
                }
            }

            return diskId?.Trim() ?? "";
        }

        //主板序列号
        string GetMainBoardId()
        {
            string boardId = "";
            using (var mc = new ManagementClass("Win32_BaseBoard"))
            {
                foreach (var mo in mc.GetInstances())
                {
                    boardId = mo["SerialNumber"]?.ToString();
                    break;
                }
            }

            return boardId ?? "";
        }

        //网卡MAC地址
        string GetMacAddress()
        {
            string mac = "";
            using (var mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            {
                foreach (var mo in mc.GetInstances())
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MACAddress"]?.ToString();
                        break;
                    }
                }
            }

            return mac ?? "";
        }
    }
}