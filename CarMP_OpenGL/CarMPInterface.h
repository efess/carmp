#pragma once

#include "OpenGLManager.h"

extern "C" _declspec(dllexport) int RegisterKeyboardCallback(void (__stdcall *pFunction)(OGL_KEYBOARD_EVENT));
extern "C" _declspec(dllexport) int RegisterMouseCallback(void (__stdcall *pFunction)(OGL_MOUSE_EVENT));
extern "C" _declspec(dllexport) int RegisterRenderCallback(void (__stdcall *pFunction)(void));

extern "C" _declspec(dllexport) int CreateOGLWindow(OGL_RECT pRectangle);

// Drawing stuff
extern "C" _declspec(dllexport) int DrawImage(OGL_RECT, int, float);

// Resource Stuff
extern "C" _declspec(dllexport) int CreateImage(const char*);

extern "C" _declspec(dllexport) int DeleteResource(int);