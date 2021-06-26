using System;
using Sentry;

namespace UnrealBinaryBuilder.UserControls
{
	/// <summary>
	/// Interaction logic for CrashReporter.xaml
	/// </summary>
	public partial class CrashReporter
	{
		public SentryId CurrentSentryId;

		public CrashReporter(Exception InException)
		{
			InitializeComponent();
			Username.Text = Environment.UserName;
			string StackTraceMessage = $"Source ->\t{InException.Source}\nMessage ->\t{InException.Message}\nTarget ->\t{InException.TargetSite}\nStackTrace ->\n{InException.StackTrace}";
			StackTraceText.Text = StackTraceMessage;
		}

		private void SubmitBtn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			string CommentText = $"{Comment.Text}\n\nExceptionDetails ->\n{StackTraceText.Text}";
			UserFeedback userFeedback = new UserFeedback(CurrentSentryId, Username.Text, Email.Text, CommentText);
			SentrySdk.CaptureUserFeedback(userFeedback);
			HandyControl.Controls.MessageBox.Success("Thank you for submitting the crash report!");
			Close();
		}

		private void CancelBtn_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Close();
		}
	}
}
