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
using UnrealBinaryBuilder.UserControls;

namespace UnrealBinaryBuilder.Classes
{
	public class PostBuildSettings
	{
		Task ZippingTask = null;
		static CancellationTokenSource ZipCancelTokenSource = new CancellationTokenSource();
		CancellationToken ZipCancelToken = ZipCancelTokenSource.Token;
		MainWindow mainWindow = null;

		public PostBuildSettings(MainWindow _mainWindow)
		{
			mainWindow = _mainWindow;
		}

		public bool CanSaveToZip()
		{
			return ShouldSaveToZip() && DirectoryIsWritable(mainWindow.ZipPath.Text);
		}

		public bool ShouldSaveToZip()
		{
			return (bool)mainWindow.bZipBuild.IsChecked && !string.IsNullOrEmpty(mainWindow.ZipPath.Text);
		}

		public bool DirectoryIsWritable(string DirectoryPath)
		{
			DirectoryInfo ZipDirectory = new FileInfo(DirectoryPath).Directory;
			bool bDirectoryExists = (ZipDirectory != null) && ZipDirectory.Exists;
			bool bHasWriteAccess = false;
			if (bDirectoryExists)
			{
				try
				{
					AuthorizationRuleCollection collection = new DirectoryInfo(ZipDirectory.FullName).GetAccessControl().GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
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

		public async void SavePluginToZip(PluginCard pluginCard, string ZipLocationToSave, bool bZipForMarketplace)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				GameAnalyticsCSharp.AddProgressStart("PluginZip", "Progress");
			});

			CompressionLevel CL = CompressionLevel.BestSpeed;
			ZippingTask = Task.Run(() =>
			{
				using (var zipFile = new ZipFile { CompressionLevel = CL })
				{
					IEnumerable<string> files = Directory.EnumerateFiles(pluginCard.DestinationPath, "*.*", SearchOption.AllDirectories);
					List<string> filesToAdd = new List<string>();

					foreach (string file in files)
					{
						bool bSkipFile = false;
						Application.Current.Dispatcher.Invoke(() =>
						{
							string CurrentFilePath = Path.GetFullPath(file).ToLower();
							if (bZipForMarketplace && (CurrentFilePath.Contains(@"\binaries\") || CurrentFilePath.Contains(@"\intermediate\")))
							{
								bSkipFile = true;
							}

							if (bSkipFile == false)
							{
								filesToAdd.Add(file);
							}
						});
					}
					Application.Current.Dispatcher.Invoke(() =>
					{
						pluginCard.ZipProgressbar.IsIndeterminate = false;
						pluginCard.ZipProgressbar.Value = 0;
						pluginCard.ZipProgressbar.Maximum = filesToAdd.Count;
					});

					foreach (string file in filesToAdd)
					{
						zipFile.AddFile(file, Path.GetDirectoryName(file).Replace(pluginCard.DestinationPath, string.Empty));
					}

					zipFile.SaveProgress += (o, args) =>
					{
						if (args.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								pluginCard.ZipProgressbar.Value = Convert.ToInt32(args.EntriesSaved + 1);
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_Completed)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								GameAnalyticsCSharp.AddProgressEnd("PluginZip", "Progress");
							});
						}
					};

					zipFile.UseZip64WhenSaving = Zip64Option.Always;
					zipFile.Save(ZipLocationToSave);
					Application.Current.Dispatcher.Invoke(() => { mainWindow.AddLogEntry($"Plugin zipped and saved to: {ZipLocationToSave}"); });
				}
			});

			await ZippingTask;
		}

