using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
//using ZNix.SuperBLT;

namespace Crackdown_Installer
{
	internal class InstallerManager
	{
		private const bool DEBUG_LOCAL_JSON_HTTPREQ = false;
		//if true, skips sending an http req for the json file,
		//and reads a local json file instead.

		private const bool DEBUG_NO_FILE_DOWNLOAD = true;
		//if true, doesn't download the file at all.

		private const bool DEBUG_NO_FILE_INSTALL = true;
		//if true, downloads the files but doesn't actually install any files to their final locations (so as not to interfere with existing installations)

		private const bool DEBUG_NO_FILE_CLEANUP = false;
		//if true, skips cleanup step and does not delete zip files and temp folder after installation,
		//so that you can manually verify them if you wish.

		private const string CURRENT_INSTALLER_VERSION = "0";
		private const string DEPENDENCIES_JSON_URI = "https://raw.githubusercontent.com/Crackdown-PD2/deathvox/autoupdate/cd_dependencies.json";
		private const string SUPERBLT_DLL_WSOCK32_URI = "https://sblt-update.znix.xyz/pd2update/updates/meta.php?id=payday2bltwsockdll"; //holds meta json info about dll updates
		private const string DLL_DIFFERENCE_INFO_URL = "https://superblt.znix.xyz/#regarding-the-iphlpapidll-vs-wsock32dll-situation"; //a page for humans to read with their eyeballs, about differences between iphlpapi and wsock32 dlls

		private const string PROVIDER_GITHUB_COMMIT_URL = "https://api.github.com/repos/$id$/commits/$branch$";
		private const string PROVIDER_GITHUB_RELEASE_URL = "https://api.github.com/repos/$id$/releases/latest";
		private const string PROVIDER_GITHUB_DIRECT_URL = "https://github.com/$id$/archive/$branch$.zip";
		//private const string PROVIDER_GITLAB_RELEASE_URL = "https://gitlab.com/$id$/-/releases";
		//private const string PROVIDER_GITLAB_DIRECT_URL = "https://gitlab.com/$id$/-/archive/$id$-$branch$.zip";

		private const string JSON_MOD_DEFINITION_NAME = "mod.txt";
		private const string XML_MOD_DEFINITION_NAME = "main.xml";

		private const string KEY_USER_ROOT = "HKEY_CURRENT_USER";
		private const string KEY_VALVE_STEAM = "Software\\Valve\\Steam";

		private const string STEAM_LIBRARY_MANIFEST_PATH = "%%STEAM%%/steamapps/libraryfolders.vdf";

		private const string PD2_APPID = "218620"; //Steam appid for PAYDAY 2

		private string? steamInstallDirectory;
		private string? pd2InstallDirectory;

		private List<Pd2ModData>? installedBltMods;
		private List<Pd2ModData>? installedBeardlibMods;

		private HttpClient clientInstance;
		/// <summary>
		/// Creates a new InstallerManager instance,
		/// which can perform a variety of installation related operations,
		/// such as searching for installed mods.
		/// </summary>
		public InstallerManager(HttpClient client)
		{
			clientInstance = client;
			//search for steam install directory as stored in registry
			object? registryValue = Registry.GetValue(KEY_USER_ROOT + "\\" + KEY_VALVE_STEAM, "SteamPath", "");
			if (registryValue != null)
			{
				steamInstallDirectory = registryValue.ToString();

				if (!String.IsNullOrEmpty(steamInstallDirectory))
				{
					//find the library folder that contains PAYDAY 2's appid
					string libraryManifestPath = STEAM_LIBRARY_MANIFEST_PATH.Replace("%%STEAM%%", steamInstallDirectory);
					if (File.Exists(libraryManifestPath)) {
						//read steam library data,
						//then search for pd2 install directory in vdf

						/* pseudo
						try {
							Vdf libraryFile = ReadVDF(libraryManifestPath);
							foreach (Element libraryData in libraryFile)
							{
								Element Apps = libraryData.GetProperty("apps");
								if (Apps.HasElement(PD2_APPID)) {
									string libraryPath = libraryData.GetProperty("path");
									if (!String.IsNullOrEmpty(libraryPath)) {
										pd2InstallDirectory = libraryPath;
										break;
									}
								}
							}
						}
						catch(Exception e) {
							LogMessage("Could not read or find Steam library manifest at " +  libraryManifestPath + ":");
							LogMessage(e.Message);
						}
						*/


					}


				}
			}
			

			//generate installed mods list

			//this.GenerateInstalledBltModsList();

		}

		/// <summary>
		/// Send a query to the Crackdown updates repo
		/// to get a list of packages that Crackdown uses
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		private JsonDocument SendQueryDependencies(HttpClient client)
		{
			var jsonResponse = AsyncJsonReq(DEPENDENCIES_JSON_URI);

			//make json document allow trailing commas
			JsonDocumentOptions jsonOptions = new JsonDocumentOptions
			{
				AllowTrailingCommas = true
			};

			JsonDocument doc = JsonDocument.Parse(jsonResponse.Result, jsonOptions);

			return doc;
		}

