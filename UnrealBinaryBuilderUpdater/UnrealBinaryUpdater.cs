using NetSparkleUpdater;
using NetSparkleUpdater.SignatureVerifiers;
using System;
using NetSparkleUpdater.Events;
using System.Linq;
using System.IO.Compression;

namespace UnrealBinaryBuilderUpdater
{
	public enum AppUpdateCheckStatus
	{
		UpdateAvailable,
		NoUpdate,
		UserSkip,
		CouldNotDetermine
	}

	public class UBBUpdater
	{
		private static readonly string APP_CAST_XML = "https://github.com/ryanjon2040/UE4-Binary-Builder/raw/master/UnrealBinaryBuilderUpdater/appcast.xml";
		private static UpdateInfo _updateInfo;
		private static SparkleUpdater _sparkle = null;
		private string _downloadPath = null;

		public event EventHandler<UpdateProgressFinishedEventArgs> SilentUpdateFinishedEventHandler;
		public event EventHandler<UpdateProgressDownloadEventArgs> UpdateProgressEventHandler;
		public event EventHandler<UpdateProgressDownloadErrorEventArgs> UpdateProgressDownloadErrorEventHandler;
		public event EventHandler<UpdateProgressDownloadStartEventArgs> UpdateDownloadStartedEventHandler;
		public event EventHandler<UpdateProgressDownloadFinishEventArgs> UpdateDownloadFinishedEventHandler;
		public event EventHandler CloseApplicationEventHandler;

		public UBBUpdater()
		{			
			Internal_SetupSparkle();
		}

		private static void Internal_SetupSparkle()
		{
			if (_sparkle == null)
			{
				_sparkle = new SparkleUpdater(APP_CAST_XML, new DSAChecker(NetSparkleUpdater.Enums.SecurityMode.UseIfPossible, "+mLdLTe3Mj6OU0Kr6+ZDeVj+TTFRsNUJvUaPhuJ7pUI="))
				{
					ShowsUIOnMainThread = false,
					UseNotificationToast = true
				};
				_sparkle.LogWriter = new LogWriter(true);
			}

			if (_sparkle.UIFactory == null)
			{
				string manifestModuleName = System.Reflection.Assembly.GetEntryAssembly().ManifestModule.FullyQualifiedName;
				var icon = System.Drawing.Icon.ExtractAssociatedIcon(manifestModuleName);
				_sparkle.UIFactory = new NetSparkleUpdater.UI.WPF.UIFactory(NetSparkleUpdater.UI.WPF.IconUtilities.ToImageSource(icon));
			}

			_sparkle.SecurityProtocolType = System.Net.SecurityProtocolType.Tls12;
		}

		public void CheckForUpdates()
		{
			Internal_SetupSparkle();
			_sparkle.CheckForUpdatesAtUserRequest();
		}

		public async void CheckForUpdatesSilently()
		{
			_sparkle.UIFactory = null;
			_updateInfo = await _sparkle.CheckForUpdatesQuietly();
			if (_updateInfo != null)
			{
				UpdateProgressFinishedEventArgs eventArgs = new UpdateProgressFinishedEventArgs();
				eventArgs.castItem = null;
				switch (_updateInfo.Status)
				{
					case NetSparkleUpdater.Enums.UpdateStatus.UpdateAvailable:
						eventArgs.appUpdateCheckStatus = AppUpdateCheckStatus.UpdateAvailable;
						eventArgs.castItem = _updateInfo.Updates.First();
						break;
					case NetSparkleUpdater.Enums.UpdateStatus.UpdateNotAvailable:
						eventArgs.appUpdateCheckStatus = AppUpdateCheckStatus.NoUpdate;
						break;
					case NetSparkleUpdater.Enums.UpdateStatus.UserSkipped:
						eventArgs.appUpdateCheckStatus = AppUpdateCheckStatus.UserSkip;
						break;
					case NetSparkleUpdater.Enums.UpdateStatus.CouldNotDetermine:
						eventArgs.appUpdateCheckStatus = AppUpdateCheckStatus.CouldNotDetermine;
						break;
				}

				OnUpdateCheckFinished(eventArgs);
			}
		}

