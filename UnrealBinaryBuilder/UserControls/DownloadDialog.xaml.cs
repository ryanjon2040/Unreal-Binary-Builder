using System.Windows.Controls;
using UnrealBinaryBuilder.Classes;

namespace UnrealBinaryBuilder.UserControls
{
	/// <summary>
	/// Interaction logic for DownloadDialog.xaml
	/// </summary>
	public partial class DownloadDialog
	{
		long CurrentFileSize;
		MainWindow _mainWindow = null;
		public string VersionText = null;
		public DownloadDialog(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
		}

		public void Initialize(long fileSize, string InVersion)
		{
			CurrentFileSize = fileSize;
			DownloadProgressbar.IsIndeterminate = false;
			DownloadProgressbar.Maximum = 100;
			DownloadProgressbar.Value = 0;
			VersionText = InVersion;
		}

		public void SetProgress(int InProgress)
		{
			DownloadProgressbar.Value = InProgress;
			DownloadProgressTextBlock.Text = $"Downloading {VersionText} - {DownloadProgressbar.Value}/{DownloadProgressbar.Maximum} (File Size: {PostBuildSettings.BytesToString(CurrentFileSize)})";
		}
	}
}
