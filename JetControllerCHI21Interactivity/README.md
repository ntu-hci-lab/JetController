# Demonstration Files for CHI 2021 Interactivity

## DualSense2Windows

The program is based on [DualSense2Xbox](https://github.com/Solla/DualSense2Xbox). 

### Before Installation

Please preinstall the ViGEm driver first.
* [ViGEm Bus](https://github.com/ViGEm/ViGEmBus)

All C# libraries should be automaticly downloaded by NuGet Package Manager.
If not, please download these libraries manually.
* [How to Use Nuget Packages](https://www.syncfusion.com/blogs/post/how-to-use-nuget-packages.aspx)

### Acknowledgement and Reference

* [Dualsense, Haptics, Leds, and More (Hid Output Report)](https://www.reddit.com/r/gamedev/comments/jumvi5/dualsense_haptics_leds_and_more_hid_output_report/)
* [BLE Inputs](https://gist.github.com/Ryochan7/91a9759deb5dff3096fc5afd50ba19e2)
* [DualSense-Windows](https://github.com/Ohjurot/DualSense-Windows)
* [DS4Windows](https://github.com/Ryochan7/DS4Windows)
* [DualSense2Xbox](https://github.com/Solla/DualSense2Xbox). 

## Half-Life 2

After the program executes, it will automatically scan SteamLibrary locations from your computer. 
To fit the demonstration, it creates mods and replace game save files of Half-Life 2.

### Detecting Firing

After the user fires, Half-Life 2 will send rumble requests to the pseudo Xbox controller that ViGEm creates.
Once we receive the rumble requests from ViGEm kernels, we then actuate our haptic device. 

## Self-designed game

The game transmits game status including surface textures and vehicle velocity via Socket.
Our program then generates haptic feedback on the controller.
