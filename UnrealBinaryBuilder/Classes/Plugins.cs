
using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace UnrealBinaryBuilder.Classes
{
	public class Plugins
	{
		public static IDictionary<string, string> GetInstalledEngines()
		{
			IDictionary<string, string> ReturnValue = new Dictionary<string, string>();

			RegistryKey EngineInstallations = Registry.LocalMachine.OpenSubKey("Software\\EpicGames\\Unreal Engine");
			string[] InstalledEngines = EngineInstallations.GetSubKeyNames();
			foreach (var s in InstalledEngines)
			{
				RegistryKey InstalledDirectoryKey = EngineInstallations.OpenSubKey(s);
				Object o = InstalledDirectoryKey.GetValue("InstalledDirectory");
				ReturnValue.Add(s, o as string);
			}
			
			return ReturnValue;
		}
	}

	class UE4PluginJson
	{
		public string FriendlyName { get; set; }
		public string Description { get; set; }
		public string MarketplaceURL { get; set; }
		public bool IsBetaVersion { get; set; }
		public bool IsExperimentalVersion { get; set; }
		public IList<UE4PluginModule> Modules { get; set; }
	}

	class UE4PluginModule
	{
		public IList<string> WhitelistPlatforms { get; set; }
	}
}
