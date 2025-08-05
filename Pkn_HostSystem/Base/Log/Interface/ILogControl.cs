using log4net;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using Wpf.Ui;


namespace Pkn_HostSystem.Base.Log.Interface
{
    public interface ILogControl
    {
        ILog Log { get; set; }
        ISnackbarService SnackbarService { get; set; }
        FlowDocument FlowDocument { get; set; }

        RichTextBox RichTextBox { get; set; }
    }
}