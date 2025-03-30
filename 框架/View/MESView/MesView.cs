using Frame;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame.View;

namespace Frame.View.MESView
{
    public partial class MesView : Form
    {
        private UploadSetPojo uploadSetPojo = new UploadSetPojo();

        private LogBase logBase = new LogBase();

        private MesServer mesServer;

        private StringBuilder stringBuilder = new StringBuilder();


        public MesView()
        {
            InitializeComponent();
            StartLoad();
        }

        public async void StartLoad()
        {
            //读取配置文件数据
            uploadSetPojo.deviceCode = ConfigurationManager.AppSettings["deviceCode"];
            uploadSetPojo.lineCode = ConfigurationManager.AppSettings["lineCode"];
            uploadSetPojo.stationCode = ConfigurationManager.AppSettings["stationCode"];
            uploadSetPojo.plcIp = ConfigurationManager.AppSettings["plcIp"];
            uploadSetPojo.MesIp = ConfigurationManager.AppSettings["MesIp"];
            if (int.TryParse(ConfigurationManager.AppSettings["plcStatus"], out int b))
            {
                uploadSetPojo.plcStatus = b;
            }
            else
            {
                uploadSetPojo.plcStatus = 0;
            }

            if (int.TryParse(ConfigurationManager.AppSettings["upload"], out int a))
            {
                uploadSetPojo.upload = a;
            }
            else
            {
                uploadSetPojo.upload = 10;
            }

            if (int.TryParse(ConfigurationManager.AppSettings["plcPort"], out int port))
            {
                uploadSetPojo.plcPort = port;
            }
            else
            {
                uploadSetPojo.plcPort = 502;
            }

            uploadSetPojo.hread =  ushort.Parse(ConfigurationManager.AppSettings["nud_send_PLC_heart"]);

            //设置页面显示
            tb_PLC_IP.Text = uploadSetPojo.plcIp;
            tb_MES_IP.Text = uploadSetPojo.MesIp;
            tb_deviceCode.Text = uploadSetPojo.deviceCode;
            tb_lineCode.Text = uploadSetPojo.lineCode;
            tb_stationCode.Text = uploadSetPojo.stationCode;
            nud_uploadtime.Value = uploadSetPojo.upload;
            nud_PLC_state.Value = uploadSetPojo.plcStatus;
            nud_PLC_port.Value = uploadSetPojo.plcPort;
            nud_send_PLC_heart.Value = uploadSetPojo.hread;
            //检查日志是否创建
            logBase.CreateFileDirectory();
            //初始化mes
            mesServer = new MesServer();

            //启用定时任务
            this.time_upload_Tick(null, null); //启动先执行一次
            time_upload.Interval = (int)uploadSetPojo.upload * 1000;
            time_upload.Start();
            timer_hread.Start();
        }

        #region 修改配置文件

        private async void button1_Click(object sender, EventArgs e)
        {
            string pattern =
                @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b";
            if (!Regex.IsMatch(tb_PLC_IP.Text, pattern))
            {
                MessageBox.Show("Ip格式不对", "提示");
                return;
            }

            updateConfigeration("deviceCode", tb_deviceCode.Text);
            updateConfigeration("lineCode", tb_lineCode.Text);
            updateConfigeration("stationCode", tb_stationCode.Text);
            updateConfigeration("plcIp", tb_PLC_IP.Text);
            updateConfigeration("MesIp", tb_MES_IP.Text);
            updateConfigeration("upload", nud_uploadtime.Value.ToString());
            updateConfigeration("plcStatus", nud_PLC_state.Value.ToString());
            updateConfigeration("plcPort", nud_PLC_port.Value.ToString());
            updateConfigeration("nud_send_PLC_heart", nud_send_PLC_heart.Value.ToString());
            //读取配置文件数据
            uploadSetPojo.deviceCode = ConfigurationManager.AppSettings["deviceCode"];
            uploadSetPojo.lineCode = ConfigurationManager.AppSettings["lineCode"];
            uploadSetPojo.stationCode = ConfigurationManager.AppSettings["stationCode"];
            uploadSetPojo.plcIp = ConfigurationManager.AppSettings["plcIp"];
            uploadSetPojo.MesIp = ConfigurationManager.AppSettings["MesIp"];
            uploadSetPojo.upload = int.Parse(ConfigurationManager.AppSettings["upload"]);
            uploadSetPojo.plcStatus = int.Parse(ConfigurationManager.AppSettings["plcStatus"]);
            uploadSetPojo.plcPort = int.Parse(ConfigurationManager.AppSettings["plcPort"]);
            uploadSetPojo.hread = ushort.Parse(ConfigurationManager.AppSettings["nud_send_PLC_heart"]);

            time_upload.Interval = (int)uploadSetPojo.upload * 1000;
            //重新连接PLC
            if (button2.Text=="断开")
            {
                await mesServer.ConnectTcp(uploadSetPojo);
            }
           
        }

        private void updateConfigeration(string key, string value)
        {
            // 加载当前配置文件
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            // 修改配置项的值
            if (config.AppSettings.Settings[key] != null)
            {
                config.AppSettings.Settings[key].Value = value;
            }
            else
            {
                config.AppSettings.Settings.Add(key, "");
            }
            
            // 保存修改
            config.Save(ConfigurationSaveMode.Modified);

            // 刷新 ConfigurationManager 缓存
            ConfigurationManager.RefreshSection("appSettings");
        }

        #endregion


        private async void time_upload_Tick(object sender, EventArgs e)
        {
            if (stringBuilder.Length == 100)
            {
                stringBuilder.Clear();
            }

            try
            {
                ushort[] ushorts = await mesServer.getPLCstate(uploadSetPojo);
                tb_viewPLCstate.Text = ushorts[0].ToString();
                string message = await mesServer.uploadState(ushorts[0], uploadSetPojo);
                stringBuilder.Append($"{DateTime.Now}: {message}\r\n");
            }
            catch (Exception exception)
            {
                await logBase.WriteLog(exception.Message);
                stringBuilder.Append($"{DateTime.Now}: {exception.Message}\r\n");
            }

            tb_log.Text = stringBuilder.ToString();
        }

    
        private async void timer_hread_Tick(object sender, EventArgs e)
        {
            await mesServer.hread(uploadSetPojo);
        }

        private void tb_log_TextChanged(object sender, EventArgs e)
        {

            //自动滑动到最后一行
            tb_log.SelectionStart = tb_log.Text.Length;
            tb_log.ScrollToCaret();

        }

        private void modbus工具ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModbusView modbusView = new ModbusView();
            modbusView.Show();
        }
        /// <summary>
        /// 断开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (button2.Text=="断开")
            {
                time_upload.Stop();
                timer_hread.Stop();
                mesServer.CloseTcp();
                button2.Text = "启动";
            }
            else
            {
                mesServer.ConnectTcp(uploadSetPojo);
                time_upload.Start();
                timer_hread.Start();
                button2.Text = "断开";
            }
        }
    }
}