using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using UE4_Binary_Builder.Properties;
using System.Text.RegularExpressions;
using System.Drawing;

namespace UE4_Binary_Builder
{
    public partial class MainWindow : Form
    {
        private static string AUTOMATION_TOOL_NAME = "AutomationTool";
        private string AutomationExePath = Settings.Default.AutomationPath;
        private Process AutomationToolProcess;

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

            bWithDDC.Checked = Settings.Default.bWithDDC;
            bSignExecutables.Checked = Settings.Default.bSignExes;
            bEnableSymStore.Checked = Settings.Default.bSymStore;
            bCleanBuild.Checked = Settings.Default.bCleanBuild;
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
        }

        private void BuildRocketUE_Click(object sender, EventArgs e)
        {
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

                BuildRocketUE.Enabled = false;
                string CommandLineArgs = "BuildGraph -target=\"Make Installed Build Win64\" -script=Engine/Build/InstalledEngineBuild.xml -set:WithDDC=" + GetConditionalString(bWithDDC.Checked) + " -set:SignExecutables=" + GetConditionalString(bSignExecutables.Checked) + " -set:EnableSymStore=" + GetConditionalString(bEnableSymStore.Checked);                

                if (bHostPlatformOnly.Checked)
                {
                    CommandLineArgs += " -set:HostPlatformOnly=true";
                }
                else
                {
                    CommandLineArgs += String.Format(" -set:WithWin64={0} -set:WithWin32={1} -set:WithMac={2} -set:WithAndroid={3} -set:WithIOS={4} -set:WithTVOS={5} -set:WithLinux={6} -set:WithHTML5={7} -set:WithSwitch={8}", 
                        GetConditionalString(bWithWin64.Checked),
                        GetConditionalString(bWithWin32.Checked),
                        GetConditionalString(bWithMac.Checked),
                        GetConditionalString(bWithAndroid.Checked),
                        GetConditionalString(bWithIOS.Checked),
                        GetConditionalString(bWithTVOS.Checked),
                        GetConditionalString(bWithLinux.Checked),
                        GetConditionalString(bWithHTML5.Checked),
                        GetConditionalString(bWithSwitch.Checked));
                }

                if (bCleanBuild.Checked)
                {
                    CommandLineArgs += " -Clean";
                }

                AddLog(string.Format("Commandline: {0}\n", CommandLineArgs));
                ProcessStartInfo AutomationStartInfo = new ProcessStartInfo();
                AutomationStartInfo.FileName = AutomationExePath;
                AutomationStartInfo.Arguments = CommandLineArgs;
                AutomationStartInfo.UseShellExecute = false;
                AutomationStartInfo.RedirectStandardError = true;
                AutomationStartInfo.RedirectStandardOutput = true;

                AutomationToolProcess = new Process();
                AutomationToolProcess.StartInfo = AutomationStartInfo;
                AutomationToolProcess.EnableRaisingEvents = true;
                AutomationToolProcess.OutputDataReceived += new DataReceivedEventHandler(AutomationToolProcess_OutputDataReceived);
                AutomationToolProcess.ErrorDataReceived += new DataReceivedEventHandler(AutomationToolProcess_ErrorDataReceived);
                AutomationToolProcess.Exited += new EventHandler(AutomationToolProcess_Exited);
                AutomationToolProcess.Start();
                AutomationToolProcess.BeginErrorReadLine();
                AutomationToolProcess.BeginOutputReadLine();
            }
        }

        private void AutomationToolBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog NewFileDialog = new OpenFileDialog();
            NewFileDialog.Filter = "exe file (*.exe)|*.exe";

            if (NewFileDialog.ShowDialog() == DialogResult.OK)
            {
                AutomationExePath = NewFileDialog.FileName;
                AutomationToolPath.Text = AutomationExePath;
                if (Path.GetFileNameWithoutExtension(AutomationExePath) == AUTOMATION_TOOL_NAME)
                {
                    BuildRocketUE.Enabled = true;
                }
                else
                {
                    MessageBox.Show("This is not Automation Tool. Please select AutomationTool.exe", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string GetConditionalString(bool bCondition)
        {
            return bCondition ? "true" : "false";
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
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

            Settings.Default.bWithDDC = bWithDDC.Checked;
            Settings.Default.bSignExes = bSignExecutables.Checked;
            Settings.Default.bSymStore = bEnableSymStore.Checked;
            Settings.Default.bCleanBuild = bCleanBuild.Checked;
            Settings.Default.Save();
        }

        private void AboutMenu_Click(object sender, EventArgs e)
        {
            MessageBox.Show("A helper application designed to create binary version of Unreal Engine 4 from source.\n\nCreated by Satheesh (ryanjon2040)");
        }

        private void GetSourceCodeMenu_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/ryanjon2040/UE4-Binary-Builder");
        }

        private void AutomationToolProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddLog(e.Data);
        }

        private void AutomationToolProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            AddLog(e.Data);
        }

        private void AutomationToolProcess_Exited(object sender, EventArgs e)
        {
            AddLog(string.Format("AutomationToolProcess exited with code {0}\n", AutomationToolProcess.ExitCode.ToString()));
        }

        private void AddLog(string Message)
        {
            string WarningPattern = @"/warning/";
            string ErrorPattern = @"/error/";

            Regex WarningRgx = new Regex(WarningPattern, RegexOptions.IgnoreCase);
            Regex ErrorRgx = new Regex(ErrorPattern, RegexOptions.IgnoreCase);
            MatchCollection WarningMatches = WarningRgx.Matches(Message);
            MatchCollection ErrorMatches = ErrorRgx.Matches(Message);

            LogWindow.ForeColor = Color.White;

            if (WarningMatches.Count > 0)
            {
                LogWindow.ForeColor = Color.Yellow;
            }

            if (ErrorMatches.Count > 0)
            {
                LogWindow.ForeColor = Color.Red;
            }

            LogWindow.Text += Message;
        }
    }
}
