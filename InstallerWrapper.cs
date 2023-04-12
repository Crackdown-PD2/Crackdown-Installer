using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Crackdown_Installer
{
	internal static class InstallerWrapper
	{
		
		public static InstallerManager? instMgr;

		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{

			//instantiate new client to handle all outgoing http reqs
			HttpClient client = new HttpClient();
			client.Timeout = TimeSpan.FromSeconds(15);

			instMgr = new(client);


			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();
			Application.Run(new Form1());
		}

	}
}