using GameAnalyticsSDK.Net;

namespace UnrealBinaryBuilder.Classes
{
	public static class GameAnalyticsCSharp
	{
		// Please DO NOT change this. If you don't want Analytics, set both GAME_KEY and SECRET_KEY to null ///////////////////
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
				mainWindow.AddLogEntry("UE4 Binary Builder Analytics Initialized.");
#if DEBUG
				GameAnalytics.AddDesignEvent("Program:Start:Debug");
#else
				GameAnalytics.AddDesignEvent("Program:Start:Release");
#endif
			}
		}

		public static void EndSession()
		{
			if (mainWindow != null)
			{
				GameAnalytics.EndSession();
			}
		}

		public static void AddDesignEvent(string InMessage)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddDesignEvent(InMessage);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Design): {InMessage}");
#endif
			}
		}

		public static void AddProgressStart(string InProgression01)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, InProgression01);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress Start): {InProgression01}");
#endif
			}
		}

		public static void AddProgressStart(string InProgression01, string InProgression02)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(EGAProgressionStatus.Start, InProgression01, InProgression02);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress Start): {InProgression01}::{InProgression02}");
#endif
			}
		}

		public static void AddProgressEnd(string InProgression01, bool bIsFail = false)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(bIsFail ? EGAProgressionStatus.Fail : EGAProgressionStatus.Complete, InProgression01);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress End): {InProgression01}");
#endif
			}
		}

		public static void AddProgressEnd(string InProgression01, string InProgression02, bool bIsFail = false)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddProgressionEvent(bIsFail ? EGAProgressionStatus.Fail : EGAProgressionStatus.Complete, InProgression01, InProgression02);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Progress End): {InProgression01}::{InProgression02}");
#endif
			}
		}

		public static void LogEvent(string InMessage, EGAErrorSeverity InLogLevel)
		{
			if (mainWindow != null)
			{
				GameAnalytics.AddErrorEvent(InLogLevel, InMessage);
#if DEBUG
				mainWindow.AddLogEntry($"UE4 Binary Builder Analytics (Log): {InMessage}");
#endif
			}
		}
	}
}
