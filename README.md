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

![image](https://user-images.githubusercontent.com/29844618/58339328-a061ff00-7e49-11e9-8f11-544741039b87.png)

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
    * `FocusedCamera` - only used when `CollectData == 0`; specify which camera is working (0 - front left camera, 1 - bottom camera)
    * `WaterCurrent` - only used when `CollectData == 0`; is water current working (`0` - no water current, `1` - water current is enabled
    * `FocusedObject` - only used when `CollectData == 1`; specify which object is focused on collecting data (input: object number from `Data collection settings`)
    * `EnableBackgroundImage` - only used when `CollectData == 1`; enable custom background (`0` - transdec as background, `1` - random images as background)


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
    * `Add New` - creates new object to generate dataset
    * `Trigger` - focus on object
    * `Object` - Entire object which position is randomized across environment
    * `Annotation` - object which is focused by camera and annotated as `TargetAnnotation`
    * `FrontCamera` or `BottomCamera` - specify which camera to use while collecting data of this object
    * `Remove` - remove object from environment
    ![image](https://user-images.githubusercontent.com/29844618/58134363-e0956780-7c26-11e9-8297-c7ad475db5ba.png)
    
  * **Debug settings** (you shouldn't reallly touch them)
    * `Force Data Collection` - execute data collection regardless of controller mode
    * `Force Noise` - execute noised data collection
    * `Force Negative Examples` - execute negative examples data collection
    
## Data collection settings
In order to add new objects to generate dataset from them open Academy.
In `'Data collection settings'`

![image](https://user-images.githubusercontent.com/29844618/58337422-08164b00-7e46-11e9-96e3-bae4b7ac4076.png)

* **Settings**
 * `Add New` - creates new object and add to end of list
 * `Add with specific id` - insert new object with entered id
 * `Object` - drag here whole object which contain annotated element (look on example)
 * `Annotation` - drag here element to focus camera and annotate it
 * `Front Camera` - choose which camera to make picture (`Front Camera` or `Bottom Camera`)
 * `Big` - choose type of object settings to generate dataset (`Big`, `Small`, `On Bottom` - Use with bottom camera)
 * `Remove` - removes object
 
**Example**
 
 ![image](https://user-images.githubusercontent.com/29844618/58337894-131dab00-7e47-11e9-9c68-276bd1e7e4a6.png)
 
 * In order to generate dataset of whole triple board object - drag triple_board gameobject to Object and Annotation
 * In order to generate dataset of Aswang - drag triple_board gameobject to Object and Aswang to Annotation
 
 To personalize settings of data collection go to Agent gameobject
 
 ![image](https://user-images.githubusercontent.com/29844618/58338412-149ba300-7e48-11e9-82c3-2fddb6a52fda.png)
 
Open Agent, then in options select specific Element that corresponds to Id of elements in `Data collection settings`

![image](https://user-images.githubusercontent.com/29844618/58338556-36952580-7e48-11e9-9a64-6fb2da50605d.png)

* **Settings**
 * Min/Max Phi - Minimum/Maximum angle of camera (in degrees)
 * Min/Max Radius - Minimum/Maximum radius where camera is allocated
 * Camera fov - Camera field of view - works only in bottom camera mode
 * Water Level - Maximum height where camera is allocated (11 - water level of environment)
 * Max Depth - Minimum height where camera is allocated (7 - flat floor of pool)
 * Other settings - Don't append on camera location 


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
