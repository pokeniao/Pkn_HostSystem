using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame.View;
namespace Frame
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
