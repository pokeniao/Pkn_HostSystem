namespace Pkn_WinForm.View.ModbusView
{
    partial class ModbusView
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.b_modbusRTU_connect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbb_modbus_com = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cbb_modbusRtu_btl = new System.Windows.Forms.ComboBox();
            this.cbb_modbusRTU_bit = new System.Windows.Forms.ComboBox();
            this.cbb_modbusRtu_tzw = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.cbb_modbusRTU_gnm = new System.Windows.Forms.ComboBox();
            this.nud_modbusRTU_startAddress = new System.Windows.Forms.NumericUpDown();
            this.nud_ModbusRTU_num = new System.Windows.Forms.NumericUpDown();
            this.b_modbusRTU_close = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.cbb_ModbusRtu_jyw = new System.Windows.Forms.ComboBox();
            this.nud_Modbus_address = new System.Windows.Forms.NumericUpDown();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgv_ModbusRTU_read = new System.Windows.Forms.DataGridView();
            this.b_modbusRTU_send = new System.Windows.Forms.Button();
            this.dgv_ModbusRTU_write = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1_modbusRTU = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbb_modbusTcp_IP = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.nud_modbusTCP_port = new System.Windows.Forms.NumericUpDown();
            this.b_modbustTcp_connect = new System.Windows.Forms.Button();
            this.b_modbusTcp_close = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nud_modbusRTU_startAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ModbusRTU_num)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Modbus_address)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ModbusRTU_read)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ModbusRTU_write)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1_modbusRTU.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_modbusTCP_port)).BeginInit();
            this.SuspendLayout();
            // 
            // b_modbusRTU_connect
            // 
            this.b_modbusRTU_connect.Location = new System.Drawing.Point(28, 159);
            this.b_modbusRTU_connect.Name = "b_modbusRTU_connect";
            this.b_modbusRTU_connect.Size = new System.Drawing.Size(75, 36);
            this.b_modbusRTU_connect.TabIndex = 0;
            this.b_modbusRTU_connect.Text = "连接";
            this.b_modbusRTU_connect.UseVisualStyleBackColor = true;
            this.b_modbusRTU_connect.Click += new System.EventHandler(this.b_modbusRTU_connect_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "端口号";
            // 
            // cbb_modbus_com
            // 
            this.cbb_modbus_com.FormattingEnabled = true;
            this.cbb_modbus_com.Location = new System.Drawing.Point(83, 22);
            this.cbb_modbus_com.Name = "cbb_modbus_com";
            this.cbb_modbus_com.Size = new System.Drawing.Size(121, 23);
            this.cbb_modbus_com.TabIndex = 2;
            this.cbb_modbus_com.DropDown += new System.EventHandler(this.cbb_modbus_com_DropDown);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "波特率";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(52, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "数据位";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 5;
            this.label4.Text = "停止位";
            // 
            // cbb_modbusRtu_btl
            // 
            this.cbb_modbusRtu_btl.FormattingEnabled = true;
            this.cbb_modbusRtu_btl.Location = new System.Drawing.Point(83, 49);
            this.cbb_modbusRtu_btl.Name = "cbb_modbusRtu_btl";
            this.cbb_modbusRtu_btl.Size = new System.Drawing.Size(121, 23);
            this.cbb_modbusRtu_btl.TabIndex = 7;
            // 
            // cbb_modbusRTU_bit
            // 
            this.cbb_modbusRTU_bit.FormattingEnabled = true;
            this.cbb_modbusRTU_bit.Location = new System.Drawing.Point(83, 76);
            this.cbb_modbusRTU_bit.Name = "cbb_modbusRTU_bit";
            this.cbb_modbusRTU_bit.Size = new System.Drawing.Size(121, 23);
            this.cbb_modbusRTU_bit.TabIndex = 8;
            // 
            // cbb_modbusRtu_tzw
            // 
            this.cbb_modbusRtu_tzw.FormattingEnabled = true;
            this.cbb_modbusRtu_tzw.Location = new System.Drawing.Point(84, 103);
            this.cbb_modbusRtu_tzw.Name = "cbb_modbusRtu_tzw";
            this.cbb_modbusRtu_tzw.Size = new System.Drawing.Size(121, 23);
            this.cbb_modbusRtu_tzw.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 382);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(52, 15);
            this.label6.TabIndex = 11;
            this.label6.Text = "站地址";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(0, 413);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 12;
            this.label7.Text = "起始地址";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(30, 444);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 15);
            this.label8.TabIndex = 13;
            this.label8.Text = "数量";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 354);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 15);
            this.label9.TabIndex = 14;
            this.label9.Text = "功能码";
            // 
            // cbb_modbusRTU_gnm
            // 
            this.cbb_modbusRTU_gnm.FormattingEnabled = true;
            this.cbb_modbusRTU_gnm.Location = new System.Drawing.Point(74, 351);
            this.cbb_modbusRTU_gnm.Name = "cbb_modbusRTU_gnm";
            this.cbb_modbusRTU_gnm.Size = new System.Drawing.Size(135, 23);
            this.cbb_modbusRTU_gnm.TabIndex = 21;
            this.cbb_modbusRTU_gnm.SelectedIndexChanged += new System.EventHandler(this.cbb_modbusRTU_gnm_SelectedIndexChanged);
            // 
            // nud_modbusRTU_startAddress
            // 
            this.nud_modbusRTU_startAddress.Location = new System.Drawing.Point(73, 411);
            this.nud_modbusRTU_startAddress.Name = "nud_modbusRTU_startAddress";
            this.nud_modbusRTU_startAddress.Size = new System.Drawing.Size(136, 25);
            this.nud_modbusRTU_startAddress.TabIndex = 22;
            this.nud_modbusRTU_startAddress.ValueChanged += new System.EventHandler(this.nud_modbusRTU_startAddress_ValueChanged);
            // 
            // nud_ModbusRTU_num
            // 
            this.nud_ModbusRTU_num.Location = new System.Drawing.Point(74, 442);
            this.nud_ModbusRTU_num.Name = "nud_ModbusRTU_num";
            this.nud_ModbusRTU_num.Size = new System.Drawing.Size(135, 25);
            this.nud_ModbusRTU_num.TabIndex = 23;
            this.nud_ModbusRTU_num.ValueChanged += new System.EventHandler(this.nud_ModbusRTU_num_ValueChanged);
            // 
            // b_modbusRTU_close
            // 
            this.b_modbusRTU_close.Location = new System.Drawing.Point(128, 159);
            this.b_modbusRTU_close.Name = "b_modbusRTU_close";
            this.b_modbusRTU_close.Size = new System.Drawing.Size(75, 36);
            this.b_modbusRTU_close.TabIndex = 24;
            this.b_modbusRTU_close.Text = "断开";
            this.b_modbusRTU_close.UseVisualStyleBackColor = true;
            this.b_modbusRTU_close.Click += new System.EventHandler(this.b_modbusRTU_close_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(10, 130);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 15);
            this.label11.TabIndex = 25;
            this.label11.Text = "奇偶校验";
            // 
            // cbb_ModbusRtu_jyw
            // 
            this.cbb_ModbusRtu_jyw.FormattingEnabled = true;
            this.cbb_ModbusRtu_jyw.Location = new System.Drawing.Point(83, 130);
            this.cbb_ModbusRtu_jyw.Name = "cbb_ModbusRtu_jyw";
            this.cbb_ModbusRtu_jyw.Size = new System.Drawing.Size(121, 23);
            this.cbb_ModbusRtu_jyw.TabIndex = 26;
            // 
            // nud_Modbus_address
            // 
            this.nud_Modbus_address.Location = new System.Drawing.Point(73, 380);
            this.nud_Modbus_address.Name = "nud_Modbus_address";
            this.nud_Modbus_address.Size = new System.Drawing.Size(136, 25);
            this.nud_Modbus_address.TabIndex = 27;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dgv_ModbusRTU_read);
            this.groupBox1.Location = new System.Drawing.Point(656, 22);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(358, 482);
            this.groupBox1.TabIndex = 28;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "返回消息";
            // 
            // dgv_ModbusRTU_read
            // 
            this.dgv_ModbusRTU_read.AllowUserToAddRows = false;
            this.dgv_ModbusRTU_read.AllowUserToDeleteRows = false;
            this.dgv_ModbusRTU_read.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ModbusRTU_read.Location = new System.Drawing.Point(6, 30);
            this.dgv_ModbusRTU_read.Name = "dgv_ModbusRTU_read";
            this.dgv_ModbusRTU_read.ReadOnly = true;
            this.dgv_ModbusRTU_read.RowHeadersWidth = 51;
            this.dgv_ModbusRTU_read.RowTemplate.Height = 27;
            this.dgv_ModbusRTU_read.Size = new System.Drawing.Size(313, 440);
            this.dgv_ModbusRTU_read.TabIndex = 34;
            // 
            // b_modbusRTU_send
            // 
            this.b_modbusRTU_send.Location = new System.Drawing.Point(122, 473);
            this.b_modbusRTU_send.Name = "b_modbusRTU_send";
            this.b_modbusRTU_send.Size = new System.Drawing.Size(75, 35);
            this.b_modbusRTU_send.TabIndex = 29;
            this.b_modbusRTU_send.Text = "发送";
            this.b_modbusRTU_send.UseVisualStyleBackColor = true;
            this.b_modbusRTU_send.Click += new System.EventHandler(this.b_modbusRTU_send_Click);
            // 
            // dgv_ModbusRTU_write
            // 
            this.dgv_ModbusRTU_write.AllowUserToAddRows = false;
            this.dgv_ModbusRTU_write.AllowUserToDeleteRows = false;
            this.dgv_ModbusRTU_write.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_ModbusRTU_write.Location = new System.Drawing.Point(28, 40);
            this.dgv_ModbusRTU_write.Name = "dgv_ModbusRTU_write";
            this.dgv_ModbusRTU_write.RowHeadersWidth = 51;
            this.dgv_ModbusRTU_write.RowTemplate.Height = 27;
            this.dgv_ModbusRTU_write.Size = new System.Drawing.Size(313, 440);
            this.dgv_ModbusRTU_write.TabIndex = 33;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgv_ModbusRTU_write);
            this.groupBox2.Location = new System.Drawing.Point(267, 13);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(367, 497);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "写多寄存器/写单线圈";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1_modbusRTU);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1046, 560);
            this.tabControl1.TabIndex = 34;
            // 
            // tabPage1_modbusRTU
            // 
            this.tabPage1_modbusRTU.Controls.Add(this.panel1);
            this.tabPage1_modbusRTU.Controls.Add(this.groupBox2);
            this.tabPage1_modbusRTU.Controls.Add(this.groupBox1);
            this.tabPage1_modbusRTU.Location = new System.Drawing.Point(4, 25);
            this.tabPage1_modbusRTU.Name = "tabPage1_modbusRTU";
            this.tabPage1_modbusRTU.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1_modbusRTU.Size = new System.Drawing.Size(1038, 531);
            this.tabPage1_modbusRTU.TabIndex = 0;
            this.tabPage1_modbusRTU.Text = "ModbusRTU";
            this.tabPage1_modbusRTU.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox4);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.b_modbusRTU_send);
            this.panel1.Controls.Add(this.nud_Modbus_address);
            this.panel1.Controls.Add(this.nud_ModbusRTU_num);
            this.panel1.Controls.Add(this.nud_modbusRTU_startAddress);
            this.panel1.Controls.Add(this.cbb_modbusRTU_gnm);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(231, 525);
            this.panel1.TabIndex = 30;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.cbb_modbus_com);
            this.groupBox3.Controls.Add(this.cbb_ModbusRtu_jyw);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.cbb_modbusRtu_tzw);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.b_modbusRTU_close);
            this.groupBox3.Controls.Add(this.cbb_modbusRTU_bit);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.b_modbusRTU_connect);
            this.groupBox3.Controls.Add(this.cbb_modbusRtu_btl);
            this.groupBox3.Location = new System.Drawing.Point(3, 139);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(223, 207);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ModbusRTU";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.cbb_modbusTcp_IP);
            this.groupBox4.Controls.Add(this.nud_modbusTCP_port);
            this.groupBox4.Controls.Add(this.b_modbustTcp_connect);
            this.groupBox4.Controls.Add(this.b_modbusTcp_close);
            this.groupBox4.Location = new System.Drawing.Point(9, 11);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(217, 122);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "ModbusTCP";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(34, 21);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 15);
            this.label5.TabIndex = 1;
            this.label5.Text = "IP";
            // 
            // cbb_modbusTcp_IP
            // 
            this.cbb_modbusTcp_IP.FormattingEnabled = true;
            this.cbb_modbusTcp_IP.Location = new System.Drawing.Point(65, 18);
            this.cbb_modbusTcp_IP.Name = "cbb_modbusTcp_IP";
            this.cbb_modbusTcp_IP.Size = new System.Drawing.Size(133, 23);
            this.cbb_modbusTcp_IP.TabIndex = 2;
            this.cbb_modbusTcp_IP.DropDown += new System.EventHandler(this.cbb_modbusTcp_IP_DropDown);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 51);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 1;
            this.label10.Text = "端口号";
            // 
            // nud_modbusTCP_port
            // 
            this.nud_modbusTCP_port.Location = new System.Drawing.Point(64, 49);
            this.nud_modbusTCP_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.nud_modbusTCP_port.Name = "nud_modbusTCP_port";
            this.nud_modbusTCP_port.Size = new System.Drawing.Size(136, 25);
            this.nud_modbusTCP_port.TabIndex = 27;
            // 
            // b_modbustTcp_connect
            // 
            this.b_modbustTcp_connect.Location = new System.Drawing.Point(24, 79);
            this.b_modbustTcp_connect.Name = "b_modbustTcp_connect";
            this.b_modbustTcp_connect.Size = new System.Drawing.Size(75, 36);
            this.b_modbustTcp_connect.TabIndex = 0;
            this.b_modbustTcp_connect.Text = "连接";
            this.b_modbustTcp_connect.UseVisualStyleBackColor = true;
            this.b_modbustTcp_connect.Click += new System.EventHandler(this.b_modbustTcp_connect_Click);
            // 
            // b_modbusTcp_close
            // 
            this.b_modbusTcp_close.Location = new System.Drawing.Point(116, 79);
            this.b_modbusTcp_close.Name = "b_modbusTcp_close";
            this.b_modbusTcp_close.Size = new System.Drawing.Size(75, 36);
            this.b_modbusTcp_close.TabIndex = 24;
            this.b_modbusTcp_close.Text = "断开";
            this.b_modbusTcp_close.UseVisualStyleBackColor = true;
            this.b_modbusTcp_close.Click += new System.EventHandler(this.b_modbusTcp_close_Click);
            // 
            // From_ModbusRTU
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1046, 560);
            this.Controls.Add(this.tabControl1);
            this.Name = "From_ModbusRTU";
            this.Text = "MesView";
            ((System.ComponentModel.ISupportInitialize)(this.nud_modbusRTU_startAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_ModbusRTU_num)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_Modbus_address)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ModbusRTU_read)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_ModbusRTU_write)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1_modbusRTU.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_modbusTCP_port)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button b_modbusRTU_connect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbb_modbus_com;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbb_modbusRtu_btl;
        private System.Windows.Forms.ComboBox cbb_modbusRTU_bit;
        private System.Windows.Forms.ComboBox cbb_modbusRtu_tzw;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cbb_modbusRTU_gnm;
        private System.Windows.Forms.NumericUpDown nud_modbusRTU_startAddress;
        private System.Windows.Forms.NumericUpDown nud_ModbusRTU_num;
        private System.Windows.Forms.Button b_modbusRTU_close;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox cbb_ModbusRtu_jyw;
        private System.Windows.Forms.NumericUpDown nud_Modbus_address;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button b_modbusRTU_send;
        private System.Windows.Forms.DataGridView dgv_ModbusRTU_write;
        private System.Windows.Forms.DataGridView dgv_ModbusRTU_read;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1_modbusRTU;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbb_modbusTcp_IP;
        private System.Windows.Forms.NumericUpDown nud_modbusTCP_port;
        private System.Windows.Forms.Button b_modbustTcp_connect;
        private System.Windows.Forms.Button b_modbusTcp_close;
    }
}

