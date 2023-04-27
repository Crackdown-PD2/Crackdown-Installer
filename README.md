# Crackdown-Installer
Source code for Crackdown's auto-installer program.

This installer, upon being run, will download the latest Crackdown files and install them into the correct locations; the installer contains no Crackdown files itself and must download them. 

The manifest of dependencies for Crackdown that this Installer will download can be found here: https://github.com/Crackdown-PD2/deathvox/blob/autoupdate/cd_dependencies.json

The total size of all Crackdown files (including those required for TOTAL CRACKDOWN) is somewhere between 1.5 to 2 GB, so it's necessary to split them up into separate download parts. For this reason, the installer was created to help users (especially those who are not technologically inclined) easily install Crackdown just by downloading the installer and running it. 

The installation can also be customized in the installer. 


## List of Dependencies

Crackdown is 100% open-source, so you can find all of the repositories for these assets here:

Classics enemy faction (including the grenadier): https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Classics

Federales enemy faction: https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Federales

Murkies enemy faction: https://github.com/Crackdown-PD2/Crackdown-Assets-Faction-Murkies

Custom maps and map replacements: https://github.com/Crackdown-PD2/Crackdown-Assets-Maps (Optional)

Custom music: https://github.com/Crackdown-PD2/Crackdown-Assets-Music (Optional, standalone)

Total Crackdown (TCD) playerside overhaul assets: https://github.com/Crackdown-PD2/Crackdown-Assets-Overhaul

# User instructions: (Windows 10 / Windows 11)

1. Download the latest release from the Releases page

[screenshot of Releases page with "latest release" button]

2. Open the Installer Executable.

[screenshot of Installer Executable with name "InstallerNameHere.exe" visible in sample downloads folder]

3. [Optional] Customize your installation. Some content is optional, such as enemy reskins, map edits, and the TOTAL CRACKDOWN Player-side Overhaul; you can choose whether or not to install this optional content at your leisure. The installer can be used again later to modify your Crackdown mod and install or uninstall any of this content at will.

[collage of optional content, including promotional images of maps, enemy reskins (eg Rino's), and TCD]

4. Enjoy!

[pink skull stamp]

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

#### Possible Explanation(s):

A) Your computer might not have enough free disk space on the disk that contains your PAYDAY 2 installation. 

**Solution:** Ensure that the disk that contains your PAYDAY 2 game installation has enough free space for all of Crackdown's dependencies. The total unzipped file size for Crackdown and all of its dependencies (including optional packages) is roughly 1.9GB, though this is subject to change, as Crackdown is still in development. You may need to delete some other mods or files on that disk.

B) Your antivirus program may be interfering. Antivirus programs are important, but often cause interference in day to day operations. Since many antivirus programs run routine scans on files (especially recent ones), it may also be interfering as the Installer attempts to move or remove a file currently being scanned, for example.

**Solution:** Try downloading and installing files manually (see above for links), temporarily disabling your antivirus program while the Installer is downloading files (you can re-enable it afterward).


----

# Troubleshooting Other Installer Problems

Any issues with the Installer itself will be logged (along with the exception-specific error message) inside the installer's session log.
If you encounter crashing, freezing, or other issues, please find the log file inside most recent Crackdown Installer log folder (in `\AppData\Local\Temp\crackdowninstaller_[random characters here]`) and send it to us.

Also feel free to give feedback on the installer, as it's still in development.

If you have questions or need troubleshooting help with the installer or with Crackdown, please feel free to join the Crackdown Community Discord and ask us!
Discord invite link: dak2zQ2
