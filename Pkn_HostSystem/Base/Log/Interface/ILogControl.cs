using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;


namespace Pkn_HostSystem.Base.Log.Interface
{
    public interface ILogControl
    {
         ObservableCollection<string> list { get; set; }

         FlowDocument flowDocument { get; set; }

         RichTextBox richTextBox { get; set; }
    }
}