using Pkn_HostSystem.Base.Log.Interface;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;


namespace Pkn_HostSystem.Base.Log
{
    public class LogControl<T> : LogBase<T>,ILogControl
    {

        
        public ObservableCollection<string>? list { get; set; }

        public FlowDocument flowDocument { get; set; }

        public RichTextBox richTextBox { get; set; }


        public LogControl(FlowDocument flowDocument)
        {
            this.flowDocument = flowDocument;
        }

        /// <summary>
        /// 通过list记录日志
        /// </summary>
        /// <param name="list"></param>
        public LogControl(ObservableCollection<string>? list)
        {
            this.list = list;
        }
        #region 记录日志通过List
        public void InfoToLogList(string message)
        {
            LogListAdd(message);

            base.Info(message);
            
        }
        public void ErrorToLogList(string message)
        {
            LogListAdd(message);
            base.Error(message);
        }

        public void LogListAdd(string message)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                if (list.Count == 500) list.Clear();

                list.Add($"{DateTime.Now.ToString("MM-dd HH:mm:ss.ffff")}:  {message}");
            });
        }


        #endregion


        #region 通过富文本记录日志

        public void InfoToRichTextBox(string message)
        {
            LogRichTextBoxAdd("Info", message);
            base.Info(message);
        }

        public void ErrorToRichTextBox(string message)
        {
           LogRichTextBoxAdd("Error",message);
            base.Error(message);
        }


        public void LogRichTextBoxAdd(string type, string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                var paragraph = new Paragraph();
                var color = type switch
                {
                    "Info" => Brushes.Gray,
                    "Warn" => Brushes.Orange,
                    "Error" => Brushes.Red,
                    _ => Brushes.Gray
                };
                paragraph.Inlines.Add(new Run($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} ") { Foreground = Brushes.Green });
                paragraph.Inlines.Add(new Run($"[{type}] "){Foreground = color});
                paragraph.Inlines.Add(new Run(message));
                //添加到flowDocument中
                flowDocument.Blocks.Add(paragraph);

                // 限制行数
                if (flowDocument.Blocks.Count > 300)
                    flowDocument.Blocks.Remove(flowDocument.Blocks.FirstBlock);//移除到首行

                //滑动到底部
                if (richTextBox!= null)
                {
                    if (!richTextBox.IsKeyboardFocusWithin)
                    {
                        richTextBox.ScrollToEnd();
                    }
                  
                }
            });
        }

        #endregion
    }
}