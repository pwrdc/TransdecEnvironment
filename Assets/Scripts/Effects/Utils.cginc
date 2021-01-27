#ifndef UTILS
#define UTILS

// without this transformation the noise would be stretched horizontally
// because uv coordinates for screen are in range <0, 1> 
// the coordinates returned from this function are in range
// x: <0, screen_width_to_height_ration>
// y: <0, 1>
float2 stretch_screen_uv_coordinates(float2 uv) {
	float screenHeight = _ScreenParams.y;
	float ratio = _ScreenParams.x / _ScreenParams.y;
	return float2(uv.x / screenHeight * ratio, uv.y / screenHeight);
}

#endif