		public async void SaveToZip(string InBuildDirectory, string ZipLocationToSave)
		{
			Application.Current.Dispatcher.Invoke(() =>
			{
				GameAnalyticsCSharp.AddProgressStart("Zip", "Progress");
				mainWindow.TotalResult.Content = "Hold on...Stats will be displayed soon.";
				mainWindow.CurrentFileSaving.Content = "Waiting...";
				mainWindow.FileSaveState.Content = "State: Preparing...";
				mainWindow.ZipStatusLabel.Visibility = Visibility.Collapsed;
				mainWindow.ZipStausStackPanel.Visibility = Visibility.Visible;
			});

			CompressionLevel CL = (bool)mainWindow.bFastCompression.IsChecked ? CompressionLevel.BestSpeed : CompressionLevel.BestCompression;

			ZippingTask = Task.Run(() =>
			{
				using (var zipFile = new ZipFile { CompressionLevel = CL })
				{
					Application.Current.Dispatcher.Invoke(() => { mainWindow.FileSaveState.Content = "State: Finding files..."; });
					IEnumerable<string> files = Directory.EnumerateFiles(InBuildDirectory, "*.*", SearchOption.AllDirectories);

					ZipCancelToken.ThrowIfCancellationRequested();

					List<string> filesToAdd = new List<string>();

					int SkippedFiles = 0;
					int AddedFiles = 0;
					int TotalFiles = files.Count();

					long TotalSize = 0;
					long TotalSizeToZip = 0;
					long SkippedSize = 0;
					string TotalSizeInString = "0B";
					string TotalSizeToZipInString = "0B";
					string SkippedSizeToZipInString = "0B";
					Application.Current.Dispatcher.Invoke(() => { mainWindow.FileSaveState.Content = "State: Preparing files for zipping..."; });
					foreach (string file in files)
					{
						bool bSkipFile = false;
						Application.Current.Dispatcher.Invoke(() =>
						{
							string CurrentFilePath = Path.GetFullPath(file).ToLower();
							if (mainWindow.bIncludePDB.IsChecked == false && Path.GetExtension(file).ToLower() == ".pdb")
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeDEBUG.IsChecked == false && Path.GetExtension(file).ToLower() == ".debug")
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeDocumentation.IsChecked == false && CurrentFilePath.Contains(@"\source\") == false && CurrentFilePath.Contains(@"\documentation\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeExtras.IsChecked == false && CurrentFilePath.Contains(@"\extras\redist\") == false && CurrentFilePath.Contains(@"\extras\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\developer\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\editor\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\programs\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\runtime\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSource.IsChecked == false && CurrentFilePath.Contains(@"\source\thirdparty\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeFeaturePacks.IsChecked == false && CurrentFilePath.Contains(@"\featurepacks\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeSamples.IsChecked == false && CurrentFilePath.Contains(@"\samples\"))
							{
								bSkipFile = true;
							}

							if (mainWindow.bIncludeTemplates.IsChecked == false && CurrentFilePath.Contains(@"\source\") == false && CurrentFilePath.Contains(@"\content\editor") == false && CurrentFilePath.Contains(@"\templates\"))
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
							//Application.Current.Dispatcher.Invoke(() => { mainWindow.AddZipLog($"File Skipped: {file}", MainWindow.ZipLogInclusionType.FileSkipped); });
						}
						else
						{
							filesToAdd.Add(file);
							AddedFiles++;
							TotalSizeToZip += new FileInfo(file).Length;
							TotalSizeToZipInString = BytesToString(TotalSizeToZip);
							//Application.Current.Dispatcher.Invoke(() => { mainWindow.AddZipLog($"File Included: {file}", MainWindow.ZipLogInclusionType.FileIncluded); });
						}

						Application.Current.Dispatcher.Invoke(() => { mainWindow.CurrentFileSaving.Content = string.Format("Total: {0}. Added: {1}. Skipped: {2}", TotalFiles, AddedFiles, SkippedFiles); });
						ZipCancelToken.ThrowIfCancellationRequested();
					}

					Application.Current.Dispatcher.Invoke(() =>
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

					Application.Current.Dispatcher.Invoke(() =>
					{
						mainWindow.OverallProgressbar.IsIndeterminate = false;
						mainWindow.FileProgressbar.IsIndeterminate = false;
					});

					zipFile.SaveProgress += (o, args) =>
					{
						ZipCancelToken.ThrowIfCancellationRequested();
						if (args.EventType == ZipProgressEventType.Saving_BeforeWriteEntry)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								mainWindow.FileSaveState.Content = "State: Begin Writing...";
								mainWindow.CurrentFileSaving.Content = string.Format("Saving File: {0} ({1}/{2})", Path.GetFileName(args.CurrentEntry.FileName), (args.EntriesSaved + 1), (args.EntriesTotal));
								mainWindow.OverallProgressbar.Value = Convert.ToInt32(args.EntriesSaved + 1);
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_EntryBytesRead)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								mainWindow.FileSaveState.Content = "State: Writing...";
								mainWindow.FileProgressbar.Value = (int)((args.BytesTransferred * 100) / args.TotalBytesToTransfer);
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_AfterWriteEntry)
						{
							ProcessedSize += new FileInfo(Path.Combine(InBuildDirectory, args.CurrentEntry.FileName)).Length;
							ProcessSizeInString = BytesToString(ProcessedSize);
							Application.Current.Dispatcher.Invoke(() => { mainWindow.TotalResult.Content = string.Format("Total Size: {0}. To Zip: {1}. Skipped: {2}. Processed: {3}", TotalSizeInString, TotalSizeToZipInString, SkippedSizeToZipInString, ProcessSizeInString); });
						}
						else if (args.EventType == ZipProgressEventType.Saving_Started)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								mainWindow.CurrentFileSaving.Content = "";
								mainWindow.FileSaveState.Content = string.Format("State: Saving zip file ({0} files) to {1}", TotalFiles, mainWindow.ZipPath.Text);
							});
						}
						else if (args.EventType == ZipProgressEventType.Saving_Completed)
						{
							Application.Current.Dispatcher.Invoke(() =>
							{
								GameAnalyticsCSharp.AddProgressEnd("Zip", "Progress");
								mainWindow.TryShutdown();
							});
						}
					};


					zipFile.UseZip64WhenSaving = Zip64Option.Always;
					zipFile.Save(ZipLocationToSave);
					Application.Current.Dispatcher.Invoke(() => { mainWindow.AddLogEntry($"Done zipping. File location: {ZipLocationToSave}"); });
				}
			}, ZipCancelToken);

			try
			{
				await ZippingTask;
			}
			catch (OperationCanceledException e)
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					mainWindow.CurrentFileSaving.Content = "";
					mainWindow.FileSaveState.Content = "Operation canceled.";
					mainWindow.AddLogEntry($"{nameof(OperationCanceledException)} with message: {e.Message}");
				});
			}
			finally
			{
				Application.Current.Dispatcher.Invoke(() =>
				{
					mainWindow.CancelZipping.Content = "Cancel Zipping";
					mainWindow.CancelZipping.IsEnabled = true;
				});
			}
		}

		public void CancelTask()
		{
			GameAnalyticsCSharp.AddProgressEnd("Zip", "Progress", true);
			mainWindow.CancelZipping.Content = "Canceling. Please wait...";
			mainWindow.CancelZipping.IsEnabled = false;
			ZipCancelTokenSource.Cancel();
		}

		public static string BytesToString(long byteCount)
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
	}
}
