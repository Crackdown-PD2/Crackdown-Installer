using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Crackdown_Installer;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
//using ZNix.SuperBLT;
using VDF;

namespace Crackdown_Installer
{
	internal class InstallerManager
	{
		private const bool DOWNLOAD_CLOBBER_ENABLED = false;
		//if true, allows file downloads to replace existing files by the same name.

		private const bool DEBUG_LOCAL_JSON_HTTPREQ = true;
		//if true, skips sending an http req for the json file,
		//and reads a local json file instead.

		private const bool DEBUG_NO_FILE_DOWNLOAD = true;
		//if true, doesn't download the file at all.

		private const bool DEBUG_NO_FILE_INSTALL = true;
		//if true, downloads the files but doesn't actually install any files to their final locations (so as not to interfere with existing installations)

		private const bool DEBUG_NO_FILE_CLEANUP = false;
		//if true, skips cleanup step and does not delete zip files and temp folder after installation,
		//so that you can manually verify them if you wish.

		private const int CURRENT_INSTALLER_VERSION = 2;
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

		private List<ModDependencyEntry> dependenciesFromServer = new();

		private HttpClient httpClientInstance;

		public List<Pd2ModFolder> installedPd2Mods = new();

		private JsonDocumentOptions DEFAULT_JSON_OPTIONS = new()
		{
			AllowTrailingCommas = true
		};

