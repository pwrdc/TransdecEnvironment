# Manual Tests

In order to prevent features from breaking as the project progresses all of them are written out here. 
To merge a branch with dev all of the following tests must pass on it. 
If a feature isn't described here it isn't guaranteed that it will work in future commits as there is easy way to check if it works. 
Each test is annotated with `#<number>` to make communication easier. 

## Main menu
- Open Scenes/MainMenu.unity file. You should be presented with three buttons: *start*, *settings* and *quit*. #1
- After clicking on settings three dropdown options should appear: *competition*, *mode* and *simulation mode* as well as back button. #2
- Build the project with menu scene selected as main scene and test if clicking exit closes the window. Alternatively you can check if Application.Quit() is called after clicking the button. #3

## Free swim mode
- Open Scenes/MainMenu.unity file.
- Set mode to *player* and simulation mode to *free swim* in settings.
- Go back and click *start*.
- A scene should open. #4
- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects. #5
- Check if following controls work:
	- `wasd` to move forwards, left, backwards and right #6
	- `qe` to rotate left and right #7
	- `rf` to move upwards and downwards #8
- go to step one and select different scene, test for each scene. When describing test failure add *.<scene_index>* to test number. 
For example if test #8 failed for scene *SAUVC* which is first on the list the failed test identifier is #8.0

## Data collection mode
- Open Scenes/MainMenu.unity file.
- Set mode to *player* and simulation mode to *data collection* in settings.
- Go back and click *start*.
- A scene should open. #9
- You should see quickly changing scene views. #10
- Click pause in the editor.
	- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects. #11
	- You should be able to see a target, you can check if an object is a target in academy #12
- go to step one and select different scene, test for each scene. When describing test failure add *.<scene_index>* to test number. 
For example if test #12 failed for scene *SAUVC* which is first on the list the failed test identifier is #12.0