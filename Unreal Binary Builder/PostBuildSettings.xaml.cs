using Ionic.Zip;
using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Unreal_Binary_Builder.Properties;

namespace Unreal_Binary_Builder
{
	/// <summary>
	/// Interaction logic for PostBuildSettings.xaml
	/// </summary>
	public partial class PostBuildSettings : Window
	{
		Task ZippingTask = null;
		static CancellationTokenSource ZipCancelTokenSource = new CancellationTokenSource();
		CancellationToken ZipCancelToken = ZipCancelTokenSource.Token;
		
		public PostBuildSettings()
		{
			InitializeComponent();
			
			Settings DefaultSettings = Settings.Default;
			bZipBuild.IsChecked = DefaultSettings.bZipBuild;
			ZipPath.Text = DefaultSettings.ZipPath;
			bIncludePDB.IsChecked = DefaultSettings.bIncludePDB;
			bIncludeDEBUG.IsChecked = DefaultSettings.bIncludeDEBUG;
			bIncludeDocumentation.IsChecked = DefaultSettings.bIncludeDocumentation;
			bIncludeExtras.IsChecked = DefaultSettings.bIncludeExtras;
			bIncludeSource.IsChecked = DefaultSettings.bIncludeSource;
			bIncludeFeaturePacks.IsChecked = DefaultSettings.bIncludeFeaturePacks;
			bIncludeSamples.IsChecked = DefaultSettings.bIncludeSamples;
			bIncludeTemplates.IsChecked = DefaultSettings.bIncludeTemplates;
			bFastCompression.IsChecked = DefaultSettings.bFastCompression;
		}

		public bool CanSaveToZip()
		{
			return ShouldSaveToZip() && DirectoryIsWritable();
		}

		public bool ShouldSaveToZip()
		{
			return (bool)bZipBuild.IsChecked && !string.IsNullOrEmpty(ZipPath.Text);
		}

		public bool DirectoryIsWritable()
		{
			DirectoryInfo ZipDirectory = new FileInfo(ZipPath.Text).Directory;
			bool bDirectoryExists = (ZipDirectory != null) && ZipDirectory.Exists;
			bool bHasWriteAccess = false;
			if(bDirectoryExists)
			{
				try
				{
					AuthorizationRuleCollection collection = Directory.GetAccessControl(ZipDirectory.FullName).GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
					foreach (FileSystemAccessRule rule in collection)
					{
						if (rule.AccessControlType == AccessControlType.Allow)
						{
							bHasWriteAccess = true;
							break;
						}
					}
				}
				catch (Exception)
				{

				}
			}

			return bDirectoryExists && bHasWriteAccess;
		}

		public void PrepareToSave()
		{
			ZipCancelTokenSource.Dispose();
			ZipCancelTokenSource = new CancellationTokenSource();
			ZipCancelToken = ZipCancelTokenSource.Token;
		}

