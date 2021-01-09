using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UnrealBinaryBuilder.UserControls;
using System.Diagnostics;

namespace UnrealBinaryBuilder.Classes
{
	public class BuilderSettingsJson
	{
		// Application settings
		public string Theme { get; set; } // Valid settings are Dark, Light, Violet
		public bool bCheckForUpdatesAtStartup { get; set; }
		public bool bEnableDDCMessages { get; set; }
		public bool bEnableEngineBuildConfirmationMessage { get; set; }
		public bool bShowHTML5DeprecatedMessage { get; set; }
		public bool bShowConsoleDeprecatedMessage { get; set; }

		public int EngineVersionIndex { get; set; }
		public string SetupBatFile { get; set; }
		public string AutomationToolPath { get; set; }
		public string CustomBuildFile { get; set; }
		public string GameConfigurations { get; set; }
		public string CustomOptions { get; set; }
		public string AnalyticsOverride { get; set; }

		public bool GitDependencyAll { get; set; }
		public List<GitPlatform> GitDependencyPlatforms { get; set; }
		public int GitDependencyThreads { get; set; }
		public int GitDependencyMaxRetries { get; set; }
		public string GitDependencyProxy { get; set; }
		public bool GitDependencyEnableCache { get; set; }
		public string GitDependencyCache { get; set; }
		public double GitDependencyCacheMultiplier { get; set; }
		public int GitDependencyCacheDays { get; set; }

		public bool bHostPlatformOnly { get; set; }
		public bool bHostPlatformEditorOnly { get; set; }
		public bool bWithWin64 { get; set; }
		public bool bWithWin32 { get; set; }
		public bool bWithMac { get; set; }
		public bool bWithLinux { get; set; }
		public bool bWithLinuxAArch64 { get; set; }
		public bool bWithAndroid { get; set; }
		public bool bWithIOS { get; set; }
		public bool bWithHTML5 { get; set; }
		public bool bWithTVOS { get; set; }
		public bool bWithSwitch { get; set; }
		public bool bWithPS4 { get; set; }
		public bool bWithXboxOne { get; set; }
		public bool bWithLumin { get; set; }
		public bool bWithHoloLens { get; set; }

		public bool bWithDDC { get; set; }
		public bool bHostPlatformDDCOnly { get; set; }
		public bool bSignExecutables { get; set; }
		public bool bEnableSymStore { get; set; }
		public bool bWithFullDebugInfo { get; set; }
		public bool bCleanBuild { get; set; }
		public bool bWithServer { get; set; }
		public bool bWithClient { get; set; }
		public bool bCompileDatasmithPlugins { get; set; }
		public bool bVS2019 { get; set; }
		public bool bShutdownPC { get; set; }
		public bool bShutdownIfBuildSuccess { get; set; }
		public bool bContinueToEngineBuild { get; set; }

		public bool bZipEngineBuild { get; set; }		
		public bool bZipEnginePDB { get; set; }
		public bool bZipEngineDebug { get; set; }
		public bool bZipEngineDocumentation { get; set; }
		public bool bZipEngineExtras { get; set; }
		public bool bZipEngineSource { get; set; }
		public bool bZipEngineFeaturePacks { get; set; }
		public bool bZipEngineSamples { get; set; }
		public bool bZipEngineTemplates { get; set; }
		public bool bZipEngineFastCompression { get; set; }
		public string ZipEnginePath { get; set; }
	}

	public class GitPlatform
	{
		public GitPlatform(string InName, bool bInclude)
		{
			Name = InName;
			bIsIncluded = bInclude;
		}

		public string Name { get; set; }
		public bool bIsIncluded { get; set; }
	}

	public static class BuilderSettings
	{
		private static readonly string PROGRAM_SAVED_PATH_BASE = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		private static readonly string PROGRAM_SAVED_PATH = Path.Combine(PROGRAM_SAVED_PATH_BASE, "UnrealBinaryBuilder");

		private static readonly string PROGRAM_SETTINGS_PATH_BASE = Path.Combine(PROGRAM_SAVED_PATH, "Saved");
		private static readonly string PROGRAM_SETTINGS_FILE_NAME = "Settings.json";

