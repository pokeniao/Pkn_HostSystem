namespace Pkn_WinForm.View.MESView
{
    partial class MesView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tb_MES_IP = new System.Windows.Forms.TextBox();
            this.nud_PLC_port = new System.Windows.Forms.NumericUpDown();
            this.tb_PLC_IP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.tb_log = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tb_viewPLCstate = new System.Windows.Forms.TextBox();
            this.nud_send_PLC_heart = new System.Windows.Forms.NumericUpDown();
            this.nud_PLC_state = new System.Windows.Forms.NumericUpDown();
            this.nud_uploadtime = new System.Windows.Forms.NumericUpDown();
            this.tb_stationCode = new System.Windows.Forms.TextBox();
            this.tb_lineCode = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tb_deviceCode = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.time_upload = new System.Windows.Forms.Timer(this.components);
            this.timer_hread = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.modbus工具ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PLC_port)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_send_PLC_heart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PLC_state)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_uploadtime)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tb_MES_IP);
            this.groupBox1.Controls.Add(this.nud_PLC_port);
            this.groupBox1.Controls.Add(this.tb_PLC_IP);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(8, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(384, 169);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "IP设置";
            // 
            // tb_MES_IP
            // 
            this.tb_MES_IP.Location = new System.Drawing.Point(131, 98);
            this.tb_MES_IP.Margin = new System.Windows.Forms.Padding(4);
            this.tb_MES_IP.Name = "tb_MES_IP";
            this.tb_MES_IP.Size = new System.Drawing.Size(226, 28);
            this.tb_MES_IP.TabIndex = 2;
            this.tb_MES_IP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nud_PLC_port
            // 
            this.nud_PLC_port.Location = new System.Drawing.Point(131, 64);
            this.nud_PLC_port.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nud_PLC_port.Name = "nud_PLC_port";
            this.nud_PLC_port.Size = new System.Drawing.Size(226, 28);
            this.nud_PLC_port.TabIndex = 1;
            this.nud_PLC_port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tb_PLC_IP
            // 
            this.tb_PLC_IP.Location = new System.Drawing.Point(131, 29);
            this.tb_PLC_IP.Margin = new System.Windows.Forms.Padding(4);
            this.tb_PLC_IP.Name = "tb_PLC_IP";
            this.tb_PLC_IP.Size = new System.Drawing.Size(226, 28);
            this.tb_PLC_IP.TabIndex = 0;
            this.tb_PLC_IP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(81, 98);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "MES";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.Location = new System.Drawing.Point(19, 66);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(105, 20);
            this.label10.TabIndex = 0;
            this.label10.Text = "PLC端口号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(81, 30);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "PLC";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(200, 565);
            this.button1.Margin = new System.Windows.Forms.Padding(4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(94, 37);
            this.button1.TabIndex = 2;
            this.button1.TabStop = false;
            this.button1.Text = "设置";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tb_log
            // 
            this.tb_log.Location = new System.Drawing.Point(8, 19);
            this.tb_log.Margin = new System.Windows.Forms.Padding(4);
            this.tb_log.MaxLength = 2100000000;
            this.tb_log.Multiline = true;
            this.tb_log.Name = "tb_log";
            this.tb_log.ReadOnly = true;
            this.tb_log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tb_log.Size = new System.Drawing.Size(589, 616);
            this.tb_log.TabIndex = 1;
            this.tb_log.TabStop = false;
            this.tb_log.TextChanged += new System.EventHandler(this.tb_log_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tb_log);
            this.groupBox2.Location = new System.Drawing.Point(507, 32);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(626, 653);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "日志显示";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tb_viewPLCstate);
            this.groupBox3.Controls.Add(this.nud_send_PLC_heart);
            this.groupBox3.Controls.Add(this.nud_PLC_state);
            this.groupBox3.Controls.Add(this.nud_uploadtime);
            this.groupBox3.Controls.Add(this.tb_stationCode);
            this.groupBox3.Controls.Add(this.tb_lineCode);
            this.groupBox3.Controls.Add(this.button2);
            this.groupBox3.Controls.Add(this.tb_deviceCode);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.groupBox1);
            this.groupBox3.Location = new System.Drawing.Point(31, 38);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(435, 616);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "设置参数";
            // 
            // tb_viewPLCstate
            // 
            this.tb_viewPLCstate.Enabled = false;
            this.tb_viewPLCstate.Location = new System.Drawing.Point(279, 450);
            this.tb_viewPLCstate.Name = "tb_viewPLCstate";
            this.tb_viewPLCstate.ReadOnly = true;
            this.tb_viewPLCstate.Size = new System.Drawing.Size(100, 28);
            this.tb_viewPLCstate.TabIndex = 5;
            // 
            // nud_send_PLC_heart
            // 
            this.nud_send_PLC_heart.Location = new System.Drawing.Point(78, 530);
            this.nud_send_PLC_heart.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nud_send_PLC_heart.Name = "nud_send_PLC_heart";
            this.nud_send_PLC_heart.Size = new System.Drawing.Size(155, 28);
            this.nud_send_PLC_heart.TabIndex = 5;
            this.nud_send_PLC_heart.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nud_PLC_state
            // 
            this.nud_PLC_state.Location = new System.Drawing.Point(78, 450);
            this.nud_PLC_state.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nud_PLC_state.Name = "nud_PLC_state";
            this.nud_PLC_state.Size = new System.Drawing.Size(155, 28);
            this.nud_PLC_state.TabIndex = 4;
            this.nud_PLC_state.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // nud_uploadtime
            // 
            this.nud_uploadtime.Location = new System.Drawing.Point(139, 369);
            this.nud_uploadtime.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nud_uploadtime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nud_uploadtime.Name = "nud_uploadtime";
            this.nud_uploadtime.Size = new System.Drawing.Size(155, 28);
            this.nud_uploadtime.TabIndex = 3;
            this.nud_uploadtime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.nud_uploadtime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tb_stationCode
            // 
            this.tb_stationCode.Location = new System.Drawing.Point(140, 317);
            this.tb_stationCode.Margin = new System.Windows.Forms.Padding(4);
            this.tb_stationCode.Name = "tb_stationCode";
            this.tb_stationCode.Size = new System.Drawing.Size(226, 28);
            this.tb_stationCode.TabIndex = 2;
            this.tb_stationCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tb_lineCode
            // 
            this.tb_lineCode.Location = new System.Drawing.Point(140, 271);
            this.tb_lineCode.Margin = new System.Windows.Forms.Padding(4);
            this.tb_lineCode.Name = "tb_lineCode";
            this.tb_lineCode.Size = new System.Drawing.Size(226, 28);
            this.tb_lineCode.TabIndex = 1;
            this.tb_lineCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button2.Location = new System.Drawing.Point(320, 565);
            this.button2.Margin = new System.Windows.Forms.Padding(4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 37);
            this.button2.TabIndex = 2;
            this.button2.TabStop = false;
            this.button2.Text = "断开";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tb_deviceCode
            // 
            this.tb_deviceCode.Location = new System.Drawing.Point(140, 224);
            this.tb_deviceCode.Margin = new System.Windows.Forms.Padding(4);
            this.tb_deviceCode.Name = "tb_deviceCode";
            this.tb_deviceCode.Size = new System.Drawing.Size(226, 28);
            this.tb_deviceCode.TabIndex = 0;
            this.tb_deviceCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label8.Location = new System.Drawing.Point(301, 373);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 18);
            this.label8.TabIndex = 3;
            this.label8.Text = "单位秒";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label11.Location = new System.Drawing.Point(276, 417);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(103, 18);
            this.label11.TabIndex = 3;
            this.label11.Text = "读取到数值";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label12.Location = new System.Drawing.Point(8, 498);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(315, 18);
            this.label12.TabIndex = 3;
            this.label12.Text = "上位机心跳发送到PLC寄存器地址(D)";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label9.Location = new System.Drawing.Point(35, 417);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(201, 18);
            this.label9.TabIndex = 3;
            this.label9.Text = "PLC状态寄存器地址(D)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label7.Location = new System.Drawing.Point(48, 373);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 18);
            this.label7.TabIndex = 3;
            this.label7.Text = "上传时间";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(48, 320);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "工位编码";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(48, 271);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(84, 18);
            this.label5.TabIndex = 3;
            this.label5.Text = "产线编码";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(48, 227);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 18);
            this.label4.TabIndex = 3;
            this.label4.Text = "设备编码";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(48, 227);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "设备编码";
            // 
            // time_upload
            // 
            this.time_upload.Tick += new System.EventHandler(this.time_upload_Tick);
            // 
            // timer_hread
            // 
            this.timer_hread.Interval = 500;
            this.timer_hread.Tick += new System.EventHandler(this.timer_hread_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.modbus工具ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1173, 28);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // modbus工具ToolStripMenuItem
            // 
            this.modbus工具ToolStripMenuItem.Name = "modbus工具ToolStripMenuItem";
            this.modbus工具ToolStripMenuItem.Size = new System.Drawing.Size(114, 24);
            this.modbus工具ToolStripMenuItem.Text = "Modbus工具";
            this.modbus工具ToolStripMenuItem.Click += new System.EventHandler(this.modbus工具ToolStripMenuItem_Click);
            // 
            // MesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 685);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("宋体", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MesView";
            this.Text = "上传数据状态";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PLC_port)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nud_send_PLC_heart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_PLC_state)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nud_uploadtime)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox tb_MES_IP;
        private System.Windows.Forms.TextBox tb_PLC_IP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_deviceCode;
        private System.Windows.Forms.NumericUpDown nud_uploadtime;
        private System.Windows.Forms.TextBox tb_stationCode;
        private System.Windows.Forms.TextBox tb_lineCode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tb_log;
        private System.Windows.Forms.Timer time_upload;
        private System.Windows.Forms.NumericUpDown nud_PLC_state;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nud_PLC_port;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox tb_viewPLCstate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.NumericUpDown nud_send_PLC_heart;
        private System.Windows.Forms.Timer timer_hread;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem modbus工具ToolStripMenuItem;
        private System.Windows.Forms.Button button2;
    }
}