		/// <summary>
		/// Creates a new InstallerManager instance,
		/// which can perform a variety of installation related operations,
		/// such as searching for installed mods.
		/// </summary>
		public InstallerManager(HttpClient client)
		{
			httpClientInstance = client;

			// Registry.GetValue("\\HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\Shell\\Associations\\URLAssociations\\https\\UserChoice", "SteamPath", "");

			tempDownloadsDirectory = Directory.CreateTempSubdirectory("crackdowninstaller_");
			//Directory.Delete(tempDownloadsDirectory.FullName + "/", true);

			pd2InstallDirectory = FindPd2InstallDirectory();
			if (string.IsNullOrEmpty(pd2InstallDirectory))
			{
				LogMessage("Unable to automatically find PD2 install directory.");
			}

			//query cd update server
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
				//				item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
//				LogMessage(jsonResponse);

				//create a throwaway temp element to hold the result of TryGetProperty()
				JsonElement tempElement = new();

				try
				{
					JsonDocument jsonDocument = JsonDocument.Parse(jsonResponse, DEFAULT_JSON_OPTIONS);
					JsonElement rootElement = jsonDocument.RootElement;
					int documentVersion;
					int minRequiredVersion;
					if (rootElement.TryGetProperty("ApiVersion", out tempElement)) {
						documentVersion = tempElement.GetInt32();
						if (documentVersion == CURRENT_INSTALLER_VERSION) {
							
						}
						else {
							if (rootElement.TryGetProperty("MinApiVersion", out tempElement)) {
								minRequiredVersion = tempElement.GetInt32();
								if (CURRENT_INSTALLER_VERSION > minRequiredVersion) {
									//todo send warning that installer may be out of date
								}
								else
								{
									throw new Exception("Installer is too out of date!");
								}
							}
						}
					}
					else {
						throw new Exception("Invalid response from update server! (no ApiVersion given)");
					}

					if (rootElement.TryGetProperty("Response", out tempElement)) {
						for (int i = 0;i<tempElement.GetArrayLength();i++){
							JsonElement dependencyItem = tempElement[i];
							JsonElement tempElement2 = new();
							string dependencyName = GetJsonAttribute("Name", dependencyItem,tempElement2);
							string dependencyDescription = GetJsonAttribute("Description", dependencyItem, tempElement2);
							string dependencyVersionType = GetJsonAttribute("VersionType", dependencyItem, tempElement2);
							string dependencyVersionId = GetJsonAttribute("VersionId", dependencyItem, tempElement2);
							string dependencyDirectoryType = GetJsonAttribute("DirectoryType", dependencyItem, tempElement2);
							string dependencyDirectoryName = GetJsonAttribute("DirectoryName", dependencyItem, tempElement2);
							string dependencyUri = GetJsonAttribute("Uri", dependencyItem, tempElement2);
							string dependencyProvider = GetJsonAttribute("Provider", dependencyItem, tempElement2);
							string dependencyBranch = GetJsonAttribute("Branch", dependencyItem, tempElement2);
							bool dependencyIsOptional;
							bool dependencyIsRelease;
							if (dependencyItem.TryGetProperty("Release", out tempElement2))
							{
								dependencyIsRelease = tempElement2.GetBoolean();
							}
							else {
								dependencyIsRelease = false;
							}
							if (dependencyItem.TryGetProperty("Optional", out tempElement2))
							{
								dependencyIsOptional = tempElement2.GetBoolean();
							}
							else {
								dependencyIsOptional = false;
							}
							ModDependencyEntry dependencyEntry = new ModDependencyEntry(
								dependencyName,
								dependencyDescription,
								dependencyDirectoryType,
								dependencyDirectoryName.Replace("/", "\\"),
								dependencyProvider,
								dependencyUri,
								dependencyBranch,
								dependencyIsRelease,
								dependencyVersionType,
								dependencyVersionId,
								dependencyIsOptional
								);

							result.Add(dependencyEntry);
						}

					}
					else
					{
						throw new Exception("Invalid response from update server! (no Response body)");
					}
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
			else
			{
				/// Send a query to the Crackdown updates repo
				/// to get a list of packages that Crackdown uses
				string jsonResponse = await AsyncJsonReq(DEPENDENCIES_JSON_URL);

				//ModDependencyList item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
				item = JsonSerializer.Deserialize<ModDependencyList>(jsonResponse);
				foreach (ModDependencyEntry entry in item.Response)
				{
					result.Add(entry);
				}
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
				string? dependencyName = entry.Name;
				string? dependencyVersionType = entry.ModVersionType;
				string? dependencyVersionId = entry.ModVersionId;
				if (!string.IsNullOrEmpty(dependencyName) && !string.IsNullOrEmpty(dependencyVersionType))
				{
					if (dependencyVersionType == "xml")
					{
						foreach (Pd2ModFolder thisPd2ModFolder in installedPd2Mods)
						{
							Pd2ModData? xmlData = thisPd2ModFolder.xmlModDefinition;
							if (xmlData != null)
							{
								if (xmlData.GetName() == dependencyName)
								{
									string modVersion = xmlData.GetVersion();
									if (modVersion != dependencyVersionId)
									{
										missingMods.Add(entry);
									}
								}
							}
						}
					}
					else if (dependencyVersionType == "json")
					{
						foreach (Pd2ModFolder thisPd2ModFolder in installedPd2Mods)
						{
							Pd2ModData? jsonData = thisPd2ModFolder.jsonModDefinition;
							if (jsonData != null)
							{
								if (jsonData.GetName() == dependencyName)
								{
									string modVersion = jsonData.GetVersion();
									if (modVersion != dependencyVersionId)
									{
										missingMods.Add(entry);
									}
								}
							}
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
		private string GetJsonAttribute(string propertyName, JsonElement element) {
			JsonElement tempElementHolder = new JsonElement();
			return GetJsonAttribute(propertyName, element, tempElementHolder);
		}

		/// <summary>
		/// Shortcut for JsonElement.TryGetProperty() which can accept an existing JsonElement instead of creating one for each call.
		/// Returns the string value of the element.
		/// Should only be used when the value of the element is known to be a string.
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="element"></param>
		/// <param name="tempElementHolder"></param>
		/// <returns></returns>
		private string GetJsonAttribute(string propertyName, JsonElement element, JsonElement tempElementHolder) {
			if (element.TryGetProperty(propertyName, out tempElementHolder)) {
				return tempElementHolder.GetString() ?? string.Empty;
			};
			return string.Empty;
		}

		public Pd2ModFolder? GetInstalledBltMod(string name) {
			foreach (Pd2ModFolder modFolder in installedPd2Mods) {
				Pd2ModData? pd2ModData = modFolder.jsonModDefinition;
				if (pd2ModData != null) {
					if (pd2ModData.GetName() == name) {
						return modFolder;
					}
				}
			}
			return null;
		}
		public bool HasBltModInstalled(string name) {
			foreach (Pd2ModFolder modFolder in installedPd2Mods) {
				Pd2ModData? pd2ModData = modFolder.jsonModDefinition;
				if (pd2ModData != null)
				{
					if (pd2ModData.GetName() == name) {
						return true;
					}
				}
			}
			return false;
		}
		public Pd2ModFolder? GetInstalledBeardlibMod(string name) {
			foreach (Pd2ModFolder modFolder in installedPd2Mods) {
				Pd2ModData? pd2ModData = modFolder.xmlModDefinition;
				if (pd2ModData != null) {
					if (pd2ModData.GetName() == name) {
						return modFolder;
					}
				}
			}
			return null;
		}
		public bool HasBeardlibModInstalled(string name) {
			foreach (Pd2ModFolder modFolder in installedPd2Mods) {
				Pd2ModData? pd2ModData = modFolder.xmlModDefinition;
				if (pd2ModData != null) {
					if (pd2ModData.GetName() == name) {
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Given the path to a folder,
		/// searches the folder for a mod.txt json file, and creates a new Pd2ModData object to hold the parsed data.
		/// </summary>
		/// <param name="modPath"></param>
		/// <returns></returns>
		private Pd2ModData? ReadJsonModData(string modPath) 
		{
			string definitionPath = Path.Combine(modPath, JSON_MOD_DEFINITION_NAME);
			if (File.Exists(definitionPath))
			{
				StreamReader? sr = null;
				try
				{
					//don't serialize since we have to be much more careful when parsing unknown files;
					//blt's json parser is very lax, especially when it comes to missing commas

					JsonElement tempElement = new();
					sr = new StreamReader(definitionPath);
					string jsonStr = sr.ReadToEnd();
					JsonDocument definitionFile = JsonDocument.Parse(jsonStr, DEFAULT_JSON_OPTIONS);
					JsonElement rootElement = definitionFile.RootElement;

					string modName = GetJsonAttribute("name", rootElement, tempElement);
					string modDescription = GetJsonAttribute("description", rootElement, tempElement);
					string modVersion = GetJsonAttribute("version", rootElement, tempElement);
					string modType = "json";

					sr.Close();
					return new Pd2ModData(modName, modDescription, modVersion, modType, modPath);
				}

				catch (JsonException e)
				{
					LogMessage(e.Message);
				}
				catch (Exception e)
				{
					LogMessage(e.Message);
				}

				if (sr != null) {
					sr.Close();
				}

			}
			return null;
		}

		/// <summary>
		/// Given the path to a folder,
		/// searches the folder for a main.xml xml file, and creates a new Pd2ModData object to hold the parsed data.
		/// /// </summary>
		/// <param name="modPath"></param>
		/// <returns></returns>
		private Pd2ModData? ReadXmlModData(string modPath)
		{
			string definitionPath = Path.Combine(modPath, XML_MOD_DEFINITION_NAME);
			if (Microsoft.VisualBasic.FileIO.FileSystem.FileExists(definitionPath))
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
							string modDescription = GetXmlAttribute(attrcoll, "description");
							//note: description is not specified in BeardLib ModCore documentation
							//but we can look for it anyway

							
							if (string.IsNullOrEmpty(modVersion))
							{
								XmlElement? assetUpdatesElement = element["AssetUpdates"];
								if (assetUpdatesElement != null)
								{
									XmlAttributeCollection assetUpdatesCollection = assetUpdatesElement.Attributes;

									modVersion = GetXmlAttribute(assetUpdatesCollection, "version");
								}
							}

							string modType = "xml";
							return new Pd2ModData(modName, modDescription, modVersion, modType, modPath);
						}
					}
				}
				catch (IOException e)
				{
					Console.WriteLine("The file could not be read:");
					Console.WriteLine(e.Message);
				}
			}
			return null;
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


		public async Task<bool> DownloadDependency(ModDependencyEntry dependencyEntry) {
			string downloadDir = tempDownloadsDirectory.FullName;

			string downloadUri = "";
			string? installDir = null;

			string provider = dependencyEntry.Provider;
			string branch = dependencyEntry.Branch;
			string uri = dependencyEntry.Uri;

			//get url to query for file download
			if (provider == "github")
			{
				string queryUrl;
				if (dependencyEntry.Release)
				{
					queryUrl = PROVIDER_GITHUB_RELEASE_URL.Replace("$id$",uri);
					try
					{
						string jsonData = await AsyncJsonReq(queryUrl);

						JsonDocument releaseData = JsonDocument.Parse(jsonData,DEFAULT_JSON_OPTIONS);
						JsonElement rootElement = releaseData.RootElement;
						downloadUri = GetJsonAttribute("zipball_url",rootElement);
					}
					catch (Exception e) {
						LogMessage(e);
						//set status fail
						return false;
					}
				}
				else
				{
					downloadUri = PROVIDER_GITHUB_DIRECT_URL.Replace("$id$", uri)
						.Replace("$branch$", branch);
					//queryUrl = PROVIDER_GITHUB_COMMIT_URL.Replace("$id$", uri)
					//	.Replace("$branch$", branch);
				}
			}
			else if (provider == "gitlab")
			{
				if (dependencyEntry.Release)
				{
					//downloadUri = PROVIDER_GITLAB_COMMIT_URL;
					//not currently supported
					return false;
				}
				else
				{
					downloadUri = PROVIDER_GITLAB_RELEASE_URL.Replace("$id$",uri);
				}
			}
			else if (provider == "direct")
			{
				downloadUri = uri;
			}
			else if (provider == "modworkshop") {
				//not yet supported
				return false;
			}
			else
			{
				//unknown provider type
				return false;
			}

			//get final installation location
			if (dependencyEntry.DirectoryType == "beardlib")
			{
				installDir = pd2InstallDirectory + "mods\\" + dependencyEntry.DirectoryName;
			}
			else if (dependencyEntry.DirectoryType == "blt")
			{
				installDir = pd2InstallDirectory + "mods\\" + dependencyEntry.DirectoryName;
			}
			else if (dependencyEntry.DirectoryType == "overrides")
			{
				installDir = pd2InstallDirectory + "assets\\mod_overrides\\" + dependencyEntry.DirectoryName;
			}
			else if (dependencyEntry.DirectoryType == "root")
			{
				installDir = pd2InstallDirectory + dependencyEntry.DirectoryName;
			}
			else
			{
				//unknown install directory type!
				return false;
			}
			if (installDir != null)
			{
				return DownloadPackage(downloadDir, downloadUri, installDir);
			}
			return false;
		}

		/// <summary>
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
			string downloadFilePath = Path.Combine(downloadDir + downloadFileName);

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
						Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(modFolder, installFilePath, DOWNLOAD_CLOBBER_ENABLED);
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
		/// 
		/// </summary>
		/// <returns></returns>
		public void CollectPd2Mods()
		{
			string installPath = GetPd2InstallDirectory();
			List<Pd2ModFolder> result = new();
			string modsPath = Path.Combine(installPath, "mods\\");
			string modOverridesPath = Path.Combine(installPath, "assets\\mod_overrides\\");


			//search mods folder 
			foreach (string subDirectory in Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(modsPath))
			{
				Pd2ModData? newBltMod = ReadJsonModData(subDirectory);
				Pd2ModData? newBeardlibMod = ReadXmlModData(subDirectory);

				//must have at least one valid mod definition file to be counted as a mod
				if (newBltMod != null || newBeardlibMod != null)
				{
					Pd2ModFolder newModFolder = new Pd2ModFolder(newBltMod, newBeardlibMod);
					result.Add(newModFolder);
				}
			}


			//search mod_overrides folder (beardlib mods only)
			foreach (string subDirectory in Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(modOverridesPath))
			{
				Pd2ModData? newBeardlibMod = ReadXmlModData(subDirectory);
				if (newBeardlibMod != null)
				{
					Pd2ModFolder newModFolder = new Pd2ModFolder(null, newBeardlibMod);
					result.Add(newModFolder);
				}
			}

			installedPd2Mods = result;
			//return result;
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
			private string ModPath{ get; set; }

			public Pd2ModData(string? name, string? description, string? version, string? modType, string? modPath)
			{
				Name = name ?? string.Empty;
				Description = description ?? string.Empty;
				ModVersion = version ?? string.Empty;
				ModType = modType ?? string.Empty;
				ModPath = modPath ?? string.Empty;
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
			
			public string GetModPath()
			{
				return ModPath;
			}

		}

		public class Pd2ModFolder
		{
			public Pd2ModData? jsonModDefinition;
			public Pd2ModData? xmlModDefinition;
			public Pd2ModFolder(Pd2ModData? jsonDefinition, Pd2ModData? xmlDefinition)
			{
				jsonModDefinition = jsonDefinition;
				xmlModDefinition = xmlDefinition;
			}
		}
		/// <summary>
		/// A class to represent the schema of the json manifest returned from the Crackdown update server
		/// </summary>
		public class ModDependencyList
		{
			public IList<ModDependencyEntry>? Response { get; set; }
			public Dictionary<string, string>? MetaResponse { get; set; }
			public ModDependencyList(IList<ModDependencyEntry> response,Dictionary<string,string> metaResponse) {
				Response = response ?? new List<ModDependencyEntry>();
				MetaResponse = metaResponse ?? new Dictionary<string, string>();
			}

		}

		/// <summary>
		/// A class to represent the schema of a dependency package (mod) that needs to be downloaded, as received from the Crackdown update server
		/// </summary>
		public class ModDependencyEntry
		{
			public string Name { get; set; }
			public string Description { get; set; }
			public string DirectoryType { get; set; }
			public string DirectoryName { get; set; }
			public string Provider { get; set; }
			public string Uri { get; set; }
			public string Branch { get; set; }
			public bool Release { get; set; }
			public string ModVersionType { get; set; }
			public string ModVersionId { get; set; }
			public bool Optional { get; set; }

			public ModDependencyEntry(string name, string description, string directoryType, string directoryName, string provider, string uri, string branch, bool release, string modVersionType, string modVersionId, bool optional)
			{
				Name = name;
				Description = description;
				DirectoryType = directoryType;
				DirectoryName = directoryName;
				Provider = provider;
				Uri = uri;
				Branch = branch;
				Release = release;
				ModVersionType = modVersionType;
				ModVersionId = modVersionId;
				Optional = optional;
			}
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
