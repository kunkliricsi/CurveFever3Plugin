#pragma once

#ifdef CURVEFEVER3PLUGIN_EXPORTS
#define CURVEFEVER3PLUGIN_API __declspec(dllexport)
#else
#define CURVEFEVER3PLUGIN_API __declspec(dllimport)
#endif // CURVEFEVER3PLUGIN_EXPORTS

namespace Plugin
{
	extern "C" 
	{ 
		bool isRunning = false;

		CURVEFEVER3PLUGIN_API void StartDrawing();

		CURVEFEVER3PLUGIN_API void StopDrawing(); 
	}
}