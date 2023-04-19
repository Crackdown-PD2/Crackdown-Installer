using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static Crackdown_Installer.InstallerManager;

namespace Crackdown_Installer
{
	internal static class InstallerWrapper
	{
		
		public static InstallerManager? instMgr;

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

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}

		public static async Task<string?> DownloadDependency(ModDependencyEntry dependency)
		{
			if (instMgr != null)
			{
				return await instMgr.DownloadDependency(dependency);
			}
			throw new Exception("CreateTemporaryDirectory() failed- InstallerManager not initalized");
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