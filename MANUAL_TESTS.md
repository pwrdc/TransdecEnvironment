# Manual Tests

In order to prevent features from breaking as the project progresses, most of them are written out here. 

To merge a branch with dev all of the following tests must pass on it. 

If a feature isn't described here it isn't guaranteed that it will work in future commits as there is no easy way to check if it works. 

Feel free to add tests here or edit them according to your judgement, just remember to note it in your commit message.

## Main menu
- Open Scenes/MainMenu.unity file. You should be presented with three buttons: *start*, *settings* and *quit*.
- After clicking on settings four dropdown options should appear: *mode* and *simulation mode*, *scenes* and *targets* as well as back button.
- Build the project with menu scene selected as main scene and test if clicking *exit* closes the window. Alternatively you can check if `Application.Quit()` is called after clicking the button.

## Free swim mode
- Open *Scenes/MainMenu.unity* file.
- Set mode to *player* and simulation mode to *free swim* in settings.
- Go back and click *start*.
- A scene should open.
- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects.
- Check if following controls work:
- Basic controls:
	- `wasd` to move forwards, left, backwards and right
	- `qe` to rotate left and right
	- `space/shift` to move upwards and downwards
	- `c` to switch to bottom camera
- Debug screen:
	- Press `b` button while moving.
	- It should pause the simulation and display debug screen with data exchanged with python. 
	- Make sure that "Send All Data" is checked RobotAgent's Agent Settings.
	- Judge if the values shown on debug screen are correct. 
	- Resume the simulation by clicking play in the editor.
	- Press `v` to close debug screen.
	- Do it five times while moving around.
- Grabber:
	- Go to an object on layer *GrabberTargets*\*
	- If there is no such object in this targets folder skip the next step.
	- Press and hold `g`, the object should follow the AUV.
- Torpedos:
	- Open the debug screen.
	- Press `t` while aiming at an empty wall.
	- There should be an information about missing the target on the debug screen.
	- Go to an object on layer *TorpedoTargets*\*
	- If there is no such object in this targets folder skip the next two steps.
	- Aim at its collider and press `t`, 
	- There should be information about hitting the target on the debug screen.
- Go to step one and select different scene, test for each scene.

\* Finding an object on layer:
- In the upper right corner find a button named *Layers*.
- Click it and in the opened window press an eye icon next to *Nothing*.
- Everything in the scene view should disappear.
- Now click an eye icon next to the desired layer name.
- Now only objects on this layer are visible and selectable in the scene view.
- Click and locate one of them.

## Data collection mode
- Open *Scenes/MainMenu.unity* file.
- Set mode to *player* and simulation mode to *data collection* in settings.
- Go back and click *start*.
- A scene should open.
- You should see quickly changing scene views.
- Press `b` button
	- It should pause the simulation and display debug screen.
	- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects.
	- You should see a green rectangle around an object.
	- Verify that the rectangle approximates the area covered by the object on screen. 
	- Judge if the 4 floating point values on debug screen after "bounding box" represent rectangle's minX, minY, maxX and maxY
	  (it should be flipped vertically relative to Unity's UI coordinates).
	- Check if the object has a target component. 
	- Resume the simulation.
	- Click `b` 4 more times and repeat the previous checks.
- Go to step one and select different scene, test for each scene and targets.

## Academy settings
- Open one of the simulation scenes in *Scenes* folder.
- Open academy object.
- Set FocusedCamera and CollectData to one and open the scene.
- Run the scene and check if the generated scene views are correct.
- Set EnableNoise to zero, there shouldn't be any objects from the noise folder on the scene.
- Set EnableNoise to one, the objects from noise folder should be placed on the scene. 
- Test if rest of the options work according to description in *README->Essential Academy parameters->Reset Parameters*
