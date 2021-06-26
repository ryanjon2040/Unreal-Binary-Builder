using Sentry;
using System.Windows;
using System.Windows.Threading;
using UnrealBinaryBuilder.UserControls;

namespace UnrealBinaryBuilder
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private CrashReporter crashReporter = null;

		public App()
		{
			DispatcherUnhandledException += App_DispatcherUnhandledException;
			SentryOptions sentryOptions = new SentryOptions();
			sentryOptions.Dsn = "https://23f478ac8a004c5782a7f6597c0b0325@o502371.ingest.sentry.io/5584682";
			sentryOptions.StackTraceMode = StackTraceMode.Enhanced;
			sentryOptions.AttachStacktrace = true;
			sentryOptions.AutoSessionTracking = true;
			sentryOptions.DetectStartupTime = StartupTimeDetectionMode.Best;
			sentryOptions.Release = UnrealBinaryBuilderHelpers.GetProductVersionString();
			sentryOptions.ReportAssembliesMode = ReportAssembliesMode.InformationalVersion;
			SentrySdk.Init(sentryOptions);
		}

		void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			SentryId sentryId = SentrySdk.CaptureException(e.Exception);
			e.Handled = true;

			crashReporter = new CrashReporter(e.Exception);
			crashReporter.Owner = Current.MainWindow;
			crashReporter.CurrentSentryId = sentryId;
			crashReporter.ShowDialog();
			crashReporter = null;
		}
	}
}
