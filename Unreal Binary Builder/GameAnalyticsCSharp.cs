using GameAnalyticsSDK.Net;

namespace Unreal_Binary_Builder
{
	public static class GameAnalyticsCSharp
	{
		// PLEASE DO NOT CHANGE THIS. IF YOU DON'T WANT ANALYTICS, SET THEM TO null ///////////////////
		private static readonly string GAME_KEY = "33551f8809806eaac2ad5be41c403c8d"; // null
		private static readonly string SECRET_KEY = "38986827a8ff55c879424d4bea7cfe8e99b44b03"; // null
		///////////////////////////////////////////////////////////////////////////////////////////////

		private static MainWindow mainWindow = null;

		public static void InitializeGameAnalytics(string InProductVersion, MainWindow InMainWindow)
		{
			if (GAME_KEY != null && SECRET_KEY != null)
			{
				GameAnalytics.ConfigureBuild($"UE4 Binary Builder {InProductVersion}");

				// https://gameanalytics.com/docs/item/c-sharp-sdk#initializing
				GameAnalytics.Initialize(GAME_KEY, SECRET_KEY);
				mainWindow = InMainWindow;
			}
		}

		public static void EndSession()
		{
			GameAnalytics.EndSession();
		}

		public static void AddDesignEvent(string InMessage)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddDesignEvent(InMessage);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Design): {InMessage}");
			}
		}

		public static void AddProgressStart(string InProgression01)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, InProgression01);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress Start): {InProgression01}");
			}
		}

		public static void AddProgressStart(string InProgression01, string InProgression02)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, InProgression01, InProgression02);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress Start): {InProgression01}::{InProgression02}");
			}
		}

		public static void AddProgressEnd(string InProgression01, bool bIsFail = false)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(bIsFail ? EGAProgressionStatus.Fail : EGAProgressionStatus.Complete, InProgression01);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress End): {InProgression01}");
			}
		}

		public static void AddProgressEnd(string InProgression01, string InProgression02, bool bIsFail = false)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(bIsFail ? EGAProgressionStatus.Fail : EGAProgressionStatus.Complete, InProgression01, InProgression02);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress End): {InProgression01}::{InProgression02}");
			}
		}

		public static void LogEvent(string InMessage, EGAErrorSeverity InLogLevel)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddErrorEvent(InLogLevel, InMessage);
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Log): {InMessage}");
			}
		}
	}
}
