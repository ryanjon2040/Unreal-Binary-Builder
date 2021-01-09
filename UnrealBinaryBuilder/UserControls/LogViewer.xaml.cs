/************************************************************************/
/* Credits to Federico Berasategui for this implementation.             */
/* https://stackoverflow.com/a/16745054                                 */
/************************************************************************/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using static UnrealBinaryBuilder.MainWindow;

namespace UnrealBinaryBuilder.UserControls
{
    public partial class LogViewer : UserControl
    {
        private ObservableCollection<LogEntry> LogEntries { get; set; }
        private bool AutoScroll = true;

        public enum EMessageType
        {
            Info,
            Debug,
            Warning,
            Error
        }

        public LogViewer()
        {
            InitializeComponent();
            DataContext = LogEntries = new ObservableCollection<LogEntry>();
        }

        public void AddZipLog(LogEntry InLogEntry, ZipLogInclusionType InType)
		{
            InLogEntry.DateTime = DateTime.Now;
            switch (InType)
			{
                case ZipLogInclusionType.FileIncluded:
                    InLogEntry.MessageColor = Brushes.Green;
                    break;
                case ZipLogInclusionType.FileSkipped:
                    InLogEntry.MessageColor = Brushes.Orange;
                    break;
                case ZipLogInclusionType.ExtensionSkipped:
                    InLogEntry.MessageColor = Brushes.OrangeRed;
                    break;
			}
            Dispatcher.BeginInvoke((Action)(() => LogEntries.Add(InLogEntry)));
        }

        public void AddLogEntry(LogEntry InLogEntry, EMessageType InMessageType)
        {
            InLogEntry.DateTime = DateTime.Now;
            switch (InMessageType)
            {
                case EMessageType.Info:
                    InLogEntry.MessageColor = Brushes.WhiteSmoke;
                    break;
                case EMessageType.Debug:
                    InLogEntry.MessageColor = Brushes.Aqua;
                    break;
                case EMessageType.Warning:
                    InLogEntry.MessageColor = Brushes.Gold;
                    break;
                case EMessageType.Error:
                    InLogEntry.MessageColor = Brushes.Red;
                    break;
                default:
                    break;

            }
            Dispatcher.BeginInvoke((Action)(() => LogEntries.Add(InLogEntry)));
        }

        public void ClearAllLogs()
        {
            Dispatcher.BeginInvoke((Action)(() => LogEntries.Clear()));
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            try
            {
                // User scroll event : set or unset autoscroll mode
                if (e.ExtentHeightChange == 0)
                {   // Content unchanged : user scroll event
                    if ((e.Source as ScrollViewer).VerticalOffset == (e.Source as ScrollViewer).ScrollableHeight)
                    {   // Scroll bar is in bottom
                        // Set autoscroll mode
                        AutoScroll = true;
                    }
                    else
                    {   // Scroll bar isn't in bottom
                        // Unset autoscroll mode
                        AutoScroll = false;
                    }
                }

                // Content scroll event : autoscroll eventually
                if (AutoScroll && e.ExtentHeightChange != 0)
                {   // Content changed and autoscroll mode set
                    // Autoscroll
                    (e.Source as ScrollViewer).ScrollToVerticalOffset((e.Source as ScrollViewer).ExtentHeight);
                }
            }
            catch (Exception ex)
            {
                LogEntry logEntry = new LogEntry();
                logEntry.Message = string.Format("APPLICATION ERROR: " + ex.Message);
                logEntry.DateTime = DateTime.Now;
                AddLogEntry(logEntry, EMessageType.Error);
            }
        }
    }

    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }));
        }
    }

    public class LogEntry : PropertyChangedBase
    {
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public Brush MessageColor { get; set; }
    }
}