		private static readonly string PROGRAM_LOG_PATH_BASE = Path.Combine(PROGRAM_SAVED_PATH, "Logs");
		private static readonly string PROGRAM_LOG_FILE_NAME = "UnrealBinaryBuilder.log";

		private static readonly string PROGRAM_SETTINGS_PATH = Path.Combine(PROGRAM_SETTINGS_PATH_BASE, PROGRAM_SETTINGS_FILE_NAME);
		private static readonly string PROGRAM_LOG_PATH = Path.Combine(PROGRAM_LOG_PATH_BASE, PROGRAM_LOG_FILE_NAME);

		private static readonly string DEFAULT_GIT_CUSTOM_CACHE_PATH = Path.Combine(PROGRAM_SAVED_PATH, "GitCache");

		private static BuilderSettingsJson GenerateDefaultSettingsJSON()
		{
			BuilderSettingsJson BSJ = new BuilderSettingsJson();
			BSJ.Theme = "Dark";
			BSJ.bCheckForUpdatesAtStartup = true;
			BSJ.bEnableDDCMessages = true;
			BSJ.bEnableEngineBuildConfirmationMessage = true;
			BSJ.bShowHTML5DeprecatedMessage = true;
			BSJ.bShowConsoleDeprecatedMessage = true;

			BSJ.EngineVersionIndex = 0;
			BSJ.SetupBatFile = null;
			BSJ.AutomationToolPath = null;
			BSJ.CustomBuildFile = null;
			BSJ.GameConfigurations = "Development;Shipping";
			BSJ.CustomOptions = null;
			BSJ.AnalyticsOverride = null;

			BSJ.GitDependencyAll = true;
			BSJ.GitDependencyPlatforms = new List<GitPlatform> { 
				new GitPlatform("Win64", true), 
				new GitPlatform("Win32", true),
				new GitPlatform("Linux", false),
				new GitPlatform("Android", false),
				new GitPlatform("Mac", false), 
				new GitPlatform("IOS", false), 
				new GitPlatform("TVOS", false),
				new GitPlatform("HoloLens", false), 
				new GitPlatform("Lumin", false) };
			BSJ.GitDependencyThreads = 4;
			BSJ.GitDependencyMaxRetries = 4;
			BSJ.GitDependencyProxy = "";
			BSJ.GitDependencyCache = DEFAULT_GIT_CUSTOM_CACHE_PATH;
			BSJ.GitDependencyCacheMultiplier = 2.0;
			BSJ.GitDependencyCacheDays = 7;
			BSJ.GitDependencyEnableCache = true;

			BSJ.bHostPlatformOnly = false;
			BSJ.bHostPlatformEditorOnly = false;
			BSJ.bWithWin64 = true;
			BSJ.bWithWin32 = true;
			BSJ.bWithMac = false;
			BSJ.bWithLinux = false;
			BSJ.bWithLinuxAArch64 = false;
			BSJ.bWithAndroid = false;
			BSJ.bWithIOS = false;
			BSJ.bWithHTML5 = false;
			BSJ.bWithTVOS = false;
			BSJ.bWithSwitch = false;
			BSJ.bWithPS4 = false;
			BSJ.bWithXboxOne = false;
			BSJ.bWithLumin = false;
			BSJ.bWithHoloLens = false;

			BSJ.bWithDDC = true;
			BSJ.bHostPlatformDDCOnly = true;
			BSJ.bSignExecutables = false;
			BSJ.bEnableSymStore = false;
			BSJ.bWithFullDebugInfo = false;
			BSJ.bCleanBuild = false;
			BSJ.bWithServer = false;
			BSJ.bWithClient = false;
			BSJ.bCompileDatasmithPlugins = false;
			BSJ.bVS2019 = false;
			BSJ.bShutdownPC = false;
			BSJ.bShutdownIfBuildSuccess = false;
			BSJ.bContinueToEngineBuild = true;

			BSJ.bZipEngineBuild = false;
			BSJ.bZipEngineDebug = false;
			BSJ.bZipEngineDocumentation = true;
			BSJ.bZipEngineExtras = true;
			BSJ.bZipEngineFastCompression = true;
			BSJ.bZipEngineFeaturePacks = true;
			BSJ.bZipEnginePDB = true;
			BSJ.bZipEngineSamples = true;
			BSJ.bZipEngineSource = true;
			BSJ.bZipEngineTemplates = true;
			BSJ.ZipEnginePath = "";

			string JsonOutput = JsonConvert.SerializeObject(BSJ, Formatting.Indented);
			File.WriteAllText(PROGRAM_SETTINGS_PATH, JsonOutput);
			LogEntry logEntry = new LogEntry();
			logEntry.Message = $"New Settings file written to {PROGRAM_SETTINGS_PATH}.";
			((MainWindow)Application.Current.MainWindow).LogControl.AddLogEntry(logEntry, LogViewer.EMessageType.Info);
			((MainWindow)Application.Current.MainWindow).OpenSettingsBtn.IsEnabled = true;
			return JsonConvert.DeserializeObject<BuilderSettingsJson>(JsonOutput);
		}