		public async void SaveToZip(MainWindow mainWindow, string InBuildDirectory, string ZipLocationToSave)
		{		
			Dispatcher.Invoke(() => 
			{
				GameAnalyticsCSharp.AddProgressStart("Zip", "Progress");
				mainWindow.ZipProgressDialog.IsOpen = true;
				mainWindow.TotalResult.Content = "";
				mainWindow.CurrentFileSaving.Content = "Waiting...";
				mainWindow.FileSaveState.Content = "State: Preparing..."; 
			});

			CompressionLevel CL = (bool)bFastCompression.IsChecked ? CompressionLevel.BestSpeed : CompressionLevel.BestCompression;

			ZippingTask = Task.Run(() => 
			{
				using (var zipFile = new ZipFile { CompressionLevel = CL })
				{
					Dispatcher.Invoke(() => { mainWindow.FileSaveState.Content = "State: Finding files..."; });
					string[] files = Directory.GetFiles(InBuildDirectory, "*", SearchOption.AllDirectories).ToArray();
					ZipCancelToken.ThrowIfCancellationRequested();

					List<string> filesToAdd = new List<string>();

					int SkippedFiles = 0;
					int AddedFiles = 0;
					int TotalFiles = files.Length;

					long TotalSize = 0;
					long TotalSizeToZip = 0;
					long SkippedSize = 0;
					string TotalSizeInString = "0B";
					string TotalSizeToZipInString = "0B";
					string SkippedSizeToZipInString = "0B";
					Dispatcher.Invoke(() => { mainWindow.FileSaveState.Content = "State: Preparing files for zipping..."; });
					foreach (string file in files)
					{						
						bool bSkipFile = false;
						Dispatcher.Invoke(() =>
						{
							string CurrentFilePath = Path.GetFullPath(file).ToLower();
							if (bIncludePDB.IsChecked == false && Path.GetExtension(file).ToLower() == ".pdb")
							{
								bSkipFile = true;							
							}

							if (bIncludeDEBUG.IsChecked == false && Path.GetExtension(file).ToLower() == ".debug")
							{
								bSkipFile = true;
							}

							if (bIncludeDocumentation.IsChecked == false && CurrentFilePath.Contains(@"\source\") == false && CurrentFilePath.Contains(@"\documentation\"))
							{
								bSkipFile = true;
							}

							if (bIncludeExtras.IsChecked == false && CurrentFilePath.Contains(@"\extras\redist\") == false && CurrentFilePath.Contains(@"\extras\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\developer\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\editor\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\programs\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\runtime\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\thirdparty\"))
							{
								bSkipFile = true;
							}

							if (bIncludeFeaturePacks.IsChecked == false && CurrentFilePath.Contains(@"\featurepacks\"))
							{
								bSkipFile = true;
							}

							if (bIncludeSamples.IsChecked == false && CurrentFilePath.Contains(@"\samples\"))
							{
								bSkipFile = true;
							}

							if (bIncludeTemplates.IsChecked == false && CurrentFilePath.Contains(@"\source\") == false && CurrentFilePath.Contains(@"\content\editor") == false && CurrentFilePath.Contains(@"\templates\"))
							{
								bSkipFile = true;
							}
							
						});

						TotalSize += new FileInfo(file).Length;
						TotalSizeInString = BytesToString(TotalSize);
						if (bSkipFile)
						{
							SkippedFiles++;
							SkippedSize += new FileInfo(file).Length;
							SkippedSizeToZipInString = BytesToString(SkippedSize);
						}
						else
						{
							filesToAdd.Add(file);
							AddedFiles++;
							TotalSizeToZip += new FileInfo(file).Length;
							TotalSizeToZipInString = BytesToString(TotalSizeToZip);							
						}					

						Dispatcher.Invoke(() => { mainWindow.CurrentFileSaving.Content = string.Format("Total: {0}. Added: {1}. Skipped: {2}", TotalFiles, AddedFiles, SkippedFiles); });
						ZipCancelToken.ThrowIfCancellationRequested();
					}

					Dispatcher.Invoke(() => 
					{
						mainWindow.TotalResult.Content = string.Format("Total Size: {0}. To Zip: {1}. Skipped: {2}", TotalSizeInString, TotalSizeToZipInString, SkippedSizeToZipInString);
						mainWindow.FileSaveState.Content = "State: Verifying...";
						mainWindow.OverallProgressbar.Maximum = filesToAdd.Count;
					});
					
					foreach (string file in filesToAdd)
					{
						ZipCancelToken.ThrowIfCancellationRequested();
						zipFile.AddFile(file, Path.GetDirectoryName(file).Replace(InBuildDirectory, string.Empty));
					}

					long ProcessedSize = 0;
					string ProcessSizeInString = "0B";
					
					Dispatcher.Invoke(() => 
					{
						mainWindow.OverallProgressbar.IsIndeterminate = false;
						mainWindow.FileProgressbar.IsIndeterminate = false; 
					});

					zipFile.SaveProgress += (o, args) =>
					{
						ZipCancelToken.ThrowIfCancellationRequested();
						if (args.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
						{							
							Dispatcher.Invoke(() => 
							{
								mainWindow.FileSaveState.Content = "State: Begin Writing...";
								mainWindow.CurrentFileSaving.Content = string.Format("Saving File: {0} ({1}/{2})", Path.GetFileName(args.CurrentEntry.FileName), (args.EntriesSaved + 1), (args.EntriesTotal));
								mainWindow.OverallProgressbar.Value = args.EntriesSaved + 1; 
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_EntryBytesRead)
						{							
							Dispatcher.Invoke(() => 
							{
								mainWindow.FileSaveState.Content = "State: Writing...";
								mainWindow.FileProgressbar.Value = (int)((args.BytesTransferred * 100) / args.TotalBytesToTransfer);
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
						{
							ProcessedSize += new FileInfo(Path.Combine(InBuildDirectory, args.CurrentEntry.FileName)).Length;
							ProcessSizeInString = BytesToString(ProcessedSize);
							Dispatcher.Invoke(() => { mainWindow.TotalResult.Content = string.Format("Total Size: {0}. To Zip: {1}. Skipped: {2}. Processed: {3}", TotalSizeInString, TotalSizeToZipInString, SkippedSizeToZipInString, ProcessSizeInString); });
						}
						else if (args.EventType == ZipProgressEventType.Saving_Started)
						{
							Dispatcher.Invoke(() => 
							{
								mainWindow.CurrentFileSaving.Content = "";
								mainWindow.FileSaveState.Content = string.Format("State: Saving zip file to LOCATION_HERE", TotalFiles); 
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_Completed)
						{
							Dispatcher.Invoke(() => 
							{
								GameAnalyticsCSharp.AddProgressEnd("Zip", "Progress");
								mainWindow.ZipProgressDialog.IsOpen = false;
								mainWindow.TryShutdown();
							});
						}
					};

					
					zipFile.UseZip64WhenSaving = Zip64Option.Always;
					zipFile.Save(ZipLocationToSave);
					Dispatcher.Invoke(() => { mainWindow.AddLogEntry($"Done zipping. File location: {ZipLocationToSave}"); });
				}
			}, ZipCancelToken);

			try
			{
				await ZippingTask;
			}
			catch (OperationCanceledException e)
			{
				Dispatcher.Invoke(() =>
				{
					mainWindow.CurrentFileSaving.Content = "";
					mainWindow.FileSaveState.Content = "Operation canceled.";
					mainWindow.AddLogEntry($"{nameof(OperationCanceledException)} with message: {e.Message}"); 
					mainWindow.ZipProgressDialog.IsOpen = false;
				});
			}
			finally
			{
				Dispatcher.Invoke(() => 
				{ 
					mainWindow.CancelZipping.Content = "Cancel Zipping";
					mainWindow.CancelZipping.IsEnabled = true;
				});
			}
		}

		public void CancelTask(MainWindow mainWindow)
		{
			GameAnalyticsCSharp.AddProgressEnd("Zip", "Progress", true);
			mainWindow.CancelZipping.Content = "Canceling. Please wait...";
			mainWindow.CancelZipping.IsEnabled = false;
			ZipCancelTokenSource.Cancel();
		}

		static string BytesToString(long byteCount)
		{
			string[] suf = { "B", "KB", "MB", "GB", "TB" };
			if (byteCount == 0)
			{
				return "0" + suf[0];
			}
			long bytes = Math.Abs(byteCount);
			int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
			double num = Math.Round(bytes / Math.Pow(1024, place), 1);
			return (Math.Sign(byteCount) * num).ToString() + suf[place];
		}

		private void SetZipPathLocation_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog SFD = new SaveFileDialog();
			SFD.DefaultExt = ".zip";
			SFD.Filter = "Zip File (.zip)|*.zip";
			DialogResult SaveDialogResult = SFD.ShowDialog();
			if (SaveDialogResult == System.Windows.Forms.DialogResult.OK)
			{
				ZipPath.Text = SFD.FileName;
			}
		}

		private void PostBuildSettingsWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Settings DefaultSettings = Settings.Default;
			DefaultSettings.bZipBuild = (bool)bZipBuild.IsChecked;
			DefaultSettings.ZipPath = ZipPath.Text;
			DefaultSettings.bIncludePDB = (bool)bIncludePDB.IsChecked;
			DefaultSettings.bIncludeDEBUG = (bool)bIncludeDEBUG.IsChecked;
			DefaultSettings.bIncludeDocumentation = (bool)bIncludeDocumentation.IsChecked;
			DefaultSettings.bIncludeExtras = (bool)bIncludeExtras.IsChecked;
			DefaultSettings.bIncludeSource = (bool)bIncludeSource.IsChecked;
			DefaultSettings.bIncludeFeaturePacks = (bool)bIncludeFeaturePacks.IsChecked;
			DefaultSettings.bIncludeSamples = (bool)bIncludeSamples.IsChecked;
			DefaultSettings.bIncludeTemplates = (bool)bIncludeTemplates.IsChecked;
			DefaultSettings.bFastCompression = (bool)bFastCompression.IsChecked;

			DefaultSettings.Save();

			e.Cancel = true;
			Visibility = Visibility.Collapsed;
		}
	}
}
