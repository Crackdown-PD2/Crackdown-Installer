using System;
using System.Drawing.Text;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using Crackdown_Installer;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
//using ZNix.SuperBLT;
using VDF; //Valve Data Format parser 
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;

//Stores result data from installation file operations
//such as finding/hashing installed mods, querying required dependencies from the server

namespace Crackdown_Installer
{
	internal class InstallerManager
	{
		private const bool DOWNLOAD_CLOBBER_ENABLED = false;
		//if true, allows file downloads to replace existing files by the same name.

		private const bool DEBUG_LOCAL_JSON_HTTPREQ = true;
		//if true, skips sending an http req for the json file,
		//and reads a local json file instead.

		private const bool DEBUG_NO_FILE_DOWNLOAD = false;
		//if true, doesn't download the file at all.

		private const bool DEBUG_NO_FILE_INSTALL = false;
		//if true, downloads the files but doesn't actually install any files to their final locations (so as not to interfere with existing installations)

		private const bool DEBUG_NO_FILE_CLEANUP = false;
		//if true, skips cleanup step and does not delete zip files and temp folder after installation,
		//so that you can manually verify them if you wish.

		private const int CURRENT_INSTALLER_VERSION = 2;
		private const string DEPENDENCIES_JSON_URL = "https://raw.githubusercontent.com/Crackdown-PD2/deathvox/autoupdate/cd_dependencies.json";
		private const string SUPERBLT_DLL_WSOCK32_URL = "https://sblt-update.znix.xyz/pd2update/updates/meta.php?id=payday2bltwsockdll"; //holds meta json info about dll updates
		private const string SUPERBLT_DLL_IPHLPAPI_URL = "https://sblt-update.znix.xyz/pd2update/updates/meta.php?id=payday2bltdll"; //holds meta json info about dll updates
		private const string DLL_DIFFERENCE_INFO_URL = "https://superblt.znix.xyz/#regarding-the-iphlpapidll-vs-wsock32dll-situation"; //a page for humans to read with their eyeballs, about differences between iphlpapi and wsock32 dlls

		private const string PROVIDER_GITHUB_COMMIT_URL = "https://api.github.com/repos/$id$/commits/$branch$";
		private const string PROVIDER_GITHUB_RELEASE_URL = "https://api.github.com/repos/$id$/releases/latest";
		private const string PROVIDER_GITHUB_DIRECT_URL = "https://github.com/$id$/archive/$branch$.zip";

		//private const string PROVIDER_GITLAB_COMMIT_URL = "https://gitlab.com/api/v4/projects/$id$/repository/branches/$branch$"; //meta json info about latest commit in branch
		private const string PROVIDER_GITLAB_RELEASE_URL = "https://gitlab.com/api/v4/projects/$id$/releases"; //direct latest release download
		private const string PROVIDER_GITLAB_DIRECT_URL = "https://gitlab.com/api/v4/projects/$id$/repository/archive.zip"; //direct latest commit download 

		private const string SUPERBLT_DLL_NAME_MAIN = "WSOCK32.dll";
		private const string SUPERBLT_DLL_NAME_ALTERNATE = "IPHLPAPI.dll";

		private const string JSON_MOD_DEFINITION_NAME = "mod.txt";
		private const string XML_MOD_DEFINITION_NAME = "main.xml";

		private const string KEY_USER_ROOT = "HKEY_CURRENT_USER";
		private const string KEY_VALVE_STEAM = "Software\\Valve\\Steam";

		private const string STEAM_LIBRARY_MANIFEST_PATH = "%%STEAM%%\\steamapps\\libraryfolders.vdf";

		private const string PD2_APPID = "218620"; //Steam appid for PAYDAY 2

		private string? steamInstallDirectory;
		private string? pd2InstallDirectory;

		private bool useAlternateDll = false;

		private DirectoryInfo? tempDirectoryInfo;

		private List<ModDependencyEntry> dependenciesFromServer = new();

		private HttpClient httpClientInstance;

		public List<Pd2ModFolder> installedPd2Mods = new(); //holds information about any mod folders which are placed in both standard install locations (mods, mod_overrides)

		public List<Pd2ModData> installedMiscPd2Mods = new(); //holds loose files that are not stored in standard mod folder format, ie modloader dll

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