		public static BuilderSettingsJson GetSettingsFile(bool bLog = false)
		{
			if (Directory.Exists(PROGRAM_LOG_PATH_BASE))
			{
				((MainWindow)Application.Current.MainWindow).OpenLogFolderBtn.IsEnabled = true;
			}

			BuilderSettingsJson ReturnValue = null;
			if (File.Exists(PROGRAM_SETTINGS_PATH))
			{
				string JsonOutput = File.ReadAllText(PROGRAM_SETTINGS_PATH);
				ReturnValue = JsonConvert.DeserializeObject<BuilderSettingsJson>(JsonOutput);
				if (bLog)
				{
					LogEntry logEntry = new LogEntry();
					logEntry.Message = $"Settings loaded from {PROGRAM_SETTINGS_PATH}.";
					((MainWindow)Application.Current.MainWindow).LogControl.AddLogEntry(logEntry, LogViewer.EMessageType.Info);
					((MainWindow)Application.Current.MainWindow).OpenSettingsBtn.IsEnabled = true;
				}
			}
			else
			{
				if (Directory.Exists(PROGRAM_SAVED_PATH) == false)
				{
					Directory.CreateDirectory(PROGRAM_SAVED_PATH);
					if (bLog)
					{
						LogEntry logEntry = new LogEntry();
						logEntry.Message = $"Directory created: {PROGRAM_SAVED_PATH}.";
						((MainWindow)Application.Current.MainWindow).LogControl.AddLogEntry(logEntry, LogViewer.EMessageType.Info);
					}
				}

				if (Directory.Exists(PROGRAM_SETTINGS_PATH_BASE) == false)
				{
					Directory.CreateDirectory(PROGRAM_SETTINGS_PATH_BASE);
					if (bLog)
					{
						LogEntry logEntry = new LogEntry();
						logEntry.Message = $"Directory created: {PROGRAM_SETTINGS_PATH_BASE}.";
						((MainWindow)Application.Current.MainWindow).LogControl.AddLogEntry(logEntry, LogViewer.EMessageType.Info);
					}
				}

				if (Directory.Exists(DEFAULT_GIT_CUSTOM_CACHE_PATH) == false)
				{
					Directory.CreateDirectory(DEFAULT_GIT_CUSTOM_CACHE_PATH);
				}

				ReturnValue = GenerateDefaultSettingsJSON();
			}
			return ReturnValue;
		}

