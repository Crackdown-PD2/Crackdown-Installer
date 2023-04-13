using System.IO.Compression;
using System.Text.Json;
using System.Xml;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
//using ZNix.SuperBLT;
using VDF;

namespace Crackdown_Installer
{
	internal class InstallerManager
	{
		private const bool DOWNLOAD_CLOBBER_ENABLED = true;
		//if true, allows file downloads to replace existing files by the same name.

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
		private const string DEPENDENCIES_JSON_URL = "https://raw.githubusercontent.com/Crackdown-PD2/deathvox/autoupdate/cd_dependencies.json";
		private const string SUPERBLT_DLL_WSOCK32_URL = "https://sblt-update.znix.xyz/pd2update/updates/meta.php?id=payday2bltwsockdll"; //holds meta json info about dll updates
		private const string DLL_DIFFERENCE_INFO_URL = "https://superblt.znix.xyz/#regarding-the-iphlpapidll-vs-wsock32dll-situation"; //a page for humans to read with their eyeballs, about differences between iphlpapi and wsock32 dlls

		private const string PROVIDER_GITHUB_COMMIT_URL = "https://api.github.com/repos/$id$/commits/$branch$";
		private const string PROVIDER_GITHUB_RELEASE_URL = "https://api.github.com/repos/$id$/releases/latest";
		private const string PROVIDER_GITHUB_DIRECT_URL = "https://github.com/$id$/archive/$branch$.zip";

		//private const string PROVIDER_GITLAB_COMMIT_URL = "https://gitlab.com/api/v4/projects/$id$/repository/branches/$branch$";
		private const string PROVIDER_GITLAB_RELEASE_URL = "https://gitlab.com/api/v4/projects/$id$/releases";
		private const string PROVIDER_GITLAB_DIRECT_URL = "https://gitlab.com/api/v4/projects/$id$/repository/archive.zip";

		private const string JSON_MOD_DEFINITION_NAME = "mod.txt";
		private const string XML_MOD_DEFINITION_NAME = "main.xml";

		private const string KEY_USER_ROOT = "HKEY_CURRENT_USER";
		private const string KEY_VALVE_STEAM = "Software\\Valve\\Steam";

		private const string STEAM_LIBRARY_MANIFEST_PATH = "%%STEAM%%/steamapps/libraryfolders.vdf";

		private const string PD2_APPID = "218620"; //Steam appid for PAYDAY 2

		private string? steamInstallDirectory;
		private string? pd2InstallDirectory;
		private DirectoryInfo? tempDownloadsDirectory = null;

		private List<Pd2ModData>? installedBltMods;
		private List<Pd2ModData>? installedBeardlibMods;

		private List<ModDependencyEntry> dependenciesFromServer;

		private HttpClient httpClientInstance;
		/// <summary>
		/// Creates a new InstallerManager instance,
		/// which can perform a variety of installation related operations,
		/// such as searching for installed mods.
		/// </summary>
		public InstallerManager(HttpClient client)
		{
			httpClientInstance = client;

			// Registry.GetValue("\\HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\URLAssociations\\https\\UserChoice", "SteamPath", "");

			//tempDownloadsDirectory = Directory.CreateTempSubdirectory("crackdowninstaller_");
			//Directory.Delete(tempDownloadsDirectory.FullName + "/", true);

			pd2InstallDirectory = FindPd2InstallDirectory();
			if (!string.IsNullOrEmpty(pd2InstallDirectory))
			{

				//generate installed mods list
				string modsDirectory = Path.Combine(pd2InstallDirectory, "mods"); //mods folder
				string modOverridesDirectory = Path.Combine(pd2InstallDirectory, "assets\\mod_overrides\\"); //mod_overrides folder

				string[] modFoldersList =
				{
					modsDirectory,
					modOverridesDirectory
				};
				installedBltMods = CollectBltMods(modsDirectory);
				installedBeardlibMods = CollectBeardlibMods(modFoldersList);

			}
			else
			{
				LogMessage("Unable to automatically find PD2 install directory.");
			}

			CollectDependencies();
		}

		public async void CollectDependencies()
		{
			List<ModDependencyEntry> result = new();
			ModDependencyList item;
			if (DEBUG_LOCAL_JSON_HTTPREQ)
			{
#pragma warning disable CS0162 // Unreachable code detected
				StreamReader sr = new("test.json");
#pragma warning restore CS0162 // Unreachable code detected
				string jsonResponse = sr.ReadToEnd();
				item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
			}
			else
			{
				/// Send a query to the Crackdown updates repo
				/// to get a list of packages that Crackdown uses
				string jsonResponse = await AsyncJsonReq(DEPENDENCIES_JSON_URL);
				
				//ModDependencyList item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
				item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
			}

			foreach (ModDependencyEntry entry in item.Response) {
				result.Add(entry);
			}
			dependenciesFromServer = result;
		}

