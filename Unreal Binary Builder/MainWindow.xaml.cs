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
        private static readonly string PRODUCT_VERSION = "2.5.1";

        private static readonly string AUTOMATION_TOOL_NAME = "AutomationToolLauncher";
        private static readonly string DEFAULT_BUILD_XML_FILE = "Engine/Build/InstalledEngineBuild.xml";
        private string AutomationExePath = Settings.Default.AutomationPath;
        private Process AutomationToolProcess;

        private int NumErrors = 0;
        private int NumWarnings = 0;

        private bool bIsBuilding = false;
        private bool bLastBuildSuccess = false;

        private readonly Stopwatch StopwatchTimer = new Stopwatch();
        private readonly DispatcherTimer DispatchTimer = new DispatcherTimer();

        private string LogMessage = "";

		private string FinalBuildPath = "";
		private PostBuildSettings postBuildSettings = new PostBuildSettings();

        public MainWindow()
        {
            InitializeComponent();

            AutomationToolPath.Text = AutomationExePath;

            if (File.Exists(AutomationExePath) && Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
            {
                BuildRocketUE.IsEnabled = true;
            }

			EngineVersionSelection.SelectedIndex = Settings.Default.EngineSelection;

            bHostPlatformOnly.IsChecked = Settings.Default.SettingHostPlatformOnly;
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

            GameConfigurations.Text = Settings.Default.GameConfigurations;
            CustomBuildXMLFile.Text = Settings.Default.CustomBuildXML;

            bShutdownWindows.IsChecked = Settings.Default.bShutdownWindows;
            bShutdownIfSuccess.IsChecked = Settings.Default.bShutdownIfSuccess;

            CustomOptions.Text = Settings.Default.CustomOptions;

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

                    Regex StepRgx = new Regex(StepPattern, RegexOptions.IgnoreCase);
                    Regex WarningRgx = new Regex(WarningPattern, RegexOptions.IgnoreCase);
                    Regex DebugRgx = new Regex(DebugPattern, RegexOptions.IgnoreCase);
                    Regex ErrorRgx = new Regex(ErrorPattern, RegexOptions.IgnoreCase);
                    if (StepRgx.IsMatch(InMessage))
                    {
                        GroupCollection captures = StepRgx.Match(InMessage).Groups;
                        ChangeStepLabel(captures[1].Value, captures[2].Value);
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
            Dispatcher.Invoke(() => { StepLabel.Content = $"Step: [{current}/{total}]"; });
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

            Settings.Default.GameConfigurations = GameConfigurations.Text;
            Settings.Default.CustomBuildXML = CustomBuildXMLFile.Text;

            Settings.Default.bShutdownWindows = (bool)bShutdownWindows.IsChecked;
            Settings.Default.bShutdownIfSuccess = (bool)bShutdownIfSuccess.IsChecked;

            Settings.Default.CustomOptions = CustomOptions.Text;

            Settings.Default.Save();
        }

        private void AboutBtn_Ok_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog.IsOpen = false;
        }

        private void AboutMenu_Click(object sender, RoutedEventArgs e)
        {
            AboutDialog.IsOpen = true;
        }

        private void GetSourceCodeMenu_Click(object sender, RoutedEventArgs e)
        {
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
                        Internal_ShutdownWindows();
                    }
                }
                else
                {
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
				}
                else
                {
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
            }

            ChangeStatusLabel("Idle.");
        }

        private void ResetDefaultBuildXML_Click(object sender, RoutedEventArgs e)
        {
            CustomBuildXMLFile.Text = DEFAULT_BUILD_XML_FILE;
            CustomOptions.IsEnabled = false;
        }

        private void BuildRocketUE_Click(object sender, RoutedEventArgs e)
        {
            bLastBuildSuccess = false;

            if (bIsBuilding)
            {
                AutomationToolProcess.Kill();
                return;
            }

			if (EngineVersionSelection.SelectedIndex == 0)
			{
				MessageBox.Show("Please select your Engine version to build. If you are unsure about the version number look into the following file:\n\n/Engine/Source/Runtime/Launch/Resources/Version.h\n\nAnd check ENGINE_MAJOR_VERSION and ENGINE_MINOR_VERSION.", "Select Engine Version.", MessageBoxButton.OK, MessageBoxImage.Exclamation);
				return;
			}

            ChangeStatusLabel("Preparing to build...");

			if (postBuildSettings.ShouldSaveToZip() && postBuildSettings.DirectoryIsWritable() == false)
			{
				MessageBox.Show(string.Format("You chose to save Engine build as a zip file but below directory is either not available or not writable.\n\n{0}", postBuildSettings.ZipPath.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

            if (CustomBuildXMLFile.Text != DEFAULT_BUILD_XML_FILE)
            {
                if (CustomBuildXMLFile.Text == string.Empty)
                {
                    ChangeStatusLabel("Error. Empty build xml file.");
                    MessageBox.Show(string.Format("Build XML cannot be empty.\n\nIf you don't have a custom build file, press \"Reset to default\" to use default InstalledEngineBuild.xml", CustomBuildXMLFile.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (File.Exists(CustomBuildXMLFile.Text) == false)
                {
                    ChangeStatusLabel("Error. Build xml does not exist.");
                    MessageBox.Show(string.Format("Build XML {0} does not exist!", CustomBuildXMLFile.Text), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

			if (SupportHTML5() == false && bWithHTML5.IsChecked == true)
			{
				bWithHTML5.IsChecked = false;
				MessageBox.Show("HTML5 support was removed from Unreal Engine 4.24 and higher. You had it enabled but since it is of no use, I disabled it.");
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
                            break;
                        case MessageBoxResult.Cancel:
                            return;
                        default:
                            break;
                    }
                }

                LogControl.ClearAllLogs();                
                AddLogEntry(string.Format("Welcome to UE4 Binary Builder v{0}", PRODUCT_VERSION));
                BuildRocketUE.Content = "Stop Build";

                if (GameConfigurations.Text == "")
                {
                    GameConfigurations.Text = "Development;Shipping";
                }

                string BuildXMLFile = CustomBuildXMLFile.Text;
                if (BuildXMLFile != DEFAULT_BUILD_XML_FILE)
                {
                    BuildXMLFile = string.Format("\"{0}\"", CustomBuildXMLFile.Text);
                }

                string CommandLineArgs = string.Format("BuildGraph -target=\"Make Installed Build Win64\" -script={0} -set:WithDDC={1} -set:SignExecutables={2} -set:EmbedSrcSrvInfo={3} -set:GameConfigurations={4} -set:WithFullDebugInfo={5}",
                    BuildXMLFile,
                    GetConditionalString(bWithDDC.IsChecked),
                    GetConditionalString(bSignExecutables.IsChecked),
                    GetConditionalString(bEnableSymStore.IsChecked),
                    GameConfigurations.Text,
                    GetConditionalString(bWithFullDebugInfo.IsChecked));

                if (bWithDDC.IsChecked == true && bHostPlatformDDCOnly.IsChecked == true)
                {
                    CommandLineArgs += " -set:HostPlatformDDCOnly=true";
                }

                if (bHostPlatformOnly.IsChecked == true)
                {
                    CommandLineArgs += " -set:HostPlatformOnly=true";
                }
                else
                {
                    if (SupportHTML5())
					{
						CommandLineArgs += string.Format(" -set:WithWin64={0} -set:WithWin32={1} -set:WithMac={2} -set:WithAndroid={3} -set:WithIOS={4} -set:WithTVOS={5} -set:WithLinux={6} -set:WithHTML5={7} -set:WithSwitch={8} -set:WithPS4={9} -set:WithXboxOne={10} -set:WithLumin={11}",
						GetConditionalString(bWithWin64.IsChecked),
						GetConditionalString(bWithWin32.IsChecked),
						GetConditionalString(bWithMac.IsChecked),
						GetConditionalString(bWithAndroid.IsChecked),
						GetConditionalString(bWithIOS.IsChecked),
						GetConditionalString(bWithTVOS.IsChecked),
						GetConditionalString(bWithLinux.IsChecked),
						GetConditionalString(bWithHTML5.IsChecked),
						GetConditionalString(bWithSwitch.IsChecked),
						GetConditionalString(bWithPS4.IsChecked),
						GetConditionalString(bWithXboxOne.IsChecked),
						GetConditionalString(bWithLumin.IsChecked));
					}
					else
					{
						CommandLineArgs += string.Format(" -set:WithWin64={0} -set:WithWin32={1} -set:WithMac={2} -set:WithAndroid={3} -set:WithIOS={4} -set:WithTVOS={5} -set:WithLinux={6} -set:WithSwitch={7} -set:WithPS4={8} -set:WithXboxOne={9} -set:WithLumin={10}",
						GetConditionalString(bWithWin64.IsChecked),
						GetConditionalString(bWithWin32.IsChecked),
						GetConditionalString(bWithMac.IsChecked),
						GetConditionalString(bWithAndroid.IsChecked),
						GetConditionalString(bWithIOS.IsChecked),
						GetConditionalString(bWithTVOS.IsChecked),
						GetConditionalString(bWithLinux.IsChecked),
						GetConditionalString(bWithSwitch.IsChecked),
						GetConditionalString(bWithPS4.IsChecked),
						GetConditionalString(bWithXboxOne.IsChecked),
						GetConditionalString(bWithLumin.IsChecked));
					}

					if (SupportLinuxAArch64()) 
                    {
						CommandLineArgs += string.Format(" -set:WithLinuxAArch64={0}", GetConditionalString(bWithLinuxAArch64.IsChecked));
					}
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
                }

                if (bCleanBuild.IsChecked == true)
                {
                    CommandLineArgs += " -Clean";
                }

                AddLogEntry(string.Format("Commandline: {0}\n", CommandLineArgs));
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
                        AutomationToolProcess.Kill();
                    }
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            SaveAllSettings();

            postBuildSettings.Close();
            postBuildSettings = null;
            Application.Current.Shutdown();
        }

		private void PostBuildSettings_Click(object sender, RoutedEventArgs e)
		{
			postBuildSettings.Owner = this;
			postBuildSettings.ShowDialog();
		}

		private void EngineVersionSelection_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			bWithServer.IsEnabled = bWithClient.IsEnabled = bWithServerLabel.IsEnabled = bWithClientLabel.IsEnabled = EngineVersionSelection.SelectedIndex > 1;
			bWithHTML5.IsEnabled = bWithHTML5Label.IsEnabled = SupportHTML5();
            bWithLinuxAArch64.IsEnabled = bWithLinuxAArch64Label.IsEnabled = SupportLinuxAArch64();
		}

		private bool SupportHTML5()
		{
			return EngineVersionSelection.SelectedIndex < 3;
		}

		private bool SupportLinuxAArch64()
        {
			return EngineVersionSelection.SelectedIndex >= 3;
		}
	}
}
