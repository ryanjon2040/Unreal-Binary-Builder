using GameAnalyticsSDK.Net;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;
using Unreal_Binary_Builder.Properties;

namespace Unreal_Binary_Builder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string PRODUCT_VERSION = "2.6.1";

        private static readonly string AUTOMATION_TOOL_NAME = "AutomationToolLauncher";
        private static readonly string DEFAULT_BUILD_XML_FILE = "Engine/Build/InstalledEngineBuild.xml";
        private string AutomationExePath = Settings.Default.AutomationPath;
        private Process AutomationToolProcess;

        private int NumErrors = 0;
        private int NumWarnings = 0;

        private int CompiledFiles = 0;
        private int CompiledFilesTotal = 0;

        private bool bIsBuilding = false;
        private bool bLastBuildSuccess = false;

        private readonly Stopwatch StopwatchTimer = new Stopwatch();
        private readonly DispatcherTimer DispatchTimer = new DispatcherTimer();

        private string LogMessage = "";

		private string FinalBuildPath = null;
		private PostBuildSettings postBuildSettings = new PostBuildSettings();

        public MainWindow()
        {
            InitializeComponent();
            GameAnalyticsCSharp.InitializeGameAnalytics(PRODUCT_VERSION, this);

            AutomationToolPath.Text = AutomationExePath;
            ProcessedFilesLabel.Content = "[Compiled: 0. Total: 0]";

            if (File.Exists(AutomationExePath) && Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
            {
                BuildRocketUE.IsEnabled = true;
            }

			EngineVersionSelection.SelectedIndex = Settings.Default.EngineSelection;

            bHostPlatformOnly.IsChecked = Settings.Default.SettingHostPlatformOnly;
            bHostPlatformEditorOnly.IsChecked = Settings.Default.SettingHostPlatformEditorOnly;
            bWithWin64.IsChecked = Settings.Default.bWithWin64;
            bWithWin32.IsChecked = Settings.Default.bWithWin32;
            bWithMac.IsChecked = Settings.Default.bWithMac;
            bWithLinux.IsChecked = Settings.Default.bWithLinux;
            bWithLinuxAArch64.IsChecked = Settings.Default.bWithLinuxAArch64;
            bWithHTML5.IsChecked = Settings.Default.bWithHTML5;
            bWithAndroid.IsChecked = Settings.Default.bWithAndroid;
            bWithIOS.IsChecked = Settings.Default.bWithIOS;
            bWithTVOS.IsChecked = Settings.Default.bWithTVOS;
            bWithSwitch.IsChecked = Settings.Default.bWithSwitch;
            bWithPS4.IsChecked = Settings.Default.bWithPS4;
            bWithXboxOne.IsChecked = Settings.Default.bWithXboxOne;
            bWithLumin.IsChecked = Settings.Default.bWithLumin;

            bWithDDC.IsChecked = Settings.Default.bWithDDC;
            bHostPlatformDDCOnly.IsChecked = Settings.Default.bHostPlatformDDCOnly;
            bSignExecutables.IsChecked = Settings.Default.bSignExes;
            bEnableSymStore.IsChecked = Settings.Default.bSymStore;
            bCleanBuild.IsChecked = Settings.Default.bCleanBuild;
            bWithFullDebugInfo.IsChecked = Settings.Default.bWithFullDebugInfo;
			bWithServer.IsChecked = Settings.Default.bWithServer;
			bWithClient.IsChecked = Settings.Default.bWithClient;
			bWithHoloLens.IsChecked = Settings.Default.bWithHoloLens;
            bCompileDatasmithPlugins.IsChecked = Settings.Default.CompileDatasmithPlugin;
            bVS2019.IsChecked = Settings.Default.VS2019;

            GameConfigurations.Text = Settings.Default.GameConfigurations;
            CustomBuildXMLFile.Text = Settings.Default.CustomBuildXML;

            bShutdownWindows.IsChecked = Settings.Default.bShutdownWindows;
            bShutdownIfSuccess.IsChecked = Settings.Default.bShutdownIfSuccess;

            CustomOptions.Text = Settings.Default.CustomOptions;
            AnalyticsOverride.Text = Settings.Default.AnalyticsType;

            ChangeStatusLabel("Idle.");
            AddLogEntry(string.Format("Welcome to UE4 Binary Builder v{0}", PRODUCT_VERSION));

            DispatchTimer.Tick += new EventHandler(DispatchTimer_Tick);
            DispatchTimer.Interval = new TimeSpan(0, 0, 1);

            CustomOptions.IsEnabled = CustomBuildXMLFile.Text != DEFAULT_BUILD_XML_FILE && CustomBuildXMLFile.Text != string.Empty;
        }

        public void AddLogEntry(string InMessage, bool bIsError = false)
        {
            if (InMessage != null)
            {
                LogEntry logEntry = new LogEntry();
                logEntry.Message = InMessage;
                logEntry.DateTime = DateTime.Now;

                LogViewer.EMessageType InMessageType = bIsError ? LogViewer.EMessageType.Error : LogViewer.EMessageType.Info;                

                if (bIsError == false)
                {
                    const string StepPattern = @"\*{6} \[(\d+)\/(\d+)\]";
                    const string WarningPattern = @"warning|\*\*\* Unable to determine ";
                    const string DebugPattern = @".+\*\s\D\d\D\d\D\s\w+|.+\*\sFor\sUE4";
                    const string ErrorPattern = @"Error_Unknown|ERROR|exited with code 1";
                    const string ProcessedFilesPattern = @"\w.+\.(cpp|cc|c|h|ispc)";

                    Regex StepRgx = new Regex(StepPattern, RegexOptions.IgnoreCase);
                    Regex WarningRgx = new Regex(WarningPattern, RegexOptions.IgnoreCase);
                    Regex DebugRgx = new Regex(DebugPattern, RegexOptions.IgnoreCase);
                    Regex ErrorRgx = new Regex(ErrorPattern, RegexOptions.IgnoreCase);
                    Regex ProcessedFilesRgx = new Regex(ProcessedFilesPattern, RegexOptions.IgnoreCase);

                    if (StepRgx.IsMatch(InMessage))
                    {
                        GroupCollection captures = StepRgx.Match(InMessage).Groups;
                        ChangeStepLabel(captures[1].Value, captures[2].Value);
                        CompiledFiles = 0;
                    }

                    if (ProcessedFilesRgx.IsMatch(InMessage))
                    {
                        CompiledFiles++;
                        CompiledFilesTotal++;
                        Dispatcher.Invoke(() => { ProcessedFilesLabel.Content = $"[Compiled: {CompiledFiles}. Total: {CompiledFilesTotal}]"; });
                    }

                    if (WarningRgx.IsMatch(InMessage))
                    {
                        NumWarnings++;
                        InMessageType = LogViewer.EMessageType.Warning;
                    }
                    else if (ErrorRgx.IsMatch(InMessage))
                    {
                        NumErrors++;
                        InMessageType = LogViewer.EMessageType.Error;
                    }
                    else if (DebugRgx.IsMatch(InMessage))
                    {
                        InMessageType = LogViewer.EMessageType.Debug;
                    }
                }

                LogControl.AddLogEntry(logEntry, InMessageType);
                LogMessage += InMessage + "\r\n";
            }
        }

        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            ChangeStatusLabel(string.Format("Building... Time Elapsed: {0:hh\\:mm\\:ss}", StopwatchTimer.Elapsed));
        }

        private void ChangeStatusLabel(string InStatus)
        {
            StatusLabel.Content = $"Status: {InStatus}";
        }

        private void ChangeStepLabel(string current, string total)
        {
            Dispatcher.Invoke(() => { StepLabel.Content = $"Step: [{current}/{total}] "; });
        }

        private string GetConditionalString(bool? bCondition)
        {
            return (bool)bCondition ? "true" : "false";
        }

        private void SaveAllSettings()
        {
            Settings.Default.AutomationPath = AutomationExePath;

			Settings.Default.EngineSelection = EngineVersionSelection.SelectedIndex;
            Settings.Default.SettingHostPlatformOnly = (bool)bHostPlatformOnly.IsChecked;
            Settings.Default.SettingHostPlatformEditorOnly = (bool)bHostPlatformEditorOnly.IsChecked;
            Settings.Default.bWithWin64 = (bool)bWithWin64.IsChecked;
            Settings.Default.bWithWin32 = (bool)bWithWin32.IsChecked;
            Settings.Default.bWithMac = (bool)bWithMac.IsChecked;
            Settings.Default.bWithLinux = (bool)bWithLinux.IsChecked;
            Settings.Default.bWithLinuxAArch64 = (bool)bWithLinuxAArch64.IsChecked;
            Settings.Default.bWithHTML5 = (bool)bWithHTML5.IsChecked;
            Settings.Default.bWithAndroid = (bool)bWithAndroid.IsChecked;
            Settings.Default.bWithIOS = (bool)bWithIOS.IsChecked;
            Settings.Default.bWithTVOS = (bool)bWithTVOS.IsChecked;
            Settings.Default.bWithSwitch = (bool)bWithSwitch.IsChecked;
            Settings.Default.bWithPS4 = (bool)bWithPS4.IsChecked;
            Settings.Default.bWithXboxOne = (bool)bWithXboxOne.IsChecked;
            Settings.Default.bWithLumin = (bool)bWithLumin.IsChecked;

            Settings.Default.bWithDDC = (bool)bWithDDC.IsChecked;
            Settings.Default.bHostPlatformDDCOnly = (bool)bHostPlatformDDCOnly.IsChecked;
            Settings.Default.bSignExes = (bool)bSignExecutables.IsChecked;
            Settings.Default.bSymStore = (bool)bEnableSymStore.IsChecked;
            Settings.Default.bCleanBuild = (bool)bCleanBuild.IsChecked;
            Settings.Default.bWithFullDebugInfo = (bool)bWithFullDebugInfo.IsChecked;
			Settings.Default.bWithServer = (bool)bWithServer.IsChecked;
			Settings.Default.bWithClient = (bool)bWithClient.IsChecked;
			Settings.Default.bWithHoloLens = (bool)bWithHoloLens.IsChecked;
            Settings.Default.CompileDatasmithPlugin = (bool)bCompileDatasmithPlugins.IsChecked;
            Settings.Default.VS2019 = (bool)bVS2019.IsChecked;

            Settings.Default.GameConfigurations = GameConfigurations.Text;
            Settings.Default.CustomBuildXML = CustomBuildXMLFile.Text;

            Settings.Default.bShutdownWindows = (bool)bShutdownWindows.IsChecked;
            Settings.Default.bShutdownIfSuccess = (bool)bShutdownIfSuccess.IsChecked;

            Settings.Default.CustomOptions = CustomOptions.Text;
            Settings.Default.AnalyticsType = AnalyticsOverride.Text;

            Settings.Default.Save();
        }

        private void AboutBtn_Ok_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog.IsOpen = false;
            GameAnalyticsCSharp.AddDesignEvent("AboutDialog:Close");
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog.IsOpen = true;
            GameAnalyticsCSharp.AddDesignEvent("AboutDialog:Open");
        }

        private void GetSourceCodeMenu_Click(object sender, RoutedEventArgs e)
        {
            GameAnalyticsCSharp.AddDesignEvent("GetSourceCode:Open");
            Process.Start("https://github.com/ryanjon2040/UE4-Binary-Builder");
        }

        private void AutomationToolProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddLogEntry(e.Data);
        }

        private void AutomationToolProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            NumErrors++;
            AddLogEntry(e.Data, true);
        }

        private void AutomationToolProcess_Exited(object sender, EventArgs e)
        {
            DispatchTimer.Stop();
            StopwatchTimer.Stop();
            bLastBuildSuccess = AutomationToolProcess.ExitCode == 0;
            AddLogEntry(string.Format("AutomationToolProcess exited with code {0}\n", AutomationToolProcess.ExitCode.ToString()));

            Dispatcher.Invoke(() =>
            {
                BuildRocketUE.Content = "Start Build";
                ChangeStatusLabel(string.Format("Build finished with code {0}. {1} errors, {2} warnings. Time elapsed: {3:hh\\:mm\\:ss}", AutomationToolProcess.ExitCode, NumErrors, NumWarnings, StopwatchTimer.Elapsed));
            });

            bIsBuilding = false;
            AutomationToolProcess.Close();
            AutomationToolProcess.Dispose();
            AutomationToolProcess = null;
            NumErrors = 0;
            NumWarnings = 0;
            AddLogEntry("==========================BUILD FINISHED==========================");
            AddLogEntry(string.Format("Compiled approximately {0} files.", CompiledFilesTotal));
            AddLogEntry(string.Format("Took {0:hh\\:mm\\:ss}", StopwatchTimer.Elapsed));
            AddLogEntry(string.Format("Build ended at {0}", DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")));
            StopwatchTimer.Reset();
            Dispatcher.Invoke(() =>
            {
				OnBuildFinished(bLastBuildSuccess);
            });
        }

        private void WriteToLogFile()
        {
            string LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnrealBinaryBuilderLog.log");
            File.WriteAllText(LogFile, LogMessage);
        }

		private void OnBuildFinished(bool bBuildSucess)
		{
			if (bBuildSucess)
			{
				if (postBuildSettings.CanSaveToZip())
				{
                    if (FinalBuildPath == null)
                    {
                        FinalBuildPath = Path.GetFullPath(AutomationExePath).Replace(@"\Engine\Binaries\DotNET", @"\LocalBuilds\Engine").Replace(Path.GetFileName(AutomationExePath), "");
                        GameAnalyticsCSharp.LogEvent("Final Build Path was null. Fixed.", GameAnalyticsSDK.Net.EGAErrorSeverity.Info);
                    }
                    AddLogEntry(string.Format("Creating ZIP file. Target Engine Directory is {0}", FinalBuildPath));
                    postBuildSettings.PrepareToSave();
					postBuildSettings.SaveToZip(this, FinalBuildPath, postBuildSettings.ZipPath.Text);
					AddLogEntry("Saving zip file to " + postBuildSettings.ZipPath.Text);
					WriteToLogFile();
					return;
				}
			}

			WriteToLogFile();
			TryShutdown();
		}

        public void TryShutdown()
        {
            if (bShutdownWindows.IsChecked == true)
            {
                if (bShutdownIfSuccess.IsChecked == true)
                {
                    if (bLastBuildSuccess)
                    {
                        GameAnalyticsCSharp.AddDesignEvent("Shutdown:BuildState:Success");
                        Internal_ShutdownWindows();
                    }
                    else
					{
                        GameAnalyticsCSharp.AddDesignEvent("Shutdown:BuildState:Failed");
                    }
                }
                else
                {
                    GameAnalyticsCSharp.AddDesignEvent("Shutdown:Started");
                    Internal_ShutdownWindows();
                }
            }
        }

        private void Internal_ShutdownWindows()
        {
            Process.Start("shutdown", "/s /t 2");
            Application.Current.Shutdown();
        }

        private void AutomationToolBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog NewFileDialog = new OpenFileDialog
            {
                Filter = "exe file (*.exe)|*.exe"
            };

            ChangeStatusLabel(string.Format("Waiting for {0}.exe", AUTOMATION_TOOL_NAME));
            if (NewFileDialog.ShowDialog() == true)
            {
                AutomationExePath = NewFileDialog.FileName;
                AutomationToolPath.Text = AutomationExePath;				
                if (Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
                {
                    BuildRocketUE.IsEnabled = true;
                    ChangeStatusLabel("Idle.");
					FinalBuildPath = Path.GetFullPath(AutomationExePath).Replace(@"\Engine\Binaries\DotNET", @"\LocalBuilds\Engine").Replace(Path.GetFileName(AutomationExePath), "");
                    AddLogEntry(string.Format("Binary build can be found at: {0}", FinalBuildPath));
				}
                else
                {
                    GameAnalyticsCSharp.AddDesignEvent($"AutomationTool:IncorrectName:{Path.GetFileNameWithoutExtension(AutomationExePath)}");
                    ChangeStatusLabel("Error. Invalid automation tool file selected.");
                    MessageBox.Show("This is not Automation Tool Launcher. Please select AutomationToolLauncher.exe", "", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                return;
            }

            ChangeStatusLabel("Idle.");
        }

        private void CustomBuildXMLBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog NewFileDialog = new OpenFileDialog
            {
                Filter = "xml file (*.xml)|*.xml"
            };

            ChangeStatusLabel("Waiting for custom build file...");
            if (NewFileDialog.ShowDialog() == true)
            {
                CustomBuildXMLFile.Text = NewFileDialog.FileName;
                CustomOptions.IsEnabled = true;
                GameAnalyticsCSharp.AddDesignEvent($"BuildXML:Custom:{NewFileDialog.FileName}");
            }

            ChangeStatusLabel("Idle.");
        }

        private void ResetDefaultBuildXML_Click(object sender, RoutedEventArgs e)
        {
            CustomBuildXMLFile.Text = DEFAULT_BUILD_XML_FILE;
            CustomOptions.IsEnabled = false;
            GameAnalyticsCSharp.AddDesignEvent("BuildXML:ResetToDefault");
        }

        private string PrepareCommandline()
        {
            string BuildXMLFile = CustomBuildXMLFile.Text;
            if (BuildXMLFile != DEFAULT_BUILD_XML_FILE)
            {
                BuildXMLFile = string.Format("\"{0}\"", CustomBuildXMLFile.Text);
            }

            if (GameConfigurations.Text == "")
            {
                GameConfigurations.Text = "Development;Shipping";
                GameAnalyticsCSharp.AddDesignEvent("CommandLine:GameConfiguration:Reset");
            }

            string CommandLineArgs = string.Format("BuildGraph -target=\"Make Installed Build Win64\" -script={0} -set:WithDDC={1} -set:SignExecutables={2} -set:EmbedSrcSrvInfo={3} -set:GameConfigurations={4} -set:WithFullDebugInfo={5} -set:HostPlatformEditorOnly={6} -set:AnalyticsTypeOverride={7}",
                    BuildXMLFile,
                    GetConditionalString(bWithDDC.IsChecked),
                    GetConditionalString(bSignExecutables.IsChecked),
                    GetConditionalString(bEnableSymStore.IsChecked),
                    GameConfigurations.Text,
                    GetConditionalString(bWithFullDebugInfo.IsChecked),
                    GetConditionalString(bHostPlatformEditorOnly.IsChecked),
                    AnalyticsOverride.Text);

            if (bWithDDC.IsChecked == true && bHostPlatformDDCOnly.IsChecked == true)
            {
                CommandLineArgs += " -set:HostPlatformDDCOnly=true";
            }

            if (bHostPlatformOnly.IsChecked == true)
            {
                CommandLineArgs += " -set:HostPlatformOnly=true";
                GameAnalyticsCSharp.AddDesignEvent("CommandLine:HostOnly");
            }
            else
            {
                CommandLineArgs += string.Format(" -set:WithWin64={0} -set:WithWin32={1} -set:WithMac={2} -set:WithAndroid={3} -set:WithIOS={4} -set:WithTVOS={5} -set:WithLinux={6} -set:WithLumin={7}",
                        GetConditionalString(bWithWin64.IsChecked),
                        GetConditionalString(bWithWin32.IsChecked),
                        GetConditionalString(bWithMac.IsChecked),
                        GetConditionalString(bWithAndroid.IsChecked),
                        GetConditionalString(bWithIOS.IsChecked),
                        GetConditionalString(bWithTVOS.IsChecked),
                        GetConditionalString(bWithLinux.IsChecked),
                        GetConditionalString(bWithLumin.IsChecked));

                if (SupportHTML5())
                {
                    CommandLineArgs += string.Format(" -set:WithHTML5={0}",
                    GetConditionalString(bWithHTML5.IsChecked));
                }
                
                if (SupportConsoles())
                {
                    CommandLineArgs += string.Format(" -set:WithSwitch={0} -set:WithPS4={1} -set:WithXboxOne={2}",
                    GetConditionalString(bWithSwitch.IsChecked),
                    GetConditionalString(bWithPS4.IsChecked),
                    GetConditionalString(bWithXboxOne.IsChecked));
                }

                if (SupportLinuxAArch64())
                {
                    CommandLineArgs += string.Format(" -set:WithLinuxAArch64={0}", GetConditionalString(bWithLinuxAArch64.IsChecked));
                }
            }

            if (IsEngineSelection425OrAbove())
            {
                CommandLineArgs += string.Format(" -set:CompileDatasmithPlugins={0} -set:VS2019={1}",
                    GetConditionalString(bCompileDatasmithPlugins.IsChecked),
                    GetConditionalString(bVS2019.IsChecked));
            }

            if (EngineVersionSelection.SelectedIndex > 1)
            {
                CommandLineArgs += string.Format(" -set:WithServer={0} -set:WithClient={1} -set:WithHoloLens={2}",
                    GetConditionalString(bWithServer.IsChecked),
                    GetConditionalString(bWithClient.IsChecked),
                    GetConditionalString(bWithHoloLens.IsChecked));
            }

            if (BuildXMLFile != DEFAULT_BUILD_XML_FILE && CustomOptions.Text != string.Empty)
            {
                CommandLineArgs += string.Format(" {0}", CustomOptions.Text);
                AddLogEntry("Using custom options...");
                GameAnalyticsCSharp.AddDesignEvent("CommandLine:UsingCustomOptions");
            }

            if (bCleanBuild.IsChecked == true)
            {
                CommandLineArgs += " -Clean";
                GameAnalyticsCSharp.AddDesignEvent("CommandLine:CleanEnabled");
            }

            return CommandLineArgs;
        }

        private void BuildRocketUE_Click(object sender, RoutedEventArgs e)
        {
            bLastBuildSuccess = false;

            if (bIsBuilding)
            {
                GameAnalyticsCSharp.AddDesignEvent("Build:AutomationTool:Killed");
                AutomationToolProcess.Kill();
                return;
            }

            if (FinalBuildPath == null && string.IsNullOrWhiteSpace(AutomationExePath) == false)
            {
                FinalBuildPath = Path.GetFullPath(AutomationExePath).Replace(@"\Engine\Binaries\DotNET", @"\LocalBuilds\Engine").Replace(Path.GetFileName(AutomationExePath), "");
            }

            if (Directory.Exists(FinalBuildPath))
            {
                MessageBoxResult MessageResult = MessageBox.Show($"Looks like an Engine build is already available at {FinalBuildPath}. Would you like to skip compiling the Engine and start zipping the existing build?\n\nPress Yes to Skip Engine build and start zipping (if enabled).\nPress No to continue with Engine Build.\nPress Cancel to do nothing.", "Zip Binary Version", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                switch (MessageResult)
                {
                    case MessageBoxResult.Yes:
                        GameAnalyticsCSharp.AddDesignEvent("Build:EngineExists:FinishBuild");
                        // We don't want the system to shutdown since user is interacting.
                        bool? bOriginalShutdownState = bShutdownWindows.IsChecked;
                        bShutdownWindows.IsChecked = false;
                        OnBuildFinished(true);
                        bShutdownWindows.IsChecked = bOriginalShutdownState;
                        return;
                    case MessageBoxResult.Cancel:
                        GameAnalyticsCSharp.AddDesignEvent("Build:EngineExists:Exit");
                        return;
                    default:
                        GameAnalyticsCSharp.AddDesignEvent("Build:EngineExists:IgnoreAndContinue");
                        break;
                }
            }

            if (EngineVersionSelection.SelectedIndex == 0)
			{
				MessageBox.Show("Please select your Engine version to build. If you are unsure about the version number look into the following file:\n\n/Engine/Source/Runtime/Launch/Resources/Version.h\n\nAnd check ENGINE_MAJOR_VERSION and ENGINE_MINOR_VERSION.", "Select Engine Version.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

            ChangeStatusLabel("Preparing to build...");

			if (postBuildSettings.ShouldSaveToZip() && postBuildSettings.DirectoryIsWritable() == false)
			{
                GameAnalyticsCSharp.AddDesignEvent("Build:ZipEnabled:InvalidSetting");
                MessageBox.Show(string.Format("You chose to save Engine build as a zip file but below directory is either not available or not writable.\n\n{0}", postBuildSettings.ZipPath.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

            if (CustomBuildXMLFile.Text != DEFAULT_BUILD_XML_FILE)
            {
                if (CustomBuildXMLFile.Text == string.Empty)
                {
                    GameAnalyticsCSharp.LogEvent("Empty Build XML.", GameAnalyticsSDK.Net.EGAErrorSeverity.Error);
                    ChangeStatusLabel("Error. Empty build xml file.");
                    MessageBox.Show(string.Format("Build XML cannot be empty.\n\nIf you don't have a custom build file, press \"Reset to default\" to use default InstalledEngineBuild.xml", CustomBuildXMLFile.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (File.Exists(CustomBuildXMLFile.Text) == false)
                {
                    GameAnalyticsCSharp.LogEvent("BuildXML does not exist.", GameAnalyticsSDK.Net.EGAErrorSeverity.Error);
                    ChangeStatusLabel("Error. Build xml does not exist.");
                    MessageBox.Show(string.Format("Build XML {0} does not exist!", CustomBuildXMLFile.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

			if (SupportHTML5() == false && bWithHTML5.IsChecked == true)
			{
                GameAnalyticsCSharp.AddDesignEvent($"Build:HTML5:IncorrectEngine:{GetEngineName()}");
                bWithHTML5.IsChecked = false;
				MessageBox.Show("HTML5 support was removed from Unreal Engine 4.24 and higher. You had it enabled but since it is of no use, I disabled it.");
			}

            if (SupportConsoles() == false && (bWithSwitch.IsChecked == true || bWithPS4.IsChecked == true || bWithXboxOne.IsChecked == true))
            {
                GameAnalyticsCSharp.AddDesignEvent($"Build:Console:IncorrectEngine:{GetEngineName()}");
                bWithSwitch.IsChecked = bWithPS4.IsChecked = bWithXboxOne.IsChecked = false;
                MessageBox.Show("Console support was removed from Unreal Engine 4.25 and higher. You had it enabled but since it is of no use, I disabled it.");
            }

            if (MessageBox.Show("You are going to build a binary version of Unreal Engine 4. This is a long process and might take time to finish. Are you sure you want to continue? ", "Build Binary Version", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (bWithDDC.IsChecked == true)
                {
                    MessageBoxResult MessageResult = MessageBox.Show("Building Derived Data Cache (DDC) is one of the slowest aspect of the build. You can skip this step if you want to. Do you want to continue with DDC enabled?\n\nPress Yes to continue with build\nPress No to continue without DDC\nPress Cancel to stop build", "Warning", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);

                    switch (MessageResult)
                    {
                        case MessageBoxResult.No:
                            bWithDDC.IsChecked = false;
                            GameAnalyticsCSharp.AddDesignEvent("Build:DDC:AutoDisabled");
                            break;
                        case MessageBoxResult.Cancel:
                            GameAnalyticsCSharp.AddDesignEvent("Build:DDC:Exit");
                            return;
                        default:
                            GameAnalyticsCSharp.AddDesignEvent("Build:DDC:IgnoreAndContinue");
                            break;
                    }
                }

                CompiledFiles = CompiledFilesTotal = 0;
                ProcessedFilesLabel.Content = "[Compiled: 0. Total: 0]";

                LogControl.ClearAllLogs();
                AddLogEntry(string.Format("Welcome to UE4 Binary Builder v{0}", PRODUCT_VERSION));
                BuildRocketUE.Content = "Stop Build";

                string CommandLineArgs = PrepareCommandline();

                ProcessStartInfo AutomationStartInfo = new ProcessStartInfo
                {
                    FileName = AutomationExePath,
                    Arguments = CommandLineArgs,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };

                DispatchTimer.Start();
                StopwatchTimer.Start();

                AutomationToolProcess = new Process();
                AutomationToolProcess.StartInfo = AutomationStartInfo;
                AutomationToolProcess.EnableRaisingEvents = true;
                AutomationToolProcess.OutputDataReceived += new DataReceivedEventHandler(AutomationToolProcess_OutputDataReceived);
                AutomationToolProcess.ErrorDataReceived += new DataReceivedEventHandler(AutomationToolProcess_ErrorDataReceived);
                AutomationToolProcess.Exited += new EventHandler(AutomationToolProcess_Exited);
                AutomationToolProcess.Start();
                AutomationToolProcess.BeginErrorReadLine();
                AutomationToolProcess.BeginOutputReadLine();

                bIsBuilding = true;
                ChangeStatusLabel("Building...");
                GameAnalyticsCSharp.AddDesignEvent("Build:Started");
            }
        }

        private void UnrealBinaryBuilderWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (bIsBuilding)
            {
                if (MessageBox.Show("AutomationTool is still running. Would you like to stop it and exit?", "Build in progress", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (AutomationToolProcess != null)
                    {
                        GameAnalyticsCSharp.AddDesignEvent("Build:AutomationTool:Killed:ExitProgram");
                        AutomationToolProcess.Kill();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            GameAnalyticsCSharp.EndSession();
            SaveAllSettings();

            postBuildSettings.Close();
            postBuildSettings = null;
            Application.Current.Shutdown();
        }

		private void PostBuildSettings_Click(object sender, RoutedEventArgs e)
		{
            GameAnalyticsCSharp.AddDesignEvent("PostBuildSettings:Open");
            postBuildSettings.Owner = this;
			postBuildSettings.ShowDialog();
		}

		private void EngineVersionSelection_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			bWithServer.IsEnabled = bWithClient.IsEnabled = bWithServerLabel.IsEnabled = bWithClientLabel.IsEnabled = EngineVersionSelection.SelectedIndex > 1;
			bWithHTML5.IsEnabled = bWithHTML5Label.IsEnabled = SupportHTML5();
            bWithLinuxAArch64.IsEnabled = bWithLinuxAArch64Label.IsEnabled = SupportLinuxAArch64();
            bWithSwitch.IsEnabled = bWithSwitchLabel.IsEnabled = bWithPS4.IsEnabled = bWithPS4Label.IsEnabled = bWithXboxOne.IsEnabled = bWithXboxOneLabel.IsEnabled = SupportConsoles();
            bCompileDatasmithPlugins.IsEnabled = bCompileDatasmithPluginsLabel.IsEnabled = bVS2019.IsEnabled = bVS2019Label.IsEnabled = IsEngineSelection425OrAbove();
		}

		private bool SupportHTML5()
		{
			return EngineVersionSelection.SelectedIndex < 3;
		}

		private bool SupportLinuxAArch64()
        {
			return EngineVersionSelection.SelectedIndex >= 3;
		}

        private bool SupportConsoles()
        {
            return EngineVersionSelection.SelectedIndex <= 3;
		}

        private bool IsEngineSelection425OrAbove()
        {
            return EngineVersionSelection.SelectedIndex >= 4;
		}

        private string GetEngineName()
		{
            string ReturnString = "Unknown";
            switch (EngineVersionSelection.SelectedIndex)
			{
                case 1:
                    ReturnString = "4.22";
                    break;
                case 2:
                    ReturnString = "4.23";
                    break;
                case 3:
                    ReturnString = "4.24";
                    break;
                case 4:
                    ReturnString = "4.25";
                    break;
            }

            return ReturnString;
		}

		private void CopyCommandLine_Click(object sender, RoutedEventArgs e)
		{
            GameAnalyticsCSharp.AddDesignEvent("CommandLine:CopyToClipboard");
            Clipboard.SetText(PrepareCommandline());
            MessageBox.Show("Commandline copied to clipboard!");
		}

		private void CancelZipping_Click(object sender, RoutedEventArgs e)
		{
            postBuildSettings.CancelTask(this);
		}
	}
}
