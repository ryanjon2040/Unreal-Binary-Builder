using System.Windows;
using UnrealBinaryBuilder.Classes;

namespace UnrealBinaryBuilder.UserControls
{
	/// <summary>
	/// Interaction logic for DownloadDialog.xaml
	/// </summary>
	public partial class DownloadDialog
	{
		long CurrentFileSize;
		private MainWindow _mainWindow = null;
		public string VersionText = null;
		public DownloadDialog(MainWindow mainWindow, string InVersion)
		{
			InitializeComponent();
			CefWebBrowser.Address = $"{CefWebBrowser.Address}#{InVersion.Replace(".", "")}";
			_mainWindow = mainWindow;
			DownloadProgressbar.Visibility = Visibility.Collapsed;
			DownloadNowBtn.Visibility = CancelBtn.Visibility = Visibility.Visible;
			VersionText = InVersion;
			DownloadProgressTextBlock.Text = $"Download {VersionText}? You are running {UnrealBinaryBuilderHelpers.GetProductVersionString()}";
		}

		public void Initialize(long fileSize)
		{
			CurrentFileSize = fileSize;
			DownloadProgressbar.IsIndeterminate = false;
			DownloadProgressbar.Maximum = 100;
			DownloadProgressbar.Value = 0;
			DownloadNowBtn.IsEnabled = CancelBtn.IsEnabled = false;
			DownloadProgressTextBlock.Text = "Downloading...";
			DownloadProgressbar.Visibility = Visibility.Visible;
		}

		public void SetProgress(int InProgress)
		{
			DownloadProgressbar.Value = InProgress;
			DownloadProgressTextBlock.Text = $"Downloading {VersionText} - {DownloadProgressbar.Value}/{DownloadProgressbar.Maximum} (File Size: {PostBuildSettings.BytesToString(CurrentFileSize)})";
		}

		private void DownloadNowBtn_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.DownloadUpdate();
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.CloseUpdateDialogWindow();
		}
	}
}
