# TransdecEnvironment

![image](https://user-images.githubusercontent.com/23311513/53693770-1e5fbe00-3da5-11e9-8d21-36170c0b334d.png)

TransdecEnvironment is a Unity3D project prepared for RoboSub competition by PWr Diving Crew (KN Robocik) at Wrocław University of Science and Technology.

The simulation environment was originally prepared for Robosub 2018 Competition. It is built on TRANSDEC facility model from [SimBox repository](https://github.com/cantren/cantren.github.io).

The project is maintained by PWr Diving Crew software team members (Unity3D section).

[KN Robocik website](http://www.robocik.pwr.edu.pl/)

Should any issues be noticed, please submit a **New issue** on Github.

## Installation

### Unity
**TransdecEnvironment** requires latest stable version of Unity development platform and is platform-independent. For Windows and Mac OS X versions visit their site: https://unity3d.com/get-unity/download.

The project uses ML-Agents Toolkit Repository for communication with Python scripts. It is not included in the project, therefore it is necessary to clone it by executing this command in a command line:

```git clone https://github.com/Unity-Technologies/ml-agents.git```

### Python
For instructions on Python API configuration please go to [**PyTransdec** repository](https://github.com/PiotrJZielinski/PyTransdec).

### The environment
Once installed, clone TransdecEnvironment GitHub repository by executing this command:

```git clone https://github.com/PiotrJZielinski/TransdecEnvironment```

To use the environment launch the Unity Editor and open the project.

![image](https://user-images.githubusercontent.com/23311513/53694275-cd53c800-3dac-11e9-95e2-1f3830b64635.png)

In the project, make sure that `Scripting Runtime Version` is set to **`.Net 4.x Equivalent`**:
![image](https://user-images.githubusercontent.com/23311513/53694528-6afcc680-3db0-11e9-8d1e-250390f7988f.png)

Using file explorer navigate to ML-Agents directory and open `UnitySDK/Assets` folder. Inside of `ML-Agents` folder remove `Examples` directory together with `Editor.meta` file. Then, go back to `UnitySDK/Assets` directory and drag the `ML-Agents` folder to the Unity Editor `Project` window.

![image](https://user-images.githubusercontent.com/23311513/53694350-03de1280-3dae-11e9-95ea-f61c06c96f41.png)

After that, your Assets should include ML-Agents folder:

![image](https://user-images.githubusercontent.com/23311513/53694366-30922a00-3dae-11e9-863b-36e63b71bd63.png)

Having done all of the above steps, double-click `RoboSub2018` scene in `Assets/Scenes` directory to open the default TransdecEnvironment scene.

![image](https://user-images.githubusercontent.com/23311513/53694507-296c1b80-3db0-11e9-87df-822953e76585.png)

Everything is set up, you may now start using **TransdecEnvironment**!

## Usage

**TransdecEnvironment** configuration is held by **Academy** object:

![image](https://user-images.githubusercontent.com/23311513/53694612-9207c800-3db1-11e9-99b7-70c264d01e26.png)

Click it to show its properties in the Inspector:
![image](https://user-images.githubusercontent.com/23311513/53694634-ddba7180-3db1-11e9-97d6-8bfb218bc361.png)