		public async void DownloadUpdate()
		{
			_sparkle.DownloadStarted -= OnDownloadStart;
			_sparkle.DownloadStarted += OnDownloadStart;

			_sparkle.DownloadFinished -= OnDownloadFinish;
			_sparkle.DownloadFinished += OnDownloadFinish;

			_sparkle.DownloadHadError -= OnDownloadError;
			_sparkle.DownloadHadError += OnDownloadError;

			_sparkle.DownloadMadeProgress += OnDownloadMadeProgress;

			await _sparkle.InitAndBeginDownload(_updateInfo.Updates.First());
		}

		public void InstallUpdate()
		{
			CloseApplication();
		}

		private void CloseApplication()
		{
			EventArgs eventArgs = new EventArgs();
			EventHandler eventHandler = CloseApplicationEventHandler;
			eventHandler(this, eventArgs);
		}

		private void OnDownloadStart(AppCastItem item, string path)
		{
			UpdateProgressDownloadStartEventArgs eventArgs = new UpdateProgressDownloadStartEventArgs();
			eventArgs.UpdateSize = item.UpdateSize;
			eventArgs.Version = item.Version;
			EventHandler<UpdateProgressDownloadStartEventArgs> eventHandler = UpdateDownloadStartedEventHandler;
			eventHandler(this, eventArgs);
		}

		private void OnDownloadFinish(AppCastItem item, string path)
		{
			_downloadPath = path;
			UpdateProgressDownloadFinishEventArgs eventArgs = new UpdateProgressDownloadFinishEventArgs();
			if (System.IO.File.Exists(_downloadPath + ".zip"))
			{
				System.IO.File.Delete(_downloadPath + ".zip");
			}

			System.IO.File.Move(_downloadPath, _downloadPath + ".zip");
			string NewFile = _downloadPath + ".zip";
			eventArgs.UpdateFilePath = NewFile;
			eventArgs.castItem = item;
			EventHandler<UpdateProgressDownloadFinishEventArgs> eventHandler = UpdateDownloadFinishedEventHandler;
			eventHandler(this, eventArgs);
		}

		private void OnDownloadMadeProgress(object sender, AppCastItem appCastItem, ItemDownloadProgressEventArgs e)
		{
			UpdateProgressDownloadEventArgs eventArgs = new UpdateProgressDownloadEventArgs();
			eventArgs.AppUpdateProgress = e.ProgressPercentage;
			EventHandler<UpdateProgressDownloadEventArgs> eventHandler = UpdateProgressEventHandler;
			eventHandler(this, eventArgs);
		}

		private void OnDownloadError(AppCastItem item, string path, Exception ex)
		{
			UpdateProgressDownloadErrorEventArgs eventArgs = new UpdateProgressDownloadErrorEventArgs();
			eventArgs.ErrorException = ex;
			EventHandler<UpdateProgressDownloadErrorEventArgs> eventHandler = UpdateProgressDownloadErrorEventHandler;
			eventHandler(this, eventArgs);
		}

		private void OnUpdateCheckFinished(UpdateProgressFinishedEventArgs e)
		{
			EventHandler<UpdateProgressFinishedEventArgs> handler = SilentUpdateFinishedEventHandler;
			handler(this, e);
		}
	}

	public class UpdateProgressFinishedEventArgs : EventArgs
	{
		public AppUpdateCheckStatus appUpdateCheckStatus { get; set; }
		public AppCastItem castItem { get; set; }
	}

	public class UpdateProgressDownloadEventArgs : EventArgs
	{
		public int AppUpdateProgress { get; set; }
	}

	public class UpdateProgressDownloadErrorEventArgs : EventArgs
	{
		public Exception ErrorException { get; set; }
	}

	public class UpdateProgressDownloadStartEventArgs : EventArgs
	{
		public long UpdateSize { get; set; }
		public string Version { get; set; }
	}

	public class UpdateProgressDownloadFinishEventArgs : EventArgs
	{
		public AppCastItem castItem { get; set; }
		public string UpdateFilePath { get; set; }
	}
}