		public static void SaveSettings()
		{
			MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
			BuilderSettingsJson BSJ = new BuilderSettingsJson();
			BSJ.Theme = mainWindow.CurrentTheme;
			BSJ.bCheckForUpdatesAtStartup = mainWindow.SettingsJSON.bCheckForUpdatesAtStartup;
			BSJ.EngineVersionIndex = mainWindow.EngineVersionSelection.SelectedIndex;
			BSJ.SetupBatFile = mainWindow.SetupBatFilePath.Text;
			BSJ.AutomationToolPath = mainWindow.AutomationToolPath.Text;
			BSJ.CustomBuildFile = mainWindow.CustomBuildXMLFile.Text;
			BSJ.GameConfigurations = mainWindow.GameConfigurations.Text;
			BSJ.CustomOptions = mainWindow.CustomOptions.Text;
			BSJ.AnalyticsOverride = mainWindow.AnalyticsOverride.Text;

			BSJ.GitDependencyAll = (bool)mainWindow.bGitSyncAll.IsChecked;
			BSJ.GitDependencyThreads = Convert.ToInt32(mainWindow.GitNumberOfThreads.Text);
			BSJ.GitDependencyMaxRetries = Convert.ToInt32(mainWindow.GitNumberOfRetries.Text);
			BSJ.GitDependencyProxy = "";
			BSJ.GitDependencyCache = mainWindow.GitCachePath.Text;
			BSJ.GitDependencyCacheMultiplier = Convert.ToDouble(mainWindow.GitCacheMultiplier.Text);
			BSJ.GitDependencyCacheDays = Convert.ToInt32(mainWindow.GitCacheDays.Text);
			BSJ.GitDependencyEnableCache = (bool)mainWindow.bGitEnableCache.IsChecked;

			BSJ.bHostPlatformOnly = (bool)mainWindow.bHostPlatformOnly.IsChecked;
			BSJ.bHostPlatformEditorOnly = (bool)mainWindow.bHostPlatformEditorOnly.IsChecked;
			BSJ.bWithWin64 = (bool)mainWindow.bWithWin64.IsChecked;
			BSJ.bWithWin32 = (bool)mainWindow.bWithWin32.IsChecked;
			BSJ.bWithMac = (bool)mainWindow.bWithMac.IsChecked;
			BSJ.bWithLinux = (bool)mainWindow.bWithLinux.IsChecked;
			BSJ.bWithLinuxAArch64 = (bool)mainWindow.bWithLinuxAArch64.IsChecked;
			BSJ.bWithAndroid = (bool)mainWindow.bWithAndroid.IsChecked;
			BSJ.bWithIOS = (bool)mainWindow.bWithIOS.IsChecked;
			BSJ.bWithHTML5 = (bool)mainWindow.bWithHTML5.IsChecked;
			BSJ.bWithTVOS = (bool)mainWindow.bWithTVOS.IsChecked;
			BSJ.bWithSwitch = (bool)mainWindow.bWithSwitch.IsChecked;
			BSJ.bWithPS4 = (bool)mainWindow.bWithPS4.IsChecked;
			BSJ.bWithXboxOne = (bool)mainWindow.bWithXboxOne.IsChecked;
			BSJ.bWithLumin = (bool)mainWindow.bWithLumin.IsChecked;
			BSJ.bWithHoloLens = (bool)mainWindow.bWithHololens.IsChecked;

			BSJ.bWithDDC = (bool)mainWindow.bWithDDC.IsChecked;
			BSJ.bHostPlatformDDCOnly = (bool)mainWindow.bHostPlatformDDCOnly.IsChecked;
			BSJ.bSignExecutables = (bool)mainWindow.bSignExecutables.IsChecked;
			BSJ.bEnableSymStore = (bool)mainWindow.bEnableSymStore.IsChecked;
			BSJ.bWithFullDebugInfo = (bool)mainWindow.bWithFullDebugInfo.IsChecked;
			BSJ.bCleanBuild = (bool)mainWindow.bCleanBuild.IsChecked;
			BSJ.bWithServer = (bool)mainWindow.bWithServer.IsChecked;
			BSJ.bWithClient = (bool)mainWindow.bWithClient.IsChecked;
			BSJ.bCompileDatasmithPlugins = (bool)mainWindow.bCompileDatasmithPlugins.IsChecked;
			BSJ.bVS2019 = (bool)mainWindow.bVS2019.IsChecked;
			BSJ.bShutdownPC = (bool)mainWindow.bShutdownWindows.IsChecked;
			BSJ.bShutdownIfBuildSuccess = (bool)mainWindow.bShutdownIfSuccess.IsChecked;
			BSJ.bContinueToEngineBuild = (bool)mainWindow.bContinueToEngineBuild.IsChecked;

			BSJ.bZipEngineBuild = (bool)mainWindow.bZipBuild.IsChecked;
			BSJ.bZipEngineDebug = (bool)mainWindow.bIncludeDEBUG.IsChecked;
			BSJ.bZipEngineDocumentation = (bool)mainWindow.bIncludeDocumentation.IsChecked;
			BSJ.bZipEngineExtras = (bool)mainWindow.bIncludeExtras.IsChecked;
			BSJ.bZipEngineFastCompression = (bool)mainWindow.bFastCompression.IsChecked;
			BSJ.bZipEngineFeaturePacks = (bool)mainWindow.bIncludeFeaturePacks.IsChecked;
			BSJ.bZipEnginePDB = (bool)mainWindow.bIncludePDB.IsChecked;
			BSJ.bZipEngineSamples = (bool)mainWindow.bIncludeSamples.IsChecked;
			BSJ.bZipEngineSource = (bool)mainWindow.bIncludeSource.IsChecked;
			BSJ.bZipEngineTemplates = (bool)mainWindow.bIncludeTemplates.IsChecked;
			BSJ.ZipEnginePath = mainWindow.ZipPath.Text;

			List<GitPlatform> GitPlatformList = mainWindow.SettingsJSON.GitDependencyPlatforms;
			IEnumerable<ComboBox> ComboBoxCollection = GetChildrenOfType<ComboBox>(mainWindow.PlatformStackPanelMain);
			foreach (GitPlatform gp in GitPlatformList)
			{
				string ComboBoxName = $"Git{gp.Name}Platform";
				foreach (ComboBox c in ComboBoxCollection)
				{
					if (c.Name.ToLower() == ComboBoxName.ToLower())
					{
						gp.bIsIncluded = c.SelectedIndex == 0;
						break;
					}
				}
			}
			BSJ.GitDependencyPlatforms = GitPlatformList;

			string JsonOutput = JsonConvert.SerializeObject(BSJ, Formatting.Indented);
			File.WriteAllText(PROGRAM_SETTINGS_PATH, JsonOutput);
			LogEntry logEntry = new LogEntry();
			logEntry.Message = $"New Settings file written to {PROGRAM_SETTINGS_PATH}.";
			mainWindow.LogControl.AddLogEntry(logEntry, LogViewer.EMessageType.Info);
		}

