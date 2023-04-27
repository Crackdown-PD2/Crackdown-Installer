using Microsoft.Win32;
using static Crackdown_Installer.InstallerManager;

namespace Crackdown_Installer
{
	internal static class InstallerWrapper
	{
		public static InstallerManager? instMgr;

		public const int CURRENT_INSTALLER_VERSION = 2;

		const string URL_CRACKDOWN_DISCORD = "https://discord.gg/dak2zQ2";
		const string URL_CRACKDOWN_HOMEPAGE = "http://crackdownmod.com/";
		const string URL_CRACKDOWN_WIKI = "https://totalcrackdown.wiki.gg/";
		const string URL_CRACKDOWN_INSTRUCTIONS = "https://github.com/Crackdown-PD2/Crackdown-Installer";

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]

		static void Main(string[] args)
		{

			//instantiate new client to handle all outgoing http reqs
			HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromMinutes(1);

			instMgr = new(client);

			// create temp directory in the appdata/local/temp/ directory for this session;
			// this temp directory will hold any downloaded zip archives, 
			// as well as the debug log generated by the installer
			CreateTemporaryDirectory();
			
			// the StreamWriter object for the installer's debug logs is active from application startup til application close
			CreateLogWriter();

			LogMessage($"Installer application launched (version {CURRENT_INSTALLER_VERSION})");

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();

			Application.ThreadException += Application_ThreadException;

			Application.ApplicationExit += new EventHandler(OnApplicationExit);

			Application.Run(new Form1());
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			LogMessage($"Application has crashed (unhandled exception): {e.Exception.Message}");
			Application.Exit();
		}

		/// <summary>
		/// Returns the api version that this installer is rated to handle, for getting the dependency manifest from the update server.
		/// </summary>
		/// <returns></returns>
		public static int GetInstallerVersion()
		{
			return CURRENT_INSTALLER_VERSION;
		}

		public static async Task<string?> DownloadDependency(ModDependencyEntry dependency,Action<double?, long, long?> callbackUpdateProgress)
		{
			if (instMgr != null)
			{
				return await instMgr.DownloadDependency(dependency, callbackUpdateProgress);
			}
			throw new Exception("DownloadDependency() failed- InstallerManager not initalized");
		}
		
		public static void LaunchPD2()
		{
			if (instMgr != null)
			{
				instMgr.LaunchPD2Game();
			}
		}

		public static void OpenTempDirectory()
		{
			if (instMgr != null)
			{
				DirectoryInfo? tempDir = instMgr.GetTempDirectory();
				if (tempDir != null)
				{
					string path = tempDir.FullName;
					try
					{
						LogMessage($"Opening temp dir at {path}");
						System.Diagnostics.Process.Start("explorer.exe", path);
					}
					catch (Exception e)
					{
						LogMessage($"An error occurred while trying to open temp dir at {path}: {e}");
					}
				}
				else
				{
					LogMessage("Could not open temp directory - no folder found.");
				}
				return;
			}
			throw new Exception("OpenTempDirectory() failed- InstallerManager not initalized");
		}

		public static string? GetSteamDirectory()
		{
			if (instMgr != null)
			{
				return instMgr.GetSteamInstallDirectory();
			}
			throw new Exception("GetSteamDirectory() failed- InstallerManager not initalized");
		}

		public static void CreateTemporaryDirectory()
		{
			if (instMgr != null)
			{
				instMgr.CreateTempDirectory();
				return;
			}
			throw new Exception("CreateTemporaryDirectory() failed- InstallerManager not initalized");
		}

		public static string? GetTempDirectory()
		{
			if (instMgr != null)
			{
				DirectoryInfo? tempDir = instMgr.GetTempDirectory();
				return tempDir?.FullName;
			}
			throw new Exception("GetTempDirectory() failed- InstallerManager not initalized");
		}

		public static void DisposeTemporaryDirectory()
		{
			if (instMgr != null)
			{
				instMgr.DisposeTempDirectory();
				return;
			}
			throw new Exception("DisposeTemporaryDirectory() failed- InstallerManager not initalized");
		}

		public static string GetPd2InstallPath()
		{
			if (instMgr != null)
			{
				return instMgr.GetPd2InstallDirectory();
			}
			throw new Exception("GetPd2InstallPath() failed- InstallerManager not initalized");
		}

		public static List<ModDependencyEntry> GetModDependencyList()
		{
			if (instMgr != null)
			{
				return instMgr.GetDependencyEntries();
			}
			throw new Exception("GetModDependencyList() failed- InstallerManager not initalized");
		}
		
		public static async void GetModDependencyListAsync(bool forceReevaluate,Action<List<ModDependencyEntry>> clbk)
		{
			if (instMgr != null)
			{
				if (forceReevaluate)
				{
					List<ModDependencyEntry> result = await instMgr.CollectDependencies();
					clbk(result);
				}
				else
				{
					clbk(instMgr.GetDependencyEntries());
				}
				return;
			}
			throw new Exception("GetModDependencyList(bool, Action) failed- InstallerManager not initalized");
		}

		public static void CollectExistingMods()
		{
			if (instMgr != null) {
				instMgr.CollectPd2Mods();
				return;
			}
			throw new Exception("CollectExistingMods() failed- InstallerManager not initalized");
		}

