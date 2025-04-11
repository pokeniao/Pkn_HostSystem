using System;
using System.Windows.Forms;
using Pkn_WinForm.View.MESView;

namespace Pkn_WinForm

{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Application.Run(new MainForm());
            Application.Run(new MesView());
        }

    }
}
