using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using UE4_Binary_Builder.Properties;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Threading;
using System.Reflection;

namespace UE4_Binary_Builder
{
    public partial class MainWindow : Form
    {
        private static readonly string AUTOMATION_TOOL_NAME = "AutomationToolLauncher";
        private static readonly string DEFAULT_BUILD_XML_FILE = "Engine/Build/InstalledEngineBuild.xml";
        private string AutomationExePath = Settings.Default.AutomationPath;
        private Process AutomationToolProcess;

        private int NumErrors = 0;
        private int NumWarnings = 0;

        private bool bIsBuilding = false;
        private bool bLastBuildSuccess = false;

        private Stopwatch StopwatchTimer = new Stopwatch();
        private DispatcherTimer DispatchTimer = new DispatcherTimer();

        private delegate void SetLogTextDelegate(string Text);

        public MainWindow()
        {
            InitializeComponent();

            AutomationToolPath.Text = AutomationExePath;            

            if (File.Exists(AutomationExePath) && Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
            {
                BuildRocketUE.Enabled = true;
            }

            bHostPlatformOnly.Checked = Settings.Default.SettingHostPlatformOnly;
            bWithWin64.Checked = Settings.Default.bWithWin64;
            bWithWin32.Checked = Settings.Default.bWithWin32;
            bWithMac.Checked = Settings.Default.bWithMac;
            bWithLinux.Checked = Settings.Default.bWithLinux;
            bWithHTML5.Checked = Settings.Default.bWithHTML5;
            bWithAndroid.Checked = Settings.Default.bWithAndroid;
            bWithIOS.Checked = Settings.Default.bWithIOS;
            bWithTVOS.Checked = Settings.Default.bWithTVOS;
            bWithSwitch.Checked = Settings.Default.bWithSwitch;
            bWithPS4.Checked = Settings.Default.bWithPS4;
            bWithXboxOne.Checked = Settings.Default.bWithXboxOne;
            bWithLumin.Checked = Settings.Default.bWithLumin;

            bWithDDC.Checked = Settings.Default.bWithDDC;
            bSignExecutables.Checked = Settings.Default.bSignExes;
            bEnableSymStore.Checked = Settings.Default.bSymStore;
            bCleanBuild.Checked = Settings.Default.bCleanBuild;
            bWithFullDebugInfo.Checked = Settings.Default.bWithFullDebugInfo;

            GameConfigurations.Text = Settings.Default.GameConfigurations;
            CustomBuildXMLFile.Text = Settings.Default.CustomBuildXML;

            bShutdownWindows.Checked = Settings.Default.bShutdownWindows;
            bShutdownIfSuccess.Checked = Settings.Default.bShutdownIfSuccess;

            ChangeStatusLabel("Idle.");
            LogWindow.Text = "Welcome to UE4 Binary Builder\r\n------------------------------------\r\n";

            DispatchTimer.Tick += new EventHandler(DispatchTimer_Tick);
            DispatchTimer.Interval = new TimeSpan(0, 0, 1);

            bShutdownIfSuccess.Enabled = bShutdownWindows.Checked;
        }

        private void DispatchTimer_Tick(object sender, EventArgs e)
        {
            ChangeStatusLabel(string.Format("Building... Time Elapsed: {0:hh\\:mm\\:ss}", StopwatchTimer.Elapsed));
        }

        private void bHostPlatformOnly_CheckedChanged(object sender, EventArgs e)
        {
            bWithWin64.Enabled = !bHostPlatformOnly.Checked;
            bWithWin32.Enabled = !bHostPlatformOnly.Checked;
            bWithMac.Enabled = !bHostPlatformOnly.Checked;
            bWithLinux.Enabled = !bHostPlatformOnly.Checked;
            bWithHTML5.Enabled = !bHostPlatformOnly.Checked;
            bWithAndroid.Enabled = !bHostPlatformOnly.Checked;
            bWithIOS.Enabled = !bHostPlatformOnly.Checked;
            bWithTVOS.Enabled = !bHostPlatformOnly.Checked;
            bWithSwitch.Enabled = !bHostPlatformOnly.Checked;
            bWithPS4.Enabled = !bHostPlatformOnly.Checked;
            bWithXboxOne.Enabled = !bHostPlatformOnly.Checked;
            bWithLumin.Enabled = !bHostPlatformOnly.Checked;
        }

        private void BuildRocketUE_Click(object sender, EventArgs e)
        {
            bLastBuildSuccess = false;

            if (bIsBuilding)
            {
                AutomationToolProcess.Kill();
                return;
            }

            ChangeStatusLabel("Preparing to build...");
            if (CustomBuildXMLFile.Text != DEFAULT_BUILD_XML_FILE)
            {
                if (CustomBuildXMLFile.Text == string.Empty)
                {
                    ChangeStatusLabel("Error. Empty build xml file.");
                    MessageBox.Show(string.Format("Build XML cannot be empty.\n\nIf you don't have a custom build file, press \"Reset to default\" to use default InstalledEngineBuild.xml", CustomBuildXMLFile.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (File.Exists(CustomBuildXMLFile.Text) == false)
                {
                    ChangeStatusLabel("Error. Build xml does not exist.");
                    MessageBox.Show(string.Format("Build XML {0} does not exist!", CustomBuildXMLFile.Text), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (MessageBox.Show("You are going to build a binary version of Unreal Engine 4. This is a long process and might take time to finish. Are you sure you want to continue? ", "Build Binary Version", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (bWithDDC.Checked)
                {
                    DialogResult MessageResult = MessageBox.Show("Building Derived Data Cache (DDC) is one of the slowest aspect of the build. You can skip this step if you want to. Do you want to continue with DDC enabled?\n\nPress Yes to continue with build\nPress No to continue without DDC\nPress Cancel to stop build", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    
                    switch (MessageResult)
                    {
                        case DialogResult.No:
                            bWithDDC.Checked = false;
                            break;
                        case DialogResult.Cancel:
                            return;
                        default:
                            break;
                    }                    
                }

                LogWindow.Text = "Welcome to UE4 Binary Builder\r\n------------------------------------\r\n";
                BuildRocketUE.Text = "Stop Build";

                if (GameConfigurations.Text == "")
                {
                    GameConfigurations.Text = "Development;Shipping";
                }

                string BuildXMLFile = CustomBuildXMLFile.Text;
                if (BuildXMLFile != DEFAULT_BUILD_XML_FILE)
                {
                    BuildXMLFile = string.Format("\"{0}\"", CustomBuildXMLFile.Text);
                }

                string CommandLineArgs = String.Format("BuildGraph -target=\"Make Installed Build Win64\" -script={0} -set:WithDDC={1} -set:SignExecutables={2} -set:EmbedSrcSrvInfo={3} -set:GameConfigurations={4} -set:WithFullDebugInfo={5}",
                    BuildXMLFile,
                    GetConditionalString(bWithDDC.Checked), 
                    GetConditionalString(bSignExecutables.Checked), 
                    GetConditionalString(bEnableSymStore.Checked), 
                    GameConfigurations.Text,
                    GetConditionalString(bWithFullDebugInfo.Checked));

                if (bHostPlatformOnly.Checked)
                {
                    CommandLineArgs += " -set:HostPlatformOnly=true";
                }
                else
                {
                    CommandLineArgs += String.Format(" -set:WithWin64={0} -set:WithWin32={1} -set:WithMac={2} -set:WithAndroid={3} -set:WithIOS={4} -set:WithTVOS={5} -set:WithLinux={6} -set:WithHTML5={7} -set:WithSwitch={8} -set:WithPS4={9} -set:WithXboxOne={10} -set:WithLumin={11}", 
                        GetConditionalString(bWithWin64.Checked),
                        GetConditionalString(bWithWin32.Checked),
                        GetConditionalString(bWithMac.Checked),
                        GetConditionalString(bWithAndroid.Checked),
                        GetConditionalString(bWithIOS.Checked),
                        GetConditionalString(bWithTVOS.Checked),
                        GetConditionalString(bWithLinux.Checked),
                        GetConditionalString(bWithHTML5.Checked),
                        GetConditionalString(bWithSwitch.Checked),
                        GetConditionalString(bWithPS4.Checked),
                        GetConditionalString(bWithXboxOne.Checked),
                        GetConditionalString(bWithLumin.Checked));
                }

                if (bCleanBuild.Checked)
                {
                    CommandLineArgs += " -Clean";
                }

                AddLog(string.Format("Commandline: {0}\n", CommandLineArgs));
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
                
                if (CustomBuildXMLFile.Text == DEFAULT_BUILD_XML_FILE)
                {
                    ChangeStatusLabel("Building...");
                }
                else
                {
                    ChangeStatusLabel("Building using custom xml...");
                }
            }
        }

        private void AutomationToolBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog NewFileDialog = new OpenFileDialog
            {
                Filter = "exe file (*.exe)|*.exe"
            };

            ChangeStatusLabel(string.Format("Waiting for {0}.exe", AUTOMATION_TOOL_NAME));
            if (NewFileDialog.ShowDialog() == DialogResult.OK)
            {
                AutomationExePath = NewFileDialog.FileName;
                AutomationToolPath.Text = AutomationExePath;
                if (Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
                {
                    BuildRocketUE.Enabled = true;
                    ChangeStatusLabel("Idle.");
                }
                else
                {
                    ChangeStatusLabel("Error. Invalid automation tool file selected.");
                    MessageBox.Show("This is not Automation Tool Launcher. Please select AutomationToolLauncher.exe", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return;
            }

            ChangeStatusLabel("Idle.");
        }

        private void CustomBuildXMLBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog NewFileDialog = new OpenFileDialog
            {
                Filter = "xml file (*.xml)|*.xml"
            };

            ChangeStatusLabel("Waiting for custom build file...");
            if (NewFileDialog.ShowDialog() == DialogResult.OK)
            {
                CustomBuildXMLFile.Text = NewFileDialog.FileName;                
            }

            ChangeStatusLabel("Idle.");
        }

        private void ResetDefaultBuildXML_Click(object sender, EventArgs e)
        {
            CustomBuildXMLFile.Text = DEFAULT_BUILD_XML_FILE;
        }

        private void bShutdownWindows_CheckedChanged(object sender, EventArgs e)
        {
            bShutdownIfSuccess.Enabled = bShutdownWindows.Checked;
        }

        private string GetConditionalString(bool bCondition)
        {
            return bCondition ? "true" : "false";
        }

        private void SaveAllSettings()
        {
            Settings.Default.AutomationPath = AutomationExePath;

            Settings.Default.SettingHostPlatformOnly = bHostPlatformOnly.Checked;
            Settings.Default.bWithWin64 = bWithWin64.Checked;
            Settings.Default.bWithWin32 = bWithWin32.Checked;
            Settings.Default.bWithMac = bWithMac.Checked;
            Settings.Default.bWithLinux = bWithLinux.Checked;
            Settings.Default.bWithHTML5 = bWithHTML5.Checked;
            Settings.Default.bWithAndroid = bWithAndroid.Checked;
            Settings.Default.bWithIOS = bWithIOS.Checked;
            Settings.Default.bWithTVOS = bWithTVOS.Checked;
            Settings.Default.bWithSwitch = bWithSwitch.Checked;
            Settings.Default.bWithPS4 = bWithPS4.Checked;
            Settings.Default.bWithXboxOne = bWithXboxOne.Checked;
            Settings.Default.bWithLumin = bWithLumin.Checked;

            Settings.Default.bWithDDC = bWithDDC.Checked;
            Settings.Default.bSignExes = bSignExecutables.Checked;
            Settings.Default.bSymStore = bEnableSymStore.Checked;
            Settings.Default.bCleanBuild = bCleanBuild.Checked;
            Settings.Default.bWithFullDebugInfo = bWithFullDebugInfo.Checked;

            Settings.Default.GameConfigurations = GameConfigurations.Text;
            Settings.Default.CustomBuildXML = CustomBuildXMLFile.Text;

            Settings.Default.bShutdownWindows = bShutdownWindows.Checked;
            Settings.Default.bShutdownIfSuccess = bShutdownIfSuccess.Checked;

            Settings.Default.Save();
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveAllSettings();
        }

        private void AboutMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A helper application designed to create binary version of Unreal Engine 4 from source.\n\nCreated by Satheesh (ryanjon2040)\n\nUpdated for 4.18 and PS4/Xbox Support by James Baxter (TheJamsh)");
        }

        private void GetSourceCodeMenu_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ryanjon2040/UE4-Binary-Builder");
        }

        private void AutomationToolProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            SetLogText(e.Data);
        }

        private void AutomationToolProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            NumErrors++;
            SetLogText(e.Data);
        }

        private void AutomationToolProcess_Exited(object sender, EventArgs e)
        {
            DispatchTimer.Stop();
            StopwatchTimer.Stop();
            bLastBuildSuccess = AutomationToolProcess.ExitCode == 0;
            SetLogText(string.Format("AutomationToolProcess exited with code {0}\n", AutomationToolProcess.ExitCode.ToString()));
            BuildRocketUE.Text = "Build";
            ChangeStatusLabel(string.Format("Build finished with code {0}. {1} errors, {2} warnings. Time elapsed: {3:hh\\:mm\\:ss}", AutomationToolProcess.ExitCode, NumErrors, NumWarnings, StopwatchTimer.Elapsed));            
            bIsBuilding = false;
            AutomationToolProcess.Close();
            AutomationToolProcess.Dispose();
            AutomationToolProcess = null;
            NumErrors = 0;
            NumWarnings = 0;
            AddLog("==========================BUILD FINISHED==========================");
            AddLog(string.Format("Took {0:hh\\:mm\\:ss}", StopwatchTimer.Elapsed));
            AddLog(string.Format("Build ended at {0}", DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss")));
            WriteToLogFile();
            StopwatchTimer.Reset();
            TryShutdown();
        }

        private void AddLog(string Message)
        {
            if (Message != null)
            {
                string WarningPattern = @"(warning)";
                string ErrorPattern = @"(error)";

                Regex WarningRgx = new Regex(WarningPattern, RegexOptions.IgnoreCase);
                Regex ErrorRgx = new Regex(ErrorPattern, RegexOptions.IgnoreCase);
                MatchCollection WarningMatches = WarningRgx.Matches(Message);
                MatchCollection ErrorMatches = ErrorRgx.Matches(Message);

                LogWindow.ForeColor = Color.White;

                if (WarningRgx.IsMatch(Message))
                {
                    NumWarnings++;
                    LogWindow.ForeColor = Color.Yellow;
                }

                if (ErrorRgx.IsMatch(Message))
                {
                    LogWindow.ForeColor = Color.Red;
                }

                LogWindow.Text += Message + "\r\n";
                LogWindow.SelectionStart = LogWindow.TextLength;
                LogWindow.ScrollToCaret();
            }
        }

        private void SetLogText(string Message)
        {
            /* InvokeRequired returns true if the calling thread ID is different from creation thread ID */
            if (LogWindow.InvokeRequired)
            {
                SetLogTextDelegate LogDelegate = new SetLogTextDelegate(SetLogText);
                Invoke(LogDelegate, new object[] { Message });
            }
            else
            {
                AddLog(Message);
            }
        }

        private void ChangeStatusLabel(string InStatus)
        {
            StatusLabel.Text = string.Format("Status: {0}", InStatus);
        }

        private void TryShutdown()
        {
            if (bShutdownWindows.Checked)
            {
                if (bShutdownIfSuccess.Checked)
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
            Application.Exit();
        }

        private void WriteToLogFile()
        {
            string LogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "UnrealBinaryBuilderLog.log");
            File.WriteAllText(LogFile, LogWindow.Text);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //Enabled double buffered log to prevent flickering
            var doubleBufferPropertyInfo = LogWindow.GetType().GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            doubleBufferPropertyInfo.SetValue(LogWindow, true, null);

        }
    }
}
