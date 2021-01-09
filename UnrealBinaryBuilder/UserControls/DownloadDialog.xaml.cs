using System.Windows.Controls;

namespace UnrealBinaryBuilder.UserControls
{
	/// <summary>
	/// Interaction logic for DownloadDialog.xaml
	/// </summary>
	public partial class DownloadDialog
	{
		MainWindow _mainWindow = null;
		public string VersionText = null;
		public DownloadDialog(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
		}

		public void Initialize(long fileSize, string InVersion)
		{
			DownloadProgressbar.IsIndeterminate = false;
			DownloadProgressbar.Maximum = fileSize;
			DownloadProgressbar.Value = 0;
			VersionText = InVersion;			
		}

		public void SetProgress(int InProgress)
		{
			DownloadProgressbar.Value = InProgress;
			DownloadProgressTextBlock.Text = $"Downloading {VersionText} - {DownloadProgressbar.Value}/{DownloadProgressbar.Maximum}";
		}
	}
}
