using Frame;
using Frame.pojo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame.View
{
    public partial class ModbusView : Form
    {
        //创建MODBUS
        private ModbusBase modbus = new ModbusBase();

        private BindingList<ModbusPojo<ushort>> bindingUshortList;
        private BindingList<ModbusPojo<bool>> bindingBoolList;

        public ModbusView()
        {
            InitializeComponent();
            StartLoading();
        }

        public void StartLoading()
        {
            //获取端口号
            string[] coms = modbus.getCOM();
            string[] btl = { "9600", "14400", "19200" };
            string[] bit = { "8", "7" };
            string[] ips = modbus.getIpAddress();
            var tzwArray = Enum.GetValues(typeof(StopBits));
            var jywArray = Enum.GetValues(typeof(Parity));

            string[] gnm = { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" };
            //数据关联和填写
            if (coms.Length > 0) cbb_modbus_com.Items.AddRange(coms);
            cbb_modbusRtu_btl.Items.AddRange(btl);
            cbb_modbusRTU_bit.Items.AddRange(bit);
            cbb_modbusRtu_tzw.DataSource = tzwArray;
            cbb_ModbusRtu_jyw.DataSource = jywArray;
            cbb_modbusRTU_gnm.Items.AddRange(gnm);
            cbb_modbusTcp_IP.Items.AddRange(ips);
            //数据默认设置
            cbb_modbus_com.SelectedIndex = 0;
            cbb_modbusRtu_btl.SelectedIndex = 0;
            cbb_modbusRTU_bit.SelectedIndex = 0;
            cbb_modbusRtu_tzw.SelectedIndex = 1;
            cbb_ModbusRtu_jyw.SelectedIndex = 0;
            cbb_modbusRTU_gnm.SelectedIndex = 0;
            cbb_modbusTcp_IP.SelectedIndex = 0;
            nud_Modbus_address.Value = 1;
            nud_modbusRTU_startAddress.Value = 0;
            nud_ModbusRTU_num.Value = 1;
            nud_modbusTCP_port.Value = 8080;
        }

        #region cbb下拉更新

        /// <summary>
        /// COM口下拉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbb_modbus_com_DropDown(object sender, EventArgs e)
        {
            //清除
            cbb_modbus_com.Items.Clear();
            //获取端口号
            string[] coms = modbus.getCOM();
            if (coms.Length > 0) cbb_modbus_com.Items.AddRange(coms);
        }

        private void cbb_modbusTcp_IP_DropDown(object sender, EventArgs e)
        {
            //清除
            cbb_modbusTcp_IP.Items.Clear();
            //获取端口号
            string[] ips = modbus.getIpAddress();
            if (ips.Length > 0) cbb_modbusTcp_IP.Items.AddRange(ips);
        }

        #endregion


        /// <summary>
        /// 状态码改变的时候
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbb_modbusRTU_gnm_SelectedIndexChanged(object sender, EventArgs e)
        {
            refreshWriteDgv();
        }

        private void nud_modbusRTU_startAddress_ValueChanged(object sender, EventArgs e)
        {
            refreshWriteDgv();
        }

        private void nud_ModbusRTU_num_ValueChanged(object sender, EventArgs e)
        {
            refreshWriteDgv();
        }

        #region 写DGV处理

        private void refreshWriteDgv()
        {
            switch (cbb_modbusRTU_gnm.Text)
            {
                case "05写单线圈":
                    nud_ModbusRTU_num.Value = 1;
                    nud_ModbusRTU_num.Enabled = false;
                    bindingBoolList = WriteView<bool>();
                    break;
                case "06写单寄存器":
                    nud_ModbusRTU_num.Value = 1;
                    nud_ModbusRTU_num.Enabled = false;
                    bindingUshortList = WriteView<ushort>();
                    break;
                case "0F写多线圈":
                    nud_ModbusRTU_num.Enabled = true;
                    bindingBoolList = WriteView<bool>();
                    break;
                case "10写多寄存器":
                    nud_ModbusRTU_num.Enabled = true;
                    bindingUshortList = WriteView<ushort>();
                    break;
                default:
                    nud_ModbusRTU_num.Enabled = true;
                    break;
            }
        }

        private BindingList<ModbusPojo<A>> WriteView<A>()
        {
            BindingList<ModbusPojo<A>> bindingList = new BindingList<ModbusPojo<A>>();
            if (typeof(A) == typeof(bool))
            {
                int key = (int)nud_modbusRTU_startAddress.Value;
                for (int i = 0; i < nud_ModbusRTU_num.Value; i++)
                {
                    bindingList.Add(new ModbusPojo<A>() { address = key++, value = (A)(object)false });
                }

                dgv_ModbusRTU_write.DataSource = null;
                dgv_ModbusRTU_write.DataSource = bindingList;
                return bindingList;
            }
            else
            {
                int currentAddress = (int)nud_modbusRTU_startAddress.Value;
                for (int i = 0; i < nud_ModbusRTU_num.Value; i++)
                {
                    bindingList.Add(new ModbusPojo<A>() { address = currentAddress, value = (A)(object)(ushort)0 });
                    currentAddress++;
                }

                dgv_ModbusRTU_write.DataSource = null;
                dgv_ModbusRTU_write.DataSource = bindingList;
                return bindingList;
            }
        }

        #endregion

        #region 发送处理

        //发送处理
        private async void b_modbusRTU_send_Click(object sender, EventArgs e)
        {
            // { "01读线圈", "02读输入状态", "03读保持寄存器", "04读输入寄存器", "05写单线圈", "06写单寄存器", "0F写多线圈", "10写多寄存器" };
            switch (cbb_modbusRTU_gnm.Text)
            {
                case "01读线圈":
                    bool[] coils01 = null;
                    try
                    {
                        coils01 = await modbus.ReadCoils_01((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value, (ushort)nud_ModbusRTU_num.Value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"读取发生错误:{exception.Message}");
                        break;
                    }

                    if (coils01 != null) readDGV(coils01);
                    break;
                case "02读输入状态":
                    bool[] inputs02 = null;
                    try
                    {
                        inputs02 = await modbus.ReadInputs_02((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value, (ushort)nud_ModbusRTU_num.Value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"读取发生错误:{exception.Message}");
                        break;
                    }

                    if (inputs02 != null) readDGV(inputs02);
                    break;
                case "03读保持寄存器":
                    ushort[] holdingRegisters03 = null;
                    try
                    {
                        holdingRegisters03 = await modbus.ReadHoldingRegisters_03((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value, (ushort)nud_ModbusRTU_num.Value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"读取发生错误:{exception.Message}");
                        break;
                    }

                    if (holdingRegisters03 != null) readDGV(holdingRegisters03);
                    break;
                case "04读输入寄存器":
                    ushort[] readInputRegisters04 = null;
                    try
                    {
                        readInputRegisters04 = await modbus.ReadInputRegisters_04((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value, (ushort)nud_ModbusRTU_num.Value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"读取发生错误:{exception.Message}");
                        break;
                    }

                    if (readInputRegisters04 != null) readDGV(readInputRegisters04);
                    break;
                case "05写单线圈":
                    try
                    {
                        await modbus.WriteCoil_05((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value,
                            bindingBoolList[0].value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"写入发生错误:{exception.Message}");
                    }

                    break;
                case "06写单寄存器":
                    try
                    {
                        await modbus.WriteRegister_06((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value,
                            bindingUshortList[0].value);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"写入发生错误:{exception.Message}");
                    }

                    break;
                case "0F写多线圈":
                    try
                    {
                        List<bool> coils = new List<bool>();
                        foreach (ModbusPojo<bool> modbusPojo in bindingBoolList.ToArray())
                        {
                            coils.Add(modbusPojo.value);
                        }

                        await modbus.WriteCoils_0F((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value,
                            coils.ToArray()
                        );
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"写入发生错误:{exception.Message}");
                    }

                    break;
                case "10写多寄存器":
                    List<ushort> registers = new List<ushort>();
                    try
                    {
                        foreach (ModbusPojo<ushort> modbusPojo in bindingUshortList.ToArray())
                        {
                            registers.Add(modbusPojo.value);
                        }

                        await modbus.WriteRegisters_10((byte)nud_Modbus_address.Value,
                            (ushort)nud_modbusRTU_startAddress.Value, registers.ToArray()
                        );
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show($"写入发生错误:{exception.Message}");
                    }

                    break;
            }
        }

        #endregion

        #region 打开关闭处理

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_modbusRTU_connect_Click(object sender, EventArgs e)
        {
            bool openRtuMaster = modbus.OpenRTUMaster(cbb_modbus_com.Text, int.Parse(cbb_modbusRtu_btl.Text),
                int.Parse(cbb_modbusRTU_bit.Text), (StopBits)cbb_modbusRtu_tzw.SelectedIndex,
                (Parity)cbb_ModbusRtu_jyw.SelectedIndex);
            if (openRtuMaster)
            {
                MessageBox.Show("打开成功", "提示");
            }
            else
            {
                MessageBox.Show("打开失败", "提示");
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_modbusRTU_close_Click(object sender, EventArgs e)
        {
            modbus.CloseRTU();
            MessageBox.Show("连接已关闭", "提示");
        }

        private void b_modbusTcp_close_Click(object sender, EventArgs e)
        {
            modbus.CloseTCP();
            MessageBox.Show("连接已关闭", "提示");
        }

        /// <summary>
        /// 连接TCP
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void b_modbustTcp_connect_Click(object sender, EventArgs e)
        {
            await modbus.OpenTcpMaster(cbb_modbusTcp_IP.Text, (int)nud_modbusTCP_port.Value);
            if (modbus.IsTCPConnect())
            {
                MessageBox.Show("打开成功", "提示");
            }
            else
            {
                MessageBox.Show("打开失败", "提示");
            }
        }

        #endregion

        #region 显示读DGV

        public void readDGV<T>(T[] value)
        {
            if (typeof(T) == typeof(bool))
            {
                int address = (int)nud_modbusRTU_startAddress.Value;
                List<ModbusPojo<bool>> modbusPojos = value.Select((b, index) => new ModbusPojo<bool>
                    { address = address++, value = (bool)(object)b }).ToList();
                dgv_ModbusRTU_read.DataSource = null;
                dgv_ModbusRTU_read.DataSource = modbusPojos;
            }
            else if (typeof(T) == typeof(ushort))
            {
                int address = (int)nud_modbusRTU_startAddress.Value;
                List<ModbusPojo<ushort>> modbusPojos = value.Select(s => new ModbusPojo<ushort>
                    { address = address++, value = (ushort)(object)s }).ToList();
                dgv_ModbusRTU_read.DataSource = null;
                dgv_ModbusRTU_read.DataSource = modbusPojos;
            }
        }

        #endregion
    }
}