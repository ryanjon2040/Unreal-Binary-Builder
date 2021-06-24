using Sentry;
using System.Windows;
using System.Windows.Threading;

namespace UnrealBinaryBuilder
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
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
			SentrySdk.CaptureException(e.Exception);

			// If you want to avoid the application from crashing:
			//e.Handled = true;
		}
	}
}
