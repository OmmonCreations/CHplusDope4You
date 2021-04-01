# DOPE Elections 4 You
Dope things should be shared! However, here are some things to keep in mind:
1. For the purpose of providing a working and self contained project, we've removed the communication with our live API and references to external images.
2. We've included example data for you to play around with. This data is solely for demonstration purposes and must not be used in another context.
3. For legal reasons, we have excluded the sound engine from this repository. In order to use it, you must obtain a copy yourself and then insert it as per instructions below.

### Installation & Setup
In order to run the Unity project contained in this repository, follow these setup steps:
1. Download and Install the Unity Hub from https://unity3d.com/get-unity/download
2. In the Unity Hub, install Unity 2020.3.0f1
3. Open the project

After the initial setup, you'll want to follow these additional steps:
* Download and install the FMOD Unity Plugin (Version 2.01.05), either from the asset store or from the official FMOD website under https://fmod.com. Replace the contents under /CHplusDope/Assets/Plugins/FMOD with the downloaded plugin. Make sure you leave the FMODUnity.asmdef untouched.
* Change the product name and organization under the Unity project settings if you plan on doing anything beyond simply looking around.

### Other useful info
* All save files and downloads are stored here: Win: C:\Users\{USER}\AppData\LocalLow\{A}\{B}\Data_DEV | Mac: ~/Library/Application Support/{A}/{B}
    * Replace {USER} with your windows username, e.g. 'Bill'
	* Replace {A} with your name that you set in the project settings under 'Company Name'. If you don't change it, it will be 'Ommon'
	* Replace {B} with the product name that you set in the project settings. If you don't change it, it will be 'DOPE Elections 4 You'
* We've prepared a file encryption for downloaded files that you can optionally enable.
    1. Open /CHplusDope/Assets/Modules/FileStore/Scripts/LocalStorage.cs
	2. On line 119 remove 'true' from the if statement
	3. On line 147 remove 'false' from the if statement
	4. Uncomment lines 29 to 36
	5. Run the game
	6. In the console copy the key and the initialization vector (iv)
	7. Insert them on lines 22 and 23
	8. Comment lines 29 to 36 again