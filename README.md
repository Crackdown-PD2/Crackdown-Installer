# User instructions

Note: You need [.NET 7.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-7.0.5-windows-x64-installer?cid=getdotnetcore) in order to use the Crackdown Installer. If you don't already have it, don't worry- attempting to run the application will automatically prompt you to download and install this runtime- you need only click to accept it, no technical expertise required.

1. **Download and unzip** the latest release from the [Releases page](https://github.com/Crackdown-PD2/Crackdown-Installer/releases). 

This will be a file called **Crackdown-Installer.zip** or something similar- NOT the source code. (Unless you want to build it yourself or something.)

2. **Run** the **Crackdown-Installer.exe**.

Some content is optional, such as enemy reskins, map edits, and the TOTAL CRACKDOWN Player-side Overhaul; in the "Select Packages" section of the Installer window, you can choose whether or not to install this optional content at your leisure. The installer can be used again later to modify your Crackdown mod and install any of this content at will.

3. Enjoy!

# About Crackdown-Installer
This repo contains the source code for Crackdown's auto-installer program.

This installer, upon being run, will download the latest Crackdown files and install them into the correct locations. 

The manifest of dependencies for Crackdown that this Installer will download can be found here: https://github.com/Crackdown-PD2/deathvox/blob/autoupdate/cd_dependencies.json

The total size of all Crackdown files (including those required for TOTAL CRACKDOWN) is somewhere between 1.5 to 2 GB, so it's necessary to split them up into separate download parts. For this reason, the installer was created to help users (especially those who are not technologically inclined) easily install Crackdown just by downloading the installer and running it. 


## List of Dependencies

Crackdown is 100% open-source, so you can find all of the repositories for these assets here:

Classics enemy faction (including the grenadier): https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Classics

Federales enemy faction: https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Federales

Murkies enemy faction: https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Murkies

Custom maps and map replacements: https://github.com/Crackdown-PD2/Crackdown-Assets-Maps (Optional)

Custom music: https://github.com/Crackdown-PD2/Crackdown-Assets-Music (Optional, standalone)

Total Crackdown (TCD) playerside overhaul assets: https://github.com/Crackdown-PD2/Crackdown-Assets-Overhaul

----

# Troubleshooting Failed Downloads

If packages downloaded or installed by the Installer fail for any reason, the general reason should be displayed at the end stage screen of the Installer. 
Here are the possible messages, what they could mean, and what you can do to mitigate such problems:

### 1: "Could not download package"

#### Possible Explanation(s):

A) Your computer might not have enough free disk space on the disk that contains your Temp folder. (The Installer creates a temporary subdirectory in your `\AppData\Local\Temp\` folder, which holds the session log, the downloaded zip archive of any downloaded packages, and the unzipped archive before it is moved to its final installation location.)

**Solution**: Please make sure that you have enough disk space on the disk containing your Temp folder. Currently, there is no menu option for changing the download location. If you are unable to clear enough space on this disk, and you have another disk with space for the packages, try downloading the packages manually (eg. through the browser of your choice, which should let you change the download destination).

B) The Installer might not have disk write permissions to download the package.

**Solution:** Try downloading the packages manually to a disk that you have permission to write to (see above).

C) Your internet might be too unstable to maintain the package download.

**Solution:** Neither you nor I controls the rate at which one's internet lives or dies; we are all at the mercy of our respective ISPs. I wish you the best of luck. Thoughts and prayers.

### 2: "Could not extract package archive":

#### Possible Explanation(s):

A) Your computer might not have enough free disk space on the disk that contains your Temp folder. 
(See above)

B) The Installer might not have disk write permissions to download the package.
(See above)

### 3: "Could not move package file(s)"

This general error message indicates that the installer downloaded the files correctly but was unable to move them all to their final installation location. As such, the installation process might have left behind a few files that need to be put in the correct place(s).

#### Possible Explanation(s):

A) Some file paths that are too long may cause some files to fail to transfer automatically. [This is a limitation of Windows](https://learn.microsoft.com/en-us/windows/win32/fileio/maximum-file-path-limitation?tabs=registry), not the Installer. We're currently examining possible long-term fixes, but in the meantime, here's what you can do:

**Solution A:** Move the files to the mods folder yourself.

Look inside your Crackdown Installer folder. Open the folder called `temp`. Inside, you will see some folders for each download that failed- drag those folders into the installationn location. You should be able to find these locations in your mods folder.

**Solution B:** Download install the dependency/dependencies manually.

In the above "List of Dependencies", download each one and use the instructions on their respective pages to install them.

B) Your computer might not have enough free disk space on the disk that contains your PAYDAY 2 installation.

**Solution:** Ensure that the disk that contains your PAYDAY 2 game installation has enough free space for all of Crackdown's dependencies. The total unzipped file size for Crackdown and all of its dependencies (including optional packages) is roughly 1.9GB, though this is subject to change, as Crackdown is still in development. You may need to delete some other mods or files on that disk.

C) Your antivirus program may be interfering. Antivirus programs are important, but often cause interference in day to day operations. Since many antivirus programs run routine scans on files (especially recent ones), it may also be interfering as the Installer attempts to move or remove a file currently being scanned, for example.

**Solution:** Try downloading and installing files manually (see above for links), temporarily disabling your antivirus program while the Installer is downloading files (you can re-enable it afterward).


----

# Troubleshooting Other Installer Problems

Any issues with the Installer itself will be logged (along with the exception-specific error message) inside the installer's session log.
If you encounter crashing, freezing, or other issues, please find the log file inside the `temp` folder, which is in the Crackdown Installer folder alongside the Installer's executable file.

Also feel free to give feedback on the installer, as it's still in development.

If you have questions or need troubleshooting help with the installer or with Crackdown, please feel free to join the Crackdown Community Discord and ask us!
Discord invite link: dak2zQ2