		public static void WriteToLogFile(string InContent)
		{
			if (Directory.Exists(PROGRAM_LOG_PATH_BASE) == false)
			{
				Directory.CreateDirectory(PROGRAM_LOG_PATH_BASE);
				MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
				mainWindow.OpenLogFolderBtn.IsEnabled = true;
			}
			File.WriteAllText(PROGRAM_LOG_PATH, InContent);
		}

		public static void UpdatePlatformInclusion(string InPlatform, bool bIncluded)
		{
			BuilderSettingsJson BSJ = GetSettingsFile();
			foreach (GitPlatform gp in BSJ.GitDependencyPlatforms)
			{
				if (gp.Name.ToLower() == InPlatform.ToLower())
				{
					gp.bIsIncluded = bIncluded;
					break;
				}
			}

			string JsonOutput = JsonConvert.SerializeObject(BSJ, Formatting.Indented);
			File.WriteAllText(PROGRAM_SETTINGS_PATH, JsonOutput);
		}

		public static void LoadInitialValues()
		{
			MainWindow mainWindow = ((MainWindow)Application.Current.MainWindow);
			List<GitPlatform> GitPlatformList = mainWindow.SettingsJSON.GitDependencyPlatforms;
			IEnumerable<ComboBox> ComboBoxCollection = GetChildrenOfType<ComboBox>(mainWindow.PlatformStackPanelMain);

			foreach (GitPlatform gp in GitPlatformList)
			{
				string ComboBoxName = $"Git{gp.Name}Platform";
				foreach (ComboBox c in ComboBoxCollection)
				{
					if (c.Name.ToLower() == ComboBoxName.ToLower())
					{
						c.SelectedIndex = gp.bIsIncluded ? 0 : 1;
						break;
					}
				}
			}
		}

		public static void OpenLogFolder()
		{
			if (Directory.Exists(PROGRAM_LOG_PATH_BASE))
			{
				Process.Start("explorer.exe", PROGRAM_LOG_PATH_BASE);
			}			
		}

		public static void OpenSettings()
		{
			if (File.Exists(PROGRAM_SETTINGS_PATH))
			{
				Process.Start("notepad.exe", PROGRAM_SETTINGS_PATH);
			}
		}

		private static IEnumerable<T> GetChildrenOfType<T>(DependencyObject dependencyObject) where T : DependencyObject
		{
			if (dependencyObject != null)
			{
				for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
				{
					DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
					if (child != null && child is T)
					{
						yield return (T)child;
					}

					foreach (T childOfChild in GetChildrenOfType<T>(child))
					{
						yield return childOfChild;
					}
				}
			}
		}
	}
}