		/// <summary>
		/// Given the path to the user's mods folder in their PAYDAY 2 installation directory,
		/// searches each folder inside for a mod.txt json file, parses it, and reads the name of the mod.
		/// Returns a list of the names of all installed BLT mods.
		/// </summary>
		/// <param name="bltModsPath"></param>
		/// <returns></returns>
		private List<Pd2ModData> GenerateInstalledBltModsList(string bltModsPath)
		{
			List<Pd2ModData> installedBltMods = new List<Pd2ModData>();

			//default json document options
			JsonDocumentOptions jsonOptions = new JsonDocumentOptions
			{
				AllowTrailingCommas = true
			};

			foreach (string subDirectory in FileSystem.GetDirectories(bltModsPath))
			{
				string definitionPath = Path.Combine(bltModsPath, subDirectory, JSON_MOD_DEFINITION_NAME);
				if (File.Exists(definitionPath))
				{
					try
					{
						JsonDocument definitionFile = JsonDocument.Parse(definitionPath, jsonOptions);
						JsonElement rootElement = definitionFile.RootElement;
						JsonElement modNameElement = rootElement.GetProperty("name");
						JsonElement modVersionElement = rootElement.GetProperty("version");
						string modName = modNameElement.GetString() ?? string.Empty;
						string modVersion = modVersionElement.GetString() ?? string.Empty;
						string modType = "blt";
						Pd2ModData modData = new Pd2ModData(modName, "", modVersion, modType);
						installedBltMods.Add(modData);
					}
					catch (JsonException e)
					{
						LogMessage(e.Message);
					}
					catch (Exception e)
					{
						LogMessage(e.Message);
					}
				}
			}
			return installedBltMods;
		}

		/// <summary>
		/// Sends an async httpreq to the given uri and returns the response.
		/// </summary>
		/// <param name="jsonUri"></param>
		/// <returns></returns>
		private async Task<string> AsyncJsonReq(string jsonUri)
		{
			var response = await clientInstance.GetStringAsync(jsonUri);
			return response;
		}

		/// <summary>
		/// Given a list of PD2 mod objects, checks for each mod in the user's PD2 installation. 
		/// Returns a list of any PD2 mod objects that were not found installed.
		/// </summary>
		/// <param name="dependenciesData"></param>
		private List<Pd2ModData> CheckMods(List<Pd2ModData> dependenciesData)
		{
			List<Pd2ModData> missingMods = new List<Pd2ModData>();
			if (dependenciesData != null)
			{
				foreach (Pd2ModData modData in dependenciesData)
				{
					string modName = modData.GetName();
					string modType = modData.GetModType();
					if (modType == "json")
					{
						if (!this.HasBltModInstalled(modName))
						{
							missingMods.Add(modData);
						}
					}
					else if (modType == "xml")
					{
						if (!this.HasBeardlibModInstalled(modName))
						{
							missingMods.Add(modData);
						}
					}
				}
			}
			return missingMods;
		}

		/// <summary>
		/// Given the name of the mod as defined in the definition file mod.txt, 
		/// searches for installed mods by this name.
		/// </summary>
		/// <param name="modName"></param>
		/// <returns></returns>
		public bool HasBltModInstalled(string modName)
		{
			if (installedBltMods == null) { return false; }

			foreach (Pd2ModData modData in installedBltMods)
			{
				if (modData.GetName() == modName)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Given the name of the mod as defined in the definition file main.xml, 
		/// searches for installed mods by this name.
		/// </summary>
		/// <param name="modName"></param>
		/// <returns></returns>
		public bool HasBeardlibModInstalled(string modName)
		{
			if (installedBeardlibMods == null) { return false; }

			foreach (Pd2ModData modData in installedBeardlibMods)
			{
				if (modData.GetName() == modName)
				{
					return true;
				}
			}
			return false;
		}

		private class Pd2ModData
		{
			private string Name { get; set; }
			private string Description { get; set; }
			private string Version { get; set; }
			private string ModType { get; set; }

			public Pd2ModData(string? name, string? description, string? version, string? modType)
			{
				Name = name ?? string.Empty;
				Description = description ?? string.Empty;
				Version = version ?? string.Empty;
				ModType = modType ?? string.Empty;
			}

			/// <summary>
			/// Get the name of this mod, as written in its definition file.
			/// </summary>
			/// <returns></returns>
			public string GetName()
			{
			   return Name;
			}

			public string GetVersion()
			{
			   return Version;
			}

			public string GetDescription()
			{
				return Description;
			}

			public string GetModType()
			{
			   return ModType;
			}

		}

		static void LogMessage(object? message)
		{
			string? s;
			if (message != null)
			{
				s = message.ToString();
				if (s != null)
				{
					/*
					string s = String.Empty;
					foreach (Object o in message) {
						s = s + o.ToString();
					}
					*/
				}
				else
				{
					s = "null";
				}
			}
			else
			{
				s = "null";
			}
			string timeStamp = DateTime.Now.ToString();
			s = "[" + timeStamp + "] " + s;
			System.Diagnostics.Debug.WriteLine(s);
		}

	}
}
