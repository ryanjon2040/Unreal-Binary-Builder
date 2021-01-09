using HandyControl.Tools;
using System.Windows;
using System.Windows.Controls;

namespace UnrealBinaryBuilder.UserControls
{
	public partial class AboutDialog
	{
		private MainWindow _mainWindow;
		public AboutDialog(MainWindow mainWindow)
		{
			InitializeComponent();
			_mainWindow = mainWindow;
		}

		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			_mainWindow.CloseAboutDialog();
		}
	}
}
