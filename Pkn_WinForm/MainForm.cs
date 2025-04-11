using System;
using System.Windows.Forms;
using Pkn_WinForm.View.ModbusView;

namespace Pkn_WinForm
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void 打开ModbusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModbusView modbusView = new ModbusView();
            modbusView.Show();
        }

        private void 设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
