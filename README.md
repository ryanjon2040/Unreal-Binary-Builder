Unreal Engine 4 Binary Builder
======================

This is a small app designed to create binary build of [Unreal Engine 4](https://www.unrealengine.com/) from [GitHub source](https://github.com/EpicGames/UnrealEngine).

![](https://img.shields.io/twitter/follow/ryanjon2040.svg?style=popout)	![](https://img.shields.io/github/last-commit/ryanjon2040/UE4-Binary-Builder.svg?style=popout) ![](https://img.shields.io/github/license/ryanjon2040/UE4-Binary-Builder.svg?style=popout) ![](https://img.shields.io/github/downloads/ryanjon2040/UE4-Binary-Builder/total.svg?style=popout) 

![](https://img.shields.io/github/languages/code-size/ryanjon2040/UE4-Binary-Builder.svg?style=flat) ![](https://img.shields.io/github/repo-size/ryanjon2040/UE4-Binary-Builder.svg?style=flat)

![Screenshot](https://i.imgur.com/z9u42gI.png)

Through this app you can also zip the final rocket build for distribution.
![Screenshot](https://i.imgur.com/oOKw8uy.png)

# How to use

I/Prepare the UE4 source
1. Clone the UE4 source from github
2. Run Setup.bat/.sh from the cloned folder, follow the instruction and resolve any error at this step
3. Run GenerateProjectFile.bat/.sh, make sure no error occured at this step
4. Open UE4.sln with Visual Studio
5. Rebuild these 2 project under Application group: AutomationTool and AutomationToolLauncher

II/Build with UE4 Binary Builder
1. Clone this git
2. Open the sln file with Visual Studio, then build the application with Release configuration
3. Copy the built binary to the UE4 source folder prepared above.
4. Open UE4 Binary Builder, the path for AutomationTool will be automatically parsed, if not, try to get it from the UE4 folder (Engine\Binaries\DotNET\AutomationToolLauncher.exe & Engine\Build\InstalledEngineBuild.xml)  
5. Choose what to build and press Start build. That's it

# Troubleshoot

**Access Denied on some files?**
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