			pd2InstallDirectory = FindPd2InstallDirectory();
			if (string.IsNullOrEmpty(pd2InstallDirectory))
			{
				LogMessage("Unable to automatically find PD2 install directory.");
			}

			//query cd update server
			CollectDependencies();
		}

		/// <summary>
		/// Query update server for manifest of dependencies needed for CD, and parse the response. Also perform secondary query if necessary.
		/// Save the resulting list to memory.
		/// </summary>
		public async void CollectDependencies()
		{
			
			List<ModDependencyEntry> result = new();
			
			try
			{

				//superblt modloader has two components: a basemod folder containing lua scripts,
				//and a dll file.
				//Both are required.
				//the dll file can be a WSOCK32.dll (Windows Sockets API) or IPHLPAPI.dll (Windows IP Helper API)
				//but only ONE should be used- not both! 

				ModDependencyEntry dllDependency;

				string dllName;
				string hash;
				string provider = "znix";
				string desc = "The DLL file required for the SuperBLT modloader to function.";
				string dllDirectoryType = "file";
				string uri;
				string branch = ""; //not applicable
				string versionType = "hash";
				string versionId;
				string downloadUrl;


				if (useAlternateDll)
				{
					dllName = SUPERBLT_DLL_NAME_ALTERNATE;
					uri = SUPERBLT_DLL_IPHLPAPI_URL;
				}
				else
				{
					dllName = SUPERBLT_DLL_NAME_MAIN;
					uri = SUPERBLT_DLL_WSOCK32_URL;
				}

				string queryUrl = uri;

				string jsonData = await AsyncJsonReq(queryUrl);
				JsonDocument releaseData = JsonDocument.Parse(jsonData, DEFAULT_JSON_OPTIONS);
				JsonElement secondaryRootElement = releaseData.RootElement[0];
				//since we have no way of easily checking the version number of the local dll,
				//we will always use comparing hashes instead, when it comes to updates for the dll specifically
				versionId = GetJsonAttribute("version", secondaryRootElement);
				//if (versionType == "hash" && string.IsNullOrEmpty(hash)) {
					hash = GetJsonAttribute("hash", secondaryRootElement);
				//}

				downloadUrl = GetJsonAttribute("download_url", secondaryRootElement);

				//string patchNotesUrl = GetJsonAttribute("patchnotes_url", secondaryRootElement);

				dllDependency = new(
					dllName,
					desc,
					dllDirectoryType,
					dllName,
					provider,
					hash,
					uri,
					branch,
					false, //release is not applicable
					versionType,
					versionId,
					false, //not optional
					downloadUrl
				);

				result.Add(dllDependency);
			}
			catch (Exception e)
			{
				LogMessage("Error getting update data for SuperBLT dll file",e);
			}


			ModDependencyList item;
			if (DEBUG_LOCAL_JSON_HTTPREQ)
#pragma warning disable CS0162 // Unreachable code detected
			{
				StreamReader sr = new("test.json");
				string jsonResponse = sr.ReadToEnd();
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
							//up to date installer version yay
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
							string name = GetJsonAttribute("Name", dependencyItem,tempElement2);
							string desc = GetJsonAttribute("Description", dependencyItem, tempElement2);
							string versionType = GetJsonAttribute("VersionType", dependencyItem, tempElement2);
							string versionId = GetJsonAttribute("VersionId", dependencyItem, tempElement2);
							string definitionType = GetJsonAttribute("DefinitionType", dependencyItem, tempElement2);
							string directoryName = GetJsonAttribute("DirectoryName", dependencyItem, tempElement2); //output folder/file name
							string hash = GetJsonAttribute("Hash", dependencyItem, tempElement2);

							string uri = GetJsonAttribute("Uri", dependencyItem, tempElement2);				//primary uri field; can be repo id or direct url, depending on provider
							string provider = GetJsonAttribute("Provider", dependencyItem, tempElement2);	//secondary uri field
							string branch = GetJsonAttribute("Branch", dependencyItem, tempElement2);       //trinary uri field; mainly used with github/gitlab repos

							bool isOptional;
							bool isRelease;
							if (dependencyItem.TryGetProperty("Release", out tempElement2))
							{
								isRelease = tempElement2.GetBoolean();
							}
							else {
								isRelease = false;
							}
							if (dependencyItem.TryGetProperty("Optional", out tempElement2))
							{
								isOptional = tempElement2.GetBoolean();
							}
							else {
								isOptional = false;
							}

							//get url to query for later file download:
							//the end result direct download link will vary according to dependency and provider,
							//so in some cases, we need to make another httpreq to the specific provider to get the url
							//this is the direct download link that the installer will use to download the archive of the dependency
							string downloadUrl = string.Empty;

							if (provider == "github")
							{
								string queryUrl;
								if (isRelease)
								{
									queryUrl = PROVIDER_GITHUB_RELEASE_URL.Replace("$id$", uri);
									try
									{
										string jsonData = await AsyncJsonReq(queryUrl);

										JsonDocument releaseData = JsonDocument.Parse(jsonData, DEFAULT_JSON_OPTIONS);
										JsonElement secondaryRootElement = releaseData.RootElement;
										downloadUrl = GetJsonAttribute("zipball_url", secondaryRootElement);
									}
									catch (Exception e)
									{
										LogMessage("Failed getting url from github provider, package " + name,e);
									}
								}
								else
								{
									downloadUrl = PROVIDER_GITHUB_DIRECT_URL.Replace("$id$", uri)
										.Replace("$branch$", branch);
									//queryUrl = PROVIDER_GITHUB_COMMIT_URL.Replace("$id$", uri)
									//	.Replace("$branch$", branch);
								}
							}
							else if (provider == "gitlab")
							{
								if (isRelease)
								{
									//downloadUri = PROVIDER_GITLAB_COMMIT_URL;
									//not currently supported
									throw new Exception("Gitlab releases are not currently supported! Package name " + name);
								}
								else
								{
									downloadUrl = PROVIDER_GITLAB_RELEASE_URL.Replace("$id$", uri);
								}
							}
							else if (provider == "direct")
							{
								downloadUrl = uri;
							}
							else if (provider == "znix")
							{
								//not used; dll meta is parsed separately as a special case earlier in this function


							}
							else if (provider == "modworkshop")
							{
								throw new Exception("ModWorkshop releases are not currently supported! Package name " + name);
								//not yet supported
							}
							else
							{
								//unknown provider type
								throw new Exception("Unknown provider type! Package name " + name);
							}

							ModDependencyEntry dependencyEntry = new ModDependencyEntry(
								name,
								desc,
								definitionType,
								directoryName.Replace("/", "\\"),
								provider,
								hash,
								uri,
								branch,
								isRelease,
								versionType,
								versionId,
								isOptional,
								downloadUrl
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

		/// <summary>
		/// Returns cached dependency entries
		/// </summary>
		/// <returns></returns>
		public List<ModDependencyEntry> GetDependencyEntries() {
			return dependenciesFromServer;
		}
		
		/// <summary>
		/// Query dependencies, save them to the cached member, and return the resulting list
		/// </summary>
		/// <returns></returns>
		public List<ModDependencyEntry> GetDependencyEntries(bool forceReevaluate) {
			if (forceReevaluate)
			{
				CollectDependencies();
			}
			return GetDependencyEntries();
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
		private string GetJsonAttribute(string propertyName, JsonElement element)
		{
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
		private string GetJsonAttribute(string propertyName, JsonElement element, JsonElement tempElementHolder)
		{
			if (element.TryGetProperty(propertyName, out tempElementHolder)) {
				return tempElementHolder.GetString() ?? string.Empty;
			};
			return string.Empty;
		}

		public Pd2ModFolder? GetInstalledBltMod(string name)
		{
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

		public bool HasBltModInstalled(string name)
		{
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
		public Pd2ModFolder? GetInstalledBeardlibMod(string name)
		{
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
		public bool HasBeardlibModInstalled(string name)
		{
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

		
		public bool HasMiscModInstalled(string name)
		{
			foreach (Pd2ModData mod in installedMiscPd2Mods) {
				if (mod.GetName() == name)
				{
					return true;
				}
			}
			return false;
		}
		public Pd2ModData? GetInstalledMiscMod(string name)
		{
			foreach (Pd2ModData mod in installedMiscPd2Mods) {
				if (mod.GetName() == name)
				{
					return mod;
				}
			}
			return null;
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
					//don't serialize since we have to be much more careful when parsing files that are subject to change
					//(eg if the manifest ever gets an update that changes its schema)
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
				catch (IOException e)
				{
					LogMessage("The file [" + definitionPath + "] could not be opened or read:");
					LogMessage(e.Message);
				}
				catch (JsonException e)
				{
					LogMessage("One or more JSON errors was encountered while reading [" + definitionPath + "]:");
					LogMessage(e.Message);
				}
				catch (Exception e)
				{
					LogMessage("Unknown error occured while reading [" + definitionPath + "]:");
					LogMessage(e.Message);
				}
				finally {
					if (sr != null)
					{
						sr.Close();
					}
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
					LogMessage("The file [" + definitionPath + "] could not be opened or read:");
					LogMessage(e.Message);
				}
				catch (XmlException e) {
					LogMessage("One or more XML errors prevented the file [" + definitionPath + "] from loading:");
					LogMessage(e.Message);
				}
				catch (Exception e) {
					LogMessage("Unknown error occured while reading [" + definitionPath + "]:",e.Message);
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

		public void CreateTempDirectory() {
			 tempDirectoryInfo = Directory.CreateTempSubdirectory("crackdowninstaller_");
		}

		public void DisposeTempDirectory()
		{
			if (!DEBUG_NO_FILE_CLEANUP)
			{
			
				tempDirectoryInfo = Directory.CreateTempSubdirectory("crackdowninstaller_");
				string tempDownloadsDirectory = tempDirectoryInfo.FullName;
				Directory.Delete(tempDownloadsDirectory + "\\", true);
				LogMessage("Downloads complete. Deleting " + tempDownloadsDirectory + ".");
			}
		}

		public async Task<string?> DownloadDependency(ModDependencyEntry dependencyEntry, Action<double?, long, long?> callbackUpdateProgress) {
			if (tempDirectoryInfo == null) {
				throw new Exception("Error: No temp directory found! Could not download dependency.");
			}
			string downloadDir = tempDirectoryInfo.FullName + "\\";

			string installDir;

			//info uri should have already been queried during dependency collection
				//string branch = dependencyEntry.GetBranch();
				//string uri = dependencyEntry.GetUri(); 
			string directoryType = dependencyEntry.GetDefinitionType();
			string downloadUri = dependencyEntry.GetDownloadUrl();
			string directoryName = dependencyEntry.GetFileName();

			//get final installation location
			if (directoryType == "beardlib")
			{
				installDir = pd2InstallDirectory + "mods\\" + directoryName;
			}
			else if (directoryType == "blt")
			{
				installDir = pd2InstallDirectory + "mods\\" + directoryName;
			}
			else if (directoryType == "overrides")
			{
				installDir = pd2InstallDirectory + "assets\\mod_overrides\\" + directoryName;
			}
			else if (directoryType == "file")
			{
				installDir = pd2InstallDirectory + directoryName;
			}
			else
			{
				//unknown install directory type!
				LogMessage("Unknown installation type: [" + directoryType + "]");
				return "Bad dependency data";
			}
			string? errorMsg = await DownloadPackage(downloadDir, downloadUri, installDir, callbackUpdateProgress);
			return errorMsg;
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
		public async Task<string?> DownloadPackage(string downloadDir, string siteUri, string installFilePath, Action<double?,long,long?> callback)
		{
			string downloadFileName = "tmp.zip";
			string downloadFilePath = Path.Combine(downloadDir + downloadFileName);


			//debug only: "skip" the actual download process
			if (DEBUG_NO_FILE_DOWNLOAD)
			{
				LogMessage("DEBUG: Pretended to download and write " + siteUri + " to " + downloadFilePath + " and move to final location " + installFilePath + " but didn't actually. :)");

				return null;
			}

			LogMessage("Downloading: " + siteUri + " to " + downloadDir + "...");

			//send download req, output to Stream
			//Stream? s = null;
			try
			{
				//				await httpClientInstance.DownloadAsync(siteUri,s,)
				//				s = await httpClientInstance.GetStreamAsync(siteUri);
				
				using (var client = new HttpClientDownloadWithProgress(siteUri, downloadFilePath))
				{
					client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
						//LogMessage("[" + progressPercentage + "%]" + "[" + totalBytesDownloaded + "] / [" + totalFileSize + "]");
						callback(progressPercentage,totalBytesDownloaded,totalFileSize);
					};

					await client.StartDownload();
				}



			}
			catch (Exception e) {
				LogMessage("Download stream aborted due to an error:");
				LogMessage(e.Message);
				return "Could not get download stream";
			}
			finally {
//				s?.Close();
			}
			/*
			FileStream? fs = null;
			try
			{
				fs = new(downloadFilePath, FileMode.OpenOrCreate);

				s.CopyTo(fs);
			}
			catch (Exception e)
			{
				LogMessage("Download write aborted due to an error:");
				LogMessage(e.Message);
				return "Could not save file";
			}
			finally
			{
				//dispose resource
				s?.Close();
				fs?.Dispose();
			}
			*/

			LogMessage("Unzipping " + downloadFilePath + " to " + downloadDir + "...");

			try
			{
				ZipFile.ExtractToDirectory(downloadFilePath, downloadDir);
			}
			catch (Exception e)
			{
				LogMessage("Extraction aborted due to an error:");
				LogMessage(e.Message);
				return "Could not unzip downloaded archive";
			}

			foreach (string modFolder in Directory.EnumerateDirectories(downloadDir, "*", System.IO.SearchOption.TopDirectoryOnly))
			{

				LogMessage("Moving " + modFolder + " to " + installFilePath + "...");
				if (!DEBUG_NO_FILE_INSTALL)
				{
					try
					{
						//there should only be one individual file/folder per dependency package download
						Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(modFolder, installFilePath, DOWNLOAD_CLOBBER_ENABLED);
						break;
					}
					catch (Exception e)
					{
						LogMessage("Installation aborted due to an error:");
						LogMessage(e.Message);
						return "Could not move downloaded package";
					}
				}
				else
				{
					LogMessage("[debug] Pretended to move unzipped download into final installation location but didn't actually");
				}
			}

			try {
				System.IO.File.Delete(downloadFilePath); //delete tmp zip file
			}
			catch (Exception e)
			{
				LogMessage("Warning: unable to delete downloaded dependency archive file");
				LogMessage(e.Message);
			}

			//success; do not return an error message
			return null;
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

			//check dll version (special case)
			string superbltDllPathMain = Path.Combine(installPath, SUPERBLT_DLL_NAME_MAIN);
			string superbltDllPathAlternate = Path.Combine(installPath, SUPERBLT_DLL_NAME_ALTERNATE);
			bool hasWsock= File.Exists(superbltDllPathMain);
			bool hasIphlpapi = File.Exists(superbltDllPathAlternate);

			if (hasWsock ^ hasIphlpapi)
			{
				string dllName;
				string dllDesc = "The SuperBLT modloader dll file, required for all mods.";
				string dllHash;
				string dllType = "file";
				string dllPath;

				if (hasWsock)
				{ //has WSOCK32.dll 
					dllHash = ZNix.SuperBLT.Hasher.HashFile(superbltDllPathMain);
					dllName = SUPERBLT_DLL_NAME_MAIN;
					dllPath = superbltDllPathMain;
				}
				else //has IPHLPAPI.dll
				{
					useAlternateDll = true;
					dllHash = ZNix.SuperBLT.Hasher.HashFile(superbltDllPathAlternate);
					dllName = SUPERBLT_DLL_NAME_ALTERNATE;
					dllPath = superbltDllPathAlternate;
				}

				dllPath = Path.Combine(installPath, dllName);

				Pd2ModData dllModData = new (dllName, dllDesc, dllHash, dllType, dllPath);
				dllModData.SetHash(dllHash);

				installedMiscPd2Mods.Add(dllModData);
				//save this as a class member and handle it as a special case
				//instead of trying to fit it into the mod folder-based schema
			}
			else if (hasWsock && hasIphlpapi) {
				//TODO
				//show warning; installing both is bad
			}
			else 
			{
				//has neither installed; do nothing
			}

			//search mods folder 
			foreach (string subDirectory in Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(modsPath))
			{
				Pd2ModData? newBltMod = ReadJsonModData(subDirectory);
				Pd2ModData? newBeardlibMod = ReadXmlModData(subDirectory);

				//must have at least one valid mod definition file to be counted as a mod
				if (newBltMod != null || newBeardlibMod != null)
				{
					Pd2ModFolder newModFolder = new Pd2ModFolder(newBltMod, newBeardlibMod, subDirectory);
					result.Add(newModFolder);
				}
			}


			//search mod_overrides folder (beardlib mods only)
			foreach (string subDirectory in Microsoft.VisualBasic.FileIO.FileSystem.GetDirectories(modOverridesPath))
			{
				Pd2ModData? newBeardlibMod = ReadXmlModData(subDirectory);
				if (newBeardlibMod != null)
				{
					Pd2ModFolder newModFolder = new Pd2ModFolder(null, newBeardlibMod,subDirectory);
					result.Add(newModFolder);
				}
			}

			installedPd2Mods = result;
			//return result;
		}

		public bool ShouldUseAlternateDll()
		{
			return useAlternateDll;
		}

		public class DependencyDownloadResult
		{
			public bool success;
			public ModDependencyEntry entry;
			public string message;
			public DependencyDownloadResult(bool success,ModDependencyEntry entry,string message)
			{
				this.success = success;
				this.entry = entry;
				this.message = message;
			}
			
		}

		/// <summary>
		/// Holds basic information about a mod, so that this data can be compared or checked later,
		/// eg. when searching a list of mods that are installed or not installed.
		/// </summary>
		public class Pd2ModData
		{
			private string name { get; set; }
			private string description { get; set; }
			private string versionId { get; set; }
			private string definitionType { get; set; }
			private string path{ get; set; }
			private string? hash { get; set; }

			public Pd2ModData(string? name, string? description, string? version, string? modType, string? modPath)
			{
				this.name = name ?? string.Empty;
				this.description = description ?? string.Empty;
				this.versionId = version ?? string.Empty;
				this.definitionType = modType ?? string.Empty;
				this.path = modPath ?? string.Empty;
			}
			public Pd2ModData(string? name, string? description, string? version, string? modType, string? modPath, string? hash)
			{
				this.name = name ?? string.Empty;
				this.description = description ?? string.Empty;
				this.versionId = version ?? string.Empty;
				this.definitionType = modType ?? string.Empty;
				this.path = modPath ?? string.Empty;
				this.hash = hash ?? string.Empty;
			}

			/// <summary>
			/// Get the name of this mod, as written in its definition file.
			/// </summary>
			/// <returns></returns>
			public string GetName() {return this.name;}

			public string GetVersion() {return this.versionId;}

			public string GetDescription() {return this.description;}

			public string GetModType() {return this.definitionType; }
			
			public string GetModPath() {return this.path;}

			public string? GetHash() {return this.hash;}

			public void SetHash(string hash) { this.hash = hash; }
		}

		public class Pd2ModFolder
		{
			public Pd2ModData? jsonModDefinition;
			public Pd2ModData? xmlModDefinition;
			public string modFolderName;
			public Pd2ModFolder(Pd2ModData? jsonDefinition, Pd2ModData? xmlDefinition, string folderName)
			{
				jsonModDefinition = jsonDefinition;
				xmlModDefinition = xmlDefinition;
				modFolderName = folderName;
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
			private string name { get; set; }
			private string description { get; set; }
			private string directoryType { get; set; }
			private string directoryName { get; set; }
			private string provider { get; set; }
			private string? hash { get; set; }
			private string uri { get; set; }
			private string branch { get; set; }
			private bool isRelease { get; set; }
			private string versionType { get; set; }
			private string versionId { get; set; }
			private bool isOptional { get; set; }
			private string downloadUrl { get; set; }


			public ModDependencyEntry(string name, string description, string directoryType, string directoryName, string provider, string hash, string uri, string branch, bool isRelease, string versionType, string versionId, bool isOptional, string downloadUrl)
			{
				this.name = name;
				this.description = description;
				this.directoryType = directoryType;
				this.directoryName = directoryName;
				this.provider = provider;
				this.hash = hash;
				this.uri = uri;
				this.branch = branch;
				this.isRelease = isRelease;
				this.versionType = versionType;
				this.versionId = versionId;
				this.isOptional = isOptional;
				this.downloadUrl = downloadUrl;
			}

			public string GetName() { return name; }
			public string GetDescription() { return description; }
			public string GetDefinitionType() {  return directoryType; }
			public string GetFileName() {  return directoryName; }
			public string GetProvider() { return provider; }
			public string? GetHash() { return hash; }
			public string GetUri() { return uri; }
			public string GetBranch() { return branch; }
			public string GetModVersionType() { return versionType; }
			public string GetModVersionId() {  return versionId; }
			public bool IsOptional() { return isOptional; }
			public bool IsRelease() { return isRelease; }
			public string GetDownloadUrl() {  return downloadUrl; }

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
