Unreal Engine 4 Binary Builder
======================

This is a small app designed to create binary build of [Unreal Engine 4](https://www.unrealengine.com/) from [GitHub source](https://github.com/EpicGames/UnrealEngine).

![](https://img.shields.io/twitter/follow/ryanjon2040.svg?style=popout)	![](https://img.shields.io/github/last-commit/ryanjon2040/UE4-Binary-Builder.svg?style=popout) ![](https://img.shields.io/github/license/ryanjon2040/UE4-Binary-Builder.svg?style=popout) ![](https://img.shields.io/github/downloads/ryanjon2040/UE4-Binary-Builder/total.svg?style=popout) 

![](https://img.shields.io/github/languages/code-size/ryanjon2040/UE4-Binary-Builder.svg?style=flat) ![](https://img.shields.io/github/repo-size/ryanjon2040/UE4-Binary-Builder.svg?style=flat)

![Screenshot](https://i.imgur.com/z9u42gI.png)

Through this app you can also zip the final rocket build for distribution.
![Screenshot](https://i.imgur.com/oOKw8uy.png)

# How to use

Step I: Prepare the UE4 source
1. Clone the UE4 source from github
2. Run Setup.bat/.sh from the cloned folder, follow the instruction and resolve any error at this step
3. Run GenerateProjectFile.bat/.sh, make sure no error occured at this step
4. Open UE4.sln with Visual Studio
5. Rebuild these 2 project under Application group: AutomationTool and AutomationToolLauncher

Choose one of these options. Either download or build:

Step II: (a) Download UE4 Binary Builder
1. Download [latest release](https://github.com/ryanjon2040/UE4-Binary-Builder/releases/latest).
2. Unzip to your preferred location and start `Unreal Binary Builder.exe`.

Step II: (b) Build UE4 Binary Builder
1. Clone this git.
2. Open the sln file with Visual Studio, switch to Release configuration and press F5 to build and start or build the application and start manually from bin folder.

Step III: Build Engine
1. Select your target engine version.
2. Choose AutomationToolLauncher.exe by browsing to Engine\Binaries\DotNET folder.
3. (a) [Optional] Click Post Build Settings, choose a location to save the zip and enable your preferred options.
3. (b) Choose your settings and build.

# Troubleshoot

**Bug with 4.25.4**</br>
There is a known issue with 4.25.4 where it fails to build with an error message: `AutomationException: Attempt to add file to temp storage manifest that does not exist (<Path To Engine>\cpp.hint)` This issue has been fixed in 4.26 but if you need to use 4.25.4 see this workaround by Bernard Rouhi: https://github.com/ryanjon2040/UE4-Binary-Builder/issues/26#issuecomment-718204352

**Access Denied on some files?**</br>
On Windows, just change the ownership to Users then try again. To change ownership on Windows, follow these steps
 - Right click on the UE4 folder, choose Properties
 - Switch to Security tab
 - Click on Advanced
 - Near the top, click on Change User
 - A new dialog will open, in the text box at bottom, type in "Users", then click Check Names
 - OK till the end.

   

# Credits

[Material Design In XAML](https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit) by [ButchersBoy](https://github.com/ButchersBoy)

[LogViewer](https://stackoverflow.com/a/16745054) by [Federico Berasategui](https://stackoverflow.com/users/643085/federico-berasategui)
