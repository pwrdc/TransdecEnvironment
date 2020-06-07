# Manual Tests

In order to prevent features from breaking as the project progresses, most of them are written out here. 

To merge a branch with dev all of the following tests must pass on it. 

If a feature isn't described here it isn't guaranteed that it will work in future commits as there is no easy way to check if it works. 

Feel free to add tests here or edit them according to your judgement, just remember to note it in your commit message.

## Main menu
- Open Scenes/MainMenu.unity file. You should be presented with three buttons: *start*, *settings* and *quit*.
- After clicking on settings three dropdown options should appear: *competition*, *mode* and *simulation mode* as well as back button.
- Build the project with menu scene selected as main scene and test if clicking *exit* closes the window. Alternatively you can check if Application.Quit() is called after clicking the button.

## Free swim mode
- Open *Scenes/MainMenu.unity* file.
- Set mode to *player* and simulation mode to *free swim* in settings.
- Go back and click *start*.
- A scene should open.
- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects.
- Check if following controls work:
	- `wasd` to move forwards, left, backwards and right
	- `qe` to rotate left and right
	- `rf` to move upwards and downwards
	- go to an object on layer *ball*, press and hold `t`, it should follow the boat
	- torpedo is left out for now as it has no visual indication
- Pause the game and click *show sent data* in academy and judge if the values there are correct, do it five times while moving around.
- Go to step one and select different scene, test for each scene.

## Data collection mode
- Open Scenes/MainMenu.unity file.
- Set mode to *player* and simulation mode to *data collection* in settings.
- Go back and click *start*.
- A scene should open.
- You should see quickly changing scene views.
- Click pause in the editor.
	- Check if you're inside of the pool, you can see upper water surface and you can recognise that you're under water by camera effects.
	- You should be able to see a target, you can check if an object is a target in academy
	- Find target annotation script and activate it, a green box should appear around the target
	- Click *show sent data* in academy and judge if the values there are correct.
- Click pause 4 more times and repeat the previous 4 checks
- Go to step one and select different scene, test for each scene.

## Python integration

Testing integration with python will require adding code on python's side. 

There will be two kinds of tests:
- transfering keyboard input read by python script through communication channel into unity, as a way of testing this channel
- testing different options related to data collection set from python