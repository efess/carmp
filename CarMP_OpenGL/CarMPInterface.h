#pragma once
#include "OpenGLManager.h"

extern "C" _declspec(dllexport) int RegisterKeyboardCallback(void (__stdcall *pFunction)(KEYBOARD_EVENT));
extern "C" _declspec(dllexport) int RegisterMouseCallback(void (__stdcall *pFunction)(MOUSE_EVENT));
extern "C" _declspec(dllexport) int RegisterRenderCallback(void (__stdcall *pFunction)(void));
extern "C" _declspec(dllexport) int TestFuncion();
extern "C" _declspec(dllexport) int CreateWindow();

// Drawing stuff
extern "C" _declspec(dllexport) int DrawImage(int, float);

// Resource Stuff
extern "C" _declspec(dllexport) int CreateImage(const char*);

extern "C" _declspec(dllexport) int DeleteResource(int);