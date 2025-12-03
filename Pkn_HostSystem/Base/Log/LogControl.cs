using log4net;
using Pkn_HostSystem.Base.Log.Interface;
using Pkn_HostSystem.Static;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Wpf.Ui;
using Wpf.Ui.Controls;
using RichTextBox = System.Windows.Controls.RichTextBox;


namespace Pkn_HostSystem.Base.Log
{
    public class LogControl<T> : ILogControl
    {
        public ILog Log { get; set; }

        public ISnackbarService SnackbarService { get; set; }

        /// <summary>
        /// 默认赋值总页面统计的FlowDocument
        /// </summary>
        public FlowDocument FlowDocument { get; set; } = GlobalManager.LogRichTextBoxDocument;
        /// <summary>
        /// 默认富文本也是总页面的
        /// </summary>
        public RichTextBox RichTextBox { get; set; } = GlobalManager.LogRichTextBox;


        public LogControl()
        {
            Log = LogManager.GetLogger(typeof(T));
        }

        /// <summary>
        /// 手动创建FlowDocument 记录当前所属日志, RichTextBox富文本为多个时,调用时,单独赋值RichTextBox
        /// </summary>
        /// <param name="flowDocument"></param>
        public LogControl(FlowDocument flowDocument):this()
        {
            FlowDocument = flowDocument;
        }
        /// <summary>
        /// 手动创建FlowDocument ,多个FlowDocument 对应一个RichTextBox时.
        /// </summary>
        /// <param name="flowDocument"></param>
        /// <param name="richTextBox"></param>

        public LogControl(FlowDocument flowDocument,RichTextBox richTextBox) : this()
        {
            FlowDocument = flowDocument;
            RichTextBox = richTextBox;
        }

        public LogControl(ISnackbarService snackbarService) : this()
        {
            SnackbarService = snackbarService;
        }

        public LogControl(ISnackbarService snackbarService, ILog log) : this(snackbarService)
        {
            Log = log;
        }


        #region 只记录
        public void Info(string message)
        {
            try
            {
                Log.Info($"[{TraceContext.Name}]--" + message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Info时发生错误:  {e}");
            }
            LogRichTextBoxAdd("Info", message);
        }
        public void Error(string message)
        {
            try
            {
                Log.Error($"[{TraceContext.Name}]--" + message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Error时发生错误:  {e}");
            }
            LogRichTextBoxAdd("Error", message);
        }
        #endregion


        #region 记录并弹窗

        public void SuccessAndShowTask(string message)
        {
            try
            {
                Log.Info(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Info时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                        TimeSpan.FromSeconds(1));
                }
            });
            LogRichTextBoxAdd("Info", message);
        }

        public void WarningAndShowTask(string message)
        {
            try
            {
                Log.Info(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Info时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                        TimeSpan.FromSeconds(1));
                }
            });
            LogRichTextBoxAdd("Warn", message);
        }


        public void ErrorAndShowTask(string message)
        {
            try
            {
                Log.Error(message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Error时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(10));
                }
            });
            LogRichTextBoxAdd("Error", message);
        }

        public void SuccessAndShowTask(string message, string logMessage)
        {
            try
            {
                Log.Info(logMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Info时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Success, new SymbolIcon(SymbolRegular.Checkmark16),
                        TimeSpan.FromSeconds(1));
                }
            });
            LogRichTextBoxAdd("Info", message);
        }

        public void WarningAndShowTask(string message, string logMessage)
        {
            try
            {
                Log.Info(logMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Info时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Caution, new SymbolIcon(SymbolRegular.Alert24),
                        TimeSpan.FromSeconds(1));
                }
            });
            LogRichTextBoxAdd("Warn", message);
        }

        public void ErrorAndShowTask(string message, string logMessage)
        {
            try
            {
                Log.Error(logMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine($"log4net Error时发生错误:  {e}");
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (SnackbarService.GetSnackbarPresenter() != null)
                {
                    SnackbarService.Show("提示", message, ControlAppearance.Danger,
                        new SymbolIcon(SymbolRegular.Alert24), TimeSpan.FromSeconds(5));
                }
            });
            LogRichTextBoxAdd("Error", message);
        }
        #endregion


        public void InfoToRichTextBox(string message, bool baseNeed = true)
        {
            LogRichTextBoxAdd("Info", message);
            if (baseNeed)
            {
                Info(message);
            }
        }

        public void ErrorToRichTextBox(string message, bool baseNeed = true)
        {
            LogRichTextBoxAdd("Error", message);
            if (baseNeed)
            {
                Error(message);
            }
        }


        public void LogRichTextBoxAdd(string type, string message)
        {
            if (FlowDocument == null)
            {
                return;
            }

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
                paragraph.Inlines.Add(new Run($"[{type}] ") { Foreground = color });
                paragraph.Inlines.Add(new Run(message));
                //添加到flowDocument中
                FlowDocument.Blocks.Add(paragraph);

                // 限制行数
                if (FlowDocument.Blocks.Count > 300)
                    FlowDocument.Blocks.Remove(FlowDocument.Blocks.FirstBlock); //移除到首行

                //滑动到底部
                if (RichTextBox != null)
                {
                    if (!RichTextBox.IsKeyboardFocusWithin)
                    {
                        RichTextBox.ScrollToEnd();
                    }
                }
            });
        }

    }
}