		//there is no generic GetModFolder method because
		//1) mod folders are often renamed by the user
		//2) mod folders can be in either of two locations, meaning that there is possibility for collision between two identically named folders in both locations
		//therefore, one should always opt to specify searching for a mod by the name defined in the json file or the xml file

		public static Pd2ModFolder? GetBltMod(string name)
		{
			if (instMgr != null)
			{
				return instMgr.GetInstalledBltMod(name);
			}
			throw new Exception("GetBltMod() failed- InstallerManager not initalized");
		}
		
		public static Pd2ModFolder? GetBeardlibMod(string name)
		{
			if (instMgr != null)
			{
				return instMgr.GetInstalledBeardlibMod(name);
			}
			throw new Exception("GetBeardlibMod() failed- InstallerManager not initalized");
		}

		public static Pd2ModData? GetMiscMod(string name)
		{
			if (instMgr != null)
			{
				return instMgr.GetInstalledMiscMod(name);
			}
			throw new Exception("GetMiscMod() failed- InstallerManager not initalized");

		}

		public static bool IsDirectory(string entryName)
		{
			return entryName.EndsWith(@"\") || entryName.EndsWith(@"/");
		}

		public static void BrowserOpenDiscord()
		{
			OpenWebLink(URL_CRACKDOWN_DISCORD);
		}
		
		public static void BrowserOpenHomepage()
		{
			OpenWebLink(URL_CRACKDOWN_HOMEPAGE);
		}

		public static void BrowserOpenWiki()
		{
			OpenWebLink(URL_CRACKDOWN_WIKI);
		}

		public static void BrowserOpenInstructions()
		{
			OpenWebLink(URL_CRACKDOWN_INSTRUCTIONS);
		}

		public static string? GetPreferredBrowser()
		{
			string? browser = null;
			RegistryKey? regKey = null;

			try
			{
				regKey = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

				//LogMessage($"Got regKey {regKey}");

				// get rid of the enclosing quotes
				browser = regKey?.GetValue(null)?.ToString()?.ToLower().Replace("" + (char)34, "");

				if (browser != null)
				{
					if (!browser.EndsWith("exe"))
					{
						// trim any arguments after the path end (after ".exe")
						browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
						LogMessage($"Detected default browser {browser}");
					}
				}
			}
			catch (Exception e)
			{
				LogMessage($"Error getting reg key for default browser: {e.Message}");
			}
			finally
			{
				regKey?.Close();
			}

			return browser;
		}

		public static void OpenWebLink(string url)
		{

			string? preferredBrowser = GetPreferredBrowser();
			
			if (preferredBrowser != null)
			{
				try
				{
					System.Diagnostics.Process.Start(String.Format(preferredBrowser,""), url);
				}
				catch (System.ComponentModel.Win32Exception ex)
				{
					LogMessage($"Could not open web link {url}: {ex.Message}");
				}
			}
			else
			{
				LogMessage("OpenWebLink() failed- Could not get preferred browser.");
			}
			
		}

		public static void CreateLogWriter()
		{
			if (instMgr != null)
			{
				instMgr.CreateLogStreamWriter();
				return;
			}
		}

		public static void DisposeLogWriter()
		{
			if (instMgr != null)
			{
				instMgr.DisposeLogStreamWriter();
				return;
			}
		}

		public static void WriteLog(string message)
		{
			if (instMgr != null)
			{
				instMgr.WriteLogMessage(message);
				return;
			}
		}

		/// <summary>
		/// This callback is called only when the user manually closes the installer via the "Quit", "Finish and Open PD2", or "Finish and Close" buttons. This occurs before the actual Application Exit event.
		/// </summary>
		public static void OnApplicationClose()
		{
			LogMessage("User requested installer application close");

			// dispose temp directory only if closed manually, and if no errors were found;

			DisposeLogWriter();
			// dispose stream writer so that the log file isn't considered open (which could prevent directory deletion)
			InstallerWrapper.DisposeTemporaryDirectory();


		}

		/// <summary>
		/// This callback is called only when the application is closed (eg by the Windows "Close window" button).
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public static void OnApplicationExit(object? sender, EventArgs e)
		{
			LogMessage("Installer application terminated (exit event)");

			// close and flush log writer
			DisposeLogWriter();
		}

	}
}


/* Outline:
 * 
 *	STAGE 0: (init)
 *		- Build download options checkbox list
 *		
 *	STAGE 1:
 *		- Welcome
 *	STAGE 2:
 *		- Find and confirm PD2 directory
 *		- Save directory path to class member
 *	STAGE 3:
 *		- Bool flag to indicate whether a query is in progress
 *		- Add button to trigger dependencies query again
 *		
 *		- Query dependencies url
 *		- Check provider for each dependency
 *		- For providers with secondary queries for versions or download urls, query them now;
 *		- Save dependency entries to class member
 *		
 *		- Populate download options checkbox list
 *	STAGE 4:
 *		- Bool flag to indicate whether a download is in progress 
 *		- Add button to trigger download again
 *		
 *		- Create temp download directory
 *		- For each dependency, download archive from given download url into temp download directory
 *		- unpack archive into temp download directory
 *		- and then install (destructive copy) into correct directory
 *		- Dispose temp download directory
 *	
 * 
 * 
 * 
 * 
 * 
 * 
 */