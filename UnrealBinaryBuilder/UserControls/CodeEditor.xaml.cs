using System.IO;
using System.Windows;

namespace UnrealBinaryBuilder.UserControls
{
	/// <summary>
	/// Interaction logic for CodeEditor.xaml
	/// </summary>
	public partial class CodeEditor
	{
		private string Internal_FilePath = null;

		private bool _isDirty = false;
		public bool IsDirty
		{
			get { return _isDirty; }
			set
			{
				if (_isDirty != value)
				{
					_isDirty = value;
					Title = _isDirty ? "Code Editor (Modified)" : "Code Editor";
					SaveBtn.IsEnabled = _isDirty;
				}
			}
		}

		public CodeEditor()
		{
			InitializeComponent();
		}

		public bool LoadFile(string FilePath)
		{
			FileInfo fileInfo = new FileInfo(FilePath);
			if (fileInfo.Exists)
			{
				Internal_FilePath = FilePath;
				TextEditor.Load(FilePath);
				if (fileInfo.IsReadOnly)
				{
					TextEditor.IsEnabled = false;
					SaveBtn.IsEnabled = false;
				}
				IsDirty = false;
				return true;
			}

			return false;
		}

		private void MainCodeEditor_Closed(object sender, System.EventArgs e)
		{
			Internal_FilePath = null;
		}

		private void SaveBtn_Click(object sender, RoutedEventArgs e)
		{
			File.WriteAllText(Internal_FilePath, TextEditor.Text);
			IsDirty = false;
		}

		private void TextEditor_TextChanged(object sender, System.EventArgs e)
		{
			IsDirty = true;
		}
	}
}
