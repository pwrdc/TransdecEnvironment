# Read before opening in editor

From my understanding Aura has a lot of shader variants for different platforms and level of detail.
Opening this project in Unity will freeze it for about **30 minutes** consuming all of your memory and CPU power until all of the compute shaders compile.
If 30 minutes passed it still frozen try clicking on the window, it might bring it back.
If that didnt' help, try terminating the Unity process from task menager, after that reopen the project. 
Bunch of error messages will show up, close them and add some whitespace to one of the Aura files, it will restart the compilation.
After compiling do not edit or move files inside of Aura folder, even the slightest whitespace change will trigger recompilation.
Besides that Aura doesn't really affect performance or compile time for the project.
The developer of Aura claims that his paid Aura 2 package has faster compile times.

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

### Python
For instructions on Python API configuration please go to [**PyTransdec** repository](https://github.com/PiotrJZielinski/PyTransdec).

### The environment
Once installed, clone TransdecEnvironment GitHub repository by executing this command:

```git clone https://github.com/PiotrJZielinski/TransdecEnvironment```

To use the environment launch the Unity Editor and open the project.

![image](https://user-images.githubusercontent.com/23311513/53694275-cd53c800-3dac-11e9-95e2-1f3830b64635.png)

In the project, make sure that `Scripting Runtime Version` is set to **`.Net 4.x Equivalent`**:
![image](https://user-images.githubusercontent.com/23311513/53694528-6afcc680-3db0-11e9-8d1e-250390f7988f.png)

Having done all of the above steps, double-click `RoboSub2018` scene in `Assets/Scenes` directory to open the default TransdecEnvironment scene.

![image](https://user-images.githubusercontent.com/23311513/53694507-296c1b80-3db0-11e9-87df-822953e76585.png)

Everything is set up, you may now start using **TransdecEnvironment**!

## Usage

**TransdecEnvironment** configuration is held by **Academy** object:

![image](https://user-images.githubusercontent.com/23311513/53694612-9207c800-3db1-11e9-99b7-70c264d01e26.png)

Click it to show its properties in the Inspector:

![image](https://user-images.githubusercontent.com/23311513/53694634-ddba7180-3db1-11e9-97d6-8bfb218bc361.png)

### Essential Academy parameters:
  * **Training Configuration** - visual observations' settings used when *training mode* is selected in Python API
  * **Inference Configuration** - visual observations' settings used when *training mode* is **not** selected in Python API
    * `Width` - camera image width
    * `Height` - camera image height (both affect performance)
    * `Quality Level` - image compression setting (`1` - lowest quality, `5` - highest quality)
    * `Time Scale` - indicates how many frames are dropped during communication (ie. how "fast" the environment behave)
    * `Target Frame Rate` - how many frames per secodn should the environment return (`-1` for maximum available)
    
  * **Reset Parameters** - parameters that can be set using Python API on environment reset (better not to modify unless you know what you are doing)
    * `CollectData` - set environment in data collection mode (`0` - standard mode, `1` - data collection mode)
    * `EnableNoise` - only used when `CollectData == 1`; enable random positioning of other objects (so that they create "noise")
    * `Positive` - only used when `CollectData == 1`; collect positive examples (`0` - negative examples, target invisible, `1` - positive examples, target visible)
    * `AgentMaxSteps` - how many steps the agent can take before resetting the environment (defaults to `0` - indefinite steps)
    
  * **Controller Settings**:
    * `Control` - agent steering method (`Player` for keyboard steering, `Python` for Python API controller); if `Control == Player` use keyboard for steering:
      * `W` - `S`: longitudinal movement (front-backward)
      * `A` - `D`: lateral movement (left-right)
      * `R` - `F`: vertical movement (upward-downward)
      * `Q` - `E`: yaw rotation (turn left-turn right)
    * `Learning Brain`, `Player Brain` - ML-Agents Brain objects (correctly set by default)
    
  * **Start position settings** - starting position drawing settings:
    * `Random Quarter` - randomly select one of 4 TRANSDEC quarters (if `false` default quarter is chosen)
    * `Random Position` - randomly move all objects on reset (if `false` objects stay in their default position)
    * `Random Orientation` - enable random rotation of the agent (at an angle of 90° or 180° to the gate)
    
  * **Data collection settings** - used when `resetParameters["CollectData"] == 1`
    * `Mode` - target whose images are collected
    * `Gate Target Object`, `Path Target Object`, ... - target objects from the environment
    
  * **Debug settings** (you shouldn't reallly touch them)
    * `Force Data Collection` - execute data collection regardless of controller mode
    * `Force Noise` - execute noised data collection
    * `Force Negative Examples` - execute negative examples data collection
    
## Updating
In order to update TransdecEnvironment you need to reset your changes by executing:

```git stash```

You can then pull latest changes from `master` branch by executing:

```git pull```

In case you want to reapply changes you can execute

```git stash pop```

but this is not guaranteed to be working. If you do not need your changes execute

``` git stash drop```

Please **do not** push your changes to `master` branch. If you find your changes useful please create another branch and create a **Pull Request**
