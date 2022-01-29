# 3.1.5
* **FIXED**: Crash when clicking Browse button in zip tab (reported by Gambit)

# 3.1.4

**THIS IS A CRITICAL UPDATE. DO NO SKIP**
* Updates to the updater.
* Show commit of current Engine.
* Add LinuxArm64 for Unreal Engine 5.
* Update some UE4 names to Unreal.
* Errors are now written to separate log file.
* Support custom Engines.
* New crash reporter.
* **FIXED**: Progressbar and cancel button not hiding after zipping..
* **FIXED**: Issue with CanSaveToZip.
* **FIXED**: Incorrect behavior when canceling build.
* **FIXED**: Issue when selecting Host DDC.
* **FIXED**: Plugin zipping crash if locations are same.

# 3.1.3

* Improve UE5 support
* Improve app update. Now shows changelog as well.
* Remove Engine Version selection. This is now automated.
* Remove Automation Tool Launcher selection. This is now automated.
* Use AutomationTool instead of AutomationToolLauncher for UE5.
* Improve messages in Zip tab.
* Updated dependencies.

* **FIXED**: Crashing when zipping UE5 build.
* **FIXED**: Incorrect method for OpenBuildFolder in zip tab.
* **FIXED**: Not Building Engine if all checkboxes are unchecked in Setup tab.


# 3.1.2

* **FIXED**: Issues with updating.

# 3.1.1

* Support **Unreal Engine 5**
* Add basic editor to edit target cs files.
* Add options to Start Build. You can now choose to run Setup, GenerateProjectFiles or AutomationTool.
* Add UnrealBuilderHelpers class
* New copy button in log viewer to copy message to clipboard.
* Add changelog link to menubar.
* Check and Install Update now shows version number.
* Selecting Engine Version is now optional.
* **FIXED**: Git dependency cache path
* **FIXED**: OpenClipboard Failed (0x800401D0 (CLIPBRD_E_CANT_OPEN))
* **FIXED**: ShadowErrors.cpp reporting as error.

# 3.1

* Add compiler info to plugin card.
* Improved plugin build.
* Add error message if **_RunUAT.bat_** is missing.
* **FIXED**: Update dialog not showing.
* **FIXED**: Unable to stop Engine build.
* **FIXED**: Crash if no Unreal Engine is installed.
* **FIXED**: Stop build button left disabled.
* **FIXED**: Crash if ___Resources\Icon128.png___ does not exist for plugin.
* **FIXED**: *[UBB-3]* Crash if Current Process file does not exist.
* **FIXED**: *[UBB-4]* Crash if settings file cannot be written when changing platform.
* **FIXED**: *[UBB-6]* Crash with message Input string not in correct format.

# 3.0

* Initial Release