		public List<ModDependencyEntry> GetDependencyEntries() {
			return dependenciesFromServer;
		}

		public List<ModDependencyEntry> CollectMissingMods() {
			List<ModDependencyEntry> missingMods = new();
			List<ModDependencyEntry> dependencyList = GetDependencyEntries();

			foreach (ModDependencyEntry entry in dependencyList)
			{
				string? modName = entry.Name;
				string? modType = entry.DirectoryType;
				if (!string.IsNullOrEmpty(modName) && !string.IsNullOrEmpty(modType))
				{
					if (modType == "blt")
					{
						if (!HasBltModInstalled(modName))
						{
							missingMods.Add(entry);
						}
					}
					else if (modType == "beardlib")
					{
						if (!HasBeardlibModInstalled(modName))
						{
							missingMods.Add(entry);
						}
					}
				}
			}

			return missingMods;
		}

		/// <summary>
		/// Returns the cached installation path for PAYDAY 2.
		/// </summary>
		/// <returns></returns>
		public string GetPd2InstallDirectory() {
			return pd2InstallDirectory ?? string.Empty;
		}

		/// <summary>
		/// Attempts to find the path to the PAYDAY 2 installation,
		/// using the Steam install location in the registry
		/// and the Steam library folder manifest vdf file in the Steam install folder.
		/// Returns true if successful.
		/// </summary>
		/// <returns></returns>
		private string? FindPd2InstallDirectory() {

			//search for steam install directory as stored in registry
			object? registryValue = Registry.GetValue(KEY_USER_ROOT + "\\" + KEY_VALVE_STEAM, "SteamPath", "");
			if (registryValue != null)
			{
				steamInstallDirectory = registryValue.ToString();

				if (!String.IsNullOrEmpty(steamInstallDirectory))
				{
					//find the library folder that contains PAYDAY 2's appid
					string libraryManifestPath = STEAM_LIBRARY_MANIFEST_PATH.Replace("%%STEAM%%", steamInstallDirectory);
					if (File.Exists(libraryManifestPath))
					{
						//read steam library data,
						//then search for pd2 install directory in vdf

						try
						{

							VDFFile libraryFile = new VDFFile(libraryManifestPath);
							var root = libraryFile.Elements;
							var libraryFoldersElement = root["libraryfolders"];

							var children = libraryFoldersElement.Children;
							//for each library folder entry listed:
							foreach (KeyValuePair<string, NestedElement> a in children)
							{
								NestedElement b = a.Value;
								Dictionary<string, NestedElement> items = b.Children;

								NestedElement libraryPathElement = items["path"];
								string libraryPath = libraryPathElement.Value;
								if (!string.IsNullOrEmpty(libraryPath))
								{
									NestedElement libraryAppsElement = items["apps"];
									//foreach item in items["apps"];
									foreach (KeyValuePair<string, NestedElement> c in libraryAppsElement.Children)
									{
										if (c.Key == PD2_APPID)
										{
											//do not use Path.Combine here
											//also replace double-escaped backslashes
											return (libraryPath + "\\steamapps\\common\\PAYDAY 2\\").Replace("\\\\", "\\");
										}
									}
								}
							}
						}
						catch (Exception e)
						{
							LogMessage(e.GetType() + ": Could not read or find Steam library manifest at " + libraryManifestPath + ":");
							LogMessage(e.Message);
						}
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Shortcut for JsonElement.TryGetProperty()
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="element"></param>
		/// <returns></returns>
		private string GetJsonAttribute(string propertyName,JsonElement element) {
			JsonElement tempElementHolder = new JsonElement();
			return GetJsonAttribute(propertyName, element, tempElementHolder);
		}

		/// <summary>
		/// Shortcut for JsonElement.TryGetProperty() which can accept an existing JsonElement instead of creating one for each call
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="element"></param>
		/// <param name="tempElementHolder"></param>
		/// <returns></returns>
		private string GetJsonAttribute(string propertyName,JsonElement element, JsonElement tempElementHolder) {
			if (element.TryGetProperty(propertyName, out tempElementHolder)) {
				return tempElementHolder.GetString() ?? string.Empty;
			};
			return string.Empty;
		}

		/// <summary>
		/// Given the path to the user's mods folder in their PAYDAY 2 installation directory,
		/// searches each folder inside for a mod.txt json file, parses it, and reads the name of the mod.
		/// Returns a list of the names of all installed BLT mods.
		/// </summary>
		/// <param name="bltModsPath"></param>
		/// <returns></returns>
		private List<Pd2ModData> CollectBltMods(string bltModsPath)
		{
			List<Pd2ModData> installedMods = new List<Pd2ModData>();

			//default json document options
			JsonDocumentOptions jsonOptions = new JsonDocumentOptions
			{
				AllowTrailingCommas = true
			};

			//create a throwaway temp element to hold the result of TryGetProperty()
			JsonElement tempElement = new();

			foreach (string subDirectory in FileSystem.GetDirectories(bltModsPath))
			{
				string definitionPath = Path.Combine(bltModsPath, subDirectory, JSON_MOD_DEFINITION_NAME);
				if (File.Exists(definitionPath))
				{
					try
					{
						//don't serialize since we have to be much more careful when parsing unknown files;
						//blt's json parser is very lax, especially when it comes to missing commas
						var sr = new StreamReader(definitionPath);
						var jsonStr = sr.ReadToEnd();
						JsonDocument definitionFile = JsonDocument.Parse(jsonStr, jsonOptions);
						JsonElement rootElement = definitionFile.RootElement;

						string modName = GetJsonAttribute("name",rootElement, tempElement);
						string modDesc = GetJsonAttribute("description",rootElement, tempElement);
						string modVersion = GetJsonAttribute("version",rootElement, tempElement);
						string modDescription = GetJsonAttribute("description",rootElement, tempElement);
						string modType = "json";
						installedMods.Add(new Pd2ModData(modName, modDescription, modVersion, modType));
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
			return installedMods;
		}

		/// <summary>
		/// Gets the attribute value (inner xml) of the given property name from the given collection.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="elementName"></param>
		/// <returns></returns>
		static string GetXmlAttribute(XmlAttributeCollection collection, string elementName)
		{
			string result = "";
			XmlAttribute? elementAttribute = collection[elementName];
			if (elementAttribute != null)
			{
				result = elementAttribute.InnerXml;
			}
			return result;
		}

		/// <summary>
		/// Gets the attribute value (inner xml) of the given property name from the given collection.
		/// Returns the fallback value, if provided.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="elementName"></param>
		/// <param name="fallback"></param>
		/// <returns></returns>
		static string GetXmlAttribute(XmlAttributeCollection collection, string elementName, string? fallback)
		{
			return GetXmlAttribute(collection, elementName) ?? fallback ?? string.Empty;
		}

		/// <summary>
		/// Finds all valid BeardLib mods, as detected by the presence of a valid XML file called main.xml.
		/// </summary>
		/// <param name="modsFolderPaths"></param>
		/// <returns></returns>
		private List<Pd2ModData> CollectBeardlibMods(string[] modsFolderPaths)
		{
			List<Pd2ModData> installedMods = new();
			foreach (string modsFolderPath in modsFolderPaths)
			{
				foreach (string modFolder in FileSystem.GetDirectories(modsFolderPath))
				{
					string definitionPath = Path.Combine(modFolder, XML_MOD_DEFINITION_NAME);

					if (FileSystem.FileExists(definitionPath))
					{
						try
						{
							XmlDocument xmlDoc = new();
							xmlDoc.Load(definitionPath);
							//Display the contents of the child nodes.
							XmlElement? element = xmlDoc.DocumentElement;
							if (element != null)
							{
								XmlAttributeCollection? attrcoll = element.Attributes;
								if (attrcoll != null)
								{
									string modName = GetXmlAttribute(attrcoll, "name");
									string modVersion = GetXmlAttribute(attrcoll, "version");
									string modDescription = "";
									//description is not specified in BeardLib ModCore documentation
									string modType = "xml";
									installedMods.Add(new Pd2ModData(modName, modVersion, modDescription, modType));
								}
							}
						}
						catch (IOException e)
						{
							Console.WriteLine("The file could not be read:");
							Console.WriteLine(e.Message);
						}
					}
				}
			}
			return installedMods;
		}

		/// <summary>
		/// 
		/// Downloads the given file to the temp directory, 
		/// unzips it there,
		/// and moves the unzipped contents to the specified destination directory.
		/// Removes the downloaded zip afterward.
		/// </summary>
		/// <param name="downloadDir"></param>
		/// <param name="siteUri"></param>
		/// <param name="installFilePath"></param>
		/// <returns></returns>
		public bool DownloadPackage(string downloadDir, string siteUri, string installFilePath)
		{
			string downloadFileName = "tmp.zip";
			string downloadFilePath = downloadDir + downloadFileName;

			if (DEBUG_NO_FILE_DOWNLOAD)
			{
				LogMessage("DEBUG: Pretended to download and write " + siteUri + " to " + downloadFilePath + " and move to final location " + installFilePath + "but didn't actually. :)");


				return true;
			}

#pragma warning disable CS0162 // Unreachable code detected
			//debug option
			try
			{


				LogMessage("Downloading: " + siteUri + " to " + downloadDir + "...");

				using (var s = httpClientInstance.GetStreamAsync(siteUri))
				{
					using (var fs = new FileStream(downloadFilePath, FileMode.OpenOrCreate))
					{
						s.Result.CopyTo(fs);
					}
				}

				LogMessage("Unzipping " + downloadFilePath + " to " + downloadDir + "...");

				ZipFile.ExtractToDirectory(downloadFilePath, downloadDir);

				foreach (string modFolder in Directory.EnumerateDirectories(downloadDir, "*", System.IO.SearchOption.TopDirectoryOnly))
				{

					LogMessage("Moving " + modFolder + " to " + installFilePath + "...");
					if (!DEBUG_NO_FILE_INSTALL)
					{
						FileSystem.MoveDirectory(modFolder, installFilePath, DOWNLOAD_CLOBBER_ENABLED);
					}
					if (!DEBUG_NO_FILE_CLEANUP)
					{
						LogMessage("Deleting " + downloadFilePath + "...");
						System.IO.File.Delete(downloadFilePath);
					}
				}

				return true;
			}
			catch (Exception e)
			{
				LogMessage("Installation aborted due to an error: ");
				LogMessage(e);
				return false;
			}
#pragma warning restore CS0162 // Unreachable code detected
			return false;
		}

		/// <summary>
		/// Sends an async httpreq to the given uri and returns the response.
		/// </summary>
		/// <param name="jsonUri"></param>
		/// <returns></returns>
		private async Task<string> AsyncJsonReq(string jsonUri)
		{
			return await httpClientInstance.GetStringAsync(jsonUri);
		}

		/// <summary>
		/// Given a list of PD2 mod objects, checks for each mod in the user's PD2 installation. 
		/// Returns a list of any PD2 mod objects that were not found installed.
		/// </summary>
		/// <param name="dependenciesData"></param>
		public List<Pd2ModData> CheckMods(List<Pd2ModData> dependenciesData)
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

		/// <summary>
		/// Holds basic information about a mod, so that this data can be compared or checked later,
		/// eg. when searching a list of mods that are installed or not installed.
		/// </summary>
		public class Pd2ModData
		{
			private string Name { get; set; }
			private string Description { get; set; }
			private string ModVersion { get; set; }
			private string ModType { get; set; }

			public Pd2ModData(string? name, string? description, string? version, string? modType)
			{
				Name = name ?? string.Empty;
				Description = description ?? string.Empty;
				ModVersion = version ?? string.Empty;
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
				return ModVersion;
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
		
		public class ModDependencyList
		{
			public IList<ModDependencyEntry>? Response { get; set; }
			public Dictionary<string, string>? MetaResponse { get; set; }
			public ModDependencyList(IList<ModDependencyEntry> response,Dictionary<string,string> metaResponse) {
				Response = response ?? new List<ModDependencyEntry>();
				MetaResponse = metaResponse ?? new Dictionary<string, string>();
			}

		}

		public class ModDependencyEntry
		{
			public string? Name { get; set; }
			public string? Description { get; set; }
			public string? DirectoryType { get; set; }
			public string? DirectoryName { get; set; }
			public string? Provider { get; set; }
			public string? Uri { get; set; }
			public string? Branch { get; set; }
			public bool? Release { get; set; }
			public bool? Optional { get; set; }

			//public string ModVersion { get; set; }
			//public string Hash { get; set }
			//maybe we'll use these for manual version checking for direct downloads later
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public static void LogMessage(params object[] message)
		{
			string s = "";
			string div = ", ";
			int i = 0; //num args
			foreach (object o in message) {
				i++;
				if (i > 1)
				{
					s = s + div;
				}

				if (o != null)
				{
					s = s + o.ToString();
				}
				else
				{
					s = s + "[null]";
				}
			}
			string timeStamp = DateTime.Now.ToString();
			s = "[" + timeStamp + "] " + s;
			System.Diagnostics.Debug.WriteLine(s);
		}

	}
}
