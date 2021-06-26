using System.Collections.Generic;
using Microsoft.Win32;

namespace UnrealBinaryBuilder.Classes
{
	public class Plugins
	{
		public static List<EngineBuild> GetInstalledEngines()
		{
			RegistryKey EngineInstallations = Registry.LocalMachine.OpenSubKey("Software\\EpicGames\\Unreal Engine");
			if (EngineInstallations == null)
			{
				return null;
			}

			List<EngineBuild> ReturnValue = new List<EngineBuild>();
			string[] InstalledEngines = EngineInstallations.GetSubKeyNames();
			foreach (var s in InstalledEngines)
			{
				RegistryKey InstalledDirectoryKey = EngineInstallations.OpenSubKey(s);
				object o = InstalledDirectoryKey.GetValue("InstalledDirectory");

				EngineBuild engineBuild = new EngineBuild();
				engineBuild.bIsCustomEngine = false;
				engineBuild.EngineAssociation = s;
				engineBuild.EngineName = s;
				engineBuild.EnginePath = o as string;

				ReturnValue.Add(engineBuild);
			}

			RegistryKey CustomEngineInstallations = Registry.CurrentUser.OpenSubKey("Software\\Epic Games\\Unreal Engine\\Builds");
			InstalledEngines = CustomEngineInstallations.GetValueNames();
			foreach (var s in InstalledEngines)
			{
				object o = CustomEngineInstallations.GetValue(s);
				string EngineBuildName = UnrealBinaryBuilderHelpers.GetEngineVersion(o as string);

				EngineBuild engineBuild = new EngineBuild();
				engineBuild.bIsCustomEngine = true;
				engineBuild.EngineAssociation = s;
				engineBuild.EngineName = $"{EngineBuildName} (Custom)";
				engineBuild.EnginePath = o as string;

				if (EngineBuildName != null /*&& ReturnValue.Contains(engineBuild) == false*/)
				{
					ReturnValue.Add(engineBuild);
				}
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

	public class EngineBuild
	{
		public string EngineName { get; set; }
		public string EnginePath { get; set; }
		public bool bIsCustomEngine { get; set; }
		public string EngineAssociation { get; set; }

		public override bool Equals(object obj)
		{
			if (obj is EngineBuild)
			{
				return ((EngineBuild)obj).EngineName == EngineName;
			}

			return false;
		}

		public override int GetHashCode()
		{
			return EngineName.GetHashCode();
		}
	}
}
